using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPGE = csPixelGameEngineCore;
using csPixelGameEngineCore;
using csPixelGameEngineCore.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace PixelGameEngineCoreTest;

internal class PGEDemo(IRenderer renderer, IPlatform platform, IOptions<ApplicationConfiguration> config, ILogger<PGEDemo> logger)
    : PixelGameEngine(renderer, platform, logger, config.Value.AppName)
{
    private Random rnd = new Random();
    private DateTime dtStartFrame = DateTime.Now;
    private DateTime dtAnimation = DateTime.Now;
    private int animationFrame = 0;
    private int animationDirection = 1;
    private int curFrameCount = 0;
    private int fps = 0;

    private Renderable[] animation;

    protected override bool OnUserCreate()
    {
        animation = loadTestAnimation();
        return true;
    }

    protected override bool OnUserUpdate(float fElapsedTime)
    {
        base.OnUserUpdate(fElapsedTime);

        Clear(csPGE.Pixel.BLUE);
        PixelMode = csPGE.Pixel.Mode.NORMAL;

        //drawRandomPixels();
        drawAnimation();
        drawMouseButtonStates(0, 10);
        drawMousePosition(0, 50);
        drawScreenInfo(0, 60);

        showFPS(0, 0);
        showPGEFPS(100, 0);

        return true;
    }

    protected override bool OnUserDestroy()
    {
        logger.LogInformation("PGEDemo.OnUserDestroy()");

        foreach (var frame in animation)
        {
            frame.Dispose();
        }

        return base.OnUserDestroy();
    }

    private void showFPS(int x, int y)
    {
        curFrameCount++;
        if ((DateTime.Now - dtStartFrame) >= TimeSpan.FromSeconds(1))
        {
            fps = curFrameCount;
            curFrameCount = 0;
            dtStartFrame = DateTime.Now;
        }

        DrawString(x, y, $"FPS: {fps}", csPGE.Pixel.WHITE);
    }

    private void showPGEFPS(int x, int y)
    {
        DrawString(x, y, $"PGE FPS: {GetFPS()}", csPGE.Pixel.GREY);
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

        //DrawSprite(0, 0, animation[animationFrame].Sprite);
        DrawDecal(new vf2d(0, 0), animation[animationFrame].Decal, new vf2d(1, 1), new Pixel(0, 0, 255, 255), csPGE.Enums.DecalMode.NORMAL);
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
    private Renderable[] loadTestAnimation()
    {
        var testAnimation = new List<Renderable>(9);
        using var rp = new ResourcePack();
        rp.LoadPack("./assets1.pack", "AReallyGoodKeyShouldBeUsed");

        // Images in pack go from 1 to 9
        for (int i = 1; i < 10; i++)
        {
            string file = $"./assets/Walking_00{i}.png";
            var animFrame = new Renderable(renderer);
            if (animFrame.Load(file, rp) != csPGE.Enums.RCode.OK)
            {
                throw new Exception($"Failed to load image {file} from resource pack");
            }
            testAnimation.Add(animFrame);
        }

        return [..testAnimation];
    }

    private void drawMouseButtonStates(int x, int y)
    {
        drawMouseButtonState(x, y, 0);
        drawMouseButtonState(x, y + 10, 1);
        drawMouseButtonState(x, y + 20, 2);
    }

    private void drawMouseButtonState(int x, int y, uint button)
    {
        var btnState = GetMouse(button);

        string display = $"BTN {button} [Released:{btnState.Released}] [Pressed:{btnState.Pressed}] [Held: {btnState.Held}]";
        DrawString(x, y, display, csPGE.Pixel.WHITE);
    }

    private void drawMousePosition(int x, int y)
    {
        string display = $"Mouse Pos: [{MousePos.x}, {MousePos.y}] Mouse Window Pos: [{WindowMousePos.x}, {WindowMousePos.y}]";
        DrawString(x, y, display, csPGE.Pixel.WHITE);
    }

    private void drawScreenInfo(int x, int y)
    {
        DrawString(x, y, $"WindowPos: [{WindowPos.x}, {WindowPos.y}]", csPGE.Pixel.WHITE);
        DrawString(x, y + 10, $"WindowSize: [{WindowSize.x}, {WindowSize.y}]", csPGE.Pixel.WHITE);
        DrawString(x, y + 20, $"ScreenSize: [{ScreenSize.x}, {ScreenSize.y}]", csPGE.Pixel.WHITE);
        DrawString(x, y + 30, $"ScreenPixelSize: [{ScreenPixelSize.x},  {ScreenPixelSize.y}]", csPGE.Pixel.WHITE);
        DrawString(x, y + 40, $"ViewPos: [{ViewPos.x}, {ViewPos.y}]", csPGE.Pixel.WHITE);
        DrawString(x, y + 50, $"ViewSize: [{ViewSize.x}, {ViewSize.y}]", csPGE.Pixel.WHITE);
    }
}
