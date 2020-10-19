using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    public interface IRenderer
    {
        void PrepareDevice();
        RCode CreateDevice(bool bFullScreen, bool bVSYNC, params object[] p);   // Slight deviation, but more C#-style
        RCode DestroyDevice();
        void DisplayFrame();
        void PrepareDrawing();
        void DrawLayerQuad(vec2d_f offset, vec2d_f scale, Pixel tint);
        void DrawDecalQuad(DecalInstance decal);
        uint CreateTexture(uint width, uint height);
        void UpdateTexture(uint id, Sprite spr);
        uint DeleteTexture(uint id);
        void ApplyTexture(uint id);
        void UpdateViewport(vec2d_i pos, vec2d_i size);
        void ClearBuffer(Pixel p, bool bDepth);

        event EventHandler<FrameUpdateEventArgs> RenderFrame;
    }
}
