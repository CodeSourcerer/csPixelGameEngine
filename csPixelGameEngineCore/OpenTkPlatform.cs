using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using csPixelGameEngineCore.Enums;
using Microsoft.Extensions.Logging;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TkKeyModifiers = OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers;
using TkKeys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace csPixelGameEngineCore;

/// <summary>
/// This loosely mimics a platform implementation. OpenTK manages the window, so it makes sense in my head
/// to make a platform implementation with it to deal with events.
/// </summary>
public class OpenTkPlatform : IPlatform
{
    private readonly ILogger<OpenTkPlatform> logger;
    private readonly GameWindow glWindow;
    private Dictionary<TkKeys, Key> mapKeys = new Dictionary<TkKeys, Key>(256);

    public int WindowWidth  { get => glWindow.ClientSize.X; }
    public int WindowHeight { get => glWindow.ClientSize.Y; }
    public vi2d WindowPosition { get => new (glWindow.Location.X, glWindow.Location.Y); }
    public int WindowPosX { get => WindowPosition.x; }
    public int WindowPosY { get => WindowPosition.y; }
    public Dictionary<int, Key> KeyMap { get => mapKeys.Cast<KeyValuePair<int, Key>>().ToDictionary(); }

    public event EventHandler<EventArgs> Closed;
    public event EventHandler<CancelEventArgs> Closing;
    public event EventHandler<EventArgs> Resize;
    public event EventHandler<EventArgs> Move;
    public event EventHandler<MouseMoveEventArgs> MouseMove;
    public event EventHandler<MouseWheelEventArgs> MouseWheel;
    public event EventHandler<MouseButtonEventArgs> MouseDown;
    public event EventHandler<MouseButtonEventArgs> MouseUp;
    public event EventHandler<FrameUpdateEventArgs> UpdateFrame;
    public event EventHandler<KeyboardEventArgs> KeyDown;
    public event EventHandler<KeyboardEventArgs> KeyUp;

    public OpenTkPlatform(GameWindow gameWindow, ILogger<OpenTkPlatform> logger)
    {
        this.logger = logger;
        glWindow = gameWindow;
        initMapKeys();
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

        Func<TkKeyModifiers, Enums.KeyModifiers> fromTkKeyModifiers = (tkModifiers) =>
        {
            Enums.KeyModifiers modifiers = 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.Alt) ? Enums.KeyModifiers.Alt : 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.CapsLock) ? Enums.KeyModifiers.CapsLock : 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.Control) ? Enums.KeyModifiers.Control : 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.NumLock) ? Enums.KeyModifiers.NumLock : 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.Shift) ? Enums.KeyModifiers.Shift : 0;
            modifiers |= tkModifiers.HasFlag(TkKeyModifiers.Super) ? Enums.KeyModifiers.Super : 0;
            return modifiers;
        };

        // Glue some event handlers to glWindow
        glWindow.Closing     += eventArgs => Closing?.Invoke(this, eventArgs);
        glWindow.Resize      += eventArgs => Resize?.Invoke(this, EventArgs.Empty);
        glWindow.Move        += eventArgs => Move?.Invoke(this, EventArgs.Empty);
        glWindow.MouseMove   += eventArgs => MouseMove?.Invoke(this, new MouseMoveEventArgs((int)eventArgs.X, (int)eventArgs.Y,
                                                                                            (int)eventArgs.DeltaX, (int)eventArgs.DeltaY));
        glWindow.MouseDown   += eventArgs => MouseDown?.Invoke(this, new MouseButtonEventArgs(fromTkMouseBtn(eventArgs.Button), true));
        glWindow.MouseUp     += eventArgs => MouseUp?.Invoke(this, new MouseButtonEventArgs(fromTkMouseBtn(eventArgs.Button), false));
        glWindow.MouseWheel  += eventArgs => MouseWheel?.Invoke(this, new MouseWheelEventArgs((int)eventArgs.OffsetX, (int)eventArgs.OffsetY));
        glWindow.UpdateFrame += eventArgs => UpdateFrame?.Invoke(this, new FrameUpdateEventArgs(eventArgs.Time));
        glWindow.KeyDown     += eventArgs => KeyDown?.Invoke(this, new KeyboardEventArgs(mapKeys[eventArgs.Key], eventArgs.ScanCode, fromTkKeyModifiers(eventArgs.Modifiers)));
        glWindow.KeyUp       += eventArgs => KeyUp?.Invoke(this, new KeyboardEventArgs(mapKeys[eventArgs.Key], eventArgs.ScanCode, fromTkKeyModifiers(eventArgs.Modifiers)));

        glWindow.Run();

        return RCode.OK;
    }

    private csMouseButton fromTkMouseBtn(MouseButton btn)
    {
        return btn switch
        {
            MouseButton.Left    => csMouseButton.Left,
            MouseButton.Middle  => csMouseButton.Middle,
            MouseButton.Right   => csMouseButton.Right,
            MouseButton.Button4 => csMouseButton.Button4,
            MouseButton.Button5 => csMouseButton.Button5,
            MouseButton.Button6 => csMouseButton.Button6,
            MouseButton.Button7 => csMouseButton.Button7,
            MouseButton.Button8 => csMouseButton.Button8,
            _ => csMouseButton.ButtonLast
        };
    }

    private void initMapKeys()
    {
        mapKeys[0x00] = Key.NONE;
        mapKeys[TkKeys.A] = Key.A; mapKeys[TkKeys.B] = Key.B; mapKeys[TkKeys.C] = Key.C; mapKeys[TkKeys.D] = Key.D; mapKeys[TkKeys.E] = Key.E;
        mapKeys[TkKeys.F] = Key.F; mapKeys[TkKeys.G] = Key.G; mapKeys[TkKeys.H] = Key.H; mapKeys[TkKeys.I] = Key.I; mapKeys[TkKeys.J] = Key.J;
        mapKeys[TkKeys.K] = Key.K; mapKeys[TkKeys.L] = Key.L; mapKeys[TkKeys.M] = Key.M; mapKeys[TkKeys.N] = Key.N; mapKeys[TkKeys.O] = Key.O;
        mapKeys[TkKeys.P] = Key.P; mapKeys[TkKeys.Q] = Key.Q; mapKeys[TkKeys.R] = Key.R; mapKeys[TkKeys.S] = Key.S; mapKeys[TkKeys.T] = Key.T;
        mapKeys[TkKeys.U] = Key.U; mapKeys[TkKeys.V] = Key.V; mapKeys[TkKeys.W] = Key.W; mapKeys[TkKeys.X] = Key.X; mapKeys[TkKeys.Y] = Key.Y;
        mapKeys[TkKeys.Z] = Key.Z;

        mapKeys[TkKeys.F1] = Key.F1; mapKeys[TkKeys.F2] = Key.F2; mapKeys[TkKeys.F3] = Key.F3;
        mapKeys[TkKeys.F4] = Key.F4; mapKeys[TkKeys.F5] = Key.F5; mapKeys[TkKeys.F6] = Key.F6;
        mapKeys[TkKeys.F7] = Key.F7; mapKeys[TkKeys.F8] = Key.F8; mapKeys[TkKeys.F9] = Key.F9; 
        mapKeys[TkKeys.F10] = Key.F10;
        mapKeys[TkKeys.F11] = Key.F11;
        mapKeys[TkKeys.F12] = Key.F12;

        mapKeys[TkKeys.Down]  = Key.DOWN;
        mapKeys[TkKeys.Left]  = Key.LEFT;
        mapKeys[TkKeys.Right] = Key.RIGHT;
        mapKeys[TkKeys.Up]    = Key.UP;

        mapKeys[TkKeys.Backspace]    = Key.BACK;
        mapKeys[TkKeys.Escape]       = Key.ESCAPE;
        mapKeys[TkKeys.Enter]        = Key.ENTER;
        mapKeys[TkKeys.Pause]        = Key.PAUSE;
        mapKeys[TkKeys.ScrollLock]   = Key.SCROLL;
        mapKeys[TkKeys.Tab]          = Key.TAB;
        mapKeys[TkKeys.Delete]       = Key.DEL;
        mapKeys[TkKeys.Home]         = Key.HOME;
        mapKeys[TkKeys.End]          = Key.END;
        mapKeys[TkKeys.PageUp]       = Key.PGUP;
        mapKeys[TkKeys.PageDown]     = Key.PGDN;
        mapKeys[TkKeys.Insert]       = Key.INS;
        mapKeys[TkKeys.LeftShift]    = Key.SHIFT;
        mapKeys[TkKeys.RightShift]   = Key.SHIFT;
        mapKeys[TkKeys.LeftControl]  = Key.CTRL;
        mapKeys[TkKeys.RightControl] = Key.CTRL;
        mapKeys[TkKeys.Space]        = Key.SPACE;

        mapKeys[TkKeys.D0] = Key.K0; mapKeys[TkKeys.KeyPad0] = Key.NP0;
        mapKeys[TkKeys.D1] = Key.K1; mapKeys[TkKeys.KeyPad1] = Key.NP1;
        mapKeys[TkKeys.D2] = Key.K2; mapKeys[TkKeys.KeyPad2] = Key.NP2;
        mapKeys[TkKeys.D3] = Key.K3; mapKeys[TkKeys.KeyPad3] = Key.NP3;
        mapKeys[TkKeys.D4] = Key.K4; mapKeys[TkKeys.KeyPad4] = Key.NP4;
        mapKeys[TkKeys.D5] = Key.K5; mapKeys[TkKeys.KeyPad5] = Key.NP5;
        mapKeys[TkKeys.D6] = Key.K6; mapKeys[TkKeys.KeyPad6] = Key.NP6;
        mapKeys[TkKeys.D7] = Key.K7; mapKeys[TkKeys.KeyPad7] = Key.NP7;
        mapKeys[TkKeys.D8] = Key.K8; mapKeys[TkKeys.KeyPad8] = Key.NP8;
        mapKeys[TkKeys.D9] = Key.K9; mapKeys[TkKeys.KeyPad9] = Key.NP9;

        mapKeys[TkKeys.KeyPadMultiply] = Key.NP_MUL;
        mapKeys[TkKeys.KeyPadAdd]      = Key.NP_ADD;
        mapKeys[TkKeys.KeyPadDivide]   = Key.NP_DIV;
        mapKeys[TkKeys.KeyPadSubtract] = Key.NP_SUB;
        mapKeys[TkKeys.KeyPadDecimal]  = Key.NP_DECIMAL;

        // Thanks scripticuk
        mapKeys[TkKeys.Semicolon]    = Key.OEM_1;     // On US and UK keyboards this is the ';:' key
        mapKeys[TkKeys.Slash]        = Key.OEM_2;     // On US and UK keyboards this is the '/?' key
        mapKeys[TkKeys.GraveAccent]  = Key.OEM_3;     // On US keyboard this is the '~' key
        mapKeys[TkKeys.LeftBracket]  = Key.OEM_4;     // On US and UK keyboards this is the '[{' key
        mapKeys[TkKeys.Backslash]    = Key.OEM_5;     // On US keyboard this is '\|' key.
        mapKeys[TkKeys.RightBracket] = Key.OEM_6;     // On US and UK keyboards this is the ']}' key
        mapKeys[TkKeys.Apostrophe]   = Key.OEM_7;     // On US keyboard this is the single/double quote key. On UK, this is the single quote/@ symbol key
        // NFI what key this is
        //mapKeys[VK_OEM_8]      = Key.OEM_8;     // miscellaneous characters. Varies by keyboard
        mapKeys[TkKeys.Equal]        = Key.EQUALS;    // the '+' key on any keyboard
        mapKeys[TkKeys.Comma]        = Key.COMMA;     // the comma key on any keyboard
        mapKeys[TkKeys.Minus]        = Key.MINUS;     // the minus key on any keyboard
        mapKeys[TkKeys.Period]       = Key.PERIOD;    // the period key on any keyboard
        mapKeys[TkKeys.CapsLock]     = Key.CAPS_LOCK;

        // other OpenTK keys that aren't mappable to PGE
        mapKeys[TkKeys.LeftSuper]  = Key.NONE;
        mapKeys[TkKeys.RightSuper] = Key.NONE;
        mapKeys[TkKeys.Menu]       = Key.NONE;
        mapKeys[TkKeys.F13]        = Key.NONE;
        mapKeys[TkKeys.F14]        = Key.NONE;
        mapKeys[TkKeys.F15]        = Key.NONE;
        mapKeys[TkKeys.F16]        = Key.NONE;
        mapKeys[TkKeys.F17]        = Key.NONE;
        mapKeys[TkKeys.F18]        = Key.NONE;
        mapKeys[TkKeys.F19]        = Key.NONE;
        mapKeys[TkKeys.F20]        = Key.NONE;
        mapKeys[TkKeys.F21]        = Key.NONE;
        mapKeys[TkKeys.F22]        = Key.NONE;
        mapKeys[TkKeys.F23]        = Key.NONE;
        mapKeys[TkKeys.F24]        = Key.NONE;
        mapKeys[TkKeys.F25]        = Key.NONE;
        mapKeys[TkKeys.LeftAlt]    = Key.NONE;
        mapKeys[TkKeys.RightAlt]   = Key.NONE;
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
        glWindow.Location   = new Vector2i(WindowPos.x, WindowPos.y);
        glWindow.ClientSize = new Vector2i(WindowSize.x, WindowSize.y);
        return RCode.OK;
    }
}
