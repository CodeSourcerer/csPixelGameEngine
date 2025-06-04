using System;
using csPixelGameEngineCore.Enums;
using Microsoft.Extensions.Logging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Windowing.Desktop;

namespace csPixelGameEngineCore;

public class Renderer_OGL21 : IRenderer
{
    private readonly GameWindow glWindow;
    private readonly ILogger<Renderer_OGL21> logger;

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

    public Renderer_OGL21(GameWindow gameWindow, ILogger<Renderer_OGL21> logger)
    {
        this.logger = logger;
        logger.LogDebug("Constructing GL21Renderer");

        if (gameWindow == null) throw new ArgumentNullException(nameof(gameWindow));

        glWindow = gameWindow;
        glWindow.RenderFrame += (sender, eventargs) =>
        {
            RenderFrame?.Invoke(sender, new FrameUpdateEventArgs(eventargs.Time));
        };
        //glWindow.RenderFrame += eventArgs =>
        //{
        //    RenderFrame?.Invoke(this, new FrameUpdateEventArgs(eventArgs.Time));
        //};
    }

    public void ApplyTexture(uint id)
    {
        GL.BindTexture(TextureTarget.Texture2D, id);
    }

    public void ClearBuffer(Pixel p, bool bDepth)
    {
        GL.ClearColor(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f);
        var clearBits = bDepth ? ClearBufferMask.DepthBufferBit : ClearBufferMask.ColorBufferBit;

        GL.Clear(clearBits);
    }

    public RCode CreateDevice(bool bFullScreen, bool bVSYNC, params object[] p)
    {
        logger.LogDebug("GL21Renderer.CreateDevice()");

        GL.Enable(EnableCap.Texture2D);
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        return RCode.OK;
    }

    public uint CreateTexture(int width, int height, bool filtered = false, bool clamp = true)
    {
        uint id = 0;
        GL.GenTextures(1, out id);
        GL.BindTexture(TextureTarget.Texture2D, id);
        if (filtered)
        {
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, [(uint)TextureMagFilter.Linear]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, [(uint)TextureMinFilter.Linear]);
        }
        else
        {
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, [(uint)TextureMagFilter.Nearest]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, [(uint)TextureMinFilter.Nearest]);
        }

        if (clamp)
        {
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, [(uint)TextureWrapMode.Clamp]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, [(uint)TextureWrapMode.Clamp]);
        }
        else
        {
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, [(uint)TextureWrapMode.Repeat]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, [(uint)TextureWrapMode.Repeat]);
        }

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
                //GL.Color4(decal.tint[n].r, decal.tint[n].g, decal.tint[n].b, decal.tint[n].a);
                GL.TexCoord4(decal.uv[n].x, decal.uv[n].y, 0.0f, decal.w[n]);
                GL.Vertex2(decal.pos[n].x, decal.pos[n].y);
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
        //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void UpdateTexture(uint id, Sprite spr)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, spr.Width, spr.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColorData);
    }

    public void ReadTexture(uint id, Sprite spr)
    {
        GL.ReadPixels(0, 0, spr.Width, spr.Height, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColorData);
    }

    public void UpdateViewport(vi2d pos, vi2d size)
    {
        GL.Viewport(pos.x, pos.y, size.x, size.y);
    }
}
