using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public struct HWButton
    {
        public bool Pressed  { get; set; }
        public bool Released { get; set; }
        public bool Held     { get; set; }
    }
}
