using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// Floating-point version of a 2D vector
    /// </summary>
    /// <remarks>
    /// This is basically the vf2d typedef. 
    /// </remarks>
    public struct vec2d_f : Ivec2d<float>
    {
        public static readonly vec2d_f ZERO = new vec2d_f(0.0f, 0.0f);
        public static readonly vec2d_f UNIT = new vec2d_f(1.0f, 1.0f);

        public float x { get; set; }
        public float y { get; set; }

        public vec2d_f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public vec2d_f(Ivec2d<float> v)
            : this(v.x, v.y)
        { }

        public float mag() => (float)Math.Sqrt(x * x + y * y);

        public float mag2() => x * x + y * y;

        public Ivec2d<float> norm()
        {
            float r = 1 / mag();
            return new vec2d_f(x * r, y * r);
        }

        public Ivec2d<float> perp() => new vec2d_f(-y, x);

        public float dot(Ivec2d<float> rhs) => x * rhs.x + y * rhs.y;

        public float cross(Ivec2d<float> rhs) => x * rhs.y - y * rhs.x;

        public static vec2d_f operator +(vec2d_f lhs, vec2d_f rhs) => new vec2d_f(lhs.x + rhs.x, lhs.y + rhs.y);
        public static vec2d_f operator -(vec2d_f lhs, vec2d_f rhs) => new vec2d_f(lhs.x - rhs.x, lhs.y - rhs.y);
        public static vec2d_f operator *(vec2d_f lhs, vec2d_f rhs) => new vec2d_f(lhs.x * rhs.x, lhs.y * rhs.y);
        public static vec2d_f operator *(vec2d_f lhs, float rhs) => new vec2d_f(lhs.x * rhs, lhs.y * rhs);
        public static vec2d_f operator *(float lhs, vec2d_f rhs) => new vec2d_f(lhs * rhs.x, lhs * rhs.y);
        public static vec2d_f operator /(vec2d_f lhs, float rhs) => new vec2d_f(lhs.x / rhs, lhs.y / rhs);
        public static vec2d_f operator /(float lhs, vec2d_f rhs) => new vec2d_f(lhs / rhs.x, lhs / rhs.y);

        public float this[int i]
        {
            get
            {
                if (i == 0)
                    return x;
                if (i == 1)
                    return y;
                throw new IndexOutOfRangeException("Index must be 0 or 1");
            }

            set
            {
                if (i == 0)
                    x = value;
                else if (i == 1)
                    y = value;
                else
                    throw new IndexOutOfRangeException("Index must be 0 or 1");
            }
        }
    }
}
