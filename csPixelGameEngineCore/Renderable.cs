using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

public class Renderable(IRenderer renderer)
{
    //public RCode Load(string file, ResourcePack pack = null, bool filter = false, bool clamp = true);
    public Decal Decal { get; set; }
    public Sprite Sprite { get; set; }

    public void Create(uint width, uint height, bool filter = false, bool clamp = true)
    {
        this.Sprite = new Sprite((int)width, (int)height);
        this.Decal = new Decal(Sprite, renderer, filter, clamp);
    }
}
