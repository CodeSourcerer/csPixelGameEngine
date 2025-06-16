using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore;

public class v_2d<T> : IEquatable<v_2d<T>>, IComparable<v_2d<T>> where T : INumber<T>
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
    public virtual T area() => x * y;

    /// <summary>
    /// Returns magnitude of vector
    /// </summary>
    /// <returns></returns>
    public virtual double mag()
    {
        var _x = double.CreateChecked<T>(x);
        var _y = double.CreateChecked<T>(y);

        return Math.Sqrt(_x * _x + _y * _y);
    }

    /// <summary>
    /// Returns magnitude squared of vector (useful for fast comparisons)
    /// </summary>
    /// <returns></returns>
    public virtual T mag2() => x * x + y * y;

    /// <summary>
    /// Returns normalised version of vector
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> norm()
    {
        var r = 1 / mag();
        var _x = double.CreateChecked(x);
        var _y = double.CreateChecked(y);
        
        return new v_2d<T>(T.CreateChecked(_x * r), T.CreateChecked(_y * r));
    }

    /// <summary>
    /// Returns vector at 90 degrees to this one
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> perp() => new v_2d<T>(-y, x);

    /// <summary>
    /// Rounds both components down
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> floor() => new v_2d<T>(T.CreateChecked(Math.Floor(double.CreateChecked(x))),
                                                  T.CreateChecked(Math.Floor(double.CreateChecked(y))));

    /// <summary>
    /// Rounds both components up
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> ceil() => new v_2d<T>(T.CreateChecked(Math.Ceiling(double.CreateChecked(x))),
                                                 T.CreateChecked(Math.Ceiling(double.CreateChecked(y))));

    /// <summary>
    /// Returns 'element-wise' max of this and another vector
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> max(v_2d<T> v) => new v_2d<T>(T.CreateChecked(Math.Max(double.CreateChecked(x), double.CreateChecked(v.x))),
                                                         T.CreateChecked(Math.Max(double.CreateChecked(y), double.CreateChecked(v.y))));

    /// <summary>
    /// Returns 'element-wise' min of this and another vector
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> min(v_2d<T> v) => new v_2d<T>(T.CreateChecked(Math.Min(double.CreateChecked(x), double.CreateChecked(v.x))),
                                                         T.CreateChecked(Math.Min(double.CreateChecked(y), double.CreateChecked(v.y))));

    /// <summary>
    /// Calculates scalar dot product between this and another vector
    /// </summary>
    /// <returns></returns>
    public virtual T dot(v_2d<T> rhs) => x * rhs.x + y * rhs.y;

    /// <summary>
    /// Calculates 'scalar' cross product between this and another vector (useful for winding orders)
    /// </summary>
    /// <returns></returns>
    public virtual T cross(v_2d<T> rhs) => x * rhs.y - y * rhs.x;

    /// <summary>
    /// Treat this as polar coordinate (R, Theta), return cartesian equivalent (X, Y)
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> cart()
    {
        var _x = double.CreateChecked(x);
        var _y = double.CreateChecked(y);
        return new v_2d<T>(T.CreateChecked(Math.Cos(_y) * _x), T.CreateChecked(Math.Sin(_y) * _x));
    }

    /// <summary>
    /// Treat this as cartesian coordinate (X, Y), return polar equivalent (R, Theta)
    /// </summary>
    /// <returns></returns>
    public virtual v_2d<T> polar()
    {
        return new v_2d<T>(T.CreateChecked(mag()), T.CreateChecked(Math.Atan2(double.CreateChecked(y), double.CreateChecked(x))));
    }

    /// <summary>
    /// Clamp the components of this vector in between the 'element-wise' minimum and maximum of 2 other vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public virtual v_2d<T> clamp(v_2d<T> v1, v_2d<T> v2) => max(v1).min(v2);

    /// <summary>
    /// Linearly interpolate between this vector, and another vector, given normalised parameter 't'
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public virtual v_2d<T> lerp(v_2d<T> v1, double t) => this * T.CreateChecked(1.0 - t) + (v1 * T.CreateChecked(t));

    /// <summary>
    /// Assuming this vector is incident, given a normal, return the reflection
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public virtual v_2d<T> reflect(v_2d<T> n) => this - T.CreateChecked(2.0) * (dot(n) * n);

    /// <summary>
    /// Compare if this vector is numerically equal to another
    /// </summary>
    /// <param name="lhs"></param>
    /// <returns></returns>
    public virtual bool Equals(v_2d<T> lhs) => x == lhs.x && y == lhs.y;

    public override bool Equals(object lhs) => Equals(lhs as v_2d<T>);

    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    public virtual int CompareTo(v_2d<T> rhs) => y == rhs.y ? int.CreateChecked(y - rhs.y) : int.CreateChecked(x - rhs.x);

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

    public static v_2d<T> operator *(v_2d<T> lhs, v_2d<T> rhs) => new(lhs.x * rhs.x, lhs.y * rhs.y);
    public static v_2d<T> operator *(T lhs, v_2d<T> rhs) => new(lhs * rhs.x, lhs * rhs.y);
    public static v_2d<T> operator *(v_2d<T> lhs, T rhs) => new(lhs.x * rhs, lhs.y * rhs);

    public static v_2d<T> operator /(v_2d<T> lhs, v_2d<T> rhs) => new(lhs.x / rhs.x, lhs.y / rhs.y);
    public static v_2d<T> operator /(T lhs, v_2d<T> rhs) => new(lhs / rhs.x, lhs / rhs.y);
    public static v_2d<T> operator /(v_2d<T> lhs, T rhs) => new(lhs.x / rhs, lhs.y / rhs);

    public static v_2d<T> operator +(v_2d<T> rhs) => new(+rhs.x, +rhs.y);
    public static v_2d<T> operator +(v_2d<T> lhs, v_2d<T> rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
    public static v_2d<T> operator +(T lhs, v_2d<T> rhs) => new(lhs + rhs.x, lhs + rhs.y);
    public static v_2d<T> operator +(v_2d<T> lhs, T rhs) => new(lhs.x + rhs, lhs.y + rhs);

    public static v_2d<T> operator -(v_2d<T> rhs) => new(-rhs.x, -rhs.y);
    public static v_2d<T> operator -(v_2d<T> lhs, v_2d<T> rhs) => new(lhs.x - rhs.x, lhs.y - rhs.y);
    public static v_2d<T> operator -(T lhs, v_2d<T> rhs) => new(lhs - rhs.x, lhs - rhs.y);
    public static v_2d<T> operator -(v_2d<T> lhs, T rhs) => new(lhs.x - rhs, lhs.y - rhs);

    public static bool operator <(v_2d<T> lhs, v_2d<T> rhs) => lhs.CompareTo(rhs) < 0;
    public static bool operator >(v_2d<T> lhs, v_2d<T> rhs) => lhs.CompareTo(rhs) > 0;
}
