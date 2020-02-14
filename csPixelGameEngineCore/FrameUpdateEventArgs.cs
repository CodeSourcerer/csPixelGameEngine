using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public delegate void FrameUpdateEventHandler(object sender, FrameUpdateEventArgs frameUpdateArgs);

    public class FrameUpdateEventArgs : EventArgs
    {
        public double ElapsedTime { get; private set; }

        public FrameUpdateEventArgs(double elapsed)
            : base()
        {
            ElapsedTime = elapsed;
        }
    }
}
