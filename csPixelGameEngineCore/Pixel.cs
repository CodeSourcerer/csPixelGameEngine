using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// Represents a 32-bit RGBA color
    /// </summary>
    /// <remarks>
    /// In the original PGE, you can default the Pixel RGBA component values to anything.
    /// C#, however, does not allow this. If you do not specify a component value, it will be 0.
    /// </remarks>
    public struct Pixel
    {
        #region Predefined colors
        public static readonly Pixel WHITE               = new Pixel(255, 255, 255);
        public static readonly Pixel GRAY                = new Pixel(192, 192, 192);
        public static readonly Pixel DARK_GRAY           = new Pixel(128, 128, 128);
        public static readonly Pixel VERY_DARK_GRAY      = new Pixel(64, 64, 64);
        public static readonly Pixel RED                 = new Pixel(255, 0, 0);
        public static readonly Pixel DARK_RED            = new Pixel(128, 0, 0);
        public static readonly Pixel VERY_DARK_RED       = new Pixel(64, 0, 0);
        public static readonly Pixel YELLOW              = new Pixel(255, 255, 0);
        public static readonly Pixel DARK_YELLOW         = new Pixel(128, 128, 0);
        public static readonly Pixel VERY_DARK_YELLOW    = new Pixel(64, 64, 0);
        public static readonly Pixel GREEN               = new Pixel(0, 255, 0);
        public static readonly Pixel DARK_GREEN          = new Pixel(0, 128, 0);
        public static readonly Pixel VERY_DARK_GREEN     = new Pixel(0, 64, 0);
        public static readonly Pixel CYAN                = new Pixel(0, 255, 255);
        public static readonly Pixel DARK_CYAN           = new Pixel(0, 128, 128);
        public static readonly Pixel VERY_DARK_CYAN      = new Pixel(0, 64, 64);
        public static readonly Pixel BLUE                = new Pixel(0, 0, 255);
        public static readonly Pixel DARK_BLUE           = new Pixel(0, 0, 128);
        public static readonly Pixel VERY_DARK_BLUE      = new Pixel(0, 0, 64);
        public static readonly Pixel MAGENTA             = new Pixel(255, 0, 255);
        public static readonly Pixel DARK_MAGENTA        = new Pixel(128, 0, 128);
        public static readonly Pixel VERY_DARK_MAGENTA   = new Pixel(64, 0, 64);
        public static readonly Pixel BLACK               = new Pixel(0, 0, 0);
        public static readonly Pixel BLANK               = new Pixel(0, 0, 0, 0);
        #endregion // Predefined colors

        public uint color { get; set; }

        public byte r
        {
            get => (byte)(color >> 24);
            set { color = (color & 0x00FFFFFF) | (uint)(value << 24); }
        }
        public byte g
        {
            get => (byte)((color & 0x00FF0000) >> 16);
            set { color = (color & 0xFF00FFFF) | (uint)(value << 16); }
        }
        public byte b
        {
            get => (byte)((color & 0x0000FF00) >> 8);
            set { color = (color & 0xFFFF00FF) | (uint)(value << 8); }
        }
        public byte a
        {
            get => (byte)(color & 0x00000000FF);
            set { color = (color & 0xFFFFFF00) | value; }
        }

        public Pixel(byte r = 0, byte g = 0, byte b = 0, byte a = 255)
        {
            color = (((uint)r << 24) | ((uint)g << 16) | ((uint)b << 8) | a);
        }

        public Pixel(uint p)
        {
            color = p;
        }

        public bool Equals(Pixel other)
        {
            return (color == other.color);
        }

        public override bool Equals(object obj)
        {
            if (obj is Pixel)
                return Equals((Pixel)obj);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Pixel p1, Pixel p2)
        {
            // Boxing necessary to prevent recursive calling of this operator
            if ((object)p1 == null && (object)p2 == null) return true;
            if ((object)p1 == null || (object)p2 == null) return false;

            return p1.Equals(p2);
        }

        public static bool operator !=(Pixel p1, Pixel p2)
        {
            // Boxing necessary to prevent recursive calling of this operator
            if ((object)p1 == null && (object)p2 == null) return false;
            if ((object)p1 == null || (object)p2 == null) return true;
            
            return !p1.Equals(p2);
        }

        public static implicit operator uint(Pixel p) => p.color;
        public static implicit operator Pixel(uint p) => new Pixel(p);
    }
}
