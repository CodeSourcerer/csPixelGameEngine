using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public interface Ivec2d<T>
    {
        float x { get; set; }
        float y { get; set; }

        T mag();
        T mag2();
        Ivec2d<T> norm();
        Ivec2d<T> perp();
        T dot(Ivec2d<T> rhs);
        T cross(Ivec2d<T> rhs);

        public T this[int i] { get; set; }
    }
}
