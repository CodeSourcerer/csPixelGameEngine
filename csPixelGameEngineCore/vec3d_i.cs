using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public struct vec3d_i : Ivec3d<int>
    {
        public int this[int i]
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

        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }

        public vec3d_i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Ivec3d<int> cross(Ivec3d<int> rhs) => 
            new vec3d_i(y * rhs.z - z * rhs.y, z * rhs.x - x * rhs.z, x * rhs.y - y * rhs.x);

        public int dot(Ivec3d<int> rhs) => x * rhs.x + y * rhs.y + z * rhs.z;

        public int mag() => (int)Math.Sqrt(x * x + y * y + z * z);

        public int mag2() => x * x + y * y + z * z;

        public Ivec3d<int> norm()

        {
            int r = 1 / mag();
            return new vec3d_i(x * r, y * r, z * r);
        }

        public static vec3d_i operator +(vec3d_i lhs, vec3d_i rhs) => new vec3d_i(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        public static vec3d_i operator -(vec3d_i lhs, vec3d_i rhs) => new vec3d_i(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        public static vec3d_i operator *(vec3d_i lhs, int rhs) => new vec3d_i(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        public static vec3d_i operator *(int lhs, vec3d_i rhs) => new vec3d_i(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
        public static vec3d_i operator /(vec3d_i lhs, int rhs) => new vec3d_i(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        public static vec3d_i operator /(int lhs, vec3d_i rhs) => new vec3d_i(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
    }
}
