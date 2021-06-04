using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// A generic 2D vector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// There's some obvious variations here between the original PGE and this port, because C# uses generics and not
    /// templates. I took the liberty in defining the v2d_generic as Ivec2d to keep the spirit of what is trying to be
    /// accomplished, but in a more C# way. Implementing this as an abstract class was problematic because of the operator
    /// overloads done. I may spend more time on trying to get it working that way later.
    /// </remarks>
    public interface Ivec2d<T>
    {
        T x { get; set; }
        T y { get; set; }

        T mag();
        T mag2();
        Ivec2d<T> norm();
        Ivec2d<T> perp();
        T dot(Ivec2d<T> rhs);
        T cross(Ivec2d<T> rhs);

        public T this[int i] { get; set; }
    }
}
