using System;

namespace csPixelGameEngineCore;

public class vi2d : v_2d<int>
{
    public vi2d() : base(0, 0) { }
    public vi2d(int x, int y) : base(x, y) { }
    public vi2d(v_2d<int> v) : base(v) { }

    public static vi2d operator *(vi2d lhs, vi2d rhs) => new vi2d((v_2d<int>)lhs * (v_2d<int>)rhs);
    public static vi2d operator *(int lhs, vi2d rhs)  => new vi2d(lhs * (v_2d<int>)rhs);
    public static vi2d operator *(vi2d lhs, int rhs)  => new vi2d((v_2d<int>)lhs * rhs);

    public static vi2d operator /(vi2d lhs, vi2d rhs) => (vi2d)(lhs / (v_2d<int>)rhs);
    public static vi2d operator /(int lhs, vi2d rhs)  => (vi2d)(lhs / (v_2d<int>)rhs);
    public static vi2d operator /(vi2d lhs, int rhs)  => (vi2d)((v_2d<int>)lhs / rhs);

    public static vf2d operator /(float lhs, vi2d rhs) => new(lhs / rhs.x, lhs / rhs.y);
    public static vf2d operator /(vi2d lhs, float rhs) => new(lhs.x / rhs, lhs.y / rhs);

}
