using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;
using OpenTK;
using OpenTK.Input;

namespace csPixelGameEngineCore
{
    public class OpenTkPlatform : IPlatform
    {
        private readonly GameWindow glWindow;

        public int WindowWidth  { get => glWindow.Size.Width; }
        public int WindowHeight { get => glWindow.Size.Height; }

        public event EventHandler<EventArgs> Closed;
        public event EventHandler<EventArgs> Resize;
        public event EventHandler<MouseMoveEventArgs> MouseMove;
        public event EventHandler<MouseWheelEventArgs> MouseWheel;
        public event EventHandler<MouseButtonEventArgs> MouseDown;
        public event EventHandler<MouseButtonEventArgs> MouseUp;
        public event EventHandler<FrameUpdateEventArgs> UpdateFrame;

        public OpenTkPlatform(GameWindow gameWindow)
        {
            glWindow = gameWindow;
        }

        public RCode CreateGraphics(bool fullscreen, bool enableVSync, vec2d_i viewPos, vec2d_i viewSize)
        {
            throw new NotImplementedException();
        }

        public RCode SetWindowTitle(string title)
        {
            if (title == null) return RCode.FAIL;

            glWindow.Title = title;

            return RCode.OK;
        }

        public RCode StartSystemEventLoop()
        {
            // Glue some event handlers to glWindow
            glWindow.Closed += (sender, eventArgs) =>
            {
                Closed?.Invoke(sender, eventArgs);
            };

            glWindow.Resize += (sender, eventArgs) =>
            {
                Resize?.Invoke(sender, eventArgs);
            };

            glWindow.MouseMove += (sender, eventArgs) =>
            {
                MouseMove?.Invoke(sender, new MouseMoveEventArgs(eventArgs.X, eventArgs.Y, eventArgs.XDelta, eventArgs.YDelta));
            };

            glWindow.MouseDown += (sender, eventArgs) =>
            {
                var btnClicked = fromTkMouseBtn(eventArgs.Button);
                if (btnClicked != csMouseButton.Unknown)
                {
                    MouseDown?.Invoke(sender, new MouseButtonEventArgs(eventArgs.X, eventArgs.Y, btnClicked, true));
                }
            };

            glWindow.MouseUp += (sender, eventArgs) =>
            {
                var btnClicked = fromTkMouseBtn(eventArgs.Button);
                if (btnClicked != csMouseButton.Unknown)
                {
                    MouseUp?.Invoke(sender, new MouseButtonEventArgs(eventArgs.X, eventArgs.Y, btnClicked, false));
                }
            };

            glWindow.MouseWheel += (sender, eventArgs) =>
            {
                MouseWheel?.Invoke(sender, new MouseWheelEventArgs(eventArgs.X, eventArgs.Y, eventArgs.Value, eventArgs.Delta));
            };

            glWindow.UpdateFrame += (sender, eventArgs) =>
            {
                UpdateFrame?.Invoke(sender, new FrameUpdateEventArgs(eventArgs.Time));
            };

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
                _ => csMouseButton.Unknown
            };
        }
    }
}
