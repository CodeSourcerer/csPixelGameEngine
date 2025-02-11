using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csPixelGameEngineCore;

public class vi2d : v2d_generic<int>
{
    public vi2d() : base(0, 0) { }
    public vi2d(int x, int y) : base(x, y) { }

    public override int mag() => (int)Math.Sqrt(x * x + y * y);
    public override int mag2() => x * x + y * y;
    public override v2d_generic<int> norm()
    {
        int r = 1 / mag();
        return new vi2d(r * r, y * r);
    }
    public override v2d_generic<int> perp() => new vi2d(-y, x);
}
