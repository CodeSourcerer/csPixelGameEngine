using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using csPixelGameEngine;

namespace TestApplication
{
    class Program
    {
        public const string AppName = "Test Application";

        static void Main(string[] args)
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
    }
}
