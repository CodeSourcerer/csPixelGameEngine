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
        RCode ApplicationStartUp();
        RCode ApplicationCleanUp();
        RCode ThreadStartUp();
        RCode ThreadCleanUp();
        RCode CreateWindowPane(vi2d windowPosition, vi2d windowSize, bool isFullScreen);
        RCode CreateGraphics(bool fullscreen, bool enableVSync, vi2d viewPos, vi2d viewSize);
        RCode ShowWindowFrame(bool showFrame = true);
        RCode SetWindowTitle(string title);
        RCode SetWindowSize(vi2d WindowPos, vi2d WindowSize);
        RCode StartSystemEventLoop();

        int WindowWidth  { get; }
        int WindowHeight { get; }

        // The C++ version does not have these, but we need them for C#
        event EventHandler<EventArgs> Closed;
        event EventHandler<EventArgs> Resize;
        event EventHandler<MouseMoveEventArgs> MouseMove;
        event EventHandler<MouseWheelEventArgs> MouseWheel;
        event EventHandler<MouseButtonEventArgs> MouseDown;
        event EventHandler<MouseButtonEventArgs> MouseUp;
        event EventHandler<FrameUpdateEventArgs> UpdateFrame;
    }
}
