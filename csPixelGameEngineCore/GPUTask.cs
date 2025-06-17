using System.Runtime.InteropServices;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

public struct GPUTask
{
    [StructLayout(LayoutKind.Explicit, Size = VertexSize)]
    public struct Vertex
    {
        // sizeof doesn't work on this struct, so... this'll have to do
        public const int VertexSize = (sizeof(float) * 6) + sizeof(uint);

        [FieldOffset(0)]
        public float x;

        [FieldOffset(sizeof(float))]
        public float y;

        [FieldOffset(sizeof(float) * 2)]
        public float z;

        [FieldOffset(sizeof(float) * 3)]
        public float w;

        [FieldOffset(sizeof(float) * 4)]
        public float u;

        [FieldOffset(sizeof(float) * 5)]
        public float v;

        [FieldOffset(sizeof(float) * 6)]
        public uint c;

        public Vertex()
        {
            x = y = z = w = u = v = c = 0;
        }
    }

    public Vertex[] vb;
    public Decal decal;
    public DecalStructure structure;
    public DecalMode mode;
    public bool depth;
    public float[] mvp;
    public CullMode cull;
    public Pixel tint;

    public GPUTask()
    {
        vb = null;
        decal = null;
        structure = DecalStructure.FAN;
        mode = DecalMode.NORMAL;
        depth = false;
        mvp = [ 1.0f, 0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f ];
        cull = CullMode.NONE;
        tint = Pixel.WHITE;
    }
}
