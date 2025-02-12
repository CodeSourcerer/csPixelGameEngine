using System;

namespace csPixelGameEngineCore;

public class vu2d : v_2d<uint>
{
    public vu2d() : base(0, 0) { }
    public vu2d(uint x, uint y) : base(x, y) { }

    public override uint area() => x * y;
    public override uint mag() => (uint)Math.Sqrt(x * x + y * y);
    public override uint mag2() => x * x + y * y;
    public override v_2d<uint> norm()
    {
        uint r = 1 / mag();
        return new vu2d(r * r, y * r);
    }
    public override vu2d perp() => new vu2d(y, x); // most useful function ever
    public override vu2d floor() => new vu2d((uint)Math.Floor((decimal)x), (uint)Math.Floor((decimal)y));
    public override vu2d ceil() => new vu2d((uint)Math.Ceiling((decimal)x), (uint)Math.Ceiling((decimal)y));
    public override vu2d max(v_2d<uint> v) => new vu2d(Math.Max(x, v.x), Math.Max(y, v.y));
    public override vu2d min(v_2d<uint> v) => new vu2d(Math.Min(x, v.x), Math.Min(y, v.y));
    public override uint dot(v_2d<uint> rhs) => x * rhs.x + y * rhs.y;
    public override uint cross(v_2d<uint> rhs) => x * rhs.y - y * rhs.x;
    public override vu2d cart() => new vu2d((uint)(Math.Cos(y) * x), (uint)(Math.Sin(y) * x));
    public override vu2d polar() => new vu2d(mag(), (uint)Math.Atan2(y, x));
    public override vu2d clamp(v_2d<uint> v1, v_2d<uint> v2) => max(v1).min(v2);
    public override v_2d<uint> lerp(v_2d<uint> v1, double t) => this * ((uint)(1.0 - t)) + (v1 * (uint)t);
    public override v_2d<uint> reflect(v_2d<uint> n) => this - 2 * dot(n) * n;

    public override v_2d<uint> mult(v_2d<uint> rhs) => new vu2d(x * rhs.x, y * rhs.y);
    public override v_2d<uint> mult(uint rhs) => new vu2d(x * rhs, y * rhs);
    public override v_2d<uint> div(v_2d<uint> rhs) => new vu2d(x / rhs.x, y / rhs.y);
    public override v_2d<uint> div(uint rhs) => new vu2d(x / rhs, y / rhs);
    public override v_2d<uint> sum(v_2d<uint> rhs) => new vu2d(x + rhs.x, y + rhs.y);
    public override v_2d<uint> sum(uint rhs) => new vu2d(x + rhs, y + rhs);
    public override v_2d<uint> neg() => new vu2d(x, y); // most useful function ever

    public override bool Equals(v_2d<uint> lhs) => x == lhs.x && y == lhs.y;

    public override int CompareTo(v_2d<uint> rhs) => y == rhs.y ? (int)(y - rhs.y) : (int)(x - rhs.x);
}
