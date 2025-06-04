using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;
using Microsoft.Extensions.Logging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
//using OpenTK.Windowing.Desktop;

namespace csPixelGameEngineCore;

public class GL33Renderer : IRenderer
{
    public const int OLC_MAX_VERTS = 128;

    private readonly ILogger<GL33Renderer> logger;

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

    private readonly GameWindow tkGameWindow;
    private bool bSync = false;

    // Fragment shader
    private int m_nFS = 0;
    // Vertex shader
    private int m_nVS = 0;
    private int m_nQuadShader = 0;
    private int m_vbQuad = 0;
    private int m_vaQuad = 0;

    private Renderable rendBlankQuad;
    private locVertex[] vertexMem = new locVertex[OLC_MAX_VERTS];

    [StructLayout(LayoutKind.Sequential)]
    private struct locVertex
    {
        public float[] pos = new float[3]; // 6 bytes
        public vf2d tex; // 4 bytes
        public Pixel col; // 4 bytes

        public const int size = (sizeof(float) * 3) + (sizeof(float) * 2) + sizeof(int);

        public locVertex()
        {

        }

        public locVertex(float[] pos, vf2d tex, Pixel col)
        {
            this.pos = pos;
            this.tex = tex;
            this.col = col;
        }
    }

    public GL33Renderer(GameWindow gameWindow, ILogger<GL33Renderer> logger)
    {
        tkGameWindow = gameWindow;
        this.logger = logger;

        tkGameWindow.RenderFrame += (sender, eventArgs) => RenderFrame?.Invoke(sender, new FrameUpdateEventArgs(eventArgs.Time));

        logger.LogDebug("GL33Renderer created");
    }

    public void ApplyTexture(uint id)
    {
        GL.BindTexture(TextureTarget.Texture2D, id);
    }

    public void ClearBuffer(Pixel p, bool bDepth)
    {
        GL.ClearColor(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        if (bDepth)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }
    }

    public RCode CreateDevice(bool bFullScreen, bool bVSYNC, params object[] p)
    {
        bSync = bVSYNC;

        m_nFS = GL.CreateShader(ShaderType.FragmentShader);

        StringBuilder FS = new StringBuilder();
        FS.AppendLine("#version 330 core");
        FS.AppendLine("out vec4 pixel;");
        FS.AppendLine("in vec2 oTex;");
        FS.AppendLine("in vec4 oCol;");
        FS.AppendLine("uniform sampler2D sprTex;");
        FS.AppendLine("void main(){pixel = texture(sprTex, oTex) * oCol;}");

        GL.ShaderSource(m_nFS, FS.ToString());
        GL.CompileShader(m_nFS);

        m_nVS = GL.CreateShader(ShaderType.VertexShader);

        StringBuilder VS = new StringBuilder();
        VS.AppendLine("#version 330 core");
        VS.AppendLine("layout(location = 0) in vec3 aPos;");
        VS.AppendLine("layout(location = 1) in vec2 aTex;");
        VS.AppendLine("layout(location = 2) in vec4 aCol;");
        VS.AppendLine("out vec2 oTex;");
        VS.AppendLine("out vec4 oCol;");
        VS.AppendLine("void main(){ float p = 1.0 / aPos.z; gl_Position = p * vec4(aPos.x, aPos.y, 0.0, 1.0); oTex = p * aTex; oCol = aCol;}");
        GL.ShaderSource(m_nVS, VS.ToString());
        GL.CompileShader(m_nVS);

        m_nQuadShader = GL.CreateProgram();
        GL.AttachShader(m_nQuadShader, m_nFS);
        GL.AttachShader(m_nQuadShader, m_nVS);
        GL.LinkProgram(m_nQuadShader);

        // Create Quad
        GL.GenBuffers(1, out m_vbQuad);
        GL.GenVertexArrays(1, out m_vaQuad);
        GL.BindVertexArray(m_vaQuad);
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);

        locVertex[] verts = new locVertex[OLC_MAX_VERTS];
        GL.BufferData(BufferTarget.ArrayBuffer, locVertex.size * OLC_MAX_VERTS, verts, BufferUsageHint.StreamDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, locVertex.size, 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, locVertex.size, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, locVertex.size, 5 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Create blank texture for spriteless decals
        rendBlankQuad = new Renderable(this);
        rendBlankQuad.Create(1, 1);
        rendBlankQuad.Sprite.ColorData[0] = Pixel.GREEN;
        rendBlankQuad.Decal.Update();

        logger.LogInformation("GL 3.3 Device created");

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

    public RCode DestroyDevice() => RCode.OK;

    public void DisplayFrame()
    {
        tkGameWindow.SwapBuffers();
    }

    public void DrawDecal(DecalInstance decal)
    {
        DecalMode = decal.mode;

        GL.BindTexture(TextureTarget.Texture2D, decal.decal?.Id ?? rendBlankQuad.Decal.Id);

        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);

        for (uint i = 0; i < decal.points; i++)
        {
            vertexMem[i] = new locVertex([decal.pos[i].x, decal.pos[i].y, decal.w[i]], new vf2d(decal.uv[i].x, decal.uv[i].y), decal.tint[i]);
        }

        GL.BufferData(BufferTarget.ArrayBuffer, (int)(locVertex.size * decal.points), vertexMem, BufferUsageHint.StreamDraw);

        if (DecalMode == DecalMode.WIREFRAME)
        {
            GL.DrawArrays(PrimitiveType.LineLoop, 0, (int)decal.points);
        }
        else
        {
            switch (decal.structure)
            {
                case DecalStructure.FAN:
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (int)decal.points);
                    break;
                case DecalStructure.STRIP:
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, (int)decal.points);
                    break;
                case DecalStructure.LIST:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, (int)decal.points);
                    break;
            }
        }
    }

    public void DrawLayerQuad(vf2d offset, vf2d scale, Pixel tint)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);
        locVertex[] verts = [new locVertex([-1.0f, -1.0f, 1.0f], new vf2d(0.0f * scale.x + offset.x, 1.0f * scale.y + offset.y), tint),
                             new locVertex([ 1.0f, -1.0f, 1.0f], new vf2d(1.0f * scale.x + offset.x, 1.0f * scale.y + offset.y), tint),
                             new locVertex([-1.0f,  1.0f, 1.0f], new vf2d(0.0f * scale.x + offset.x, 0.0f * scale.y + offset.y), tint),
                             new locVertex([ 1.0f,  1.0f, 1.0f], new vf2d(1.0f * scale.x + offset.x, 0.0f * scale.y + offset.y), tint)];
        GL.BufferData(BufferTarget.ArrayBuffer, locVertex.size * 4, verts, BufferUsageHint.StreamDraw);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
    }

    public void PrepareDevice()
    {
        
    }

    public void PrepareDrawing()
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.UseProgram(m_nQuadShader);
        GL.BindVertexArray(m_vaQuad);
    }

    public void ReadTexture(uint id, Sprite spr)
    {
        GL.ReadPixels(0, 0, spr.Width, spr.Height, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColorData);
    }

    public void UpdateTexture(uint id, Sprite spr)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, spr.Width, spr.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColorData);
    }

    public void UpdateViewport(vi2d pos, vi2d size)
    {
        //logger.LogDebug("Updating viewport [pos.x,pos.y:{posX},{posY}] [size.x,size.y:{sizeX},{sizeY}]", pos.x, pos.y, size.x, size.y);
        GL.Viewport(pos.x, pos.y, size.x, size.y);
    }
}
