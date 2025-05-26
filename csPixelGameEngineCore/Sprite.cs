using System;
using System.Drawing;
using System.IO;
using csPixelGameEngineCore.Enums;
using log4net;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// A 2D drawing surface.
    /// </summary>
    public class Sprite
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Sprite));

        public enum Mode { NORMAL, PERIODIC };

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Pixel[] ColorData { get; private set; }

        public Mode ModeSample { get; private set; } = Mode.NORMAL;

        private Sprite()
        {
            // Don't allow creation of sprites without dimensions
        }
        
        public Sprite(int w, int h)
        {
            if (w <= 0) throw new ArgumentException("Argument must be greater than 0", nameof(w));
            if (h <= 0) throw new ArgumentException("Argument must be greater than 0", nameof(h));

            Width = w;
            Height = h;
            ColorData = new Pixel[Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                ColorData[i] = Pixel.BLACK;
            }
        }

        public Sprite(string sImageFile, ResourcePack pack)
        {
            LoadFromFile(sImageFile, pack);
        }

        private delegate void DataReader(BinaryReader br);

        public RCode LoadFromPGESprFile(string sImageFile, ResourcePack pack)
        {
            DataReader ReadData = (br) =>
            {
                Width = br.ReadInt32();
                Height = br.ReadInt32();
                ColorData = new Pixel[Width * Height];
                for (int i = 0; i < (Width * Height); i++)
                {
                    ColorData[i] = br.ReadUInt32();
                }
            };

            try
            {
                if (pack == null)
                {
                    using (BinaryReader br = new BinaryReader(File.OpenRead(sImageFile)))
                    {
                        ReadData(br);
                        return RCode.OK;
                    }
                }
                else
                {
                    ResourceBuffer rb = pack.GetFileBuffer(sImageFile);
                    using (BinaryReader br = new BinaryReader(new MemoryStream(rb.Memory.ToArray())))
                    {
                        ReadData(br);
                    }
                    return RCode.OK;
                }
            }
            catch (Exception)
            {
                return RCode.FAIL;
            }
        }

        public RCode SaveToPGESprFile(string sImageFile)
        {
            if (ColorData == null)
            {
                return RCode.FAIL;
            }

            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(sImageFile)))
                {
                    bw.Write(Width);
                    bw.Write(Height);
                    for (int i = 0; i < (Width * Height); i++)
                    {
                        bw.Write(ColorData[i]);
                    }
                    bw.Close();
                }
                return RCode.OK;
            }
            catch (Exception)
            {
                return RCode.FAIL;
            }
        }

        public RCode LoadFromFile(string sImageFile, ResourcePack pack)
        {
            Bitmap bmp;

            try
            {
                if (pack != null)
                {
                    // Load sprite from input stream
                    ResourceBuffer rb = pack.GetFileBuffer(sImageFile);
                    var imageData = rb.Memory.ToArray();
                    bmp = new Bitmap(new MemoryStream(imageData));
                }
                else
                {
                    // Load sprite from file
                    bmp = new Bitmap(sImageFile);
                }
            }
            catch (Exception ex)
            {
                Log.Warn($"Unable to load image {sImageFile}", ex);
                return RCode.NO_FILE;
            }

            Width = bmp.Width;
            Height = bmp.Height;
            ColorData = new Pixel[Width * Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color p = bmp.GetPixel(x, y);
                    SetPixel(x, y, new Pixel(p.R, p.G, p.B, p.A));
                }
            }

            bmp.Dispose();

            return RCode.OK;
        }

        public void Fill(Pixel col)
        {
            Span<Pixel> dest = ColorData;
            dest.Fill(col);
        }

        /// <summary>
        /// Get the pixel value at the given x and y coordinates within this sprite.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The pixel value. If x and y coordinates are outside of the sprite boundaries, Pixel.BLANK is returned.</returns>
        public Pixel GetPixel(int x, int y)
        {
            if (ModeSample == Mode.NORMAL)
            {
                if (x < Width && y < Height)
                    return ColorData[y * Width + x];
                else
                    return Pixel.BLANK;
            }
            else
            {
                // "Periodic" mode
                return ColorData[Math.Abs(y % Height) * Width + Math.Abs(x % Width)];
            }
        }

        /// <summary>
        /// Set a pixel in this sprite to the given pixel value
        /// </summary>
        /// <param name="x">x coordinate in sprite to set</param>
        /// <param name="y">y coordinate in sprite to set</param>
        /// <param name="p">Pixel value to use</param>
        /// <returns>true if set, false if not (outside of sprite boundaries)</returns>
        public bool SetPixel(int x, int y, Pixel p)
        {
            if (x < Width && y < Height)
            {
                ColorData[y * Width + x] = p;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fills a rect within this sprite with a given pixel.
        /// </summary>
        /// <param name="x">Left x coordinate of rectangle in sprite to start filling at</param>
        /// <param name="y">Upper y coordinate of rectangle in sprite to start filling at</param>
        /// <param name="width">Width of rectangle to fill</param>
        /// <param name="height">Height of rectangle to fill</param>
        /// <param name="p">Pixel value to fill with</param>
        public void FillRect(int x, int y, int width, int height, Pixel p)
        {
            int x2 = x + width;
            int y2 = y + height;

            // TODO: Write unit tests for below...its kinda suspect
            if (x >= Width)
                x = (Width - 1);
            if (y >= Height)
                y = (Height - 1);
            if (x2 >= Width)
                x2 = (Width - 1);
            if (y2 >= Height)
                y2 = (Height - 1);

            for (int y1 = y; y1 < y2; y1++)
            {
                Span<Pixel> row = new Span<Pixel>(ColorData, y1 * Width + x, x2 - x);
                row.Fill(p);
            }

            //for (uint x1 = x; x1 < x2; x1++)
            //{
            //    for (uint y1 = y; y1 < y2; y1++)
            //    {
            //        ColorData[y1 * Width + x1] = p;
            //    }
            //}
        }

        public Pixel Sample(float x, float y)
        {
            int sx = Math.Min((int)(x * Width), Width - 1);
            int sy = Math.Min((int)(y * Height), Height - 1);
            return GetPixel(sx, sy);
        }

        public Pixel SampleBL(float u, float v)
        {
            u = u * Width - 0.5f;
            v = v * Height - 0.5f;
            int y = (int)Math.Floor(v); // Thanks @joshinils
            int x = (int)Math.Floor(u); // cast to int rounds toward zero, not downward
            float u_ratio = u - x;
            float v_ratio = v - y;
            float u_opposite = 1 - u_ratio;
            float v_opposite = 1 - v_ratio;

            Pixel p1 = GetPixel(Math.Max(x, 0), Math.Max(y, 0));
            Pixel p2 = GetPixel(Math.Min(x + 1, Width - 1), Math.Max(y, 0));
            Pixel p3 = GetPixel(Math.Max(x, 0), Math.Min(y + 1, Height - 1));
            Pixel p4 = GetPixel(Math.Min(x + 1, Width - 1), Math.Min(y + 1, Height - 1));

            return new Pixel(
                (byte)((p1.r * u_opposite + p2.r * u_ratio) * v_opposite + (p3.r * u_opposite + p4.r * u_ratio) * v_ratio),
                (byte)((p1.g * u_opposite + p2.g * u_ratio) * v_opposite + (p3.g * u_opposite + p4.g * u_ratio) * v_ratio),
                (byte)((p1.b * u_opposite + p2.b * u_ratio) * v_opposite + (p3.b * u_opposite + p4.b * u_ratio) * v_ratio));
        }

        /// <summary>
        /// If you need to do a fast copy of sprites, use this. Note that it cannot
        /// utilize pixel blending, so this is best used for backgrounds or sprites
        /// that do not utilize a mask.
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src_x"></param>
        /// <param name="src_y"></param>
        /// <param name="dst_x"></param>
        /// <param name="dst_y"></param>
        public void CopyTo(Sprite dest, int src_x, int src_y, int dst_x, int dst_y)
        {
            // Check that src and dst positions are valid
            if (src_x >= Width) return;
            if (src_y >= Height) return;
            if (dst_x >= dest.Width) return;
            if (dst_y >= dest.Height) return;
            if ((dst_x + dest.Width) <= 0) return;
            if ((dst_y + dest.Height) <= 0) return;

            if (dst_x < 0)
            {
                src_x += Math.Abs(dst_x);
                dst_x = 0;
            }
            if (dst_y < 0)
            {
                src_y += Math.Abs(dst_y);
                dst_y = 0;
            }

            int w = Math.Min((Width - src_x) - 1, (dest.Width - dst_x) - 1);
            int h = Math.Min((Height - src_y) - 1, (dest.Height - dst_y) - 1);

            for (int sy = src_y, dy = dst_y; sy < (src_y + h); sy++, dy++)
            {
                Memory<Pixel> srcPixelsRow = new Memory<Pixel>(ColorData, Width * sy + src_x, w);
                Memory<Pixel> dstPixelsRow = new Memory<Pixel>(dest.ColorData, dest.Width * dy + dst_x, w);
                srcPixelsRow.CopyTo(dstPixelsRow);
            }
        }
    }
}
