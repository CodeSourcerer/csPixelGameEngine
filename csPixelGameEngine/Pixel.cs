using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngine
{
    public class Pixel
    {
        public byte r { get; set; }
        public byte g { get; set; }
        public byte b { get; set; }
        public byte a { get; set; }

        public Pixel() :
            this(0, 0, 0, 255)
        {
            
        }

        public Pixel(byte r, byte g, byte b, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Pixel(int p)
        {
            this.r = (byte)(p >> 24);
            this.g = (byte)((p & 0x00FF0000) >> 16);
            this.b = (byte)((p & 0x0000FF00) >> 8);
            this.a = (byte)(p & 0x000000FF);
        }
    }
}
