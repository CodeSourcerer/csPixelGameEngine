using System;

namespace csPixelGameEngineCore;

public struct vec3d_f : Ivec3d<float>
{
    public float this[int i]
    {
        get => i switch
        {
            0 => x,
            1 => y,
            2 => z,
            _ => throw new IndexOutOfRangeException("Index must be between 0 and 2")
        };
        set
        {
            switch (i)
            {
                case 0:
                    x = value;
                    break;
                case 1:
                    y = value;
                    break;
                case 2:
                    z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Index must be between 0 and 2");
            }
        }
    }

    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public vec3d_f(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Ivec3d<float> cross(Ivec3d<float> rhs) =>
        new vec3d_f(y * rhs.z - z * rhs.y, z * rhs.x - x * rhs.z, x * rhs.y - y * rhs.x);

    public float dot(Ivec3d<float> rhs) => x * rhs.x + y * rhs.y + z * rhs.z;

    public float mag() => (float)Math.Sqrt(x * x + y * y + z * z);

    public float mag2() => x * x + y * y + z * z;

    public Ivec3d<float> norm()

    {
        float r = 1.0f / mag();
        return new vec3d_f(x * r, y * r, z * r);
    }

    public static vec3d_f operator +(vec3d_f lhs, vec3d_f rhs) => new vec3d_f(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
    public static vec3d_f operator -(vec3d_f lhs, vec3d_f rhs) => new vec3d_f(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
    public static vec3d_f operator *(vec3d_f lhs, float rhs) => new vec3d_f(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
    public static vec3d_f operator *(float lhs, vec3d_f rhs) => new vec3d_f(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
    public static vec3d_f operator /(vec3d_f lhs, float rhs) => new vec3d_f(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
    public static vec3d_f operator /(float lhs, vec3d_f rhs) => new vec3d_f(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
}
