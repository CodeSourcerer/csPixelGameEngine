using System;
using csPixelGameEngineCore;
using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTK.Windowing.Desktop;
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
                .MinimumLevel.Verbose()
                .CreateLogger();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile($"{Environment.CurrentDirectory}/appsettings.json");

            var configuration = configBuilder.Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<GameWindow, GLWindow>()
                .AddSingleton<PixelGameEngine, PGEDemo>()
                .AddSingleton<IRenderer, Renderer_OGL33>()
                //.AddSingleton<IRenderer, GL21Renderer>()
                .AddSingleton<IPlatform, OpenTkPlatform>()
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

    private PixelGameEngine pge;

    public DemoApp(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void Run()
    {
        var configuration = serviceProvider.GetRequiredService<IOptions<ApplicationConfiguration>>();
        pge = serviceProvider.GetRequiredService<PixelGameEngine>();
        pge.Construct(configuration.Value.ScreenWidth, configuration.Value.ScreenHeight,
            configuration.Value.PixelWidth, configuration.Value.PixelHeight, false, false);

        // I know this is hacky, but... the original uses globals, so you do what you gotta do.
        var gameWindow = serviceProvider.GetRequiredService<GameWindow>();
        ((GLWindow)gameWindow).pge = pge;

        pge.Start();
    }
}
