using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngine
{
    public class Triangle
    {
        public vec3d[] p { get; private set; }
        public vec2d[] t { get; private set; }
        public Pixel col { get; private set; }

        public Triangle()
        {
            this.p = new vec3d[3];
            this.t = new vec2d[3];
            this.col = new Pixel();
        }
    }
}
