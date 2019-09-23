using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace csPixelGameEngine
{
    public class GLWindow : GameWindow
    {
        public GLWindow(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
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
    }
}
