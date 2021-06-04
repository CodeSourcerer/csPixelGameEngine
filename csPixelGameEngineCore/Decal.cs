using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore
{
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
        public vec2d_f vUVScale { get; private set; } = new vec2d_f { x = 1.0f, y = 1.0f };

        private readonly IRenderer renderer;

        public Decal(Sprite spr, IRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));

            if (spr == null)
                return;

            sprite = spr;
            this.renderer = renderer;

            // I don't really like this here, but I'll leave it for now...
            Id = (int)this.renderer.CreateTexture(sprite.Width, sprite.Height);
            Update();
        }

        public void Update()
        {
            if (sprite == null)
                return;

            if (Id >= 0)
            {
                vUVScale = new vec2d_f(1.0f / sprite.Width, 1.0f / sprite.Height);
                renderer.ApplyTexture((uint)Id);
                renderer.UpdateTexture((uint)Id, sprite);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
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
                    renderer.DeleteTexture((uint)Id);
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
}
