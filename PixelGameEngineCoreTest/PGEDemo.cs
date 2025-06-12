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

/// <summary>
/// PixelGameEngine Demo. Just like PGE, we extend the PixelGameEngine class.
/// </summary>
/// <param name="renderer"></param>
/// <param name="platform"></param>
/// <param name="config"></param>
/// <param name="logger"></param>
internal class PGEDemo(IRenderer renderer, IPlatform platform, IOptions<ApplicationConfiguration> config, ILogger<PGEDemo> logger)
    : PixelGameEngine(renderer, platform, logger, config.Value.AppName)
{
    private Random rnd = new Random();
    private DateTime dtStartFrame = DateTime.Now;
    private DateTime dtAnimation = DateTime.Now;
    private int animationFrame = 0;
    private int animationDirection = 1;
    private TimeSpan tsRandomCrap = TimeSpan.Zero;
    private uint randomCrapLayer;
    private uint textLayer;
    private int curFrameCount = 0;
    private int fps = 0;

    private Renderable[] animation;
    private Renderable texture;

    protected override bool OnUserCreate()
    {
        animation = loadTestAnimation();
        texture = new Renderable(renderer);
        texture.Load("texture.bmp", null);

        randomCrapLayer = CreateLayer();
        Layers[(int)randomCrapLayer].bShow = true;
        textLayer = CreateLayer();
        Layers[(int)textLayer].bShow = true;

        return true;
    }

    protected override bool OnUserUpdate(float fElapsedTime)
    {
        base.OnUserUpdate(fElapsedTime);

        Clear(csPGE.Pixel.BLUE);
        PixelMode = csPGE.Pixel.Mode.NORMAL;

        //drawRandomPixels();
        drawRandomStuff(fElapsedTime);
        drawAnimation();
        
        drawTo(textLayer, true);

        drawMouseButtonStates(0, 10);
        drawMousePosition(0, 50);
        drawScreenInfo(0, 60);
        showFPS(0, 0);
        showPGEFPS(100, 0);

        SetDrawTarget(null);

        return true;
    }

    protected override bool OnUserDestroy()
    {
        this.logger.LogInformation("PGEDemo.OnUserDestroy()");

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
        DrawDecal(new vf2d(0, 0), animation[animationFrame].Decal, new vf2d(0.5f, 0.5f), new Pixel(255, 255, 255, 255), csPGE.Enums.DecalMode.NORMAL);
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

    private struct GradientTriangle
    {
        public vi2d[] points { get; init; }
        public uint[] colors { get; init; }

        public GradientTriangle()
        {
            points = [new(), new(), new()];
            colors = [0, 0, 0];
        }
    }

    private GradientTriangle[] gradTris = [new(), new(), new(), new(), new()];

    /// <summary>
    /// So random!
    /// </summary>
    private void drawRandomStuff(float elapsedTime)
    {
        tsRandomCrap += TimeSpan.FromSeconds(elapsedTime);

        if (tsRandomCrap.TotalSeconds > 2)
        {
            tsRandomCrap = TimeSpan.Zero;

            drawTo(randomCrapLayer, true);

            // Draw some random lines
            for (int line = 0; line < 10; line++)
            {
                uint pattern = (uint)rnd.Next(int.MaxValue);
                uint color = (uint)rnd.Next(0xFFFFFF) | 0xFF000000;
                DrawLine(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y), rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y), color, pattern);
            }

            // Bigger lines
            for (int line = 0; line < 10; line++)
            {
                uint pattern = (uint)rnd.Next(int.MaxValue);
                uint color = (uint)rnd.Next(0xFFFFFF) | 0xFF000000;
                vi2d p1 = new(-rnd.Next(ScreenSize.x), -rnd.Next(ScreenSize.y));
                vi2d p2 = new(rnd.Next(ScreenSize.x) * 2, rnd.Next(ScreenSize.y) * 2);
                if (!ClipLineToScreen(ref p1, ref p2))
                {
                    // both points make a line that is not visible. Try again
                    //this.logger.LogDebug("Line not visible, generating another");
                    line--;
                    continue;
                }

                DrawLine(p1, p2, csPGE.Pixel.DARK_GREEN);
            }

            // Draw some random circles
            for (int circle = 0; circle < 10; circle++)
            {
                uint color = (uint)rnd.Next(0xFFFFFF) | 0xFF000000;
                DrawCircle(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y), rnd.Next(ScreenSize.y / 2), color);
                FillCircle(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y), rnd.Next(50), color);
            }

            // Draw some random triangles
            for (int tris = 0; tris < 5; tris++)
            {
                uint color = (uint)rnd.Next(0xFFFFFF) | 0xFF000000;
                vi2d p1 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                vi2d p2 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                vi2d p3 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                DrawTriangle(p1, p2, p3, color);

                p1 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                p2 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                p3 = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                FillTriangle(p1, p2, p3, color);

                gradTris[tris].points[0] = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                gradTris[tris].points[1] = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                gradTris[tris].points[2] = new(rnd.Next(ScreenSize.x), rnd.Next(ScreenSize.y));
                gradTris[tris].colors[0] = (uint)rnd.Next(int.MaxValue) | 0xFF000000;
                gradTris[tris].colors[1] = (uint)rnd.Next(int.MaxValue) | 0xFF000000;
                gradTris[tris].colors[2] = (uint)rnd.Next(int.MaxValue) | 0xFF000000;
            }

            //FillTexturedTriangle([new vi2d(0, 0), new vi2d(0, ScreenSize.y), new vi2d(ScreenSize.x, ScreenSize.y)],
            //                     [new vf2d(0, 0), new vf2d(0, 1), new vf2d(1, 1)],
            //                     [csPGE.Pixel.WHITE, csPGE.Pixel.WHITE, csPGE.Pixel.WHITE],
            //                     texture.Sprite);

            SetDrawTarget(null);
        }

        // Draw decals - they must be drawn every frame
        drawTo(randomCrapLayer, false);

        foreach (var tri in gradTris)
        {
            //FillTriangleDecal(tri.points[0], tri.points[1], tri.points[2], tri.colors[0]);
            GradientTriangleDecal(tri.points[0], tri.points[1], tri.points[2], tri.colors[0], tri.colors[1], tri.colors[2]);
        }

        SetDrawTarget(null);
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

    private void drawTo(uint layer, bool clear)
    {
        var ld = Layers[(int)layer];
        SetDrawTarget(ld.DrawTarget.Sprite);
        if (clear)
        {
            Clear(csPGE.Pixel.BLANK);
        }
        ld.bUpdate = true;
    }
}
