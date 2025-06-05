using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using csPixelGameEngineCore.Enums;
using Microsoft.Extensions.Logging;
using OpenTK;
using OpenTK.Input;
//using OpenTK.Mathematics;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Windowing.GraphicsLibraryFramework;

namespace csPixelGameEngineCore;

/// <summary>
/// This loosely mimics a platform implementation. OpenTK manages the window, so it makes sense in my head
/// to make a platform implementation with it to deal with events.
/// </summary>
public class OpenTkPlatform : IPlatform
{
    private readonly ILogger<OpenTkPlatform> logger;
    private readonly GameWindow glWindow;

    //public int WindowWidth  { get => glWindow.ClientSize.X; }
    //public int WindowHeight { get => glWindow.ClientSize.Y; }
    public int WindowWidth { get => glWindow.Width; }
    public int WindowHeight { get => glWindow.Height; }
    public vi2d WindowPosition { get => new vi2d(glWindow.Location.X, glWindow.Location.Y); }
    public int WindowPosX { get => WindowPosition.x; }
    public int WindowPosY { get => WindowPosition.y; }

    public event EventHandler<EventArgs> Closed;
    public event EventHandler<CancelEventArgs> Closing;
    public event EventHandler<EventArgs> Resize;
    public event EventHandler<EventArgs> Move;
    public event EventHandler<MouseMoveEventArgs> MouseMove;
    public event EventHandler<MouseWheelEventArgs> MouseWheel;
    public event EventHandler<MouseButtonEventArgs> MouseDown;
    public event EventHandler<MouseButtonEventArgs> MouseUp;
    public event EventHandler<FrameUpdateEventArgs> UpdateFrame;

    public OpenTkPlatform(GameWindow gameWindow, ILogger<OpenTkPlatform> logger)
    {
        this.logger = logger;
        glWindow = gameWindow;
    }

    public RCode SetWindowTitle(string title)
    {
        if (title == null) return RCode.FAIL;

        glWindow.Title = title;

        return RCode.OK;
    }

    public RCode StartSystemEventLoop()
    {
        logger.LogDebug("OpenTkPlatform.StartSystemEventLoop()");

        // Glue some event handlers to glWindow
        glWindow.Closing     += (sender, eventArgs) => Closing?.Invoke(sender, eventArgs);
        glWindow.Resize      += (sender, eventArgs) => Resize?.Invoke(sender, eventArgs);
        glWindow.Move        += (sender, eventArgs) => Move?.Invoke(sender, eventArgs);
        glWindow.MouseMove   += (sender, eventArgs) => MouseMove?.Invoke(sender, new MouseMoveEventArgs(eventArgs.X, eventArgs.Y,
                                                                                                        eventArgs.XDelta, eventArgs.YDelta));
        glWindow.MouseDown   += (sender, eventArgs) => MouseDown?.Invoke(sender, new MouseButtonEventArgs(fromTkMouseBtn(eventArgs.Button), true));
        glWindow.MouseUp     += (sender, eventArgs) => MouseUp?.Invoke(sender, new MouseButtonEventArgs(fromTkMouseBtn(eventArgs.Button), false));
        glWindow.MouseWheel  += (sender, eventArgs) => MouseWheel?.Invoke(sender, new MouseWheelEventArgs(eventArgs.X, eventArgs.Y));
        glWindow.UpdateFrame += (sender, eventArgs) => UpdateFrame?.Invoke(sender, new FrameUpdateEventArgs(eventArgs.Time));

        //glWindow.Closing += eventArgs => Closing?.Invoke(this, eventArgs);

        //glWindow.Resize += eventArgs => Resize?.Invoke(this, EventArgs.Empty);

        //glWindow.Move += eventArgs => Move?.Invoke(this, EventArgs.Empty);

        //glWindow.MouseMove += eventArgs => MouseMove?.Invoke(this, new MouseMoveEventArgs((int)eventArgs.X, (int)eventArgs.Y, (int)eventArgs.DeltaX, (int)eventArgs.DeltaY));

        //glWindow.MouseDown += eventArgs =>
        //{
        //    var btnClicked = fromTkMouseBtn(eventArgs.Button);
        //    if (btnClicked != csMouseButton.Unknown)
        //    {
        //        MouseDown?.Invoke(this, new MouseButtonEventArgs(btnClicked, true));
        //    }
        //};

        //glWindow.MouseUp += eventArgs =>
        //{
        //    var btnClicked = fromTkMouseBtn(eventArgs.Button);
        //    if (btnClicked != csMouseButton.Unknown)
        //    {
        //        MouseUp?.Invoke(this, new MouseButtonEventArgs(btnClicked, false));
        //    }
        //};

        //glWindow.MouseWheel += eventArgs => MouseWheel?.Invoke(this, new MouseWheelEventArgs((int)eventArgs.OffsetX, (int)eventArgs.OffsetY));

        //glWindow.UpdateFrame += eventArgs => UpdateFrame?.Invoke(this, new FrameUpdateEventArgs(eventArgs.Time));

        glWindow.Run();

        return RCode.OK;
    }

    private csMouseButton fromTkMouseBtn(MouseButton btn)
    {
        return btn switch
        {
            MouseButton.Left => csMouseButton.Left,
            MouseButton.Middle => csMouseButton.Middle,
            MouseButton.Right => csMouseButton.Right,
            MouseButton.Button1 => csMouseButton.Button1,
            MouseButton.Button2 => csMouseButton.Button2,
            MouseButton.Button3 => csMouseButton.Button3,
            MouseButton.Button4 => csMouseButton.Button4,
            MouseButton.Button5 => csMouseButton.Button5,
            MouseButton.Button6 => csMouseButton.Button6,
            MouseButton.Button7 => csMouseButton.Button7,
            MouseButton.Button8 => csMouseButton.Button8,
            MouseButton.Button9 => csMouseButton.Button9,
            _ => csMouseButton.ButtonLast
        };
    }

    public RCode CreateGraphics(bool fullscreen, bool enableVSync, vi2d viewPos, vi2d viewSize) => RCode.OK;

    public RCode ApplicationStartUp() => RCode.OK;

    public RCode ApplicationCleanUp() => RCode.OK;

    public RCode ThreadStartUp() => RCode.OK;

    public RCode ThreadCleanUp() => RCode.OK;

    public RCode CreateWindowPane(vi2d windowPosition, vi2d windowSize, bool isFullScreen)
    {
        logger.LogDebug("OpenTkPlatform.CreateWindowPane(windowPosition:{x},{y},windowSize:{width},{height},isFullScreen:{isFullScreen})",
            windowPosition.x, windowPosition.y, windowSize.x, windowSize.y, isFullScreen);
        SetWindowSize(windowPosition, windowSize);
        glWindow.WindowState = isFullScreen ? WindowState.Fullscreen : WindowState.Normal;

        return RCode.OK;
    }

    public RCode ShowWindowFrame(bool showFrame = true) => RCode.OK;

    public RCode SetWindowSize(vi2d WindowPos, vi2d WindowSize)
    {
        glWindow.Location   = new Point(WindowPos.x, WindowPos.y);  // new Vector2i(WindowPos.x, WindowPos.y);
        glWindow.ClientSize = new Size(WindowSize.x, WindowSize.y); // new Vector2i(WindowSize.x, WindowSize.y);
        return RCode.OK;
    }
}
