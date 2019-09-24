using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngine.enums;

namespace csPixelGameEngine
{
    public class Sprite
    {
        public uint Width   { get; private set; }
        public uint Height  { get; private set; }

        public enum Mode { NORMAL, PERIODIC };

        public static int OverdrawCount;

        public Pixel[] colorData { get; private set; }
        private Mode modeSample = Mode.NORMAL;

        public Sprite()
        {
            Width = Height = 0;
        }

        public Sprite(string imageFile)
        {
            LoadFromFile(imageFile);
        }

        public Sprite(string imageFile, ResourcePack pack)
        {
            LoadFromPGESprFile(imageFile, pack);
        }

        public Sprite(uint w, uint h)
        {
            this.Width = w;
            this.Height = h;
            colorData = new Pixel[this.Width * this.Height];

            for (int i = 0; i < Width * Height; i++)
            {
                colorData[i] = new Pixel();
            }
        }

        public rcode LoadFromFile(string imageFile, ResourcePack pack = null)
        {
            return rcode.OK;
        }

        public rcode LoadFromPGESprFile(string imageFile, ResourcePack pack = null)
        {
            return rcode.OK;
        }

        public rcode SaveToPGESprFile(string imageFile)
        {
            return rcode.OK;
        }

        public void SetSampleMode(Mode mode = Mode.NORMAL)
        {
            this.modeSample = mode;
        }

        public Pixel GetPixel(uint x, uint y)
        {
            throw new NotImplementedException();
        }

        public bool SetPixel(uint x, uint y, Pixel p)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                colorData[y * Width + x] = p;
                return true;
            }
            else
                return false;
        }

        public Pixel Sample(float x, float y)
        {
            uint sx = Math.Min((uint)((x * Width)), Width - 1);
            uint sy = Math.Min((uint)((y * Height)), Height - 1);
            return GetPixel(sx, sy);
        }

        public Pixel SampleBL(float u, float v)
        {
            u = u * Width - 0.5f;
            v = v * Height - 0.5f;
            uint y = (uint)Math.Floor(v); // Thanks @joshinils
            uint x = (uint)Math.Floor(u); // cast to int rounds toward zero, not downward
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

    }
}
