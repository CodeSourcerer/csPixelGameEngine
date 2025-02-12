using System;

namespace csPixelGameEngineCore;

public class vi2d : v_2d<int>
{
    public vi2d() : base(0, 0) { }
    public vi2d(int x, int y) : base(x, y) { }

    public override int area() => x * y;
    public override int mag() => (int)Math.Sqrt(x * x + y * y);
    public override int mag2() => x * x + y * y;
    public override v_2d<int> norm()
    {
        int r = 1 / mag();
        return new vi2d(r * r, y * r);
    }
    public override vi2d perp() => new vi2d(-y, x);
    public override vi2d floor() => new vi2d((int)Math.Floor((decimal)x), (int)Math.Floor((decimal)y));
    public override vi2d ceil() => new vi2d((int)Math.Ceiling((decimal)x), (int)Math.Ceiling((decimal)y));
    public override vi2d max(v_2d<int> v) => new vi2d(Math.Max(x, v.x), Math.Max(y, v.y));
    public override vi2d min(v_2d<int> v) => new vi2d(Math.Min(x, v.x), Math.Min(y, v.y));
    public override int dot(v_2d<int> rhs) => x * rhs.x + y * rhs.y;
    public override int cross(v_2d<int> rhs) => x * rhs.y - y * rhs.x;
    public override vi2d cart() => new vi2d((int)(Math.Cos(y) * x), (int)(Math.Sin(y) * x));
    public override vi2d polar() => new vi2d(mag(), (int)Math.Atan2(y, x));
    public override vi2d clamp(v_2d<int> v1, v_2d<int> v2) => max(v1).min(v2);
    public override v_2d<int> lerp(v_2d<int> v1, double t) => this * ((int)(1.0 - t)) + (v1 * (int)t);
    public override v_2d<int> reflect(v_2d<int> n) => this - 2 * dot(n) * n;

    public override v_2d<int> mult(v_2d<int> rhs) => new vi2d(x * rhs.x, y * rhs.y);
    public override v_2d<int> mult(int rhs) => new vi2d(x * rhs, y * rhs);
    public override v_2d<int> div(v_2d<int> rhs) => new vi2d(x / rhs.x, y / rhs.y);
    public override v_2d<int> div(int rhs) => new vi2d(x / rhs, y / rhs);
    public override v_2d<int> sum(v_2d<int> rhs) => new vi2d(x + rhs.x, y + rhs.y);
    public override v_2d<int> sum(int rhs) => new vi2d(x + rhs, y + rhs);
    public override v_2d<int> neg() => new vi2d(-x, -y);

    public override bool Equals(v_2d<int> lhs) => x == lhs.x && y == lhs.y;

    public override int CompareTo(v_2d<int> rhs) => y == rhs.y ? (y - rhs.y) : (x - rhs.x);
}
