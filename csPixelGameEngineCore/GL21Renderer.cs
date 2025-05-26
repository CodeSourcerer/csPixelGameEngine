using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using csPixelGameEngineCore.Enums;
using log4net;

namespace csPixelGameEngineCore
{
    public class GL21Renderer : IRenderer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GL21Renderer));
        private readonly GameWindow glWindow;

        private DecalMode _decalMode;
        public DecalMode DecalMode
        {
            get => _decalMode;
            set
            {
                if (value != _decalMode)
                {
                    switch (value)
                    {
                        case DecalMode.NORMAL:
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                            break;
                        case DecalMode.ADDITIVE:
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                            break;
                        case DecalMode.MULTIPLICATIVE:
                            GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.One);
                            break;
                        case DecalMode.STENCIL:
                            GL.BlendFunc(BlendingFactor.Zero, BlendingFactor.SrcAlpha);
                            break;
                        case DecalMode.ILLUMINATE:
                            GL.BlendFunc(BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha);
                            break;
                        case DecalMode.WIREFRAME:
                            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                            break;
                    }

                    _decalMode = value;
                }
            }
        }

        public event EventHandler<FrameUpdateEventArgs> RenderFrame;

        public GL21Renderer(GameWindow gameWindow)
        {
            Log.Debug("Constructing GL21Renderer");

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
            Log.Debug("GL21Renderer.CreateDevice()");

            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            return RCode.OK;
        }

        public uint CreateTexture(int width, int height, bool filtered = false, bool clamp = true)
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

        public void DrawDecal(DecalInstance decal)
        {
            DecalMode = decal.mode;

            if (decal.decal == null)
                GL.BindTexture(TextureTarget.Texture2D, 0);
            else
                GL.BindTexture(TextureTarget.Texture2D, decal.decal.Id);

            if (decal.depth)
            {
                GL.Enable(EnableCap.DepthTest);
            }

            if (DecalMode == DecalMode.WIREFRAME)
                GL.Begin(PrimitiveType.LineLoop);
            else
            {
                if (decal.structure == DecalStructure.FAN)
                    GL.Begin(PrimitiveType.TriangleFan);
                else if (decal.structure == DecalStructure.STRIP)
                    GL.Begin(PrimitiveType.TriangleStrip);
                else if (decal.structure == DecalStructure.LIST)
                    GL.Begin(PrimitiveType.Triangles);
            }

            if (decal.depth)
            {
                // Render as 3D Spatial Entity
                for (uint n = 0; n < decal.points; n++)
                {
                    GL.Color4(decal.tint[n].r, decal.tint[n].g, decal.tint[n].b, decal.tint[n].a);
                    GL.TexCoord4(decal.uv[n].x, decal.uv[n].y, 0.0f, decal.w[n]);
                    GL.Vertex3(decal.pos[n].x, decal.pos[n].y, decal.z[n]);
                }
            }
            else
            {
                // Render as 2D Spatial entity
                for (uint n = 0; n < decal.points; n++)
                {
                    GL.Color4(decal.tint[n].r, decal.tint[n].g, decal.tint[n].b, decal.tint[n].a);
                    GL.TexCoord4(decal.uv[0].x, decal.uv[0].y, 0.0f, decal.w[0]);
                    GL.Vertex2(decal.pos[0].x, decal.pos[0].y);
                }
            }
            
            GL.End();

            if (decal.depth)
            {
                GL.Disable(EnableCap.DepthTest);
            }
        }

        public void DrawLayerQuad(vf2d offset, vf2d scale, Pixel tint)
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
            DecalMode = DecalMode.NORMAL;
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void UpdateTexture(uint id, Sprite spr)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)spr.Width, (int)spr.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColorData);
        }

        public void UpdateViewport(vi2d pos, vi2d size)
        {
            GL.Viewport(pos.x, pos.y, size.x, size.y);
        }
    }
}
