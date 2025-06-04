using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore.Extensions;

public abstract class PGEX
{
    protected readonly PixelGameEngine pge;

    public abstract void OnBeforeUserCreate();
    public abstract void OnAfterUserCreate();
    public abstract bool OnBeforeUserUpdate(float fElapsedTime);
    public abstract void OnAfterUserUpdate(float fElapsedTime);

    public PGEX(PixelGameEngine pge)
    {
        this.pge = pge;
    }
}
