using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    public class MouseButtonEventArgs : MouseEventArgs
    {
        public csMouseButton Button { get; private set; }
        public bool IsPressed { get; private set; }

        public MouseButtonEventArgs()
            : base()
        { }

        public MouseButtonEventArgs(int x, int y, csMouseButton button, bool pressed)
            : base(x, y)
        {
            Button = button;
            IsPressed = pressed;
        }
    }
}
