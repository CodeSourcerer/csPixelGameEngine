using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public interface Ivec3d<T>
    {
        T x { get; set; }
        T y { get; set; }
        T z { get; set; }

        T mag();
        T mag2();
        Ivec3d<T> norm();
        T dot(Ivec3d<T> rhs);
        Ivec3d<T> cross(Ivec3d<T> rhs);

        public T this[int i] { get; set; }
    }
}
