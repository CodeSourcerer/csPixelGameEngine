﻿using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace csPixelGameEngineCore;

/// <summary>
/// A GPU resident storage of a Sprite
/// </summary>
/// <remarks>
/// The original PGE does not require a renderer to be passed in, but that is because the renderer
/// is global.
/// </remarks>
public class Decal : IDisposable
{
    public int     Id       { get; private set; } = -1;
    public Sprite  sprite   { get; private set; }
    public vf2d    vUVScale { get; private set; } = new vf2d { x = 1.0f, y = 1.0f };

    private readonly IRenderer _renderer;

    public Decal(Sprite spr, IRenderer renderer, bool filter = false, bool clamp = true)
    {
        Id = -1;
        if (spr == null) return;
        sprite = spr;
        _renderer = renderer;
        Id = (int)renderer.CreateTexture(sprite.Width, sprite.Height, filter, clamp);
        Update();
    }

    /// <summary>
    /// Reuses an existing texture resource
    /// </summary>
    /// <param name="nExistingTextureResource"></param>
    /// <param name="spr">I honestly don't know the purpose of this... just don't pass a null</param>
    public Decal(int nExistingTextureResource, Sprite spr)
    {
        if (spr == null) return;

        Id = nExistingTextureResource;
    }

    public void Update()
    {
        if (sprite == null)
            return;

        if (Id >= 0)
        {
            vUVScale = new vf2d(1.0f / sprite.Width, 1.0f / sprite.Height);
            _renderer.ApplyTexture((uint)Id);
            _renderer.UpdateTexture((uint)Id, sprite);
        }
    }

    public void UpdateSprite()
    {
        if (sprite == null)
        {
            return;
        }

        _renderer.ApplyTexture((uint)Id);
        _renderer.ReadTexture((uint)Id, sprite);
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        Log.Logger.Debug("Decal.Dispose({disposing})", disposing);
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.
            if (Id != -1)
            {
                Log.Logger.Debug("Deleting texture [id:{id}]", Id);
                _renderer.DeleteTexture((uint)Id);
                Id = -1;
            }

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    ~Decal()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        GC.SuppressFinalize(this);
    }
    #endregion
}
