using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    public class DecalInstance
    {
        public Decal     decal  { get; set; }
        public vf2d[]    pos    { get; private set; }
        public vf2d[]    uv     { get; private set; }
        public float[]   w      { get; private set; }
        public float[]   z      { get; private set; }
        public Pixel[]   tint   { get; set; }
        public uint      points { get; private set; } = 0;
        public bool      depth  { get; private set; } = false;
        public DecalMode mode   { get; set; } = DecalMode.NORMAL;
        public DecalStructure structure { get; set; } = DecalStructure.FAN;

        public DecalInstance()
        {
            pos = [new vf2d(), new vf2d(), new vf2d(), new vf2d()];
            uv = [new vf2d(0.0f, 0.0f), new vf2d(0.0f, 1.0f), new vf2d(1.0f, 1.0f), new vf2d(1.0f, 0.0f)];
            w = [1.0f, 1.0f, 1.0f, 1.0f];
        }
    }
}
