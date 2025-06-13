using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using csPixelGameEngineCore.Enums;
using csPixelGameEngineCore.Extensions;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;
using static csPixelGameEngineCore.Sprite;

/*
	+-------------------------------------------------------------+
	|           OneLoneCoder Pixel Game Engine v2.28              |
	|  "What do you need? Pixels... Lots of Pixels..." - javidx9  |
	+-------------------------------------------------------------+

	What is this?
	~~~~~~~~~~~~~
    This is a C# port of the Pixel Game Engine.

	License (OLC-3)
	~~~~~~~~~~~~~~~

	Copyright 2018 - 2024 OneLoneCoder.com

	Redistribution and use in source and binary forms, with or without modification,
	are permitted provided that the following conditions are met:

	1. Redistributions or derivations of source code must retain the above copyright
	notice, this list of conditions and the following disclaimer.

	2. Redistributions or derivative works in binary form must reproduce the above
	copyright notice. This list of conditions and the following	disclaimer must be
	reproduced in the documentation and/or other materials provided with the distribution.

	3. Neither the name of the copyright holder nor the names of its contributors may
	be used to endorse or promote products derived from this software without specific
	prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS	"AS IS" AND ANY
	EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
	OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT
	SHALL THE COPYRIGHT	HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
	INCIDENTAL,	SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
	TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
	BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
	CONTRACT, STRICT LIABILITY, OR TORT	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
	ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
	SUCH DAMAGE.

Please refer to the original PixelGameEngine source here:
https://github.com/OneLoneCoder/olcPixelGameEngine

Notes from the Porting Author
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
I have tried to keep this as true to the original as I can, but I have taken the liberty
of making some changes. I deviate from the original in the following cases:

- It doesn't affect portability between the original
  Ex: Checking inputs to functions and throwing exceptions
      Using different, but similar data types
- Something isn't supported in C# that is in C++
  Ex: Generics in C# are not the same as templates in C++, so some changes
      had to be made there.
- C# Naming conventions vs C++
  Ex: C++ tends to use the hungarian notation for variables, C# does not.
*/
namespace csPixelGameEngineCore;

public delegate Pixel PixelBlender(int x, int y, Pixel pSrc, Pixel pDst);

public class PixelGameEngine
{
    protected readonly ILogger<PixelGameEngine> logger;

    public const byte TabSizeInSpaces = 4;

    #region Engine properties
    public string   AppName             { get; private set; }
    public bool     FullScreen          { get; private set; }
    public bool     EnableVSYNC         { get; private set; }
    public bool     ResizeRequested     { get; private set; } = false;
    public vi2d     ViewPos             { get; private set; } = new vi2d(0, 0);
    public vi2d     ViewSize            { get; private set; } = new vi2d(0, 0);
    public vi2d     WindowSize          { get; private set; } = new vi2d(0, 0);
    public vi2d     WindowPos           { get; private set; } = new vi2d(0, 0);
    public int      DrawTargetWidth     { get; private set; }
    public int      DrawTargetHeight    { get; private set; }
    public Sprite   DrawTarget          { get; private set; }
    public vi2d     MousePos            { get; private set; } = new vi2d(0, 0);
    public vi2d     WindowMousePos      { get; private set; } = new vi2d(0, 0);
    public int      MouseWheelDelta     { get; private set; }
    public vi2d     PixelSize           { get; private set; } = new vi2d(4, 4);
    public vi2d     ScreenPixelSize     { get; private set; } = new vi2d(4, 4);
    public vi2d     ScreenSize          { get; private set; } = new vi2d(256, 240);
    public vf2d     InvScreenSize       { get; private set; } = new vf2d(1.0f / 256.0f, 1.0f / 240.0f);
    public vi2d[]   FontSpacing         { get; private set; }
    public double   LastElapsed         { get; private set; } = 0.0;
    public double   FrameTimer          { get; private set; } = 1.0;
    public uint     FrameCount          { get; private set; } = 0;
    public uint     LastFPS             { get; private set; } = 0;
    public uint     FPS                 { get; private set; }
    public DecalMode DecalMode          { get; set; } = DecalMode.NORMAL;
    public DecalStructure DecalStructure { get; set; } = DecalStructure.FAN;

    /// <summary>
    /// Pixel aspect ratio
    /// </summary>
    public vf2d     Pixel               { get; private set; } = new vf2d(1.0f, 1.0f);
    public uint     TargetLayer         { get; private set; }
    public List<LayerDesc>  Layers      { get; private set; } = new List<LayerDesc>();
    
    private List<PGEX> _extensions = new List<PGEX>();

    private float blendFactor;
    public float BlendFactor
    {
        get => blendFactor;

        set => blendFactor = value switch
        {
            _ when value < 0.0f => 0.0f,
            _ when value > 1.0f => 1.0f,
            _ => value
        };
    }

    private PixelBlender funcPixelBlender;
    public PixelBlender CustomPixelBlender
    {
        get => funcPixelBlender;

        set
        {
            // Reset blend mode to normal if blender function is removed
            PixelMode = value == null ? csPixelGameEngineCore.Pixel.Mode.NORMAL : csPixelGameEngineCore.Pixel.Mode.CUSTOM;

            funcPixelBlender = value;
        }
    }

    private Pixel.Mode _pixelBlendMode = csPixelGameEngineCore.Pixel.Mode.NORMAL;
    public Pixel.Mode PixelMode
    {
        get => _pixelBlendMode;

        set
        {
            // Don't allow custom blender if no blender function defined
            if (funcPixelBlender == null && value == csPixelGameEngineCore.Pixel.Mode.CUSTOM)
                return;

            _pixelBlendMode = value;
        }
    }

    private bool _bPixelCohesion = false;
    private bool _bRealWindowMode = false;
    // Timing vars
    private long _tp1 = 0;
    private long _tp2 = 0;

    protected readonly IRenderer renderer;
    public readonly IPlatform Platform;
    private Renderable fontRenderable;

    // Keyboard and mouse states
    private bool[] keyNewState = new bool[256];
    private bool[] keyOldState = new bool[256];
    private HWButton[] keyboardState = new HWButton[256];

    public const byte MouseButtons = 5;
    private HWButton[] btnStates = [ new (), new (), new (), new (), new ()];

    #endregion // Engine properties

    #region Events

    /// <summary>
    /// This event fires once per frame and replaces OnUserUpdate()
    /// </summary>
    public event FrameUpdateEventHandler OnFrameUpdate;

    public event FrameUpdateEventHandler OnFrameRender;

    /// <summary>
    /// Event that fires before starting main loop. Use to load resources
    /// </summary>
    public event EventHandler OnCreate;

    /// <summary>
    /// Event that fires after window closes.
    /// </summary>
    public event EventHandler<CancelEventArgs> OnDestroy;

    #endregion // Events

    public PixelGameEngine(IRenderer renderer, IPlatform platform, ILogger<PixelGameEngine> logger, string appName = null)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (platform == null) throw new ArgumentNullException(nameof(platform));

        this.logger = logger;
        this.renderer = renderer;
        this.Platform = platform;

        AppName = string.IsNullOrWhiteSpace(appName) ? "Undefined" : appName;

        olc_ConfigureSystem();
    }

    /// <summary>
    /// Build the engine object with the given window.
    /// </summary>
    /// <param name="screen_w"></param>
    /// <param name="screen_h"></param>
    /// <param name="window"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    /// Original returns an error code. I opted to throw an exception instead.
    /// </remarks>
    public RCode Construct(int screen_w, int screen_h, int pixel_w, int pixel_h, bool full_screen = false,
        bool vsync = false, bool cohesion = false, bool realwindow = false)
    {
        if (screen_w <= 0) throw new ArgumentException("Must be at least 1", nameof(screen_w));
        if (screen_h <= 0) throw new ArgumentException("Must be at least 1", nameof(screen_h));
        if (pixel_w <= 0) throw new ArgumentException("Must be at least 1", nameof(pixel_w));
        if (pixel_h <= 0) throw new ArgumentException("Must be at least 1", nameof(pixel_h));

        _bPixelCohesion = cohesion;
        _bRealWindowMode = realwindow;
        ScreenSize = new vi2d(screen_w, screen_h);
        InvScreenSize = new vf2d(1.0f / screen_w, 1.0f / screen_h);
        PixelSize = new vi2d(pixel_w, pixel_h);
        WindowSize = ScreenSize * PixelSize;
        FullScreen = full_screen;
        EnableVSYNC = vsync;
        Pixel = 2.0f / ScreenSize;

        HWButton keyState = new();
        Span<HWButton> keyStates = new Span<HWButton>(keyboardState);
        keyStates.Fill(keyState);

        return RCode.OK;
    }

    private void construct_fontSheet()
    {
        logger.LogInformation("Constructing font sheet");

        StringBuilder data = new StringBuilder(1024);
        data.Append("?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000");
        data.Append("O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400");
        data.Append("Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000");
        data.Append("720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000");
        data.Append("OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000");
        data.Append("ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000");
        data.Append("Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000");
        data.Append("70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000");
        data.Append("OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000");
        data.Append("00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000");
        data.Append("<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000");
        data.Append("O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000");
        data.Append("00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000");
        data.Append("Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0");
        data.Append("O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000");
        data.Append("?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020");

        fontRenderable = new Renderable(renderer);
        fontRenderable.Create(128, 48);

        int px = 0, py = 0;
        for (int b = 0; b < 1024; b += 4)
        {
            uint sym1 = (uint)data[b + 0] - 48;
            uint sym2 = (uint)data[b + 1] - 48;
            uint sym3 = (uint)data[b + 2] - 48;
            uint sym4 = (uint)data[b + 3] - 48;
            uint r = sym1 << 18 | sym2 << 12 | sym3 << 6 | sym4;

            for (int i = 0; i < 24; i++)
            {
                byte k = (r & (1 << i)) != 0 ? (byte)255 : (byte)0;
                fontRenderable.Sprite.SetPixel(px, py, new Pixel(k, k, k, k));
                if (++py == 48) { px++; py = 0; }
            }
        }

        fontRenderable.Decal.Update();

        byte[] spacing = [
            0x03,0x25,0x16,0x08,0x07,0x08,0x08,0x04,0x15,0x15,0x08,0x07,0x15,0x07,0x24,0x08,
            0x08,0x17,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x24,0x15,0x06,0x07,0x16,0x17,
            0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x17,0x08,0x08,0x17,0x08,0x08,0x08,
            0x08,0x08,0x08,0x08,0x17,0x08,0x08,0x08,0x08,0x17,0x08,0x15,0x08,0x15,0x08,0x08,
            0x24,0x18,0x17,0x17,0x17,0x17,0x17,0x17,0x17,0x33,0x17,0x17,0x33,0x18,0x17,0x17,
            0x17,0x17,0x17,0x17,0x07,0x17,0x17,0x18,0x18,0x17,0x17,0x07,0x33,0x07,0x08,0x00 ];

        FontSpacing = (from c in spacing select new vi2d(c >> 4, c & 15)).ToArray();
    }

    public Sprite GetFontSprite() => fontRenderable.Sprite;

    public uint GetFPS() => LastFPS;
    public double GetElapsedTime() => LastElapsed;

    public HWButton GetKey(Key k) => keyboardState[(byte)k];

    #region Screen / Window attributes

    /// <summary>
    /// Set the screen size
    /// </summary>
    /// <param name="w">Width, in pixels</param>
    /// <param name="h">Height, in pixels</param>
    public void SetScreenSize(int w, int h)
    {
        if (w <= 0) throw new ArgumentException("Must be at least 1", nameof(w));
        if (h <= 0) throw new ArgumentException("Must be at least 1", nameof(h));

        ScreenSize.x = w;
        ScreenSize.y = h;

        InvScreenSize = 1.0f / ScreenSize;

        foreach (var layer in Layers)
        {
            layer.DrawTarget.Create((uint)ScreenSize.x, (uint)ScreenSize.y);
            layer.bUpdate = true;
        }

        SetDrawTarget(0);

        if (!_bRealWindowMode)
        {
            renderer.ClearBuffer(csPixelGameEngineCore.Pixel.BLACK, true);
            renderer.DisplayFrame();
            renderer.ClearBuffer(csPixelGameEngineCore.Pixel.BLACK, true);
        }

        renderer.UpdateViewport(ViewPos, ViewSize);
    }

    public RCode SetWindowSize(vi2d vPos, vi2d vSize)
    {
        return Platform.SetWindowSize(vPos, vSize);
    }

    // For compatability
    public int ScreenWidth() => ScreenSize.x;
    public int ScreenHeight() => ScreenSize.y;
    public vi2d GetScreenSize() => ScreenSize;
    public vi2d GetWindowSize() => WindowSize;
    public vi2d GetWindowPos() => WindowPos;
    public vi2d GetPixelSize() => PixelSize;
    public vi2d GetScreenPixelSize() => ScreenPixelSize;

    #endregion // Screen / Window attributes

    /// <summary>
    /// This starts the engine and the rendering loop.
    /// </summary>
    /// <param name="maxUpdateRate"></param>
    /// <returns></returns>
    public RCode Start()
    {
        logger.LogDebug("PixelGameEngine.Start()");

        if (Platform.ApplicationStartUp() != RCode.OK) return RCode.FAIL;

        if (Platform.CreateWindowPane(new vi2d(30, 30), WindowSize, FullScreen) != RCode.OK) return RCode.FAIL;
        olc_UpdateWindowSize(WindowSize.x, WindowSize.y);

        // The C++ implementation creates an engine thread here, which does a whole lot of things .NET handles for us.
        // Instead, we will do the gist of what it was doing by using event handlers.
        olc_PrepareEngine();

        _extensions.ForEach(ext => ext.OnBeforeUserCreate());
        if (!OnUserCreate()) return RCode.FAIL;
        _extensions.ForEach(ext => ext.OnAfterUserCreate());

        // This simulates "OnUserCreate()"
        OnCreate?.Invoke(this, new EventArgs());

        Platform.Closing += (sender, eventArgs) =>
        {
            if (!OnUserDestroy())
            {
                eventArgs.Cancel = true;
            }
            OnDestroy?.Invoke(sender, eventArgs);
        };

        Platform.Resize += (sender, eventArgs) =>
        {
            olc_UpdateWindowSize(Platform.WindowWidth, Platform.WindowHeight);
        };

        Platform.Move += (sender, eventArgs) => olc_UpdateWindowPos(Platform.WindowPosX, Platform.WindowPosY);

        Platform.UpdateFrame += (sender, frameEventArgs) =>
        {
            //Log.Debug("Platform.UpdateFrame handler");
            OnFrameUpdate?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.ElapsedTime));
            // Reset wheel delta after frame
            MouseWheelDelta = 0;
        };

        renderer.RenderFrame += (sender, frameEventArgs) =>
        {
            //Log.Debug("Renderer.RenderFrame handler");
            // This mainly does rendering, which is why I put it here.
            // In PGE, this is public but not overridable... it doesn't appear to be intended to be called by anything
            // outside of the engine, so I may break this up.
            olc_CoreUpdate();
            OnFrameRender?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.ElapsedTime));
        };

        Platform.KeyDown += (sender, eventArgs) =>
        {
            logger.LogDebug("Key pressed: {key}, ScanCode: {scanCode}", eventArgs.PressedKey, eventArgs.ScanCode);
            olc_UpdateKeyState((int)eventArgs.PressedKey, true);
        };

        Platform.KeyUp += (sender, eventArgs) => olc_UpdateKeyState((int)eventArgs.PressedKey, false);

        Platform.MouseWheel += (sender, mouseWheelEventArgs) =>
        {
            MouseWheelDelta = mouseWheelEventArgs.OffsetY;
        };

        Platform.MouseMove += (sender, mouseMoveEventArgs) =>
        {
            MousePos.x = mouseMoveEventArgs.X;
            MousePos.y = mouseMoveEventArgs.Y;
        };

        Platform.MouseDown += (sender, mouseButtonEventArgs) =>
        {
            updateMouseButtonStates(mouseButtonEventArgs);
        };

        Platform.MouseUp += (sender, mouseButtonEventArgs) =>
        {
            updateMouseButtonStates(mouseButtonEventArgs);
        };

        Platform.StartSystemEventLoop();

        return 0;
    }

    // Externalized API

    /// <summary>
    /// Looking at what this is used for in the C++ code, this is completely unnecessary in C#.
    /// </summary>
    public void olc_ConfigureSystem()
    {

    }

    public void olc_Terminate()
    {

    }

    public void olc_Reanimate()
    {

    }

    public bool olc_IsRunning() => true;

    public void olc_DropFiles(int x, int y, string[] files)
    {

    }

    public void olc_UpdateMouse(int x, int y)
    {

    }

    public void olc_UpdateMouseWheel(int delta)
    {

    }

    public void olc_UpdateMouseState(int button, bool state)
    {

    }

    public void olc_UpdateMouseFocus(bool state)
    {

    }

    public void olc_ConstructFontSheet()
    {

    }

    public void olc_UpdateKeyState(int key, bool state) => keyNewState[key] = state;

    public void olc_UpdateKeyFocus(bool state)
    {

    }

    public void olc_PrepareEngine()
    {
        if (Platform.CreateGraphics(FullScreen, EnableVSYNC, ViewPos, ViewSize) == RCode.FAIL) return;

        construct_fontSheet();

        renderer.CreateDevice(FullScreen, EnableVSYNC, ViewPos, ViewSize);

        // Create Primary layer "0"
        CreateLayer();
        Layers[0].bUpdate = true;
        Layers[0].bShow = true;
        SetDrawTarget(0);
        //DrawTarget = Layers[0].DrawTarget;

        _tp1 = DateTime.Now.Ticks;
        _tp2 = DateTime.Now.Ticks;
    }

    public void olc_CoreUpdate()
    {
        _tp2 = DateTime.Now.Ticks;
        TimeSpan tsElapsed = TimeSpan.FromTicks(_tp2 - _tp1);
        _tp1 = DateTime.Now.Ticks;

        var elapsedTime = tsElapsed.TotalSeconds;
        LastElapsed = elapsedTime;

        // TODO:
        //if (bConsoleSuspendTime)
        //    fElapsedTime = 0.0f;

        Action<HWButton[], bool[], bool[], uint> ScanHardware = (keys, oldState, newState, keyCount) =>
        {
            for (uint i = 0; i < keyCount; i++)
            {
                keys[i].Pressed = false;
                keys[i].Released = false;
                if (newState[i] != oldState[i])
                {
                    if (newState[i])
                    {
                        keys[i].Pressed = !keys[i].Held;
                        keys[i].Held = true;
                    }
                    else
                    {
                        keys[i].Released = true;
                        keys[i].Held = false;
                    }
                }
                oldState[i] = newState[i];
            }
        };

        ScanHardware(keyboardState, keyOldState, keyNewState, 256);

        // Handle mouse button held state
        for (int btn = 0; btn < btnStates.Length; btn++)
        {
            btnStates[btn].Held = btnStates[btn].Pressed;
        }

        OnUserUpdate((float)tsElapsed.TotalSeconds);

        if (_bRealWindowMode)
        {
            PixelSize = new(1, 1);
            ViewSize = ScreenSize;
            ViewPos = new(0, 0);
        }

        // TODO: add "ManualRenderEnable" check
        // TODO - implement this:
        // if (bConsoleShow)
        // {
        //     SetDrawTarget((uint8_t)0);
        //     UpdateConsole();
        // }

        // Display Frame
        renderer.UpdateViewport(ViewPos, ViewSize);
        renderer.ClearBuffer(csPixelGameEngineCore.Pixel.MAGENTA, true);

        // Ensure layer 0 is active
        Layers[0].bUpdate = true;
        Layers[0].bShow = true;
        DecalMode = DecalMode.NORMAL;
        renderer.PrepareDrawing();

        // Draw all active layers
        foreach (var layer in Layers)
        {
            if (layer.bShow)
            {
                if (layer.funcHook == null)
                {
                    renderer.ApplyTexture((uint)layer.DrawTarget.Decal.Id);
                    if (layer.bUpdate /* && !bSuspendTextureTransfer */) // TODO: Implement whatever the hell this is for
                    {
                        layer.DrawTarget.Decal.Update();
                        layer.bUpdate = false;
                    }

                    renderer.DrawLayerQuad(layer.vOffset, layer.vScale, layer.Tint);

                    // Display decals in order for this layer
                    foreach (var decal in layer.DecalInstance)
                    {
                        renderer.DrawDecal(decal);
                    }

                    layer.DecalInstance.Clear();
                }
                else
                {
                    // Yee-haw!
                    layer.funcHook();
                }
            }
        }

        renderer.DisplayFrame();

        if (ResizeRequested)
        {
            ResizeRequested = false;
            SetScreenSize(WindowSize.x, WindowSize.y);
        }

        FrameTimer += elapsedTime;
        FrameCount++;
        if (FrameTimer >= 1.0)
        {
            LastFPS = FrameCount;
            FrameTimer -= 1.0;
            FrameCount = 0;
        }
    }

    public void olc_UpdateWindowPos(int x, int y)
    {
        WindowPos.x = x;
        WindowPos.y = y;

        olc_UpdateViewport();
    }

    public void olc_UpdateWindowSize(int width, int height)
    {
        WindowSize.x = width;
        WindowSize.y = height;

        if (_bRealWindowMode)
        {
            //vResizeRequested = WindowSize;
            ResizeRequested = true;
        }

        olc_UpdateViewport();
    }

    public void olc_UpdateViewport()
    {
        if (_bRealWindowMode)
        {
            PixelSize.x = 1;
            PixelSize.y = 1;
            ViewSize = ScreenSize;
            ViewPos.x = 0;
            ViewPos.y = 0;
            
            return;
        }

        int windowWidth = ScreenSize.x * PixelSize.x;
        int windowHeight = ScreenSize.y * PixelSize.y;
        float windowAspectRatio = windowWidth / (float)windowHeight;

        if (_bPixelCohesion)
        {
            ScreenPixelSize = WindowSize / ScreenSize;
            ViewSize = (WindowSize / ScreenSize) * ScreenSize;
        }
        else
        {
            ViewSize.x = WindowSize.x;
            ViewSize.y = (int)(ViewSize.x / windowAspectRatio);

            if (ViewSize.y > WindowSize.y)
            {
                ViewSize.y = WindowSize.y;
                ViewSize.x = (int)(ViewSize.y * windowAspectRatio);
            }
        }

        ViewPos = new vi2d((WindowSize - ViewSize) / 2);
    }

    private void updateMouseButtonStates(MouseButtonEventArgs mouseButtonEvent)
    {
        // Event not generated when button held, so below does not work.
        //btnStates[(int)mouseButtonEvent.Button].Held     = btnStates[(int)mouseButtonEvent.Button].Pressed && mouseButtonEvent.IsPressed;
        btnStates[(int)mouseButtonEvent.Button].Pressed  =  mouseButtonEvent.IsPressed;
        btnStates[(int)mouseButtonEvent.Button].Released = !mouseButtonEvent.IsPressed;
    }

    /// <summary>
    /// Get the state of a specific mouse button
    /// </summary>
    /// <param name="b">button</param>
    /// <returns></returns>
    public HWButton GetMouse(uint button) => btnStates[button];

    // For compatibility
    public int GetMouseX() => MousePos.x;
    public int GetMouseY() => MousePos.y;
    public vi2d GetMousePos() => MousePos;
    public int GetMouseWheel() => MouseWheelDelta;
    public vi2d GetWindowMouse() => WindowMousePos;

    #region Drawing Methods

    public bool Draw(vi2d pos, Pixel p)
    {
        return Draw(pos.x, pos.y, p);
    }

    /// <summary>
    /// Draws a pixel. This is all you need. The rest is fluff.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="p">Pixel color (leave null for white)</param>
    /// <returns></returns>
    public virtual bool Draw(int x, int y, Pixel p)
    {
        // Make sure we have a draw target
        if (DrawTarget == null)
        {
            return false;
        }

        // This just makes the switch expression look cleaner.
        Func<int, int, Pixel, Pixel> alphaBlend = (x, y, p) =>
        {
            Pixel d = DrawTarget.GetPixel(x, y);
            float a = (p.a / 255.0f) * BlendFactor;
            float c = 1.0f - a;
            float r = a * p.r + c * d.r;
            float g = a * p.g + c * d.g;
            float b = a * p.b + c * d.b;
            return new Pixel((byte)r, (byte)g, (byte)b);
        };

        return PixelMode switch
        {
            csPixelGameEngineCore.Pixel.Mode.NORMAL               => DrawTarget.SetPixel(x, y, p),
            csPixelGameEngineCore.Pixel.Mode.MASK when p.a == 255 => DrawTarget.SetPixel(x, y, p),
            csPixelGameEngineCore.Pixel.Mode.ALPHA                => DrawTarget.SetPixel(x, y, alphaBlend(x, y, p)),
            csPixelGameEngineCore.Pixel.Mode.CUSTOM               => DrawTarget.SetPixel(x, y, CustomPixelBlender(x, y, DrawTarget.GetPixel(x, y), p)),
            _ => false
        };
    }

    public void DrawLine(vi2d pos1, vi2d pos2, Pixel p, uint pattern = 0xFFFFFFFF) => DrawLine(pos1.x, pos1.y, pos2.x, pos2.y, p, pattern);

    /// <summary>
    /// Draws a line from (x1,y1) to (x2,y2)
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="p"></param>
    /// <param name="pattern"></param>
    public void DrawLine(int x1, int y1, int x2, int y2, Pixel p, uint pattern = 0xFFFFFFFF)
    {
        if (p == default)
        {
            p = csPixelGameEngineCore.Pixel.WHITE;
        }

        int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
        dx = x2 - x1; dy = y2 - y1;

        Func<uint, uint> rol = (pat) =>
        {
            pat = (pat << 1) | (pat >> 31);
            return pat;
        };

        // straight lines idea by gurkanctn
        if (dx == 0) // Line is vertical
        {
            if (y2 < y1) PGEMath.Swap(ref y1, ref y2);
            for (y = y1; y <= y2; y++)
            {
                pattern = rol(pattern);
                if ((pattern & 1) == 1)
                    Draw(x1, y, p);
            }
            return;
        }

        if (dy == 0) // Line is horizontal
        {
            if (x2 < x1) PGEMath.Swap(ref x1, ref x2);
            for (x = x1; x <= x2; x++)
            {
                pattern = rol(pattern);
                if ((pattern & 1) == 1)
                    Draw(x, y1, p);
            }
            return;
        }

        // Line is Funk-aye
        dx1 = Math.Abs(dx);
        dy1 = Math.Abs(dy);
        px = 2 * dy1 - dx1;
        py = 2 * dx1 - dy1;
        if (dy1 <= dx1)
        {
            if (dx >= 0)
            {
                x = x1; y = y1; xe = x2;
            }
            else
            {
                x = x2; y = y2; xe = x1;
            }

            pattern = rol(pattern);
            if ((pattern & 1) == 1)
                Draw(x, y, p);

            for (i = 0; x < xe; i++)
            {
                x = x + 1;
                if (px < 0)
                    px = px + 2 * dy1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        y = y + 1;
                    else
                        y = y - 1;
                    px = px + 2 * (dy1 - dx1);
                }
                pattern = rol(pattern);
                if ((pattern & 1) == 1)
                    Draw(x, y, p);
            }
        }
        else
        {
            if (dy >= 0)
            {
                x = x1; y = y1; ye = y2;
            }
            else
            {
                x = x2; y = y2; ye = y1;
            }

            pattern = rol(pattern);
            if ((pattern & 1) == 1)
                Draw(x, y, p);

            for (i = 0; y < ye; i++)
            {
                y = y + 1;
                if (py <= 0)
                    py = py + 2 * dx1;
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                        x = x + 1;
                    else
                        x = x - 1;
                    py = py + 2 * (dx1 - dy1);
                }
                pattern = rol(pattern);
                if ((pattern & 1) == 1)
                    Draw(x, y, p);
            }
        }
    }

    public void DrawCircle(vi2d pos, int radius, Pixel p, byte mask = 0xFF)
    {
        DrawCircle(pos.x, pos.y, radius, p, mask);
    }

    // Draws a circle located at (x,y) with radius
    public void DrawCircle(int x, int y, int radius, Pixel p, byte mask = 0xFF)
    {
        if (radius == 0) return;

        int x0 = 0;
        int y0 = radius;
        int d = 3 - 2 * radius;

        while (y0 >= x0) // only formulate 1/8 of circle
        {
            if ((mask & 0x01) != 0) Draw(x + x0, y - y0, p);
            if ((mask & 0x02) != 0) Draw(x + y0, y - x0, p);
            if ((mask & 0x04) != 0) Draw(x + y0, y + x0, p);
            if ((mask & 0x08) != 0) Draw(x + x0, y + y0, p);
            if ((mask & 0x10) != 0) Draw(x - x0, y + y0, p);
            if ((mask & 0x20) != 0) Draw(x - y0, y + x0, p);
            if ((mask & 0x40) != 0) Draw(x - y0, y - x0, p);
            if ((mask & 0x80) != 0) Draw(x - x0, y - y0, p);
            if (d < 0) d += 4 * x0++ + 6;
            else d += 4 * (x0++ - y0--) + 10;
        }
    }

    /// <summary>
    /// FillCircle, vector edition
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    /// <param name="p"></param>
    public void FillCircle(vi2d pos, int radius, Pixel p)
    {
        FillCircle(pos.x, pos.y, radius, p);
    }

    // Fills a circle located at (x,y) with radius
    public void FillCircle(int x, int y, int radius, Pixel p)
    {
        if (radius == 0) return;

        // Taken from wikipedia
        int x0 = 0;
        int y0 = radius;
        int d = 3 - 2 * radius;

        Action<int, int, int> drawline = (sx, ex, ny) =>
        {
            Parallel.For(sx, ex, (i) => Draw(i, ny, p));
        };

        while (y0 >= x0)
        {
            // Modified to draw scan-lines instead of edges
            drawline(x - x0, x + x0, y - y0);
            drawline(x - y0, x + y0, y - x0);
            drawline(x - x0, x + x0, y + y0);
            drawline(x - y0, x + y0, y + x0);
            if (d < 0) d += 4 * x0++ + 6;
            else d += 4 * (x0++ - y0--) + 10;
        }
    }

    /// <summary>
    /// DrawRect, vector edition
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="p"></param>
    public void DrawRect(vi2d pos, vi2d size, Pixel p)
    {
        DrawRect(pos.x, pos.y, size.x, size.y, p);
    }

    // Draws a rectangle at (x,y) to (x+w,y+h)
    public void DrawRect(int x, int y, int w, int h, Pixel p)
    {
        if (p == default)
            p = csPixelGameEngineCore.Pixel.WHITE;

        DrawLine(x, y, x + w, y, p);
        DrawLine(x + w, y, x + w, y + h, p);
        DrawLine(x + w, y + h, x, y + h, p);
        DrawLine(x, y + h, x, y, p);
    }

    public void FillRect(vi2d pos, vi2d size, Pixel p)
    {
        FillRect(pos.x, pos.y, size.x, size.y, p);
    }

    /// <summary>
    /// Fills a rectangle at (x,y) to (x+w,y+h)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="p"></param>
    public void FillRect(int x, int y, int w, int h, Pixel p)
    {
        int x2 = x + w;
        int y2 = y + h;

        if (x < 0)
            x = 0;
        if (x >= DrawTargetWidth)
            x = DrawTargetWidth;
        if (y < 0)
            y = 0;
        if (y >= DrawTargetHeight)
            y = DrawTargetHeight;

        if (x2 < 0)
            x2 = 0;
        if (x2 >= DrawTargetWidth)
            x2 = DrawTargetWidth;
        if (y2 < 0)
            y2 = 0;
        if (y2 >= DrawTargetHeight)
            y2 = DrawTargetHeight;

        for (int i = x; i < x2; i++)
            for (int j = y; j < y2; j++)
                Draw(i, j, p);
    }

    /// <summary>
    /// DrawTriangle, vector edition
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="pos3"></param>
    /// <param name="p"></param>
    public void DrawTriangle(vi2d pos1, vi2d pos2, vi2d pos3, Pixel p)
    {
        DrawTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p);
    }

    /// <summary>
    /// Draws a triangle between points (x1,y1), (x2,y2) and (x3,y3)
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="x3"></param>
    /// <param name="y3"></param>
    /// <param name="p"></param>
    public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
    {
        DrawLine(x1, y1, x2, y2, p);
        DrawLine(x2, y2, x3, y3, p);
        DrawLine(x3, y3, x1, y1, p);
    }

    public void FillTriangle(vi2d pos1, vi2d pos2, vi2d pos3, Pixel p)
    {
        FillTriangle(pos1.y, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p);
    }

    // Flat fills a triangle between points (x1,y1), (x2,y2) and (x3,y3)
    // https://www.avrfreaks.net/sites/default/files/triangles.c
    public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
    {
        Action<int, int, int> drawline = (sx, ex, ny) =>
        {
            for (int i = sx; i <= ex; i++)
            {
                Draw(i, ny, p);
            }
        };

        int t1x, t2x, y, minx, maxx, t1xp, t2xp;
        bool changed1 = false;
        bool changed2 = false;
        int signx1, signx2, dx1, dy1, dx2, dy2;
        int e1, e2;
        // Sort vertices
        if (y1 > y2) { PGEMath.Swap(ref y1, ref y2); PGEMath.Swap(ref x1, ref x2); }
        if (y1 > y3) { PGEMath.Swap(ref y1, ref y3); PGEMath.Swap(ref x1, ref x3); }
        if (y2 > y3) { PGEMath.Swap(ref y2, ref y3); PGEMath.Swap(ref x2, ref x3); }

        t1x = t2x = x1; y = y1;   // Starting points
        dx1 = x2 - x1;
        if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
        else signx1 = 1;
        dy1 = (y2 - y1);

        dx2 = (x3 - x1);
        if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
        else signx2 = 1;
        dy2 = (y3 - y1);

        if (dy1 > dx1)
        {   // swap values
            PGEMath.Swap(ref dx1, ref dy1);
            changed1 = true;
        }
        if (dy2 > dx2)
        {   // swap values
            PGEMath.Swap(ref dy2, ref dx2);
            changed2 = true;
        }

        e2 = (dx2 >> 1);
        // Flat top, just process the second half
        if (y1 != y2)
        {
            e1 = (dx1 >> 1);

            for (int i = 0; i < dx1;)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }

                while (i < dx1)
                {
                    i++;
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1)
                            t1xp = signx1;//t1x += signx1;
                        else
                            goto next1;// break;
                    }
                    if (changed1)
                        break;
                    else
                        t1x += signx1;
                }
                // Move line
                next1:
                // process second line until y value is about to change
                while (true)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2)
                            t2xp = signx2;//t2x += signx2;
                        else
                            //break;
                            goto next2;
                    }
                    if (changed2)
                        break;
                    else t2x += signx2;
                }
                next2:

                if (minx > t1x) minx = t1x;
                if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x;
                if (maxx < t2x) maxx = t2x;
                drawline(minx, maxx, y);    // Draw line from min to max points found on the y
                                            // Now increase y
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y == y2) break;
            }
        }

        // Second half
        dx1 = (x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
        else signx1 = 1;
        dy1 = (y3 - y2);
        t1x = x2;

        if (dy1 > dx1)
        {   // swap values
            PGEMath.Swap(ref dy1, ref dx1);
            changed1 = true;
        }
        else changed1 = false;

        e1 = (dx1 >> 1);

        for (int i = 0; i <= dx1; i++)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x) { minx = t1x; maxx = t2x; }
            else { minx = t2x; maxx = t1x; }
            // process first line until y value is about to change
            while (i < dx1)
            {
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;
                    if (changed1)
                    {
                        t1xp = signx1;
                        break;
                    }//t1x += signx1;
                    else
                        //break;
                        goto next3;
                }
                if (changed1) break;
                else t1x += signx1;
                if (i < dx1) i++;
            }
            next3:

            // process second line until y value is about to change
            while (t2x != x3)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2)
                        t2xp = signx2;
                    else
                        //break;
                        goto next4;
                }
                if (changed2) break;
                else t2x += signx2;
            }
            next4:

            if (minx > t1x) minx = t1x;
            if (minx > t2x) minx = t2x;
            if (maxx < t1x) maxx = t1x;
            if (maxx < t2x) maxx = t2x;
            drawline(minx, maxx, y);
            if (!changed1) t1x += signx1;
            t1x += t1xp;
            if (!changed2) t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y > y3) return;
        }
    }

    /// <summary>
    /// Draw a textured triangle
    /// </summary>
    /// <param name="points">Array of 3 points</param>
    /// <param name="tex">Array of 3 texture coordinates</param>
    /// <param name="color">Array of 3 colors</param>
    /// <param name="sprTex">Sprite with the texture</param>
    /// <exception cref="ArgumentException"></exception>
    public void FillTexturedTriangle(vf2d[] points, vf2d[] tex, Pixel[] color, Sprite sprTex)
    {
        if (points.Length < 3) throw new ArgumentException("Must pass array of 3 points", nameof(points));
        if (tex.Length < 3) throw new ArgumentException("Must pass array of 3 texture coordinates", nameof(tex));
        if (color.Length < 3) throw new ArgumentException("Must pass array of 3 colors", nameof(color));

        vf2d p1 = points[0];
		vf2d p2 = points[1];
		vf2d p3 = points[2];

		if (p2.y < p1.y)
        {
            (p1.y, p2.y) = (p2.y, p1.y);
            (p1.x, p2.x) = (p2.x, p1.x);
            (tex[0].x, tex[1].x) = (tex[1].x, tex[0].x);
            (tex[0].y, tex[1].y) = (tex[1].y, tex[0].y);
            (color[0], color[1]) = (color[1], color[0]);
        }
		if (p3.y < p1.y)
        {
            (p1.y, p3.y) = (p3.y, p1.y);
            (p1.x, p3.x) = (p3.x, p1.x);
            (tex[0].x, tex[2].x) = (tex[2].x, tex[0].x);
            (tex[0].y, tex[2].y) = (tex[2].y, tex[0].y);
            (color[0], color[2]) = (color[2], color[0]);
        }
		if (p3.y < p2.y)
        {
            (p2.y, p3.y) = (p3.y, p2.y);
            (p2.x, p3.x) = (p3.x, p2.x);
            (tex[1].x, tex[2].x) = (tex[2].x, tex[1].x);
            (tex[1].y, tex[2].y) = (tex[2].y, tex[1].y);
            (color[1], color[2]) = (color[2], color[1]);
        }

		var dPos1 = p2 - p1;
		vf2d dTex1 = tex[1] - tex[0];
		int dcr1 = color[1].r - color[0].r;
		int dcg1 = color[1].g - color[0].g;
		int dcb1 = color[1].b - color[0].b;
		int dca1 = color[1].a - color[0].a;

		var dPos2 = p3 - p1;
		vf2d dTex2 = tex[2] - tex[0];
		int dcr2 = color[2].r - color[0].r;
		int dcg2 = color[2].g - color[0].g;
		int dcb2 = color[2].b - color[0].b;
		int dca2 = color[2].a - color[0].a;

		float dax_step = 0, dbx_step = 0, dcr1_step = 0, dcr2_step = 0,	dcg1_step = 0, dcg2_step = 0, dcb1_step = 0, dcb2_step = 0,	dca1_step = 0, dca2_step = 0;
		vf2d vTex1Step = new (), vTex2Step = new ();

		if (dPos1.y != 0)
		{
			dax_step = dPos1.x / (float)Math.Abs(dPos1.y);
			vTex1Step = dTex1 / (float)Math.Abs(dPos1.y);
			dcr1_step = dcr1 / (float)Math.Abs(dPos1.y);
			dcg1_step = dcg1 / (float)Math.Abs(dPos1.y);
			dcb1_step = dcb1 / (float)Math.Abs(dPos1.y);
			dca1_step = dca1 / (float)Math.Abs(dPos1.y);
		}

		if (dPos2.y != 0)
		{
			dbx_step = dPos2.x / (float)Math.Abs(dPos2.y);
			vTex2Step = dTex2 / (float)Math.Abs(dPos2.y);
			dcr2_step = dcr2 / (float)Math.Abs(dPos2.y);
			dcg2_step = dcg2 / (float)Math.Abs(dPos2.y);
			dcb2_step = dcb2 / (float)Math.Abs(dPos2.y);
			dca2_step = dca2 / (float)Math.Abs(dPos2.y);
		}

		vf2d vStart;
		vf2d vEnd;
		int vStartIdx;

        for (int pass = 0; pass < 2; pass++)
        {
            if (pass == 0)
            {
                vStart = p1;
                vEnd = p2;
                vStartIdx = 0;
            }
            else
            {
                dPos1 = p3 - p2;
                dTex1 = tex[2] - tex[1];
                dcr1 = color[2].r - color[1].r;
                dcg1 = color[2].g - color[1].g;
                dcb1 = color[2].b - color[1].b;
                dca1 = color[2].a - color[1].a;
                dcr1_step = 0; dcg1_step = 0; dcb1_step = 0; dca1_step = 0;

                if (dPos2.y != 0)
                {
                    dbx_step = dPos2.x / (float)Math.Abs(dPos2.y);
                }

                if (dPos1.y != 0)
                {
                    dax_step = dPos1.x / (float)Math.Abs(dPos1.y);
                    vTex1Step = dTex1 / (float)Math.Abs(dPos1.y);
                    dcr1_step = dcr1 / (float)Math.Abs(dPos1.y);
                    dcg1_step = dcg1 / (float)Math.Abs(dPos1.y);
                    dcb1_step = dcb1 / (float)Math.Abs(dPos1.y);
                    dca1_step = dca1 / (float)Math.Abs(dPos1.y);
                }

                vStart = p2; vEnd = p3; vStartIdx = 1;
            }

            if (dPos1.y != 0)
            {
                for (int i = (int)vStart.y; i <= vEnd.y; i++)
                {
                    int ax = (int)(vStart.x + (i - vStart.y) * dax_step);
                    int bx = (int)(p1.x + (i - p1.y) * dbx_step);

                    vf2d tex_s = new(tex[vStartIdx].x + (i - vStart.y) * vTex1Step.x, tex[vStartIdx].y + (i - vStart.y) * vTex1Step.y);
                    vf2d tex_e = new(tex[0].x + (i - p1.y) * vTex2Step.x, tex[0].y + (i - p1.y) * vTex2Step.y);

                    Pixel col_s = new((byte)(color[vStartIdx].r + (byte)((i - vStart.y) * dcr1_step)),
                                      (byte)(color[vStartIdx].g + (byte)((i - vStart.y) * dcg1_step)),
                                      (byte)(color[vStartIdx].b + (byte)((i - vStart.y) * dcb1_step)),
                                      (byte)(color[vStartIdx].a + (byte)((i - vStart.y) * dca1_step)));

                    Pixel col_e = new((byte)(color[0].r + (byte)((i - p1.y) * dcr2_step)),
                                      (byte)(color[0].g + (byte)((i - p1.y) * dcg2_step)),
                                      (byte)(color[0].b + (byte)((i - p1.y) * dcb2_step)),
                                      (byte)(color[0].a + (byte)((i - p1.y) * dca2_step)));

                    if (ax > bx)
                    {
                        (ax, bx) = (bx, ax);
                        (tex_s, tex_e) = (tex_e, tex_s);
                        (col_s, col_e) = (col_e, col_s);
                    }

                    float tstep = 1.0f / ((float)(bx - ax));
                    float t = 0.0f;

                    for (int j = ax; j < bx; j++)
                    {
                        Pixel pixel = csPixelGameEngineCore.Pixel.PixelLerp(col_s, col_e, t);
                        if (sprTex != null)
                        {
                            var lerped = tex_s.lerp(tex_e, t);
                            pixel *= sprTex.Sample(lerped.x, lerped.y);
                        }
                        Draw(j, i, pixel);
                        t += tstep;
                    }
                }
            }
        }
    }

    public void FillTexturedPolygon(vf2d[] points, vf2d[] tex, Pixel[] color, Sprite sprTex, DecalStructure structure)
    {
        if (structure == DecalStructure.LINE)
        {
            return;
        }

        if (points.Length < 3 || tex.Length < 3 || color.Length < 3)
        {
            return;
        }

        if (structure == DecalStructure.LIST)
        {
            for (int tri = 0; tri < points.Length / 3; tri++)
            {
                vf2d[] vP = [ points[tri * 3 + 0], points[tri * 3 + 1], points[tri * 3 + 2] ];
                vf2d[] vT = [ tex[tri * 3 + 0], tex[tri * 3 + 1], tex[tri * 3 + 2] ];
                Pixel[] vC = [ color[tri * 3 + 0], color[tri * 3 + 1], color[tri * 3 + 2] ];
                FillTexturedTriangle(vP, vT, vC, sprTex);
            }
        }
        else if (structure == DecalStructure.STRIP)
        {
            for (int tri = 2; tri < points.Length; tri++)
            {
                vf2d[] vP = [ points[tri - 2], points[tri - 1], points[tri] ];
                vf2d[] vT = [ tex[tri - 2], tex[tri - 1], tex[tri] ];
                Pixel[] vC = [ color[tri - 2], color[tri - 1], color[tri] ];
                FillTexturedTriangle(vP, vT, vC, sprTex);
            }
        }
        else if (structure == DecalStructure.FAN)
        {
            for (int tri = 2; tri < points.Length; tri++)
            {
                vf2d[] vP = [ points[0], points[tri - 1], points[tri] ];
                vf2d[] vT = [ tex[0], tex[tri - 1], tex[tri] ];
                Pixel[] vC = { color[0], color[tri - 1], color[tri] };
                FillTexturedTriangle(vP, vT, vC, sprTex);
            }
        }
    }

    public void DrawSprite(vi2d pos, Sprite sprite, int scale = 1, Flip flip = Flip.NONE) => DrawSprite(pos.x, pos.y, sprite, scale, flip);

    /// <summary>
    /// Draws an entire sprite at location (x,y)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sprite"></param>
    /// <param name="scale"></param>
    public void DrawSprite(int x, int y, Sprite sprite, int scale = 1, Flip flip = Flip.NONE)
    {
        if (sprite == null)
            return;

        int fxs = 0, fxm = 1, fx = 0;
        int fys = 0, fym = 1, fy = 0;
        if (flip == Flip.HORIZ)
        {
            fxs = sprite.Width - 1;
            fxm = -1;
        }
        if (flip == Flip.VERT)
        {
            fys = sprite.Height - 1;
            fym = -1;
        }

        if (scale > 1)
        {
            fx = fxs;
            for (int i = 0; i < sprite.Width; i++, fx += fxm)
            {
                fy = fys;
                for (int j = 0; j < sprite.Height; j++, fy += fym)
                    for (uint @is = 0; @is < scale; @is++)
                        for (uint js = 0; js < scale; js++)
                            Draw((int)(x + (i * scale) + @is), (int)(y + (j * scale) + js), sprite.GetPixel(fx, fy));
            }
        }
        else
        {
            fx = fxs;
            for (int i = 0; i < sprite.Width; i++, fx += fxm)
            {
                fy = fys;
                for (int j = 0; j < sprite.Height; j++, fy += fym)
                    Draw(x + i, y + j, sprite.GetPixel(fx, fy));
            }
        }
    }

    public void DrawPartialSprite(vi2d pos, Sprite sprite, vi2d sourcepos, vi2d size, uint scale = 1, Flip flip = Flip.NONE)
    {
        DrawPartialSprite(pos.x, pos.y, sprite, sourcepos.x, sourcepos.y, size.x, size.y, scale, flip);
    }

    /// <summary>
    /// Draws an area of a sprite at location (x,y), where the
    /// selected area is (ox,oy) to (ox+w,oy+h)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sprite"></param>
    /// <param name="ox"></param>
    /// <param name="oy"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <param name="scale"></param>
    public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h, uint scale = 1, Flip flip = Flip.NONE)
    {
        if (sprite == null)
            return;

        int fxs = 0, fxm = 1, fx = 0;
        int fys = 0, fym = 1, fy = 0;
        if (flip == Flip.HORIZ)
        {
            fxs = w - 1;
            fxm = -1;
        }
        if (flip == Flip.VERT)
        {
            fys = h - 1;
            fym = -1;
        }

        if (scale > 1)
        {
            fx = fxs;
            for (int i = 0; i < w; i++, fx += fxm)
            {
                fy = fys;
                for (int j = 0; j < h; j++, fy += fym)
                    for (uint @is = 0; @is < scale; @is++)
                        for (uint js = 0; js < scale; js++)
                            Draw((int)(x + (i * scale) + @is), (int)(y + (j * scale) + js), sprite.GetPixel(fx + ox, fy + oy));
            }
        }
        else
        {
            fx = fxs;
            for (int i = 0; i < w; i++, fx += fxm)
            {
                fy = fys;
                for (int j = 0; j < h; j++, fy += fym)
                    Draw(x + i, y + j, sprite.GetPixel(fx + ox, fy + oy));
            }
        }
    }

    /// <summary>
    /// Takes 2 points and clips them to be within the visible area of the screen.
    /// </summary>
    /// <param name="in_p1"></param>
    /// <param name="in_p2"></param>
    /// <returns>True if both points are within the visible area, false if line drawn is outside visible area</returns>
    public bool ClipLineToScreen(ref vi2d in_p1, ref vi2d in_p2)
    {
        // https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
        int SEG_I = 0b0000, SEG_L = 0b0001, SEG_R = 0b0010, SEG_B = 0b0100, SEG_T = 0b1000;
        Func<vi2d, int> Segment = v =>
		{
            int i = SEG_I;
            if (v.x < 0) i |= SEG_L; else if (v.x > ScreenSize.x) i |= SEG_R;
            if (v.y < 0) i |= SEG_B; else if (v.y > ScreenSize.y) i |= SEG_T;
            return i;
        };

        int s1 = Segment(in_p1), s2 = Segment(in_p2);

        while (true)
        {
            if ((s1 | s2) == 0)
            {
                return true;
            }
            else if ((s1 & s2) != 0)
            {
                return false;
            }
            else
            {
                int s3 = s2 > s1 ? s2 : s1;
                vi2d n = new();
                if ((s3 & SEG_T) != 0)
                {
                    n.x = in_p1.x + (in_p2.x - in_p1.x) * (ScreenSize.y - in_p1.y) / (in_p2.y - in_p1.y);
                    n.y = ScreenSize.y;
                }
                else if ((s3 & SEG_B) != 0)
                {
                    n.x = in_p1.x + (in_p2.x - in_p1.x) * (0 - in_p1.y) / (in_p2.y - in_p1.y); n.y = 0;
                }
                else if ((s3 & SEG_R) != 0)
                {
                    n.x = ScreenSize.x;
                    n.y = in_p1.y + (in_p2.y - in_p1.y) * (ScreenSize.x - in_p1.x) / (in_p2.x - in_p1.x);
                }
                else if ((s3 & SEG_L) != 0)
                {
                    n.x = 0;
                    n.y = in_p1.y + (in_p2.y - in_p1.y) * (0 - in_p1.x) / (in_p2.x - in_p1.x);
                }

                if (s3 == s1)
                {
                    in_p1 = n;
                    s1 = Segment(in_p1);
                }
                else
                {
                    in_p2 = n;
                    s2 = Segment(in_p2);
                }
            }
        }
    }

    /// <summary>
    /// Clears entire draw target to Pixel
    /// </summary>
    /// <param name="p"></param>
    public void Clear(Pixel p)
    {
        //int pixelCount = DrawTarget.ColorData.Length;
        DrawTarget.ColData.AsSpan().Fill(p);
        //for (int i = 0; i < pixelCount; i++)
        //    DrawTarget.ColorData[i] = p;
    }

    public void ClearBuffer(Pixel p, bool bDepth) => renderer.ClearBuffer(p, bDepth);

    //=====================================================================
    // String Functions
    //=====================================================================

    public void DrawString(vi2d pos, string sText, Pixel col, uint scale = 1) => DrawString(pos.x, pos.y, sText, col, scale);

    /// <summary>
    /// Draws a single line of text
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sText"></param>
    /// <param name="col"></param>
    /// <param name="scale"></param>
    public void DrawString(int x, int y, string sText, Pixel col, uint scale = 1)
    {
        int sx = 0;
        int sy = 0;
        Pixel.Mode m = PixelMode;
        if (col.a != 255)
            PixelMode = csPixelGameEngineCore.Pixel.Mode.ALPHA;
        else
            PixelMode = csPixelGameEngineCore.Pixel.Mode.MASK;

        foreach (var c in sText)
        {
            if (c == '\n')
            {
                sx = 0;
                sy += (int)(8 * scale);
            }
            else
            {
                int ox = (c - 32) % 16;
                int oy = (c - 32) / 16;

                if (scale > 1)
                {
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            if (fontRenderable.Sprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                for (int is_ = 0; is_ < scale; is_++)
                                    for (int js = 0; js < scale; js++)
                                        Draw((int)(x + sx + (i * scale) + is_), (int)(y + sy + (j * scale) + js), col);
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                        for (int j = 0; j < 8; j++)
                            if (fontRenderable.Sprite.GetPixel(i + ox * 8, j + oy * 8).r > 0)
                                Draw(x + sx + i, y + sy + j, col);
                }
                sx += (int)(8 * scale);
            }
        }
        PixelMode = m;
    }

    public vi2d GetTextSize(string s)
    {
        vi2d size = new (0, 1);
        vi2d pos = new (0, 1);
        foreach (char c in s)
        {
            if (c == '\n')
            {
                pos.y++;
                pos.x = 0;
            }
            else if(c == '\t')
            {
                pos.x += TabSizeInSpaces;
            }
            else
            {
                pos.x++;
            }
            size.x = Math.Max(size.x, pos.x);
            size.y = Math.Max(size.y, pos.y);
        }
        return size * 8;
    }

    public vi2d GetTextSizeProp(string s)
    {
        vi2d size = new (0, 1);
        vi2d pos = new (0, 1);
        foreach (char c in s)
        {
            if (c == '\n')
            {
                pos.y++;
                pos.x = 0;
            }
            else if(c == '\t')
            {
                pos.x += TabSizeInSpaces;
            }
            else
            {
                pos.x += FontSpacing[c - 32].y;
            }
            size.x = Math.Max(size.x, pos.x);
            size.y = Math.Max(size.y, pos.y);
        }

        size.y *= 8;
        return size;
    }

    //=====================================================================
    // Decal Drawing
    //=====================================================================

    /// <summary>
    /// Draws a whole decal, with optional scale and tinting
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="decal"></param>
    /// <param name="scale">If null, will use vec2d_f.UNIT</param>
    /// <param name="tint">If null, will use Pixel.WHITE</param>
    /// <param name="decalMode">I want a way to override the class one that is ALWAYS F*ING SET TO NORMAL, so I'm providing.</param>
    public void DrawDecal(vf2d pos, Decal decal, vf2d scale, Pixel tint, DecalMode decalMode = DecalMode.NORMAL)
    {
        vf2d vScreenSpacePos = new vf2d
        {
            x = pos.x * InvScreenSize.x * 2.0f - 1.0f,
            y = (pos.y * InvScreenSize.y * 2.0f - 1.0f) * -1.0f
        };
        vf2d vScreenSpaceDim = new vf2d
        {
            x = vScreenSpacePos.x + (2.0f * decal.sprite.Width * InvScreenSize.x) * scale.x,
            y = vScreenSpacePos.y - (2.0f * decal.sprite.Height * InvScreenSize.y) * scale.y
        };

        DecalInstance di = new DecalInstance
        {
            decal = decal,
            points = 4,
            tint = [tint, tint, tint, tint],
            pos = [ vScreenSpacePos, new vf2d(vScreenSpacePos.x, vScreenSpaceDim.y), vScreenSpaceDim, new vf2d(vScreenSpaceDim.x, vScreenSpacePos.y) ],
            uv = [ new vf2d(0.0f, 0.0f), new vf2d(0.0f, 1.0f), new vf2d(1.0f, 1.0f), new vf2d(1.0f, 0.0f) ],
            w = [ 1, 1, 1, 1 ],
            mode = decalMode,
            structure = DecalStructure
        };

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPartialDecal(vf2d pos, Decal decal, vf2d source_pos, vf2d source_size, vf2d scale, Pixel tint)
    {
        vf2d vScreenSpacePos = new()
        {
            x =   (pos.x * InvScreenSize.x) * 2.0f - 1.0f,
            y = -((pos.y * InvScreenSize.y) * 2.0f - 1.0f)
        };


        vf2d vScreenSpaceDim = new()
        {
            x =   ((pos.x + source_size.x * scale.x) * InvScreenSize.x) * 2.0f - 1.0f,
            y = -(((pos.y + source_size.y * scale.y) * InvScreenSize.y) * 2.0f - 1.0f)
        };

        vf2d vWindow = ViewSize;
        var vQuantisedPos = ((vScreenSpacePos * vWindow) + new vf2d(0.5f,  0.5f)).floor() / vWindow;
        var vQuantisedDim = ((vScreenSpaceDim * vWindow) + new vf2d(0.5f, -0.5f)).ceil()  / vWindow;

        vf2d uvtl = (source_pos + new vf2d(0.0001f, 0.0001f)) * decal.vUVScale;
        vf2d uvbr = (source_pos + source_size - new vf2d(0.0001f, 0.0001f)) * decal.vUVScale;

        DecalInstance di = new()
        {
            points = 4,
            decal = decal,
            tint = [ tint, tint, tint, tint ],
            pos = [ new vf2d(vQuantisedPos.x, vQuantisedPos.y), new vf2d(vQuantisedPos.x, vQuantisedDim.y),
                    new vf2d(vQuantisedDim.x, vQuantisedDim.y), new vf2d(vQuantisedDim.x, vQuantisedPos.y) ],
            uv = [ new vf2d(uvtl.x, uvtl.y), new vf2d(uvtl.x, uvbr.y), new vf2d(uvbr.x, uvbr.y), new vf2d(uvbr.x, uvtl.y) ],
            w = [ 1,1,1,1 ],
            mode = DecalMode,
            structure = DecalStructure
        };
        
        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPartialDecal(vf2d pos, vf2d size, Decal decal, vf2d source_pos, vf2d source_size, Pixel tint)
    {
        vf2d vScreenSpacePos = new()
        {
            x =  (pos.x * InvScreenSize.x) * 2.0f - 1.0f,
            y = ((pos.y * InvScreenSize.y) * 2.0f - 1.0f) * -1.0f
        };

        vf2d vScreenSpaceDim = new()
        {
            x = vScreenSpacePos.x + (2.0f * size.x * InvScreenSize.x),
            y = vScreenSpacePos.y - (2.0f * size.y * InvScreenSize.y)
        };

        vf2d uvtl = (source_pos) * decal.vUVScale;
        vf2d uvbr = uvtl + (source_size * decal.vUVScale);

        DecalInstance di = new()
        {
            points = 4,
            decal = decal,
            tint = [ tint, tint, tint, tint ],
            pos = [ new vf2d(vScreenSpacePos.x, vScreenSpacePos.y), new vf2d(vScreenSpacePos.x, vScreenSpaceDim.y),
                    new vf2d(vScreenSpaceDim.x, vScreenSpaceDim.y), new vf2d(vScreenSpaceDim.x, vScreenSpacePos.y) ],
            uv = [ new vf2d(uvtl.x, uvtl.y), new vf2d(uvtl.x, uvbr.y), new vf2d(uvbr.x, uvbr.y), new vf2d(uvbr.x, uvtl.y) ],
            w = [ 1,1,1,1 ],
            mode = DecalMode,
            structure = DecalStructure
        };
        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawExplicitDecal(Decal decal, vf2d[] pos, vf2d[] uv, Pixel[] col, uint elements)
    {
        DecalInstance di = new()
        {
            decal = decal,
            pos = new vf2d[elements],
            uv = new vf2d[elements],
            w = new float[elements],
            tint = new Pixel[elements],
            points = elements,
            mode = DecalMode,
            structure = DecalStructure
        };

        for (uint i = 0; i < elements; i++)
        {
            di.pos[i]  = new vf2d((pos[i].x * InvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * InvScreenSize.y) * 2.0f - 1.0f) * -1.0f);
            di.uv[i]   = uv[i];
            di.tint[i] = col[i];
            di.w[i]    = 1.0f;
        }

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawRotatedDecal(vf2d pos, Decal decal, float angle, vf2d center, vf2d scale, Pixel tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            pos = [ (new vf2d(0, 0) - center) * scale,
                    (new vf2d(0, decal.sprite.Height) - center) * scale,
                    (new vf2d(decal.sprite.Width, decal.sprite.Height) - center) * scale,
                    (new vf2d(decal.sprite.Width, 0) - center) * scale],
            uv = [ new(0, 0), new(0, 1), new(1, 1), new(1, 0) ],
            w = [ 1, 1, 1, 1 ],
            tint = [ tint, tint, tint, tint ],
            points = 4,
            mode = DecalMode,
            structure = DecalStructure
        };

        float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);

        for (int i = 0; i < 4; i++)
        {
            di.pos[i] = pos + new vf2d(di.pos[i].x * c - di.pos[i].y * s, di.pos[i].x * s + di.pos[i].y * c);
            di.pos[i] = di.pos[i] * InvScreenSize * 2.0f - new vf2d(1, 1);
            di.pos[i].y *= -1.0f;
        }

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPartialRotatedDecal(vf2d pos, Decal decal, float angle, vf2d center, vf2d source_pos, vf2d source_size, vf2d scale, Pixel tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            pos = [ (new vf2d(0, 0) - center) * scale,
                    (new vf2d(0, decal.sprite.Height) - center) * scale,
                    (new vf2d(decal.sprite.Width, decal.sprite.Height) - center) * scale,
                    (new vf2d(decal.sprite.Width, 0) - center) * scale],
            w = [ 1, 1, 1, 1 ],
            tint = [ tint, tint, tint, tint ],
            points = 4,
            mode = DecalMode,
            structure = DecalStructure
        };

        float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);

        for (int i = 0; i < 4; i++)
        {
            di.pos[i] = pos + new vf2d(di.pos[i].x * c - di.pos[i].y * s, di.pos[i].x * s + di.pos[i].y * c);
            di.pos[i] = di.pos[i] * InvScreenSize * 2.0f - new vf2d(1, 1);
            di.pos[i].y *= -1.0f;
        }

        vf2d uvtl = source_pos * decal.vUVScale;
        vf2d uvbr = uvtl + (source_size * decal.vUVScale);
        di.uv = [ new(uvtl.x, uvtl.y), new(uvtl.x, uvbr.y), new(uvbr.x, uvbr.y), new(uvbr.x, uvtl.y) ];

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPartialWarpedDecal(Decal decal, vf2d[] pos, vf2d source_pos, vf2d source_size, Pixel tint)
    {
        if (pos.Length != 4) throw new ArgumentException("Should contain 4 points", nameof(pos));

        DecalInstance di = new()
        {
            decal = decal,
            pos = [ new vf2d(), new vf2d(), new vf2d(), new vf2d() ],
            w = [ 1, 1, 1, 1 ],
            tint = [ tint, tint, tint, tint ],
            points = 4,
            uv = [ new(0, 0), new(0, 1), new(1, 1), new(1, 0) ]
        };
        vf2d center = new();
        float rd = (pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y);
        if (rd != 0)
        {
            vf2d uvtl = source_pos * decal.vUVScale;
            vf2d uvbr = uvtl + (source_size * decal.vUVScale);
            di.uv = [ new(uvtl.x, uvtl.y), new(uvtl.x, uvbr.y), new(uvbr.x, uvbr.y), new(uvbr.x, uvtl.y) ];

            rd = 1.0f / rd;
            float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
            float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
            if (!(rn < 0.0f || rn > 1.0f || sn < 0.0f || sn > 1.0f))
            {
                center = pos[0] + rn * (pos[2] - pos[0]);
            }
            float[] d = [0,0,0,0];
            for (int i = 0; i < 4; i++)
            {
                d[i] = (float)(pos[i] - center).mag();
            }
            for (int i = 0; i < 4; i++)
            {
                float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
                di.uv[i] *= q;
                di.w[i] *= q;
                di.pos[i] = new((pos[i].x * InvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * InvScreenSize.y) * 2.0f - 1.0f) * -1.0f);
            }

            di.mode = DecalMode;
            di.structure = DecalStructure;
            Layers[(int)TargetLayer].DecalInstance.Add(di);
        }
    }

    public void DrawPolygonDecal(Decal decal, vf2d[] pos, vf2d[] uv, Pixel tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            points = (uint)pos.Length,
            pos = new vf2d[pos.Length],
            uv = new vf2d[pos.Length],
            w = new float[pos.Length],
            tint = new Pixel[pos.Length],
            mode = DecalMode,
            structure = DecalStructure
        };

        for (uint i = 0; i < di.points; i++)
        {
            di.pos[i] = new vf2d(pos[i].x * InvScreenSize.x * 2.0f - 1.0f, (pos[i].y * InvScreenSize.y * 2.0f - 1.0f) * -1.0f);
            di.uv[i] = uv[i];
            di.tint[i] = tint;
            di.w[i] = 1.0f;
        }
        
        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPolygonDecal(Decal decal, vf2d[] pos, vf2d[] uv, Pixel[] tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            points = (uint)pos.Length,
            pos = new vf2d[pos.Length],
            uv = new vf2d[pos.Length],
            w = new float[pos.Length],
            tint = new Pixel[pos.Length],
            mode = DecalMode,
            structure = DecalStructure
        };

        for (uint i = 0; i < di.points; i++)
        {
            di.pos[i] = new vf2d(pos[i].x * InvScreenSize.x * 2.0f - 1.0f, (pos[i].y * InvScreenSize.y * 2.0f - 1.0f) * -1.0f);
            di.uv[i] = uv[i];
            di.tint[i] = tint[i];
            di.w[i] = 1.0f;
        }

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPolygonDecal(Decal decal, vf2d[] pos, float[] depth, vf2d[] uv, Pixel tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            points = (uint)pos.Length,
            pos = new vf2d[pos.Length],
            uv = new vf2d[pos.Length],
            w = new float[pos.Length],
            tint = new Pixel[pos.Length],
            mode = DecalMode,
            structure = DecalStructure
        };

        for (uint i = 0; i < di.points; i++)
        {
            di.pos[i] = new vf2d(pos[i].x * InvScreenSize.x * 2.0f - 1.0f, (pos[i].y * InvScreenSize.y * 2.0f - 1.0f) * -1.0f);
            di.uv[i] = uv[i];
            di.tint[i] = tint;
            di.w[i] = depth[i];
        }

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPolygonDecal(Decal decal, vf2d[] pos, float[] depth, vf2d[] uv, Pixel[] colors, Pixel tint)
    {
        DecalInstance di = new()
        {
            decal = decal,
            points = (uint)pos.Length,
            pos = new vf2d[pos.Length],
            uv = new vf2d[pos.Length],
            w = new float[pos.Length],
            tint = new Pixel[pos.Length],
            mode = DecalMode,
            structure = DecalStructure
        };

        for (uint i = 0; i < di.points; i++)
        {
            di.pos[i] = new vf2d(pos[i].x * InvScreenSize.x * 2.0f - 1.0f, (pos[i].y * InvScreenSize.y * 2.0f - 1.0f) * -1.0f);
            di.uv[i] = uv[i];
            di.tint[i] = colors[i] * tint;
            di.w[i] = depth[i];
        }

        Layers[(int)TargetLayer].DecalInstance.Add(di);
    }

    public void DrawPolygonDecal(Decal decal, vf2d[] pos, vf2d[] uv, Pixel[] colors, Pixel tint)
    {
        Pixel[] newColors = new Pixel[colors.Length];
        new Span<Pixel>(newColors).Fill(csPixelGameEngineCore.Pixel.WHITE);

        for (int i = 0; i < colors.Length; i++)
        {
            newColors[i] = colors[i] * tint;
        }

        DrawPolygonDecal(decal, pos, uv, newColors);
    }

    public void DrawLineDecal(vf2d pos1, vf2d pos2, Pixel p)
    {
        var dm = DecalMode;
        DecalMode = DecalMode.WIREFRAME;
        DrawPolygonDecal(null, [pos1, pos2], [new (0, 0), new (0, 0)], p);
        DecalMode = dm;
    }

    public void DrawRectDecal(vf2d pos, vf2d size, Pixel col)
    {
        var dm = DecalMode;
        DecalMode = DecalMode.WIREFRAME;
        vf2d newSize = size;
        vf2d[] points = [ pos, new (pos.x, pos.y + newSize.y), pos + newSize, new (pos.x + newSize.x, pos.y) ];
        vf2d[] uvs = [ new (), new (), new (), new () ];
        Pixel[] cols = [ col, col, col, col ];
        DrawExplicitDecal(null, points, uvs, cols, 4);
        DecalMode = dm;
    }

    public void FillRectDecal(vf2d pos, vf2d size, Pixel col)
    {
        vf2d vNewSize = size;
        vf2d[] points = [ pos, new (pos.x, pos.y + vNewSize.y), pos + vNewSize, new (pos.x + vNewSize.x, pos.y) ];
        vf2d[] uvs = [ new (0, 0), new (0, 0), new (0, 0), new (0, 0) ];
        Pixel[] cols = [ col, col, col, col ];
        DrawExplicitDecal(null, points, uvs, cols, 4);
    }

    public void GradientFillRectDecal(vf2d pos, vf2d size, Pixel colTL, Pixel colBL, Pixel colBR, Pixel colTR)
    {
        vf2d[] points = [ pos, new(pos.x, pos.y + size.y), pos + size, new(pos.x + size.x, pos.y) ];
        vf2d[] uvs = [ new(0, 0), new(0, 0), new(0, 0), new(0, 0) ];
        Pixel[] cols = [ colTL, colBL, colBR, colTR ];
        DrawExplicitDecal(null, points, uvs, cols, 4);
    }

    public void FillTriangleDecal(vf2d p0, vf2d p1, vf2d p2, Pixel col)
    {
        vf2d[] points = [ p0, p1, p2 ];
        vf2d[] uvs = [ new(0, 0), new(0, 0), new(0, 0) ];
        Pixel[] cols = [ col, col, col ];
        DrawExplicitDecal(null, points, uvs, cols, 3);
    }

    public void GradientTriangleDecal(vf2d p0, vf2d p1, vf2d p2, Pixel c0, Pixel c1, Pixel c2)
    {
        vf2d[] points = [ p0, p1, p2 ];
        vf2d[] uvs = [ new(0, 0), new(0, 0), new(0, 0) ];
        Pixel[] cols = [ c0, c1, c2 ];
        DrawExplicitDecal(null, points, uvs, cols, 3);
    }

    public void SetDecalMode(DecalMode mode) => DecalMode = mode;
    public void SetDecalStructure(DecalStructure structure) => DecalStructure = structure;

    #endregion // Drawing Methods

    #region Layer Methods
    public uint CreateLayer()
    {
        var ld = new LayerDesc(renderer);
        ld.DrawTarget.Create((uint)ScreenSize.x, (uint)ScreenSize.y);

        logger.LogDebug("Created layer {layerNum}. GL Texture ID: {textureId}", Layers.Count, ld.DrawTarget.Decal.Id);

        Layers.Add(ld);
        return (uint)(Layers.Count - 1);
    }

    /// <summary>
    /// Sets the draw target to the given sprite. Null can be used for layer 0.
    /// </summary>
    /// <param name="target">Sprite to set draw target to. Null for layer 0.</param>
    public void SetDrawTarget(Sprite target)
    {
        if (target != null)
        {
            DrawTarget = target;
        }
        else
        {
            TargetLayer = 0;
            if (!(Layers.Count == 0))
                DrawTarget = Layers[0].DrawTarget.Sprite;
        }
    }

    public void SetDrawTarget(byte layer, bool dirty = true)
    {
        if (layer < Layers.Count)
        {
            logger.LogDebug("Drawing to layer {layer}", layer);

            DrawTarget = Layers[layer].DrawTarget.Sprite;
            Layers[layer].bUpdate = dirty;
            TargetLayer = layer;
        }
    }

    public Sprite GetDrawTarget() => DrawTarget;

    public int GetDrawTargetWidth() => DrawTarget?.Width ?? 0;

    public int GetDrawTargetHeight() => DrawTarget?.Height ?? 0;

    public void EnableLayer(byte layer, bool enabled)
    {
        if (layer < Layers.Count)
        {
            Layers[layer].bShow = enabled;
        }
    }

    public void SetLayerOffset(byte layer, vf2d offset) => SetLayerOffset(layer, offset.x, offset.y);

    public void SetLayerOffset(byte layer, float xOffset, float yOffset)
    {
        if (layer < Layers.Count)
        {
            Layers[layer].vOffset = new vf2d(xOffset, yOffset);
        }
    }

    public void SetLayerScale(byte layer, vf2d scale) => SetLayerScale(layer, scale.x, scale.y);

    public void SetLayerScale(byte layer, float xScale, float yScale)
    {
        if (layer < Layers.Count)
        {
            Layers[layer].vScale = new vf2d(xScale, yScale);
        }
    }

    public void SetLayerTint(byte layer, Pixel tint)
    {
        if (layer < Layers.Count)
        {
            Layers[layer].Tint = tint;
        }
    }

    public void SetLayerCustomRenderFunction(byte layer, Action func)
    {
        if (layer < Layers.Count)
        {
            Layers[layer].funcHook = func;
        }
    }

    public LayerDesc[] GetLayers() => [.. Layers];

    #endregion // Layer Methods

    // More functions for compatibility
    public void SetPixelMode(Pixel.Mode m) => PixelMode = m;
    public Pixel.Mode GetPixelMode() => PixelMode;
    public void SetPixelBlend(float blend) => BlendFactor = blend;

    /// <summary>
    /// This sets a custom pixel blending function and switches to the CUSTOM pixel blending mode.
    /// 
    /// You can also set the CustomPixelBlender property.
    /// </summary>
    /// <param name="pixelBlendFunc">Pixel blending function</param>
    public void SetPixelMode(PixelBlender pixelBlendFunc) => CustomPixelBlender = pixelBlendFunc;

    #region Overrideable methods you should not override

    // I would not recommend doing things this way, but if you must (cause you want to do things the OLC way)
    // then I have provided them here. I would consider using the events instead, when possible.

    /// <summary>
    /// Override if you must do it the OLC way
    /// </summary>
    /// <returns></returns>
    protected virtual bool OnUserCreate()
    {
        return true;
    }

    /// <summary>
    /// Override if you must do it the OLC way
    /// </summary>
    /// <param name="fElapsedTime"></param>
    /// <returns></returns>
    protected virtual bool OnUserUpdate(float fElapsedTime)
    {
        OnFrameUpdate?.Invoke(this, new FrameUpdateEventArgs(fElapsedTime));
        return true;
    }

    /// <summary>
    /// Override if you must do it the OLC way
    /// </summary>
    /// <returns></returns>
    protected virtual bool OnUserDestroy()
    {
        return true;
    }

    /// <summary>
    /// Called when a text entry is confirmed with the enter key
    /// </summary>
    /// <param name="text"></param>
    protected virtual void OnTextEntryComplete(string text)
    {

    }

    /// <summary>
    /// Called when a console command is executed
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    protected virtual bool OnConsoleCommand(string command)
    {
        return true;
    }

    #endregion // Overrideable methods you should not override

    public void pgex_Register(PGEX pgex)
    {
        // This should probably be improved
        if (!_extensions.Any(ext => ext == pgex))
        {
            _extensions.Add(pgex);
        }
    }
}
