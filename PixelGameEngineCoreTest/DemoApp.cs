using System;
using csPixelGameEngineCore;

namespace PixelGameEngineCoreTest
{
    class DemoApp
    {
        public  const string AppName        = "Test Application";
        private const uint   screenWidth    = 1024;
        private const uint   screenHeight   = 768;

        private ResourcePack rp;
        private Sprite[] testAnimation;

        static void Main(string[] args)
        {
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
            pge.Start();
        }

        private void updateFrame(object sender, FrameUpdateEventArgs frameUpdateArgs)
        {
            pge.Clear(Pixel.BLUE);
            pge.DrawSprite(0, 0, testAnimation[1]);

            _curFrameCount++;
            if ((DateTime.Now - _dtStartFrame) >= TimeSpan.FromSeconds(1))
            {
                _fps = _curFrameCount;
                _curFrameCount = 0;
                _dtStartFrame = DateTime.Now;
            }
            pge.DrawString(0, 0, $"FPS: {_fps}", Pixel.WHITE);
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
