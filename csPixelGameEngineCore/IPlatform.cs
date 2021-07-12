using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    /// <summary>
    /// This will serve a different purpose than the original, since .NET Core is already platform independent. 
    /// 
    /// We will mainly use this for attaching window event handlers to the underlying window system.
    /// </summary>
    public interface IPlatform
    {
        RCode CreateGraphics(bool fullscreen, bool enableVSync, vec2d_i viewPos, vec2d_i viewSize);
        RCode SetWindowTitle(string title);
        RCode StartSystemEventLoop();

        int WindowWidth  { get; }
        int WindowHeight { get; }

        // The C++ version does not have these, but we need them for C#
        KeyboardState KeyboardState { get; }
        event EventHandler<EventArgs> Closed;
        event EventHandler<EventArgs> Resize;
        event EventHandler<MouseMoveEventArgs> MouseMove;
        event EventHandler<MouseWheelEventArgs> MouseWheel;
        event EventHandler<MouseButtonEventArgs> MouseDown;
        event EventHandler<MouseButtonEventArgs> MouseUp;
        event EventHandler<KeyboardEventArgs> KeyDown;
        event EventHandler<FrameUpdateEventArgs> UpdateFrame;
    }
}
