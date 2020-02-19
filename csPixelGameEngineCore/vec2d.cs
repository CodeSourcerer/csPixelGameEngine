using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore
{
    public struct vec2d
    {
        public static vec2d Unit = new vec2d(0, 0, 1);

        public float u;
        public float v;
        public float w;

        public vec2d(float u = 0, float v = 0, float w = 1)
        {
            this.u = u;
            this.v = v;
            this.w = w;
        }
    }
}
