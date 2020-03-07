using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore.Extensions
{
    public abstract class PGEX
    {
        protected readonly PixelGameEngine pge;

        public PGEX(PixelGameEngine pge)
        {
            this.pge = pge;
        }
    }
}
