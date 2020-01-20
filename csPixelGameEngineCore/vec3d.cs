using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore
{
    public class vec3d
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public vec3d(float x = 0, float y = 0, float z = 0, float w = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}
