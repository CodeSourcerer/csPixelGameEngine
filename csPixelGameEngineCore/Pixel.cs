using System;

namespace csPixelGameEngineCore;

/// <summary>
/// Represents a 32-bit RGBA color
/// </summary>
/// <remarks>
/// In the C++ PGE, you can default the Pixel RGBA component values to anything.
/// C#, however, does not allow this. If you do not specify a component value, it will be 0.
/// </remarks>
public struct Pixel
{
    #region Predefined colors
    public static readonly Pixel WHITE               = new Pixel(255, 255, 255);
    public static readonly Pixel GREY                = new Pixel(192, 192, 192);
    public static readonly Pixel DARK_GREY           = new Pixel(128, 128, 128);
    public static readonly Pixel VERY_DARK_GREY      = new Pixel(64, 64, 64);
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

    public enum Mode { NORMAL, MASK, ALPHA, CUSTOM };

    // C# Does not have unions, but we can sorta fake it.
    // Note: This is my original implementation. My current one is a bit faster. Just leaving this here
    // for later reference.

    // We'll make an internal r,g,b struct like PGE does and 'n' can just combine all of the
    // properties to a single uint. If there's a better way, I'm all ears.

    //private struct rgba
    //{
    //    public byte r;
    //    public byte g;
    //    public byte b;
    //    public byte a;
    //}

    //private rgba _rgba;

    //public uint n
    //{
    //    get => (uint)((_rgba.r << 24) | (_rgba.g << 16) | (_rgba.b << 8) | (_rgba.a));
    //    set
    //    {
    //        _rgba.r = (byte)((value & 0xFF000000) >> 24);
    //        _rgba.g = (byte)((value & 0x00FF0000) >> 16);
    //        _rgba.b = (byte)((value & 0x0000FF00) >> 8);
    //        _rgba.a = (byte)(value & 0x000000FF);
    //    }
    //}

    //public byte r
    //{
    //    get => _rgba.r;
    //    set => _rgba.r = value;
    //}
    //public byte g
    //{
    //    get => _rgba.g;
    //    set => _rgba.g = value;
    //}
    //public byte b
    //{
    //    get => _rgba.b;
    //    set => _rgba.b = value;
    //}
    //public byte a
    //{
    //    get => _rgba.a;
    //    set => _rgba.a = value;
    //}

    public uint n;

    public byte r
    {
        get => (byte)(n & 0x000000FF);
        set => n = (n & 0xFFFFFF00) | value;
    }
    public byte g
    {
        get => (byte)((n & 0x0000FF00) >> 8);
        set => n = (n & 0xFFFF00FF) | (uint)(value << 8);
    }
    public byte b
    {
        get => (byte)((n & 0x00FF0000) >> 16);
        set => n = (n & 0xFF00FFFF) | ((uint)value << 16);
    }
    public byte a
    {
        get => (byte)((n & 0xFF000000) >> 24);
        set => n = (n & 0x00FFFFFF) | ((uint)value << 24);
    }

    public Pixel(byte r = 0, byte g = 0, byte b = 0, byte a = 255)
    {
        n = (uint)(r | (g << 8) | (b << 16) | (a << 24));
        //this.r = r;
        //this.g = g;
        //this.b = b;
        //this.a = a;
    }

    public Pixel(uint p)
    {
        n = p;
    }

    public Pixel inv()
    {
        byte nR = (byte)Math.Min(255, Math.Max(0, 255 - r));
        byte nG = (byte)Math.Min(255, Math.Max(0, 255 - g));
        byte nB = (byte)Math.Min(255, Math.Max(0, 255 - b));
        return new Pixel(nR, nG, nB, a);
    }

    public Pixel PixelF(float red, float green, float blue, float alpha) =>
        new Pixel((byte)(red * 255.0f), (byte)(green * 255.0f), (byte)(blue * 255.0f), (byte)(alpha * 255.0f));

    public Pixel PixelLerp(Pixel p1, Pixel p2, float t) => (p2 * t) + p1 * (1.0f - t);

    public bool Equals(Pixel other) => n == other.n;

    public override bool Equals(object obj)
    {
        if (obj is Pixel)
            return Equals((Pixel)obj);

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode() ^ a.GetHashCode();
    }

    #region Operator overloads

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

    public static Pixel operator *(Pixel lhs, float i)
    {
        float fR = Math.Min(255.0f, Math.Max(0.0f, lhs.r * i));
        float fG = Math.Min(255.0f, Math.Max(0.0f, lhs.g * i));
        float fB = Math.Min(255.0f, Math.Max(0.0f, lhs.b * i));
        return new Pixel((byte)fR, (byte)fG, (byte)fB);
    }

    public static Pixel operator *(Pixel lhs, Pixel rhs)
    {
        byte nR = (byte)Math.Min(255.0f, Math.Max(0.0f, lhs.r * rhs.r / 255.0f));
        byte nG = (byte)Math.Min(255.0f, Math.Max(0.0f, lhs.g * rhs.g / 255.0f));
        byte nB = (byte)Math.Min(255.0f, Math.Max(0.0f, lhs.b * rhs.b / 255.0f));
        byte nA = (byte)Math.Min(255.0f, Math.Max(0.0f, lhs.a * rhs.a / 255.0f));
        return new Pixel(nR, nG, nB);
    }

    public static Pixel operator /(Pixel lhs, float i)
    {
        float fR = Math.Min(255.0f, Math.Max(0.0f, lhs.r / i));
        float fG = Math.Min(255.0f, Math.Max(0.0f, lhs.g / i));
        float fB = Math.Min(255.0f, Math.Max(0.0f, lhs.b / i));
        return new Pixel((byte)fR, (byte)fG, (byte)fB);
    }

    public static Pixel operator +(Pixel lhs, Pixel p)
    {
        byte fR = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.r + p.r)));
        byte fG = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.g + p.g)));
        byte fB = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.b + p.b)));
        return new Pixel(fR, fG, fB);
    }

    public static Pixel operator -(Pixel lhs, Pixel p)
    {
        byte fR = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.r - p.r)));
        byte fG = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.g - p.g)));
        byte fB = Math.Min((byte)255, Math.Max((byte)0, (byte)(lhs.b - p.b)));
        return new Pixel(fR, fG, fB);
    }

    public static implicit operator uint(Pixel p) => p.n;
    public static implicit operator Pixel(uint p) => new Pixel(p);
    
    #endregion // Operator overloads
}
