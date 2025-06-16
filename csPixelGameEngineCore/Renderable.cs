using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;
using Serilog;

namespace csPixelGameEngineCore;

/// <summary>
/// Convenince class to keep a sprite and a decal together
/// </summary>
/// <param name="renderer"></param>
public class Renderable(IRenderer renderer) : IDisposable
{
    private bool disposedValue;

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
            Log.Logger.Warning("Renderable failed to load image {file}", sFile);
            Sprite = null;
            return RCode.FAIL;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        Log.Logger.Debug("Renderable.Dispose({disposing})", disposing);

        if (!disposedValue)
        {
            if (disposing)
            {
                Decal?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Renderable()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
