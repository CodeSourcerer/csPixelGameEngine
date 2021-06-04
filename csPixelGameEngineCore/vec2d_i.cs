﻿using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// Integer version of a 2D vector
    /// </summary>
    /// <remarks>
    /// This is basically the vi2d typedef. 
    /// </remarks>
    public struct vec2d_i : Ivec2d<int>
    {
        public int x { get; set; }
        public int y { get; set; }

        public vec2d_i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public vec2d_i(Ivec2d<int> v)
            : this(v.x, v.y)
        { }

        public readonly int mag() => (int)Math.Sqrt(x * x + y * y);

        public readonly int mag2() => x * x + y * y;

        public readonly Ivec2d<int> norm()
        {
            int r = 1 / mag();
            return new vec2d_i(x * r, y * r);
        }

        public readonly Ivec2d<int> perp() => new vec2d_i(-y, x);

        public readonly int dot(Ivec2d<int> rhs) => x * rhs.x + y * rhs.y;

        public readonly int cross(Ivec2d<int> rhs) => x * rhs.y - y * rhs.x;

        public static vec2d_i operator +(vec2d_i lhs, vec2d_i rhs) => new vec2d_i(lhs.x + rhs.x, lhs.y + rhs.y);
        public static vec2d_i operator -(vec2d_i lhs, vec2d_i rhs) => new vec2d_i(lhs.x - rhs.x, lhs.y - rhs.y);
        public static vec2d_i operator *(vec2d_i lhs, int rhs)     => new vec2d_i(lhs.x * rhs, lhs.y * rhs);
        public static vec2d_i operator *(int lhs, vec2d_i rhs)     => new vec2d_i(lhs * rhs.x, lhs * rhs.y);
        public static vec2d_f operator *(float lhs, vec2d_i rhs)   => new vec2d_f(lhs * rhs.x, lhs * rhs.y);
        public static vec2d_i operator /(vec2d_i lhs, int rhs)     => new vec2d_i(lhs.x / rhs, lhs.y / rhs);
        public static vec2d_i operator /(int lhs, vec2d_i rhs)     => new vec2d_i(lhs / rhs.x, lhs / rhs.y);
        public static vec2d_f operator /(float lhs, vec2d_i rhs)   => new vec2d_f(lhs / rhs.x, lhs / rhs.y);

        public int this[int i]
        {
            readonly get
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
