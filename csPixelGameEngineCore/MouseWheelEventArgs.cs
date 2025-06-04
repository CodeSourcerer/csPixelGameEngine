using System;

namespace csPixelGameEngineCore;

public class MouseWheelEventArgs : EventArgs
{
    public int OffsetX { get; private set; }
    public int OffsetY { get; private set; }

    public MouseWheelEventArgs()
        : base()
    { }

    public MouseWheelEventArgs(int offsetX, int offsetY)
        : base()
    {
        OffsetX = offsetX;
        OffsetY = offsetY;
    }
}
