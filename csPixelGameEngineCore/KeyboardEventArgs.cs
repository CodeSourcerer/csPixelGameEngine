using csPixelGameEngineCore.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class KeyboardEventArgs : EventArgs
    {
        public Key Key { get; }
        public uint ScanCode { get; }
        public bool Alt { get; }
        public bool Control { get; }
        public bool Shift { get; }
        public KeyModifiers Modifiers { get; }
        public bool IsRepeat { get; }

        public KeyboardEventArgs(Key key, uint scanCode, KeyModifiers modifiers)
        {
            this.Key = key;
            this.ScanCode = scanCode;
            this.Modifiers = modifiers;
            Alt = modifiers.HasFlag(KeyModifiers.Alt);
            Control = modifiers.HasFlag(KeyModifiers.Control);
            Shift = modifiers.HasFlag(KeyModifiers.Shift);
        }
    }
}
