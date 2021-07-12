using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore.Enums
{
    [Flags]
    public enum KeyModifiers : byte
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Command = 8
    }
}
