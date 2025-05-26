using System;

namespace csPixelGameEngineCore;

public class vf2d : v_2d<float>
{
    public vf2d() : base(0.0f, 0.0f) { }
    public vf2d(float x, float y) : base(x, y) { }
    public vf2d(v_2d<float> v) : base(v) { }

    public static implicit operator vf2d(vi2d v) => new vf2d(v.x, v.y);

    public static vf2d operator *(vf2d lhs, vf2d rhs)  => new(lhs.x * rhs.x, lhs.x * rhs.y);
    public static vf2d operator *(float lhs, vf2d rhs) => new(lhs * rhs.x, lhs * rhs.y);
    public static vf2d operator *(vf2d lhs, float rhs) => new(lhs.x * rhs, lhs.y * rhs);

    public static vf2d operator /(vf2d lhs, vf2d rhs)  => new(lhs.x / rhs.x, lhs.y / rhs.y);
    public static vf2d operator /(float lhs, vf2d rhs) => new(lhs / rhs.x, lhs / rhs.y);
    public static vf2d operator /(vf2d lhs, float rhs) => new(lhs.x / rhs, lhs.y / rhs);

    public static vf2d operator +(vf2d rhs) => new(+rhs.x, +rhs.y);
    public static vf2d operator +(vf2d lhs, vf2d rhs)  => new(lhs.x + rhs.x, lhs.y + rhs.y);
    public static vf2d operator +(float lhs, vf2d rhs) => new(lhs + rhs.x, lhs + rhs.y);
    public static vf2d operator +(vf2d lhs, float rhs) => new(lhs.x + rhs, lhs.y + rhs);

    public static vf2d operator -(vf2d rhs) => new(-rhs.x, -rhs.y);
    public static vf2d operator -(vf2d lhs, vf2d rhs)  => new(lhs.x - rhs.x, lhs.y - rhs.y);
    public static vf2d operator -(float lhs, vf2d rhs) => new(lhs - rhs.x, lhs - rhs.y);
    public static vf2d operator -(vf2d lhs, float rhs) => new(lhs.x - rhs, lhs.y - rhs);
}
