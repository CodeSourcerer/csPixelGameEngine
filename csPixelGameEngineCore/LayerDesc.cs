using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
    public class LayerDesc
    {
        public vec2d_f vOffset    { get; set; } = new vec2d_f();
        public vec2d_f vScale     { get; set; } = new vec2d_f(1.0f, 1.0f);
        public bool    bShow      { get; set; } = false;
        public bool    bUpdate    { get; set; } = false;
        public Sprite  DrawTarget { get; set; }
        public uint    ResID      { get; set; }
        public Pixel   Tint       { get; set; } = Pixel.WHITE;
        public Action  funcHook   { get; set; }
        public List<DecalInstance> DecalInstance { get; } = new List<DecalInstance>();
    }
}
