using System;
using System.IO;
using System.Reflection;
using csPixelGameEngineCore;
using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTK;
using OpenTK.Graphics;
using Serilog;

namespace PixelGameEngineCoreTest;

// Normally we would extend the PixelGameEngine class here to be more in line with the original design,
// but in C++ PGE, platform and renderer are static globals, which is blasphemy (and not allowed) in C#.
// So we shall inject those objects into PGE like good civilized C# developers. This does however make
// instantiation different, so we will use this class to just set up DI and launch the PGE-derived class.
class DemoApp
{
    private IServiceProvider serviceProvider;

    static void Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.CurrentDirectory}/appsettings.json");

            var configuration = configBuilder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<GameWindow, GLWindow>()
                .AddSingleton<PGEDemo>()
                .AddScoped<IRenderer, GL21Renderer>()
                .AddScoped<IPlatform, OpenTkPlatform>()
                .Configure<ApplicationConfiguration>(configuration.GetSection("Application"))
                .AddLogging(builder =>
                {
                    var logConfig = new LoggerConfiguration().ReadFrom.Configuration(configuration);
                    builder.AddSerilog(logConfig.CreateLogger(), true);
                })
                .BuildServiceProvider();

            DemoApp app = new DemoApp(serviceProvider);
            app.Run();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private PGEDemo pge; // = new PixelGameEngine(AppName);

    private DateTime _dtStartFrame = DateTime.Now;
    private int _curFrameCount = 0;
    private int _fps = 0;
    private float _rotation = 0.0f;
    private float _rotationStep = (float)(-Math.PI / 32);
    private float _fullCircle = (float)(2 * Math.PI);

    public DemoApp(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void Run()
    {
        //testAnimation = new Sprite[10];
        //testAnimationDecal = new Decal[10];
        //loadTestAnimation();

        var configuration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>();
        pge = serviceProvider.GetRequiredService<PGEDemo>();
        pge.Construct(configuration.Value.ScreenWidth, configuration.Value.ScreenHeight,
            configuration.Value.PixelWidth, configuration.Value.PixelHeight, false, false);
        pge.Start();
    }

    //private void updateFrame(object sender, FrameUpdateEventArgs frameUpdateArgs)
    //{
    //    pge.Clear(Pixel.BLUE);
    //    pge.PixelMode = Pixel.Mode.NORMAL;
    //    //testAnimation[1].CopyTo(pge.DefaultDrawTarget, 0, 0, -100, -100);
    //    //pge.DrawSprite(0, 0, testAnimation[1]);
    //    //pge.DrawDecal(new vec2d_f(), testAnimationDecal[1]);
    //    //_rotation += _rotationStep % _fullCircle;
    //    //pge.DrawRotatedDecal(new vf2d(testAnimationDecal[1].sprite.Width / 2.0f, testAnimationDecal[1].sprite.Height / 2.0f),
    //    //                     testAnimationDecal[1],
    //    //                     _rotation,
    //    //                     new vf2d(testAnimationDecal[1].sprite.Width / 2.0f, testAnimationDecal[1].sprite.Height / 2.0f));
    //    //pge.DrawWarpedDecal(testAnimationDecal[1],
    //    //                    new vf2d[] {
    //    //                        new vf2d(400.0f, 200.0f),
    //    //                        new vf2d(780.0f, 550.0f),
    //    //                        new vf2d(10.0f,  500.0f),
    //    //                        new vf2d(200.0f, 120.0f)
    //    //                    });
    //    //showCursorPos(0, 20);
    //    //showMouseWheelDelta(0, 30);

    //    //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;

    //    //pge.DrawCircle(100, 100, 100, Pixel.RED);
    //    //pge.FillCircle(500, 500, 30, Pixel.GREEN);
    //    //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.ALPHA;
    //    //pge.FillTriangle(new vec2d_i(304, 200),
    //    //                 new vec2d_i(544, 381),
    //    //                 new vec2d_i(444, 500),
    //    //                 Pixel.MAGENTA);
    //    //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;


    //    //pge.DrawStringDecal(new vf2d(0, 10), $"FPS: {_fps}", Pixel.BLACK, new vf2d(1, 1));
    //}

    private void showCursorPos(int x, int y)
    {
        //pge.DrawStringDecal(x, y, $"Mouse: {pge.MousePos.x}, {pge.MousePos.y}", Pixel.BLACK);
    }

    private void showMouseWheelDelta(int x, int y)
    {
        //pge.DrawStringDecal(x, y, $"Wheel Delta: {pge.MouseWheelDelta}", Pixel.BLACK);
    }

}
