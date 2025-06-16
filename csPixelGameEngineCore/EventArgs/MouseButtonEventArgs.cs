using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

public class MouseButtonEventArgs : System.EventArgs
{
    public csMouseButton Button { get; private set; }
    public bool IsPressed { get; private set; }

    public MouseButtonEventArgs()
        : base()
    { }

    public MouseButtonEventArgs(csMouseButton button, bool pressed)
        : base()
    {
        Button = button;
        IsPressed = pressed;
    }
}
