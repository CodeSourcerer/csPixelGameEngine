using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace csPixelGameEngineCore
{
    public class GLWindow : GameWindow
    {
        public Color4   BackgroundColor { get; set; }
        public Sprite   DrawTarget      { get; set; }
        public uint     PixelWidth      { get; set; }
        public uint     PixelHeight     { get; set; }
        public uint     ScreenWidth     { get; set; }
        public uint     ScreenHeight    { get; set; }
        public uint     ViewWidth       { get; set; }
        public uint     ViewHeight      { get; set; }
        public uint     ViewX           { get; set; }
        public uint     ViewY           { get; set; }

        public GLWindow(uint screen_width, uint screen_height, uint pixel_w, uint pixel_h, string title)
            : base ((int)(screen_width * pixel_w), (int)(screen_height * pixel_h), GraphicsMode.Default, title,
                    GameWindowFlags.Default, DisplayDevice.Default, 2, 1, GraphicsContextFlags.ForwardCompatible)
        {
            Width           = (int)(screen_width * pixel_w);
            Height          = (int)(screen_height * pixel_h);
            ScreenWidth     = screen_width;
            ScreenHeight    = screen_height;
            DrawTarget      = new Sprite(screen_width, screen_height);
            BackgroundColor = new Color4(255, 0, 0, 255);
            PixelWidth      = pixel_w;
            PixelHeight     = pixel_h;
            ViewX           = 0;
            ViewY           = 0;
            this.VSync = VSyncMode.Off;
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

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, (int)ScreenWidth, (int)ScreenHeight, PixelFormat.Rgba, PixelType.UnsignedInt8888, DrawTarget.ColorData);

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
            uint ww = ScreenWidth * PixelWidth;
            uint wh = ScreenHeight * PixelHeight;
            float asp_ratio = ww / (float)wh;

            ViewWidth = ww;
            ViewHeight = (uint)(ViewWidth / asp_ratio);

            if (ViewHeight > wh)
            {
                ViewHeight = wh;
                ViewWidth = (uint)(ViewHeight * asp_ratio);
            }

            ViewX = (ww - ViewWidth) / 2;
            ViewY = (wh - ViewHeight) / 2;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(BackgroundColor);

            GL.Enable(EnableCap.Texture2D);
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)All.Nearest });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)All.Nearest });
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Decal);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)ScreenWidth, (int)ScreenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, DrawTarget.ColorData);

            base.OnLoad(e);
        }

        public void SetBackgroundColor(Pixel backColor)
        {
            BackgroundColor = new Color4(backColor.r, backColor.g, backColor.b, backColor.a);
        }
    }
}
