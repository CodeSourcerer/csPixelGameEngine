using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPGE = csPixelGameEngineCore;
using csPixelGameEngineCore;
using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Options;

namespace PixelGameEngineCoreTest;

internal class PGEDemo : PixelGameEngine
{
    private Random rnd = new Random();
    private DateTime dtStartFrame = DateTime.Now;
    private DateTime dtAnimation = DateTime.Now;
    private int animationFrame = 0;
    private int animationDirection = 1;
    private int curFrameCount = 0;
    private int fps = 0;

    private Sprite[] animation;

    public PGEDemo(IRenderer renderer, IPlatform platform, IOptions<ApplicationConfiguration> config)
        : base(renderer, platform, config.Value.AppName)
    {
    }

    protected override bool OnUserCreate()
    {
        animation = loadTestAnimation();

        return true;
    }

    protected override bool OnUserUpdate(float fElapsedTime)
    {
        base.OnUserUpdate(fElapsedTime);

        Clear(csPGE.Pixel.BLUE);
        PixelMode = csPGE.Pixel.Mode.MASK;

        //drawRandomPixels();
        drawAnimation();
        drawMouseButtonStates();

        showFPS();

        return true;
    }

    private void showFPS()
    {
        curFrameCount++;
        if ((DateTime.Now - dtStartFrame) >= TimeSpan.FromSeconds(1))
        {
            fps = curFrameCount;
            curFrameCount = 0;
            dtStartFrame = DateTime.Now;
        }

        DrawString(0, 0, $"FPS: {fps}", csPGE.Pixel.WHITE);
    }

    /// <summary>
    /// Animate the character loaded from the resource pack
    /// </summary>
    private void drawAnimation()
    {
        if ((DateTime.Now - dtAnimation) >= TimeSpan.FromMilliseconds(50))
        {
            dtAnimation = DateTime.Now;
            animationFrame += animationDirection;
            if (animationFrame >= 8)
            {
                animationDirection = -1;
            }
            else if (animationFrame <= 0)
            {
                animationDirection = 1;
            }
        }
        DrawSprite(0, 0, animation[animationFrame]);
    }

    /// <summary>
    /// Draw a bunch of random pixels
    /// </summary>
    private void drawRandomPixels()
    {
        uint c = 0;
        for (int x = 0; x < ScreenSize.x; x++)
        {
            for (int y = 0; y < ScreenSize.y; y++)
            {
                // Random RGB value with 255 alpha
                c = (uint)rnd.Next(0xFFFFFF) | 0xFF000000;
                Draw(x, y, new Pixel(c));
            }
        }
    }

    /// <summary>
    /// Load a test animation from a resource pack
    /// </summary>
    /// <returns></returns>
    private Sprite[] loadTestAnimation()
    {
        var testAnimation = new Sprite[10];
        var rp = new ResourcePack();
        rp.LoadPack("./assets1.pack", "AReallyGoodKeyShouldBeUsed");

        // Images in pack go from 1 to 9
        for (int i = 1, spr_i = 0; i < 10; i++, spr_i++)
        {
            string file = $"./assets/Walking_00{i}.png";
            testAnimation[spr_i] = new Sprite(file, rp);
            //testAnimationDecal[i] = new Decal(testAnimation[i], serviceProvider.GetService<IRenderer>());
        }

        return testAnimation;
    }

    private void drawMouseButtonStates()
    {
        showMouseButtonState(0, 30, 0);
        showMouseButtonState(0, 40, 1);
        showMouseButtonState(0, 50, 2);
    }

    private void showMouseButtonState(int x, int y, uint button)
    {
        var btnState = GetMouse(button);

        string display = $"BTN {button} [Released:{btnState.Released}] [Pressed:{btnState.Pressed}] [Held: {btnState.Held}]";
        DrawString(x, y, display, csPGE.Pixel.WHITE);
    }
}
