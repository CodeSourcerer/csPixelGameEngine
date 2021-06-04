using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class MouseWheelEventArgs : MouseEventArgs
    {
        public int Value { get; private set; }
        public int Delta { get; private set; }

        public MouseWheelEventArgs()
            : base()
        { }

        public MouseWheelEventArgs(int x, int y, int value, int delta)
            : base(x, y)
        {
            Value = value;
            Delta = delta;
        }
    }
}
