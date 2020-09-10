using System;
using System.IO;
using System.Reflection;
using csPixelGameEngineCore;
using log4net;
using log4net.Config;

namespace PixelGameEngineCoreTest
{
    class DemoApp
    {
        public  const string AppName        = "csPixelGameEngine Demo";
        private const uint   screenWidth    = 1024;
        private const uint   screenHeight   = 768;

        private ResourcePack rp;
        private Sprite[] testAnimation;

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            DemoApp app = new DemoApp();
            app.Run();
        }

        private GLWindow window;
        private PixelGameEngine pge = new PixelGameEngine(AppName);

        private Random rnd = new Random();
        private DateTime _dtStartFrame = DateTime.Now;
        private int _curFrameCount = 0;
        private int _fps = 0;

        public void Run()
        {
            testAnimation = new Sprite[10];
            loadTestAnimation();

            window = new GLWindow((int)screenWidth, (int)screenHeight, 1, 1, AppName);
            pge.OnFrameUpdate += updateFrame;
            pge.Construct(screenWidth, screenHeight, window);
            pge.BlendFactor = 0.5f;
            pge.Start();
        }

        private void updateFrame(object sender, FrameUpdateEventArgs frameUpdateArgs)
        {
            pge.Clear(Pixel.BLUE);
            pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.MASK;
            //testAnimation[1].CopyTo(pge.DefaultDrawTarget, 0, 0, -100, -100);
            pge.DrawSprite(0, 0, testAnimation[1]);
            showCursorPos(0, 10);
            showMouseWheelDelta(0, 20);
            showMouseButtonState(0, 30, 0);
            showMouseButtonState(0, 40, 1);
            showMouseButtonState(0, 50, 2);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;

            //pge.DrawCircle(100, 100, 100, Pixel.RED);
            //pge.FillCircle(500, 500, 30, Pixel.GREEN);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.ALPHA;
            //pge.FillTriangle(new vec2d_i(304, 200),
            //                 new vec2d_i(544, 381),
            //                 new vec2d_i(444, 500),
            //                 Pixel.MAGENTA);
            //pge.PixelBlendMode = csPixelGameEngineCore.Enums.BlendMode.NORMAL;

            _curFrameCount++;
            if ((DateTime.Now - _dtStartFrame) >= TimeSpan.FromSeconds(1))
            {
                _fps = _curFrameCount;
                _curFrameCount = 0;
                _dtStartFrame = DateTime.Now;
            }
            pge.DrawString(0, 0, $"FPS: {_fps}", Pixel.BLACK);
        }

        private void showCursorPos(int x, int y)
        {
            pge.DrawString(x, y, $"Mouse: {pge.MousePosX}, {pge.MousePosY}", Pixel.BLACK);
        }

        private void showMouseWheelDelta(int x, int y)
        {
            pge.DrawString(x, y, $"Wheel Delta: {pge.MouseWheelDelta}", Pixel.BLACK);
        }

        private void showMouseButtonState(int x, int y, uint button)
        {
            var btnState = pge.GetMouse(button);

            string display = $"BTN {button} [Released:{btnState.Released}] [Pressed:{btnState.Pressed}] [Held: {btnState.Held}]";
            pge.DrawString(x, y, display, Pixel.BLACK);
        }

        private void drawRandomPixels()
        {
            for (uint x = 0; x < pge.ScreenWidth; x++)
                for (uint y = 0; y < pge.ScreenHeight; y++)
                    pge.Draw(x, y, new Pixel((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255)));
        }

        private void loadTestAnimation()
        {
            rp = new ResourcePack();
            rp.LoadPack("./assets1.pack", "AReallyGoodKeyShouldBeUsed");

            for (int i = 0; i < 10; i++)
            {
                string file = $"./assets/Walking_00{i}.png";
                testAnimation[i] = new Sprite(file, rp);
            }
        }
    }
}
