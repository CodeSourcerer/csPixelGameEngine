using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Options;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace csPixelGameEngineCore;

public class GLWindow : GameWindow
{
    private PixelGameEngine _pge;
    public PixelGameEngine pge { get => _pge; set => _pge = _pge ?? value; }
    private readonly ApplicationConfiguration appConfig;

    public GLWindow(IOptions<ApplicationConfiguration> appConfig)
        : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            API = ContextAPI.OpenGL,
            Flags = ContextFlags.Default,
            Title = appConfig.Value.AppName,
            ClientSize = new Vector2i(appConfig.Value.ScreenWidth * appConfig.Value.PixelWidth, appConfig.Value.ScreenHeight * appConfig.Value.PixelHeight),
            Vsync = VSyncMode.Off,
            WindowState = WindowState.Normal
        })
    {
        this.appConfig = appConfig.Value;

    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Keys.Escape)
        {
            Close();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        pge.olc_UpdateWindowSize(e.Width, e.Height);

        GL.Viewport(pge.WindowPos.x, pge.WindowPos.y, pge.ViewSize.x, pge.ViewSize.y);
    }

    protected override void OnMove(WindowPositionEventArgs e)
    {
        base.OnMove(e);

        pge.olc_UpdateWindowPos(e.X, e.Y);
    }
}
