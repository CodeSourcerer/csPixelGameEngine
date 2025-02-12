using System;

namespace csPixelGameEngineCore;

public class vf2d : v_2d<float>
{
    public vf2d() : base(0.0f, 0.0f) { }
    public vf2d(float x, float y) : base(x, y) { }

    public override float area() => x * y;
    public override float mag() => (float)Math.Sqrt(x * x + y * y);
    public override float mag2() => x * x + y * y;
    public override v_2d<float> norm()
    {
        float r = 1 / mag();
        return new vf2d(r * r, y * r);
    }
    public override vf2d perp() => new vf2d(-y, x);
    public override vf2d floor() => new vf2d((float)Math.Floor(x), (float)Math.Floor(y));
    public override vf2d ceil() => new vf2d((float)Math.Ceiling(x), (float)Math.Ceiling(y));
    public override vf2d max(v_2d<float> v) => new vf2d(Math.Max(x, v.x), Math.Max(y, v.y));
    public override vf2d min(v_2d<float> v) => new vf2d(Math.Min(x, v.x), Math.Min(y, v.y));
    public override float dot(v_2d<float> rhs) => x * rhs.x + y * rhs.y;
    public override float cross(v_2d<float> rhs) => x * rhs.y - y * rhs.x;
    public override vf2d cart() => new vf2d((float)(Math.Cos(y) * x), (float)(Math.Sin(y) * x));
    public override vf2d polar() => new vf2d(mag(), (float)Math.Atan2(y, x));
    public override vf2d clamp(v_2d<float> v1, v_2d<float> v2) => max(v1).min(v2);
    public override v_2d<float> lerp(v_2d<float> v1, double t) => this * ((float)(1.0 - t)) + (v1 * (float)t);
    public override v_2d<float> reflect(v_2d<float> n) => this - 2 * dot(n) * n;

    public override v_2d<float> mult(v_2d<float> rhs) => new vf2d(x * rhs.x, y * rhs.y);
    public override v_2d<float> mult(float rhs) => new vf2d(x * rhs, y * rhs);
    public override v_2d<float> div(v_2d<float> rhs) => new vf2d(x / rhs.x, y / rhs.y);
    public override v_2d<float> div(float rhs) => new vf2d(x / rhs, y / rhs);
    public override v_2d<float> sum(v_2d<float> rhs) => new vf2d(x + rhs.x, y + rhs.y);
    public override v_2d<float> sum(float rhs) => new vf2d(x + rhs, y + rhs);
    public override v_2d<float> neg() => new vf2d(-x, -y);

    public override bool Equals(v_2d<float> lhs) => x == lhs.x && y == lhs.y;

    public override int CompareTo(v_2d<float> rhs) => y == rhs.y ? (int)(y - rhs.y) : (int)(x - rhs.x);
}
