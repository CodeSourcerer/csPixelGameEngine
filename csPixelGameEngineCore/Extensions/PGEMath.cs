using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore.Extensions;

public static class PGEMath
{
    public static (T, T) Swap<T>(T v1, T v2)
    {
        return (v2, v1);
    }
    public static void Swap<T>(ref T v1, ref T v2)
    {
        T temp = v1;
        v1 = v2;
        v2 = temp;
    }
}
