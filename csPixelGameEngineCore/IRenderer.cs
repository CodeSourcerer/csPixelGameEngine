using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore;

/// <summary>
/// Representation of the rendering system.
/// </summary>
/// <remarks>
/// This roughly correlates to the Renderer class in PGE, which is pure abstract. I renamed it to
/// avoid name clashes.
/// </remarks>
public interface IRenderer
{
    void PrepareDevice();
    RCode CreateDevice(bool bFullScreen, bool bVSYNC, params object[] p);   // Slight deviation, but more C#-style
    RCode DestroyDevice();
    void DisplayFrame();
    void PrepareDrawing();
    void DrawLayerQuad(vf2d offset, vf2d scale, Pixel tint);
    void DrawDecal(DecalInstance decal);
    void DoGPUTask(GPUTask task);
    void Set3DProjection(float[] mat);
    uint CreateTexture(int width, int height, bool filtered = false, bool clamp = true);
    void UpdateTexture(uint id, Sprite spr);
    void ReadTexture(uint id, Sprite spr);
    uint DeleteTexture(uint id);
    void ApplyTexture(uint id);
    void UpdateViewport(vi2d pos, vi2d size);
    void ClearBuffer(Pixel p, bool bDepth);
    void SetDecalMode(DecalMode mode);

    DecalMode DecalMode { get; set; }

    event EventHandler<FrameUpdateEventArgs> RenderFrame;
}
