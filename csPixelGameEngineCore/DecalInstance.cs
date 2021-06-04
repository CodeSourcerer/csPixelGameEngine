using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class DecalInstance
    {
        public Decal     decal  { get; set; }
        public vec2d_f[] pos    { get; private set; }
        public vec2d_f[] uv     { get; private set; }
        public float[]   w      { get; private set; }
        public Pixel     tint   { get; set; }

        public DecalInstance()
        {
            pos = new vec2d_f[] { new vec2d_f(), new vec2d_f(), new vec2d_f(), new vec2d_f() };
            uv = new vec2d_f[] { new vec2d_f(0.0f, 0.0f), new vec2d_f(0.0f, 1.0f), new vec2d_f(1.0f, 1.0f), new vec2d_f(1.0f, 0.0f) };
            w = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        }
    }
}
