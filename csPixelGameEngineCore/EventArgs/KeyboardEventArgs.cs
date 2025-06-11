using System;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

public class KeyboardEventArgs : EventArgs
{
    public Key PressedKey { get; init; }
    public int ScanCode { get; init; }
    public KeyModifiers Modifiers { get; init; }

    public KeyboardEventArgs(Key pressedKey, int scanCode, KeyModifiers modifiers)
    {
        PressedKey = pressedKey;
        ScanCode = scanCode;
        Modifiers = modifiers;
    }
}
