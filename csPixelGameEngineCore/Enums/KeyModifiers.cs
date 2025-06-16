using System;

namespace csPixelGameEngineCore.Enums;

[Flags]
public enum KeyModifiers
{
    /// <summary>
    /// One or more Shift keys are held down.
    /// </summary>
    Shift = 0x0001,

    /// <summary>
    /// One or more Control keys are held down.
    /// </summary>
    Control = 0x0002,

    /// <summary>
    /// One or more Alt keys are held down.
    /// </summary>
    Alt = 0x0004,

    /// <summary>
    /// One or more Super keys are held down.
    /// </summary>
    Super = 0x0008,

    /// <summary>
    /// Caps lock is enabled.
    /// </summary>
    CapsLock = 0x0010,

    /// <summary>
    /// Num lock is enabled.
    /// </summary>
    NumLock = 0x0020
}
