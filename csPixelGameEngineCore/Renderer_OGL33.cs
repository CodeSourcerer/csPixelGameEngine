using System;
using System.Runtime.InteropServices;
using System.Text;
using csPixelGameEngineCore.Enums;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace csPixelGameEngineCore;

public class Renderer_OGL33 : IRenderer
{
    public const int OLC_MAX_VERTS = 128;

    private readonly ILogger<Renderer_OGL33> logger;

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

    private int m_uniMVP = 0;
    private int m_uniIs3D = 0;
    private int m_uniTint = 0;

    private Renderable rendBlankQuad;
    private locVertex[] vertexMem = new locVertex[OLC_MAX_VERTS];
    private float[] matProjection = [ 1, 0, 0, 0,
                                      0, 1, 0, 0,
                                      0, 0, 1, 0,
                                      0, 0, 0, 1 ];

    // sizeof doesn't work on this struct, so... this'll have to do
    public const int locVertexSize = (sizeof(float) * 6) + sizeof(uint);

    [StructLayout(LayoutKind.Explicit, Size = locVertexSize)]
    private struct locVertex
    {
        [FieldOffset(0)]
        public float posX;

        [FieldOffset(sizeof(float))]
        public float posY;

        [FieldOffset(sizeof(float) * 2)]
        public float posZ;

        [FieldOffset(sizeof(float) * 3)]
        public float w;

        [FieldOffset(sizeof(float) * 4)]
        public float texX;

        [FieldOffset(sizeof(float) * 5)]
        public float texY;

        [FieldOffset(sizeof(float) * 6)]
        public uint col;

        public locVertex()
        {

        }

        public locVertex(float[] pos, float[] tex, float w, Pixel col)
        {
            posX = pos[0];
            posY = pos[1];
            posZ = pos[2];
            this.w = w;
            texX = tex[0];
            texY = tex[1];
            this.col = col;
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="gameWindow">OpenTk GameWindow object. We need it here cause it does the double buffering swap-a-roo.</param>
    /// <param name="logger"></param>
    public Renderer_OGL33(GameWindow gameWindow, ILogger<Renderer_OGL33> logger)
    {
        tkGameWindow = gameWindow;
        this.logger = logger;

        tkGameWindow.RenderFrame += eventArgs => RenderFrame?.Invoke(this, new FrameUpdateEventArgs(eventArgs.Time));

        logger.LogDebug("GL33Renderer created. locVertexSize {locVertexSize}", locVertexSize);
    }

    public void SetDecalMode(DecalMode mode) => DecalMode = mode;

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
        tkGameWindow.VSync = bVSYNC ? VSyncMode.On : VSyncMode.Off;

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
        VS.AppendLine("layout(location = 0) in vec4 aPos;");
        VS.AppendLine("layout(location = 1) in vec2 aTex;");
        VS.AppendLine("layout(location = 2) in vec4 aCol;");
        VS.AppendLine("uniform mat4 mvp;");
        VS.AppendLine("uniform int is3d;");
        VS.AppendLine("uniform vec4 tint;");
        VS.AppendLine("out vec2 oTex;");
        VS.AppendLine("out vec4 oCol;");
        VS.AppendLine("void main(){ if(is3d!=0) {gl_Position = mvp * vec4(aPos.x, aPos.y, aPos.z, 1.0); oTex = aTex;} else {float p = 1.0 / aPos.z; gl_Position = p * vec4(aPos.x, aPos.y, 0.0, 1.0); oTex = p * aTex;} oCol = aCol * tint;}");
        GL.ShaderSource(m_nVS, VS.ToString());
        GL.CompileShader(m_nVS);

        m_nQuadShader = GL.CreateProgram();
        GL.AttachShader(m_nQuadShader, m_nFS);
        GL.AttachShader(m_nQuadShader, m_nVS);
        GL.LinkProgram(m_nQuadShader);

        m_uniMVP = GL.GetUniformLocation(m_nQuadShader, "mvp");
        m_uniIs3D = GL.GetUniformLocation(m_nQuadShader, "is3d");
        m_uniTint = GL.GetUniformLocation(m_nQuadShader, "tint");
        GL.Uniform1(m_uniIs3D, 0);
        GL.UniformMatrix4(m_uniMVP, 16, false, matProjection);
        float[] f = [ 100.0f, 100.0f, 100.0f, 100.0f ];
        GL.Uniform4(m_uniTint, 4, f);

        // Create Quad
        GL.GenBuffers(1, out m_vbQuad);
        GL.GenVertexArrays(1, out m_vaQuad);
        GL.BindVertexArray(m_vaQuad);
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);

        locVertex[] verts = new locVertex[OLC_MAX_VERTS];
        GL.BufferData(BufferTarget.ArrayBuffer, locVertexSize * OLC_MAX_VERTS, verts, BufferUsageHint.StreamDraw);
        GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, locVertexSize, 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, locVertexSize, 4 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, locVertexSize, 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        // Create blank texture for spriteless decals
        rendBlankQuad = new Renderable(this);
        rendBlankQuad.Create(1, 1);
        rendBlankQuad.Sprite.ColData[0] = Pixel.WHITE;
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
        GL.Disable(EnableCap.CullFace);
        DecalMode = decal.mode;

        GL.BindTexture(TextureTarget.Texture2D, decal.decal?.Id ?? rendBlankQuad.Decal.Id);

        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);

        for (uint i = 0; i < decal.points; i++)
        {
            vertexMem[i] = new locVertex([decal.pos[i].x, decal.pos[i].y, decal.w[i]], [decal.uv[i].x, decal.uv[i].y], 0.0f, decal.tint[i]);
        }

        GL.BufferData(BufferTarget.ArrayBuffer, (int)(locVertexSize * decal.points), vertexMem, BufferUsageHint.StreamDraw);
        GL.Uniform1(m_uniIs3D, 0);

        float[] f = [ 1.0f, 1.0f, 1.0f, 1.0f ];
        GL.Uniform4(m_uniTint, 1, f);

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
                case DecalStructure.LINE:
                    GL.DrawArrays(PrimitiveType.Lines, 0, (int)decal.points);
                    break;
            }
        }
    }

    public void DrawLayerQuad(vf2d offset, vf2d scale, Pixel tint)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);
        locVertex[] verts = [new locVertex([-1.0f, -1.0f, 1.0f], [0.0f * scale.x + offset.x, 1.0f * scale.y + offset.y], 0.0f, tint),
                             new locVertex([ 1.0f, -1.0f, 1.0f], [1.0f * scale.x + offset.x, 1.0f * scale.y + offset.y], 0.0f, tint),
                             new locVertex([-1.0f,  1.0f, 1.0f], [0.0f * scale.x + offset.x, 0.0f * scale.y + offset.y], 0.0f, tint),
                             new locVertex([ 1.0f,  1.0f, 1.0f], [1.0f * scale.x + offset.x, 0.0f * scale.y + offset.y], 0.0f, tint)];
        GL.BufferData(BufferTarget.ArrayBuffer, locVertexSize * 4, verts, BufferUsageHint.StreamDraw);

        GL.Uniform1(m_uniIs3D, 0);
        float[] f = [ 1.0f, 1.0f, 1.0f, 1.0f ];
        GL.Uniform4(m_uniTint, 1, f);

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
        float[] f = [ 1.0f, 1.0f, 1.0f, 1.0f ];
        GL.Uniform4(m_uniTint, 1, f);

        GL.Uniform1(m_uniIs3D, 0);
        GL.UniformMatrix4(m_uniMVP, 1, false, matProjection);
        GL.Disable(EnableCap.CullFace);
        GL.DepthFunc(DepthFunction.Less);
    }

    public void ReadTexture(uint id, Sprite spr)
    {
        GL.ReadPixels(0, 0, spr.Width, spr.Height, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColData);
    }

    public void UpdateTexture(uint id, Sprite spr)
    {
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, spr.Width, spr.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, spr.ColData);
    }

    public void UpdateViewport(vi2d pos, vi2d size)
    {
        //logger.LogDebug("Updating viewport [pos.x,pos.y:{posX},{posY}] [size.x,size.y:{sizeX},{sizeY}]", pos.x, pos.y, size.x, size.y);
        GL.Viewport(pos.x, pos.y, size.x, size.y);
    }

    public void Set3DProjection(float[] mat)
    {
        if (mat.Length != 16)
        {
            throw new ArgumentOutOfRangeException(nameof(mat), "Projection matrix must be an array of 16 floats");
        }

        matProjection = mat;
    }

    public void DoGPUTask(GPUTask task)
    {
        DecalMode = task.mode;
        if (task.decal == null)
        {
            GL.BindTexture(TextureTarget.Texture2D, rendBlankQuad.Decal.Id);
        }
        else
        {
            GL.BindTexture(TextureTarget.Texture2D, task.decal.Id);
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbQuad);

        GL.BufferData(BufferTarget.ArrayBuffer, GPUTask.Vertex.VertexSize * task.vb.Length, task.vb, BufferUsageHint.StreamDraw);

        // Use 3D shader
        GL.Uniform1(m_uniIs3D, 1);

        // Use MVP Matrix
        float[] matMVP = new float[16];
        for (int c = 0; c < 4; c++)
        {
            for (int r = 0; r < 4; r++)
            {
                matMVP[c * 4 + r] = matProjection[0 * 4 + r] * task.mvp[c * 4 + 0] +
                                    matProjection[1 * 4 + r] * task.mvp[c * 4 + 1] +
                                    matProjection[2 * 4 + r] * task.mvp[c * 4 + 2] +
                                    matProjection[3 * 4 + r] * task.mvp[c * 4 + 3];
            }
        }
        GL.UniformMatrix4(m_uniMVP, 1, false, matMVP);

        float[] f = [ task.tint.r / 255, task.tint.g / 255, task.tint.b / 255, task.tint.a / 255 ];
        GL.Uniform4(m_uniTint, 1, f);

        if (task.cull == CullMode.NONE)
        {
            GL.CullFace(TriangleFace.Front);
            GL.Disable(EnableCap.CullFace);
        }
        else if (task.cull == CullMode.CW)
        {
            GL.CullFace(TriangleFace.Front);
            GL.Enable(EnableCap.CullFace);
        }
        else if (task.cull == CullMode.CCW)
        {
            GL.CullFace(TriangleFace.Back);
            GL.Enable(EnableCap.CullFace);
        }

        if (task.depth)
        {
            GL.Enable(EnableCap.DepthTest);
        }

        if (DecalMode == DecalMode.WIREFRAME)
        {
            GL.DrawArrays(PrimitiveType.LineLoop, 0, task.vb.Length);
        }
        else
        {
            switch (task.structure)
            {
                case DecalStructure.FAN:
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, task.vb.Length);
                    break;

                case DecalStructure.STRIP:
                    GL.DrawArrays(PrimitiveType.TriangleStrip, 0, task.vb.Length);
                    break;

                case DecalStructure.LIST:
                    GL.DrawArrays(PrimitiveType.Triangles, 0, task.vb.Length);
                    break;

                case DecalStructure.LINE:
                    GL.DrawArrays(PrimitiveType.Lines, 0, task.vb.Length);
                    break;
            }
        }

        if (task.depth)
        {
            GL.Disable(EnableCap.DepthTest);
        }
    }
}
