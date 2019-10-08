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

        public uint color { get; set; }

        public byte r
        {
            get { return (byte)(color >> 24); }
            set { this.color = (this.color & 0x00FFFFFF) | (uint)(value << 24); }
        }
        public byte g
        {
            get { return (byte)((color & 0x00FF0000) >> 16); }
            set { this.color = (this.color & 0xFF00FFFF) | (uint)(value << 16); }
        }
        public byte b
        {
            get { return (byte)((this.color & 0x0000FF00) >> 8); }
            set { this.color = (this.color & 0xFFFF00FF) | (uint)(value << 8); }
        }
        public byte a
        {
            get { return (byte)(this.color & 0x00000000FF); }
            set { this.color = (this.color & 0xFFFFFF00) | value; }
        }

        public Pixel(byte r = 0, byte g = 0, byte b = 0, byte a = 255)
        {
            this.color = (uint)((r << 24) | (g << 16) | (b << 8) | a);
        }

        public Pixel(uint p)
        {
            this.color = p;
            //this.r = (byte)(p >> 24);
            //this.g = (byte)((p & 0x00FF0000) >> 16);
            //this.b = (byte)((p & 0x0000FF00) >> 8);
            //this.a = (byte)(p & 0x000000FF);
        }

        public bool Equals(Pixel other)
        {
            return (this.color == other.color);
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
