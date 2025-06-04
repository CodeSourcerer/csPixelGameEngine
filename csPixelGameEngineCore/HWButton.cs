using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// Represents the state of a hardware button (mouse/key/joy)
    /// </summary>
    public struct HWButton
    {
        public bool Pressed  { get; set; } = false;
        public bool Released { get; set; } = true;
        public bool Held     { get; set; } = false;

        public HWButton() { }
    }
}
