using System;
using System.Collections.Generic;

namespace csPixelGameEngineCore;

public class LayerDesc(IRenderer renderer)
{
    public vf2d       vOffset    { get; set; } = new vf2d();
    public vf2d       vScale     { get; set; } = new vf2d(1.0f, 1.0f);
    public bool       bShow      { get; set; } = false;
    public bool       bUpdate    { get; set; } = false;
    public Renderable DrawTarget { get; set; } = new Renderable(renderer);
    public uint       ResID      { get; set; } = 0;
    public Pixel      Tint       { get; set; } = Pixel.WHITE;
    public Action     funcHook   { get; set; }
    public List<DecalInstance> DecalInstance { get; } = new List<DecalInstance>();
}
