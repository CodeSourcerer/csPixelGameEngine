using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngine
{
    public enum BlendMode { NORMAL, MASK, ALPHA, CUSTOM };

    public struct Pixel
    {
        public static Pixel WHITE   = new Pixel(255, 255, 255);
        public static Pixel GRAY    = new Pixel(192, 192, 192);
        public static Pixel RED     = new Pixel(255, 0, 0);
        public static Pixel YELLOW  = new Pixel(255, 255, 0);
        public static Pixel GREEN   = new Pixel(0, 255, 0);
        public static Pixel CYAN    = new Pixel(0, 255, 255);
        public static Pixel BLUE    = new Pixel(0, 0, 255);
        public static Pixel MAGENTA = new Pixel(255, 0, 255);
        public static Pixel BLACK   = new Pixel(0, 0, 0);
        public static Pixel BLANK   = new Pixel(0, 0, 0, 0);

        public byte r { get; set; }
        public byte g { get; set; }
        public byte b { get; set; }
        public byte a { get; set; }

        public Pixel(byte r = 0, byte g = 0, byte b = 0, byte a = 255)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Pixel(uint p)
        {
            this.r = (byte)(p >> 24);
            this.g = (byte)((p & 0x00FF0000) >> 16);
            this.b = (byte)((p & 0x0000FF00) >> 8);
            this.a = (byte)(p & 0x000000FF);
        }

        public bool Equals(Pixel other)
        {
            return (this.r == other.r &&
                    this.g == other.g &&
                    this.b == other.b &&
                    this.a == other.a);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Pixel p1, Pixel p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Pixel p1, Pixel p2)
        {
            return !p1.Equals(p2);
        }
    }
}
