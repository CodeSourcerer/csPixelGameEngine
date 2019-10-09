using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using csPixelGameEngine;

namespace TestApplication
{
    class PixelGameEngineTests
    {
        public const string AppName = "Test Application";

        static void Main(string[] args)
        {
            PixelGameEngineTests tests = new PixelGameEngineTests();
            tests.RunTests();
        }

        public void RunDemo()
        {
            const uint screenWidth = 250;
            const uint screenHeight = 250;
            GLWindow window = new GLWindow((int)screenWidth, (int)screenHeight, 4, 4, AppName);
            PixelGameEngine pge = new PixelGameEngine(AppName);
            pge.Construct(screenWidth, screenHeight, window);
            pge.OnFrameUpdate += (sender, frameUpdateArgs) =>
            {
                Random rnd = new Random();

                for (uint x = 0; x < pge.ScreenWidth; x++)
                    for (uint y = 0; y < pge.ScreenHeight; y++)
                        pge.Draw(x, y, new Pixel((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255)));

                pge.DrawString(0, 0, "Sup?", Pixel.WHITE);
            };
            pge.Start();
        }

        public void RunTests()
        {
            PixelTests pixelTests = new PixelTests();

            bool testsPassed = true;

            try
            {
                pixelTests.ConstructorWithUintPixelValue();
                pixelTests.ConstructorWithRGBAPixelValue();
                pixelTests.ConstructorWithRGBPixelValue();
                pixelTests.ConstructorWithoutParameters();
                pixelTests.SettingRed_LeavesOtherColors();
                pixelTests.SettingGreen_LeavesOtherColors();
                pixelTests.SettingBlue_LeavesOtherColors();
                pixelTests.SettingAlpha_LeavesOtherColors();
            }
            catch (ApplicationException ae)
            {
                testsPassed = false;
                Console.WriteLine("[Failed: {0}]", ae.Message);
            }

            if (testsPassed)
                Console.WriteLine("All tests passed");
        }
    }
}
