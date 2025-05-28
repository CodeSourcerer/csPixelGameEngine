using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

/// <summary>
/// Convenince class to keep a sprite and a decal together
/// </summary>
/// <param name="renderer"></param>
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

    public RCode Load(string sFile, ResourcePack pack, bool filter = false, bool clamp = true)
    {
        try
        {
            Sprite = new Sprite(sFile, pack);
            Decal = new Decal(Sprite, renderer, filter, clamp);
            return RCode.OK;
        }
        catch
        {
            Sprite = null;
            return RCode.FAIL;
        }
    }
}
