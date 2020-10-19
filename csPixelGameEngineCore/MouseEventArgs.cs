using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class MouseEventArgs : EventArgs
    {
        public int X { get => Position.x; }
        public int Y { get => Position.y; }
        public vec2d_i Position { get; }

        public MouseEventArgs()
            : base()
        { }

        public MouseEventArgs(int xPos, int yPos)
        {
            Position = new vec2d_i { x = xPos, y = yPos };
        }
    }
}
