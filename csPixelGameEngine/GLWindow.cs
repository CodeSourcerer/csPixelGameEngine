using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace csPixelGameEngine
{
    public class GLWindow : GameWindow
    {
        public Color4 BackgroundColor { get; set; }

        public GLWindow(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default,
                  DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)
        {

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(this.BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        public void SetBackgroundColor(Pixel backColor)
        {
            this.BackgroundColor = new Color4(backColor.r, backColor.g, backColor.b, backColor.a);
        }
    }
}
