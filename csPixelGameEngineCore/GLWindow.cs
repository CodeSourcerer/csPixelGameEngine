using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Microsoft.Extensions.Options;
using csPixelGameEngineCore.Configuration;
//using OpenTK.Windowing.Desktop;
//using OpenTK.Mathematics;
//using OpenTK.Windowing.Common;
//using OpenTK.Windowing.GraphicsLibraryFramework;
using Microsoft.Extensions.DependencyInjection;

namespace csPixelGameEngineCore;

public class GLWindow : GameWindow
{
    //public Color4  BackgroundColor { get; set; }
    //public Sprite  DrawTarget      { get; set; }
    //public int     PixelWidth      { get; set; }
    //public int     PixelHeight     { get; set; }
    //public int     ScreenWidth     { get; set; }
    //public int     ScreenHeight    { get; set; }
    //public int     ViewWidth       { get; set; }
    //public int     ViewHeight      { get; set; }
    //public int     ViewX           { get; set; }
    //public int     ViewY           { get; set; }
    private PixelGameEngine _pge;
    public PixelGameEngine pge { get => _pge; set => _pge = _pge ?? value; }
    private readonly ApplicationConfiguration appConfig;

    //public GLWindow(IOptions<ApplicationConfiguration> config)
    //    : base(GameWindowSettings.Default, new NativeWindowSettings()
    //    {
    //        API         = ContextAPI.OpenGL,
    //        Flags       = ContextFlags.Default,
    //        Title       = config.Value.AppName,
    //        ClientSize  = new Vector2i(config.Value.ScreenWidth * config.Value.PixelWidth, config.Value.ScreenHeight * config.Value.PixelHeight),
    //        Vsync       = VSyncMode.Off,
    //        WindowState = WindowState.Normal
    //    })

    public GLWindow(IOptions<ApplicationConfiguration> appConfig)
        : base(appConfig.Value.ScreenWidth * appConfig.Value.PixelWidth, appConfig.Value.ScreenHeight * appConfig.Value.PixelHeight,
               GraphicsMode.Default, appConfig.Value.AppName, GameWindowFlags.Default, DisplayDevice.Default, 3, 3,
               GraphicsContextFlags.ForwardCompatible)
    {
        this.appConfig = appConfig.Value;
        //GraphicsMode.Default, GameWindowFlags.Default,
        //DisplayDevice.Default
        //Width           = appConfig.ScreenWidth * appConfig.PixelWidth;
        //Height          = appConfig.ScreenHeight * appConfig.PixelHeight;
        
        //ScreenWidth     = appConfig.ScreenWidth;
        //ScreenHeight    = appConfig.ScreenHeight;
        //DrawTarget      = new Sprite(appConfig.ScreenWidth, appConfig.ScreenHeight);
        //BackgroundColor = new Color4(255, 0, 0, 255);
        //PixelWidth      = appConfig.PixelWidth;
        //PixelHeight     = appConfig.PixelHeight;
        //ViewX           = 0;
        //ViewY           = 0;
        //VSync           = VSyncMode.Off;
    }

    //public GLWindow(int screen_width, int screen_height, int pixel_w, int pixel_h, string title)
    //    : base (screen_width * pixel_w, screen_height * pixel_h, GraphicsMode.Default, title,
    //            GameWindowFlags.Default, DisplayDevice.Default, 2, 1, GraphicsContextFlags.ForwardCompatible)
    //{
    //    Width           = screen_width * pixel_w;
    //    Height          = screen_height * pixel_h;
    //    ScreenWidth     = screen_width;
    //    ScreenHeight    = screen_height;
    //    DrawTarget      = new Sprite(screen_width, screen_height);
    //    BackgroundColor = new Color4(255, 0, 0, 255);
    //    PixelWidth      = pixel_w;
    //    PixelHeight     = pixel_h;
    //    ViewX           = 0;
    //    ViewY           = 0;
    //    VSync           = VSyncMode.Off;
    //}

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
        {
            Exit();
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        pge.olc_UpdateWindowSize(Width, Height);

        GL.Viewport(X, Y, Width, Height);
    }

    protected override void OnMove(EventArgs e)
    {
        base.OnMove(e);

        pge.olc_UpdateWindowPos(X, Y);
    }

    //protected override void OnUpdateFrame(FrameEventArgs e)
    //{
    //    if (KeyboardState.IsKeyDown(Keys.Escape))
    //    {
    //        this.Close();
    //    }

    //    base.OnUpdateFrame(e);
    //}

    //protected override void OnResize(ResizeEventArgs e)
    //{
    //    base.OnResize(e);

    //    pge.olc_UpdateWindowSize(e.Width, e.Height);

    //    GL.Viewport(0, 0, pge.ViewSize.x, pge.ViewSize.y);
    //}

    //protected override void OnMove(WindowPositionEventArgs e)
    //{
    //    base.OnMove(e);

    //    pge.olc_UpdateWindowPos(e.X, e.Y);
    //}

    //protected override void OnLoad(EventArgs e)
    //{
    //    GL.ClearColor(BackgroundColor);

    //    GL.Enable(EnableCap.Texture2D);
    //    int tex = GL.GenTexture();
    //    GL.BindTexture(TextureTarget.Texture2D, tex);
    //    GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)All.Nearest });
    //    GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)All.Nearest });
    //    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)All.Decal);
    //    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)ScreenWidth, (int)ScreenHeight, 0, PixelFormat.Rgba, PixelType.UnsignedInt8888, DrawTarget.ColorData);

    //    Width = ScreenWidth * PixelWidth;
    //    Height = ScreenHeight * PixelHeight;
    //    //ClientSize = new Size(Width, Height);

    //    base.OnLoad(e);
    //}

    //public void SetBackgroundColor(Pixel backColor)
    //{
    //    BackgroundColor = new Color4(backColor.r, backColor.g, backColor.b, backColor.a);
    //}
}
