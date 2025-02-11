using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore;

public abstract class v2d_generic<T> where T : struct
{
    protected T x = default(T);
    protected T y = default(T);

    public v2d_generic(T _x, T _y)
    {
        x = _x;
        y = _y;
    }

    public v2d_generic(v2d_generic<T> v)
    {
        x = v.x;
        y = v.y;
    }

    public abstract T mag();
    public abstract T mag2();
    public abstract v2d_generic<T> norm();
    public abstract v2d_generic<T> perp();
}
