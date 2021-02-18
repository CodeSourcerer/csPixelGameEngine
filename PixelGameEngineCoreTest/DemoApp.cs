using System;
using System.IO;
using System.Reflection;
using csPixelGameEngineCore;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using OpenTK;
using OpenTK.Graphics;

namespace PixelGameEngineCoreTest
{
    class DemoApp
    {
        public  const string AppName        = "csPixelGameEngine Demo";
        private const int    screenWidth    = 1024;
        private const int    screenHeight   = 768;

        private ResourcePack rp;
        private Sprite[] testAnimation;
        private Decal[] testAnimationDecal;
        private static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            var gameWindow = new GameWindow(screenWidth, screenHeight, GraphicsMode.Default, AppName, GameWindowFlags.Default, DisplayDevice.Default, 2, 1,
                GraphicsContextFlags.Default);

            serviceProvider = new ServiceCollection()
                .AddSingleton(gameWindow)
                .AddScoped<IRenderer, GL21Renderer>()
                .AddScoped<IPlatform, OpenTkPlatform>()
                .BuildServiceProvider();

            DemoApp app = new DemoApp();
            app.Run();
        }

        private PixelGameEngine pge; // = new PixelGameEngine(AppName);

        private Random rnd = new Random();
        private DateTime _dtStartFrame = DateTime.Now;
        private int _curFrameCount = 0;
        private int _fps = 0;
        private float _rotation = 0.0f;
        private float _rotationStep = (float)(-Math.PI / 32);
        private float _fullCircle = (float)(2 * Math.PI);

        public void Run()
        {
            testAnimation = new Sprite[10];
            testAnimationDecal = new Decal[10];
            loadTestAnimation();

            //window = new GLWindow((int)screenWidth, (int)screenHeight, 1, 1, AppName);
            pge = new PixelGameEngine(serviceProvider.GetService<IRenderer>(), serviceProvider.GetService<IPlatform>(), AppName);
            pge.OnFrameUpdate += updateFrame;
            pge.Construct(screenWidth, screenHeight, 1, 1, false, false);
            pge.BlendFactor = 0.5f;
            pge.Start();
        }

        private void updateFrame(object sender, FrameUpdateEventArgs frameUpdateArgs)
        {
            pge.Clear(Pixel.BLUE);
            pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.MASK;
            //testAnimation[1].CopyTo(pge.DefaultDrawTarget, 0, 0, -100, -100);
            //pge.DrawSprite(0, 0, testAnimation[1]);
            //pge.DrawDecal(new vec2d_f(), testAnimationDecal[1]);
            _rotation += _rotationStep % _fullCircle;
            pge.DrawRotatedDecal(new vec2d_f(testAnimationDecal[1].sprite.Width / 2.0f, testAnimationDecal[1].sprite.Height / 2.0f),
                                 testAnimationDecal[1],
                                 _rotation,
                                 new vec2d_f(testAnimationDecal[1].sprite.Width / 2.0f, testAnimationDecal[1].sprite.Height / 2.0f));
            showCursorPos(0, 20);
            showMouseWheelDelta(0, 30);
            showMouseButtonState(0, 40, 0);
            showMouseButtonState(0, 50, 1);
            showMouseButtonState(0, 60, 2);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;

            //pge.DrawCircle(100, 100, 100, Pixel.RED);
            //pge.FillCircle(500, 500, 30, Pixel.GREEN);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.ALPHA;
            //pge.FillTriangle(new vec2d_i(304, 200),
            //                 new vec2d_i(544, 381),
            //                 new vec2d_i(444, 500),
            //                 Pixel.MAGENTA);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;

            //drawRandomPixels();

            _curFrameCount++;
            if ((DateTime.Now - _dtStartFrame) >= TimeSpan.FromSeconds(1))
            {
                _fps = _curFrameCount;
                _curFrameCount = 0;
                _dtStartFrame = DateTime.Now;
            }
            pge.DrawStringDecal(0, 10, $"FPS: {_fps}", Pixel.BLACK);
        }

        private void showCursorPos(int x, int y)
        {
            pge.DrawStringDecal(x, y, $"Mouse: {pge.MousePosX}, {pge.MousePosY}", Pixel.BLACK);
        }

        private void showMouseWheelDelta(int x, int y)
        {
            pge.DrawStringDecal(x, y, $"Wheel Delta: {pge.MouseWheelDelta}", Pixel.BLACK);
        }

        private void showMouseButtonState(int x, int y, uint button)
        {
            var btnState = pge.GetMouse(button);

            string display = $"BTN {button} [Released:{btnState.Released}] [Pressed:{btnState.Pressed}] [Held: {btnState.Held}]";
            pge.DrawStringDecal(x, y, display, Pixel.BLACK);
        }

        private void drawRandomPixels()
        {
            for (uint x = 0; x < pge.ScreenSize.x; x++)
                for (uint y = 0; y < pge.ScreenSize.y; y++)
                    pge.Draw(x, y, new Pixel((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255)));
        }

        // Example of loading a resource pack
        private void loadTestAnimation()
        {
            rp = new ResourcePack();
            rp.LoadPack("./assets1.pack", "AReallyGoodKeyShouldBeUsed");

            for (int i = 0; i < 10; i++)
            {
                string file = $"./assets/Walking_00{i}.png";
                testAnimation[i] = new Sprite(file, rp);
                testAnimationDecal[i] = new Decal(testAnimation[i], serviceProvider.GetService<IRenderer>());
            }
        }
    }
}
