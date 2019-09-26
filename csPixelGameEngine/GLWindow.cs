using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace csPixelGameEngine
{
    public class GLWindow : GameWindow
    {
        public Color4   BackgroundColor { get; set; }
        public Sprite   DrawTarget      { get; set; }
        public uint     PixelWidth      { get; private set; }
        public uint     PixelHeight     { get; private set; }
        public uint     ScreenWidth     { get; private set; }
        public uint     ScreenHeight    { get; private set; }
        public uint     ViewWidth       { get; private set; }
        public uint     ViewHeight      { get; private set; }
        public uint     ViewX           { get; private set; }
        public uint     ViewY           { get; private set; }

        public GLWindow(uint screen_width, uint screen_height, uint pixel_w, uint pixel_h, string title)
            : base((int)(screen_width * pixel_w), (int)(screen_height * pixel_h), GraphicsMode.Default, title, GameWindowFlags.Default,
                  DisplayDevice.Default, 2, 1, GraphicsContextFlags.ForwardCompatible)
        {
            this.Width = (int)(screen_width * pixel_w);
            this.Height = (int)(screen_height * pixel_h);
            this.ScreenWidth = screen_width;
            this.ScreenHeight = screen_height;
            this.DrawTarget = new Sprite(screen_width, screen_height);
            this.BackgroundColor = new Color4(255, 0, 0, 255);
            this.PixelWidth = pixel_w;
            this.PixelHeight = pixel_h;
            this.ViewX = 0;
            this.ViewY = 0;
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
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Viewport((int)ViewX, (int)ViewY, (int)ViewWidth, (int)ViewHeight);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (int)ScreenWidth, (int)ScreenHeight, PixelFormat.Rgba, PixelType.UnsignedByte, this.DrawTarget.colorData);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0, 1.0); GL.Vertex3(-1.0f, -1.0f, 0.0f);
            GL.TexCoord2(0.0, 0.0); GL.Vertex3(-1.0f,  1.0f, 0.0f);
            GL.TexCoord2(1.0, 0.0); GL.Vertex3( 1.0f,  1.0f, 0.0f);
            GL.TexCoord2(1.0, 1.0); GL.Vertex3( 1.0f, -1.0f, 0.0f);
            GL.End();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            update_viewport();

            GL.Viewport((int)ViewX, (int)ViewY, (int)ViewWidth, (int)ViewHeight);
        }

        private void update_viewport()
        {
            int ww = (int)(ScreenWidth * PixelWidth);
            int wh = (int)(ScreenHeight * PixelHeight);
            float asp_ratio = (float)ww / (float)wh;

            ViewWidth = (uint)ww;
            ViewHeight = (uint)((float)ViewWidth / asp_ratio);

            if (ViewHeight > wh)
            {
                ViewHeight = (uint)wh;
                ViewWidth = (uint)(ViewHeight * asp_ratio);
            }

            ViewX = (uint)((ww - ViewWidth) / 2);
            ViewY = (uint)((wh - ViewHeight) / 2);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(this.BackgroundColor);

            GL.Enable(EnableCap.Texture2D);
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)All.Nearest });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)All.Nearest });
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Decal);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)ScreenWidth, (int)ScreenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, this.DrawTarget.colorData);

            base.OnLoad(e);
        }

        public void SetBackgroundColor(Pixel backColor)
        {
            this.BackgroundColor = new Color4(backColor.r, backColor.g, backColor.b, backColor.a);
        }
    }
}
