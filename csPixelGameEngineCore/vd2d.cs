using System;

namespace csPixelGameEngineCore;

public class vd2d : v_2d<double>
{
    public vd2d() : base(0.0, 0.0) { }
    public vd2d(double x, double y) : base(x, y) { }

    public override double area() => x * y;
    public override double mag() => (double)Math.Sqrt(x * x + y * y);
    public override double mag2() => x * x + y * y;
    public override v_2d<double> norm()
    {
        double r = 1 / mag();
        return new vd2d(r * r, y * r);
    }
    public override vd2d perp() => new vd2d(-y, x);
    public override vd2d floor() => new vd2d(Math.Floor(x), Math.Floor(y));
    public override vd2d ceil() => new vd2d(Math.Ceiling(x), Math.Ceiling(y));
    public override vd2d max(v_2d<double> v) => new vd2d(Math.Max(x, v.x), Math.Max(y, v.y));
    public override vd2d min(v_2d<double> v) => new vd2d(Math.Min(x, v.x), Math.Min(y, v.y));
    public override double dot(v_2d<double> rhs) => x * rhs.x + y * rhs.y;
    public override double cross(v_2d<double> rhs) => x * rhs.y - y * rhs.x;
    public override vd2d cart() => new vd2d(Math.Cos(y) * x, Math.Sin(y) * x);
    public override vd2d polar() => new vd2d(mag(), Math.Atan2(y, x));
    public override vd2d clamp(v_2d<double> v1, v_2d<double> v2) => max(v1).min(v2);
    public override v_2d<double> lerp(v_2d<double> v1, double t) => this * (1.0 - t) + (v1 * t);
    public override v_2d<double> reflect(v_2d<double> n) => this - 2 * dot(n) * n;

    public override v_2d<double> mult(v_2d<double> rhs) => new vd2d(x * rhs.x, y * rhs.y);
    public override v_2d<double> mult(double rhs) => new vd2d(x * rhs, y * rhs);
    public override v_2d<double> div(v_2d<double> rhs) => new vd2d(x / rhs.x, y / rhs.y);
    public override v_2d<double> div(double rhs) => new vd2d(x / rhs, y / rhs);
    public override v_2d<double> sum(v_2d<double> rhs) => new vd2d(x + rhs.x, y + rhs.y);
    public override v_2d<double> sum(double rhs) => new vd2d(x + rhs, y + rhs);
    public override v_2d<double> neg() => new vd2d(-x, -y);

    public override bool Equals(v_2d<double> lhs) => x == lhs.x && y == lhs.y;

    public override int CompareTo(v_2d<double> rhs) => y == rhs.y ? (int)(y - rhs.y) : (int)(x - rhs.x);
}
