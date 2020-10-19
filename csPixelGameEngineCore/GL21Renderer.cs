using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    public class GL21Renderer : IRenderer
    {
        private readonly GameWindow glWindow;

        public event EventHandler<FrameUpdateEventArgs> RenderFrame;

        public GL21Renderer(GameWindow gameWindow)
        {
            if (gameWindow == null) throw new ArgumentNullException(nameof(gameWindow));

            glWindow = gameWindow;
            glWindow.RenderFrame += (sender, eventArgs) =>
            {
                RenderFrame?.Invoke(sender, new FrameUpdateEventArgs(eventArgs.Time));
            };
        }

        public void ApplyTexture(uint id)
        {
            GL.BindTexture(TextureTarget.Texture2D, id);
        }

        public void ClearBuffer(Pixel p, bool bDepth)
        {
            GL.ClearColor(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f);
            var clearBits = ClearBufferMask.ColorBufferBit;
            if (bDepth) clearBits |= ClearBufferMask.DepthBufferBit;

            GL.Clear(clearBits);
        }

        public RCode CreateDevice(bool bFullScreen, bool bVSYNC, params object[] p)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            return RCode.OK;
        }

        public uint CreateTexture(uint width, uint height)
        {
            uint id = 0;
            GL.GenTextures(1, out id);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TextureParameterI(id, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Nearest });
            GL.TextureParameterI(id, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Nearest });
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            return id;
        }

        public uint DeleteTexture(uint id)
        {
            GL.DeleteTextures(1, ref id);
            return id;
        }

        public RCode DestroyDevice()
        {
            return RCode.OK;
        }

        public void DisplayFrame()
        {
            glWindow.SwapBuffers();
        }

        public void DrawDecalQuad(DecalInstance decal)
        {
            throw new NotImplementedException();
        }

        public void DrawLayerQuad(vec2d_f offset, vec2d_f scale, Pixel tint)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(tint.r, tint.g, tint.b, tint.a);
            GL.TexCoord2(0.0f * scale.x + offset.x, 1.0f * scale.y + offset.y);
            GL.Vertex3(-1.0f, -1.0f, 0.0f);
            GL.TexCoord2(0.0f * scale.x + offset.x, 0.0f * scale.y + offset.y);
            GL.Vertex3(-1.0f, 1.0f, 0.0f);
            GL.TexCoord2(1.0f * scale.x + offset.x, 0.0f * scale.y + offset.y);
            GL.Vertex3(1.0f, 1.0f, 0.0f);
            GL.TexCoord2(1.0f * scale.x + offset.x, 1.0f * scale.y + offset.y);
            GL.Vertex3(1.0f, -1.0f, 0.0f);
            GL.End();
        }

        public void PrepareDevice()
        {
            
        }

        public void PrepareDrawing()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void UpdateTexture(uint id, Sprite spr)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)spr.Width, (int)spr.Height, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, spr.ColorData);
        }

        public void UpdateViewport(vec2d_i pos, vec2d_i size)
        {
            GL.Viewport(pos.x, pos.y, size.x, size.y);
        }
    }
}
