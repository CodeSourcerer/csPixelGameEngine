using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

/// <summary>
/// This is the ImageLoader class in PGE, which is essentially an interface - so I made it one.
/// </summary>
public interface IImageLoader
{
    RCode LoadImageResource(Sprite spr, string imageFile, ResourcePack pack);
    RCode SaveImageResource(Sprite spr, string imageFile);
}
