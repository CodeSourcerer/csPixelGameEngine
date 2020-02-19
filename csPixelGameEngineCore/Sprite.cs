using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// A 2D drawing surface.
    /// </summary>
    public class Sprite
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public Pixel[] ColorData { get; private set; }

        private Sprite()
        {
            // Don't allow creation of sprites without dimensions
        }
        
        public Sprite(uint w, uint h)
        {
            if (w == 0)
                throw new ArgumentException("Argument must be greater than 0", nameof(w));
            if (h == 0)
                throw new ArgumentException("Argument must be greater than 0", nameof(h));

            Width = w;
            Height = h;
            ColorData = new Pixel[Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                ColorData[i] = Pixel.BLACK;
            }
        }

        public Pixel GetPixel(uint x, uint y)
        {
            if (x < Width && y < Height)
                return ColorData[y * Width + x];
            else
                return Pixel.BLANK;
        }

        public bool SetPixel(uint x, uint y, Pixel p)
        {
            if (x < Width && y < Height)
            {
                ColorData[y * Width + x] = p;
                return true;
            }

            return false;
        }

        public void FillRect(uint x, uint y, uint width, uint height, Pixel p)
        {
            if (p == default)
                p = Pixel.WHITE;

            uint x2 = x + width;
            uint y2 = y + height;

            if (x > Width)
                x = Width;
            if (y > Height)
                y = Height;
            if (x2 >= Width)
                x2 = Width;
            if (y2 >= Height)
                y2 = Height;

            for (uint x1 = x; x1 < x2; x1++)
            {
                for (uint y1 = y; y1 < y2; y1++)
                {
                    ColorData[y1 * Width + x1] = p;
                }
            }
        }
    }
}
