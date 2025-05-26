using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPGE = csPixelGameEngineCore;
using csPixelGameEngineCore;
using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Options;

namespace PixelGameEngineCoreTest;

internal class PGEDemo : PixelGameEngine
{
    public PGEDemo(IRenderer renderer, IPlatform platform, IOptions<ApplicationConfiguration> config)
        : base(renderer, platform, config.Value.AppName)
    {
    }

    protected override bool OnUserCreate()
    {
        BlendFactor = 0.5f;

        return true;
    }

    protected override bool OnUserUpdate(float fElapsedTime)
    {
        base.OnUserUpdate(fElapsedTime);

        Clear(csPGE.Pixel.BLUE);
        PixelMode = csPGE.Pixel.Mode.NORMAL;

        return true;
    }
}
