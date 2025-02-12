using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore;

public abstract class v_2d<T> : IEquatable<v_2d<T>>, IComparable<v_2d<T>> where T : struct
{
    public T x = default(T);
    public T y = default(T);

    /// <summary>
    /// Default constructor
    /// </summary>
    public v_2d() { }

    /// <summary>
    /// Specific constructor
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    public v_2d(T _x, T _y)
    {
        x = _x;
        y = _y;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="v"></param>
    public v_2d(v_2d<T> v)
    {
        x = v.x;
        y = v.y;
    }

    /// <summary>
    /// Returns rectangular area of vector
    /// </summary>
    /// <returns></returns>
    public abstract T area();

    /// <summary>
    /// Returns magnitude of vector
    /// </summary>
    /// <returns></returns>
    public abstract T mag();

    /// <summary>
    /// Returns magnitude squared of vector (useful for fast comparisons)
    /// </summary>
    /// <returns></returns>
    public abstract T mag2();

    /// <summary>
    /// Returns normalised version of vector
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> norm();

    /// <summary>
    /// Returns vector at 90 degrees to this one
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> perp();

    /// <summary>
    /// Rounds both components down
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> floor();

    /// <summary>
    /// Rounds both components up
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> ceil();

    /// <summary>
    /// Returns 'element-wise' max of this and another vector
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> max(v_2d<T> v);

    /// <summary>
    /// Returns 'element-wise' min of this and another vector
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> min(v_2d<T> v);

    /// <summary>
    /// Calculates scalar dot product between this and another vector
    /// </summary>
    /// <returns></returns>
    public abstract T dot(v_2d<T> rhs);

    /// <summary>
    /// Calculates 'scalar' cross product between this and another vector (useful for winding orders)
    /// </summary>
    /// <returns></returns>
    public abstract T cross(v_2d<T> rhs);

    /// <summary>
    /// Treat this as polar coordinate (R, Theta), return cartesian equivalent (X, Y)
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> cart();

    /// <summary>
    /// Treat this as cartesian coordinate (X, Y), return polar equivalent (R, Theta)
    /// </summary>
    /// <returns></returns>
    public abstract v_2d<T> polar();

    /// <summary>
    /// Clamp the components of this vector in between the 'element-wise' minimum and maximum of 2 other vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public abstract v_2d<T> clamp(v_2d<T> v1, v_2d<T> v2);

    /// <summary>
    /// Linearly interpolate between this vector, and another vector, given normalised parameter 't'
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public abstract v_2d<T> lerp(v_2d<T> v1, double t);

    /// <summary>
    /// Assuming this vector is incident, given a normal, return the reflection
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public abstract v_2d<T> reflect(v_2d<T> n);

    public abstract v_2d<T> mult(v_2d<T> rhs);
    public abstract v_2d<T> mult(T rhs);
    public abstract v_2d<T> div(v_2d<T> rhs);
    public abstract v_2d<T> div(T rhs);
    public abstract v_2d<T> sum(v_2d<T> rhs);
    public abstract v_2d<T> sum(T rhs);
    public abstract v_2d<T> neg();

    /// <summary>
    /// Compare if this vector is numerically equal to another
    /// </summary>
    /// <param name="lhs"></param>
    /// <returns></returns>
    public abstract bool Equals(v_2d<T> lhs);

    public override bool Equals(object lhs) => Equals(lhs as v_2d<T>);

    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    public abstract int CompareTo(v_2d<T> rhs);

    /// <summary>
    /// Compare if this vector is numerically equal to another
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator ==(v_2d<T> lhs, v_2d<T> rhs) => lhs.Equals(rhs);

    /// <summary>
    /// Compare if this vector is not numerically equal to another
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator !=(v_2d<T> lhs, v_2d<T> rhs) => !lhs.Equals(rhs);

    public static v_2d<T> operator *(v_2d<T> lhs, v_2d<T> rhs) => lhs.mult(rhs);
    public static v_2d<T> operator *(T lhs, v_2d<T> rhs) => rhs.mult(lhs);
    public static v_2d<T> operator *(v_2d<T> lhs, T rhs) => lhs.mult(rhs);

    public static v_2d<T> operator /(v_2d<T> lhs, v_2d<T> rhs) => lhs.div(rhs);
    public static v_2d<T> operator /(T lhs, v_2d<T> rhs) => rhs.div(lhs);
    public static v_2d<T> operator /(v_2d<T> lhs, T rhs) => lhs.div(rhs);

    public static v_2d<T> operator +(v_2d<T> lhs, v_2d<T> rhs) => lhs.sum(rhs);
    public static v_2d<T> operator +(T lhs, v_2d<T> rhs) => rhs.sum(lhs);
    public static v_2d<T> operator +(v_2d<T> lhs, T rhs) => lhs.sum(rhs);

    public static v_2d<T> operator -(v_2d<T> rhs) => rhs.neg();
    public static v_2d<T> operator -(v_2d<T> lhs, v_2d<T> rhs) => lhs.sum(-rhs);
    public static v_2d<T> operator -(T lhs, v_2d<T> rhs) => rhs.neg().sum(lhs);
    public static v_2d<T> operator -(v_2d<T> lhs, T rhs) => lhs.neg().sum(rhs);

    public static bool operator <(v_2d<T> lhs, v_2d<T> rhs) => lhs.CompareTo(rhs) < 0;
    public static bool operator >(v_2d<T> lhs, v_2d<T> rhs) => lhs.CompareTo(rhs) > 0;
}
