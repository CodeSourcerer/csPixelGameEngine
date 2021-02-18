using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngineCore.Enums;
using csPixelGameEngineCore.Extensions;
using log4net;
using OpenTK.Input;

/*
	+-------------------------------------------------------------+
	|           OneLoneCoder Pixel Game Engine v1.23              |
	| "Like the command prompt console one, but not..." - javidx9 |
	+-------------------------------------------------------------+
    ... ported to C#!

License (OLC-3)
~~~~~~~~~~~~~~~
Copyright 2018 - 2019 OneLoneCoder.com
Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:
1. Redistributions or derivations of source code must retain the above copyright
notice, this list of conditions and the following disclaimer.
2. Redistributions or derivative works in binary form must reproduce the above
copyright notice. This list of conditions and the following disclaimer must be
reproduced in the documentation and/or other materials provided with the distribution.
3. Neither the name of the copyright holder nor the names of its contributors may
be used to endorse or promote products derived from this software without specific
prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS	"AS IS" AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.IN NO EVENT
SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

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
    - Easier to use with C# conventions than C++ conventions
        Ex: Using delegates vs lambdas, using properties vs attributes
*/
namespace csPixelGameEngineCore
{
    public delegate Pixel PixelBlender(uint x, uint y, Pixel pSrc, Pixel pDst);

    public class PixelGameEngine
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PixelGameEngine));

        public string   AppName             { get; private set; }
        //public GLWindow Window              { get; private set; }
        public bool     FullScreen          { get; private set; }
        public bool     EnableVSYNC         { get; private set; }
        public vec2d_i  ViewPos             { get; private set; }
        public vec2d_i  ViewSize            { get; private set; }
        public vec2d_i  WindowSize          { get; private set; }
        public int      DrawTargetWidth     { get; private set; }
        public int      DrawTargetHeight    { get; private set; }
        public Sprite   DefaultDrawTarget   { get; private set; }
        public int      MousePosX           { get; private set; }
        public int      MousePosY           { get; private set; }
        public int      MouseWheelDelta     { get; private set; }
        public vec2d_i  PixelSize           { get; private set; }
        public vec2d_f  PixelRatio          { get; private set; }  // no idea what this is right now...
        public uint     FPS                 { get; private set; }
        public uint     TargetLayer         { get; private set; }

        public List<LayerDesc> Layers       { get; private set; }

        private vec2d_i screenSize;
        public vec2d_i ScreenSize
        {
            get => screenSize;
            private set
            {
                screenSize = value;
                invScreenSize = 1.0f / value;
            }
        }

        public Sprite DrawTarget
        {
            get;
            // Use SetDrawTarget()
            private set;
        }

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
                PixelBlendMode = value == null ? BlendMode.NORMAL : BlendMode.CUSTOM;

                funcPixelBlender = value;
            }
        }

        private BlendMode pixelBlendMode;
        public BlendMode PixelBlendMode
        {
            get => pixelBlendMode;

            set
            {
                // Don't allow custom blender if no blender function defined
                if (funcPixelBlender == null && value == BlendMode.CUSTOM)
                    return;

                pixelBlendMode = value;
            }
        }

        private vec2d_f invScreenSize;
        private readonly IRenderer renderer;
        private readonly IPlatform platform;
        private Sprite fontSprite;
        private Decal fontDecal;
        private HWButton[] btnStates = {
            new HWButton { Released = true, Pressed = false, Held = false },
            new HWButton { Released = true, Pressed = false, Held = false },
            new HWButton { Released = true, Pressed = false, Held = false }
        };

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
        public event EventHandler OnDestroy;

        #endregion // Events

        public PixelGameEngine(IRenderer renderer, IPlatform platform, string appName = null)
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            if (platform == null) throw new ArgumentNullException(nameof(platform));

            this.renderer = renderer;
            this.platform = platform;
            AppName = string.IsNullOrWhiteSpace(appName) ? "Undefined" : appName;
            Layers = new List<LayerDesc>();
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
        public void Construct(uint screen_w, uint screen_h, uint pixel_w, uint pixel_h, bool full_screen, bool vsync)
        {
            if (screen_w == 0) throw new ArgumentException("Must be at least 1", nameof(screen_w));
            if (screen_h == 0) throw new ArgumentException("Must be at least 1", nameof(screen_h));
            if (pixel_w == 0) throw new ArgumentException("Must be at least 1", nameof(pixel_w));
            if (pixel_h == 0) throw new ArgumentException("Must be at least 1", nameof(pixel_h));

            //Window = window;
            ScreenSize = new vec2d_i { x = (int)screen_w, y = (int)screen_h };
            FullScreen = full_screen; //(window.WindowState == OpenTK.WindowState.Fullscreen);
            EnableVSYNC = vsync; //(window.VSync == OpenTK.VSyncMode.On);
            PixelSize = new vec2d_i { x = (int)pixel_w, y = (int)pixel_h };
            PixelRatio = 2.0f / ScreenSize;

            // Create a sprite that represents the primary drawing target
            //DefaultDrawTarget = window.DrawTarget;
            //drawTarget = DefaultDrawTarget;
        }

        private void construct_fontSheet()
        {
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

            fontSprite = new Sprite(128, 48);
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
                    byte k = (r & (1 << i)) != 0 ? (byte)0xFF : (byte)0x00;
                    fontSprite.SetPixel((uint)px, (uint)py, new Pixel(k, k, k, k));
                    if (++py == 48) { px++; py = 0; }
                }
            }

            fontDecal = new Decal(fontSprite, renderer);
        }

        /// <summary>
        /// Set the screen size
        /// </summary>
        /// <param name="w">Width, in pixels</param>
        /// <param name="h">Height, in pixels</param>
        public void SetScreenSize(uint w, uint h)
        {
            if (w == 0) throw new ArgumentException("Must be at least 1", nameof(w));
            if (h == 0) throw new ArgumentException("Must be at least 1", nameof(h));

            ScreenSize = new vec2d_i { x = (int)w, y = (int)h };
            // TODO: Update layers

            //Window.DrawTarget = new Sprite(ScreenWidth, ScreenHeight);
            DrawTarget = null;

            renderer.ClearBuffer(Pixel.BLACK, true);
            renderer.DisplayFrame();
            renderer.ClearBuffer(Pixel.BLACK, true);
            renderer.UpdateViewport(ViewPos, ViewSize);
        }

        /// <summary>
        /// This starts the engine and the rendering loop.
        /// </summary>
        /// <param name="maxUpdateRate"></param>
        /// <returns></returns>
        public int Start()
        {
            // In the C++ implementation, there is a platform class for handling platform specific things.
            // This is .NET Core, which runs on all platforms it supports so we don't need it and thus, we
            // deviate a bit. Instead of creating a window, we will set up the renderer here.
            olc_UpdateWindowSize(WindowSize.x, WindowSize.y);

            // The C++ implementation creates an engine thread here, which does a whole lot of things .NET handles for us.
            // Instead, we will do the gist of what it was doing by using event handlers.
            olc_PrepareEngine();

            // This simulates "OnUserCreate()"
            OnCreate?.Invoke(this, new EventArgs());
            
            platform.Closed += (sender, eventArgs) =>
            {
                OnDestroy?.Invoke(sender, EventArgs.Empty);
            };

            platform.Resize += (sender, eventArgs) =>
            {
                olc_UpdateWindowSize(platform.WindowWidth, platform.WindowHeight);
            };

            platform.UpdateFrame += (sender, frameEventArgs) =>
            {
                OnFrameUpdate?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.ElapsedTime));
                // Reset wheel delta after frame
                MouseWheelDelta = 0;
            };

            renderer.RenderFrame += (sender, frameEventArgs) =>
            {
                OnFrameRender?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.ElapsedTime));
                olc_CoreUpdate();
            };

            platform.MouseWheel += (sender, mouseWheelEventArgs) =>
            {
                MouseWheelDelta = mouseWheelEventArgs.Delta;
            };

            platform.MouseMove += (sender, mouseMoveEventArgs) =>
            {
                MousePosX = mouseMoveEventArgs.X;
                MousePosY = mouseMoveEventArgs.Y;
            };

            platform.MouseDown += (sender, mouseButtonEventArgs) =>
            {
                updateMouseButtonStates(mouseButtonEventArgs);
            };

            platform.MouseUp += (sender, mouseButtonEventArgs) =>
            {
                updateMouseButtonStates(mouseButtonEventArgs);
            };

            platform.StartSystemEventLoop();

            return 0;
        }

        public void olc_PrepareEngine()
        {
            renderer.CreateDevice(FullScreen, EnableVSYNC, ViewPos, ViewSize);

            // TODO: Implement this
            CreateLayer();
            Layers[0].bUpdate = true;
            Layers[0].bShow = true;
            DrawTarget = Layers[0].DrawTarget;

            construct_fontSheet();
        }

        public void olc_CoreUpdate()
        {
            // Handle mouse button held state
            for (int btn = 0; btn < btnStates.Length; btn++)
            {
                btnStates[btn].Held = btnStates[btn].Pressed;
            }

            renderer.UpdateViewport(ViewPos, ViewSize);
            renderer.ClearBuffer(Pixel.BLACK, true);

            // Ensure layer 0 is active
            Layers[0].bUpdate = true;
            Layers[0].bShow = true;

            renderer.PrepareDrawing();

            // Draw all active layers
            foreach (var layer in Layers)
            {
                if (layer.bShow)
                {
                    if (layer.funcHook == null)
                    {
                        renderer.ApplyTexture(layer.ResID);
                        if (layer.bUpdate)
                        {
                            renderer.UpdateTexture(layer.ResID, layer.DrawTarget);
                            layer.bUpdate = false;
                        }

                        renderer.DrawLayerQuad(layer.vOffset, layer.vScale, layer.Tint);

                        // Display decals in order for this layer
                        foreach (var decal in layer.DecalInstance)
                        {
                            renderer.DrawDecalQuad(decal);
                        }

                        layer.DecalInstance.Clear();
                    }
                    else
                    {
                        layer.funcHook();
                    }
                }
            }

            renderer.DisplayFrame();
        }

        public void olc_UpdateWindowSize(int width, int height)
        {
            WindowSize = new vec2d_i { x = width, y = height };
            olc_UpdateViewport();
        }

        public void olc_UpdateViewport()
        {
            int windowWidth = ScreenSize.x * PixelSize.x;
            int windowHeight = ScreenSize.y * PixelSize.y;
            float windowAspectRatio = windowWidth / (float)windowHeight;

            var viewY = (int)(WindowSize.x / windowAspectRatio);
            if (viewY > WindowSize.y)
            {
                viewY = WindowSize.y;
            }

            ViewSize = new vec2d_i { x = WindowSize.x, y = viewY };
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
        public HWButton GetMouse(uint button)
        {
            return btnStates[button];
        }

        #region Drawing Methods

        public bool Draw(vec2d_i pos, Pixel p)
        {
            return Draw((uint)pos.x, (uint)pos.y, p);
        }

        /// <summary>
        /// Draws a single Pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p">Pixel color (leave null for white)</param>
        /// <returns></returns>
        public virtual bool Draw(uint x, uint y, Pixel p)
        {
            if (DrawTarget == null)
                return false;

            return PixelBlendMode switch
            {
                BlendMode.NORMAL => DrawTarget.SetPixel(x, y, p),
                BlendMode.MASK when p.a == 255 => DrawTarget.SetPixel(x, y, p),
                BlendMode.ALPHA => DrawTarget.SetPixel(x, y, alphaBlend(x, y, p)),
                BlendMode.CUSTOM => DrawTarget.SetPixel(x, y, CustomPixelBlender(x, y, DrawTarget.GetPixel(x, y), p)),
                _ => false
            };
        }

        private Pixel alphaBlend(uint x, uint y, Pixel p)
        {
            Pixel d = DrawTarget.GetPixel(x, y);
            float a = (p.a / 255.0f) * BlendFactor;
            float c = 1.0f - a;
            float r = a * p.r + c * d.r;
            float g = a * p.g + c * d.g;
            float b = a * p.b + c * d.b;
            return new Pixel((byte)r, (byte)g, (byte)b);
        }

        // Draws a line from (x1,y1) to (x2,y2)
        public void DrawLine(int x1, int y1, int x2, int y2, Pixel p, uint pattern = 0xFFFFFFFF)
        {
            if (p == default)
                p = Pixel.WHITE;

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
                        Draw((uint)x1, (uint)y, p);
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
                        Draw((uint)x, (uint)y1, p);
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
                    Draw((uint)x, (uint)y, p);

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
                        Draw((uint)x, (uint)y, p);
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
                    Draw((uint)x, (uint)y, p);

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
                        Draw((uint)x, (uint)y, p);
                }
            }
        }

        public void DrawCircle(vec2d_i pos, int radius, Pixel p, byte mask = 0xFF)
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
                if ((mask & 0x01) != 0) Draw((uint)(x + x0), (uint)(y - y0), p);
                if ((mask & 0x02) != 0) Draw((uint)(x + y0), (uint)(y - x0), p);
                if ((mask & 0x04) != 0) Draw((uint)(x + y0), (uint)(y + x0), p);
                if ((mask & 0x08) != 0) Draw((uint)(x + x0), (uint)(y + y0), p);
                if ((mask & 0x10) != 0) Draw((uint)(x - x0), (uint)(y + y0), p);
                if ((mask & 0x20) != 0) Draw((uint)(x - y0), (uint)(y + x0), p);
                if ((mask & 0x40) != 0) Draw((uint)(x - y0), (uint)(y - x0), p);
                if ((mask & 0x80) != 0) Draw((uint)(x - x0), (uint)(y - y0), p);
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
        public void FillCircle(vec2d_i pos, int radius, Pixel p)
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
                Parallel.For(sx, ex, (i) => Draw((uint)i, (uint)ny, p));
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
        public void DrawRect(vec2d_i pos, vec2d_i size, Pixel p)
        {
            DrawRect(pos.x, pos.y, size.x, size.y, p);
        }

        // Draws a rectangle at (x,y) to (x+w,y+h)
        public void DrawRect(int x, int y, int w, int h, Pixel p)
        {
            if (p == default)
                p = Pixel.WHITE;

            DrawLine(x, y, x + w, y, p);
            DrawLine(x + w, y, x + w, y + h, p);
            DrawLine(x + w, y + h, x, y + h, p);
            DrawLine(x, y + h, x, y, p);
        }

        public void FillRect(vec2d_i pos, vec2d_i size, Pixel p)
        {
            FillRect(pos.x, pos.y, size.x, size.y, p);
        }

        // Fills a rectangle at (x,y) to (x+w,y+h)
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
                    Draw((uint)i, (uint)j, p);
        }

        /// <summary>
        /// DrawTriangle, vector edition
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        /// <param name="p"></param>
        public void DrawTriangle(vec2d_i pos1, vec2d_i pos2, vec2d_i pos3, Pixel p)
        {
            DrawTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p);
        }

        // Draws a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
        }

        public void FillTriangle(vec2d_i pos1, vec2d_i pos2, vec2d_i pos3, Pixel p)
        {
            FillTriangle(pos1.y, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p);
            //DrawFilledTriangle(pos1.y, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p);
        }

        // Flat fills a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        // https://www.avrfreaks.net/sites/default/files/triangles.c
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            Action<int, int, int> drawline = (sx, ex, ny) =>
            {
                for (int i = sx; i <= ex; i++)
                {
                    Draw((uint)i, (uint)ny, p);
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

        // Draws an entire sprite at location (x,y)
        public void DrawSprite(int x, int y, Sprite sprite, uint scale = 1)
        {
            if (sprite == null)
                return;

            Pixel p;
            uint iScale, jScale;
            if (scale > 1)
            {
                for (uint i = 0; i < sprite.Width; i++)
                    for (uint j = 0; j < sprite.Height; j++)
                    {
                        p = sprite.GetPixel(i, j);
                        iScale = (uint)x + i * scale;
                        jScale = (uint)y + j * scale;
                        for (uint _is = 0; _is < scale; _is++)
                            for (uint js = 0; js < scale; js++)
                                Draw(iScale + _is, jScale + js, p);
                    }
            }
            else
            {
                for (uint i = 0; i < sprite.Width; i++)
                    for (uint j = 0; j < sprite.Height; j++)
                        Draw((uint)(x + i), (uint)(y + j), sprite.GetPixel(i, j));
            }
        }

        public void DrawPartialSprite(vec2d_i pos, Sprite sprite, vec2d_i sourcepos, vec2d_i size, uint scale = 1)
        {
            DrawPartialSprite(pos.x, pos.y, sprite, sourcepos.x, sourcepos.y, size.x, size.y, scale);
        }

        // Draws an area of a sprite at location (x,y), where the
        // selected area is (ox,oy) to (ox+w,oy+h)
        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h, uint scale = 1)
        {
            if (sprite == null)
                return;

            if (scale > 1)
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        Pixel p = sprite.GetPixel((uint)(i + ox), (uint)(j + oy));
                        for (uint _is = 0; _is < scale; _is++)
                        {
                            for (uint js = 0; js < scale; js++)
                            {
                                Draw((uint)(x + (i * scale) + _is), (uint)(y + (j * scale) + js), p);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        Draw((uint)(x + i), (uint)(y + j), sprite.GetPixel((uint)(i + ox), (uint)(j + oy)));
                    }
                }
            }
        }

        public void DrawString(vec2d_i pos, string sText, Pixel col, uint scale = 1)
        {
            DrawString(pos.x, pos.y, sText, col, scale);
        }

        // Draws a single line of text
        public void DrawString(int x, int y, string sText, Pixel col, uint scale = 1)
        {
            int sx = 0;
            int sy = 0;
            BlendMode m = PixelBlendMode;
            if (col.a != 255)
                PixelBlendMode = BlendMode.ALPHA;
            else
                PixelBlendMode = BlendMode.MASK;

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
                        for (uint i = 0; i < 8; i++)
                            for (uint j = 0; j < 8; j++)
                                if (fontSprite.GetPixel((uint)(i + ox * 8), (uint)(j + oy * 8)).r > 0)
                                    for (uint is_ = 0; is_ < scale; is_++)
                                        for (uint js = 0; js < scale; js++)
                                            Draw((uint)(x + sx + (i * scale) + is_), (uint)(y + sy + (j * scale) + js), col);
                    }
                    else
                    {
                        for (uint i = 0; i < 8; i++)
                            for (uint j = 0; j < 8; j++)
                                if (fontSprite.GetPixel((uint)(i + ox * 8), (uint)(j + oy * 8)).r > 0)
                                    Draw((uint)(x + sx + i), (uint)(y + sy + j), col);
                    }
                    sx += (int)(8 * scale);
                }
            }
            PixelBlendMode = m;
        }

        // Clears entire draw target to Pixel
        public void Clear(Pixel p)
        {
            int pixelCount = DrawTarget.ColorData.Length;
            for (int i = 0; i < pixelCount; i++)
                DrawTarget.ColorData[i] = p;
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
        public void DrawDecal(vec2d_f pos, Decal decal, vec2d_f? scale = null, Pixel? tint = null)
        {
            if (!scale.HasValue) scale = vec2d_f.UNIT;
            if (!tint.HasValue) tint = Pixel.WHITE;

            vec2d_f vScreenSpacePos = new vec2d_f
            {
                x = (pos.x * invScreenSize.x) * 2.0f - 1.0f,
                y = ((pos.y * invScreenSize.y) * 2.0f - 1.0f) * -1.0f
            };
            vec2d_f vScreenSpaceDim = new vec2d_f
            {
                x = vScreenSpacePos.x + (2.0f * decal.sprite.Width * invScreenSize.x) * scale.Value.x,
                y = vScreenSpacePos.y - (2.0f * decal.sprite.Height * invScreenSize.y) * scale.Value.y
            };

            DecalInstance di = new DecalInstance
            {
                decal = decal,
                tint = tint.Value
            };
            di.pos[0] = new vec2d_f { x = vScreenSpacePos.x, y = vScreenSpacePos.y };
            di.pos[1] = new vec2d_f { x = vScreenSpacePos.x, y = vScreenSpaceDim.y };
            di.pos[2] = new vec2d_f { x = vScreenSpaceDim.x, y = vScreenSpaceDim.y };
            di.pos[3] = new vec2d_f { x = vScreenSpaceDim.x, y = vScreenSpacePos.y };

            Layers[(int)TargetLayer].DecalInstance.Add(di);
        }

        /// <summary>
        /// Draws a region of a decal, with optional scale and tinting
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="decal"></param>
        /// <param name="source_pos"></param>
        /// <param name="source_size"></param>
        /// <param name="scale">If null, will use vec2d_f.UNIT</param>
        /// <param name="tint">If null, will use Pixel.WHITE</param>
        public void DrawPartialDecal(vec2d_f pos, Decal decal, vec2d_f source_pos, vec2d_f source_size, vec2d_f? scale = null, Pixel? tint = null)
        {
            if (!scale.HasValue) scale = vec2d_f.UNIT;
            if (!tint.HasValue) tint = Pixel.WHITE;

            vec2d_f vScreenSpacePos = new vec2d_f
            {
                x = (pos.x * invScreenSize.x) * 2.0f - 1.0f,
                y = ((pos.y * invScreenSize.y) * 2.0f - 1.0f) * -1.0f
            };
            vec2d_f vScreenSpaceDim = new vec2d_f
            {
                x = vScreenSpacePos.x + (2.0f * source_size.x * invScreenSize.x) * scale.Value.x,
                y = vScreenSpacePos.y - (2.0f * source_size.y * invScreenSize.y) * scale.Value.y
            };

            DecalInstance di = new DecalInstance
            {
                decal = decal,
                tint = tint.Value
            };
            di.pos[0] = new vec2d_f { x = vScreenSpacePos.x, y = vScreenSpacePos.y };
            di.pos[1] = new vec2d_f { x = vScreenSpacePos.x, y = vScreenSpaceDim.y };
            di.pos[2] = new vec2d_f { x = vScreenSpaceDim.x, y = vScreenSpaceDim.y };
            di.pos[3] = new vec2d_f { x = vScreenSpaceDim.x, y = vScreenSpacePos.y };

            vec2d_f uvtl = source_pos * decal.vUVScale;
            vec2d_f uvbr = uvtl + (source_size * decal.vUVScale);
            di.uv[0] = new vec2d_f { x = uvtl.x, y = uvtl.y };
            di.uv[1] = new vec2d_f { x = uvtl.x, y = uvbr.y };
            di.uv[2] = new vec2d_f { x = uvbr.x, y = uvbr.y };
            di.uv[3] = new vec2d_f { x = uvbr.x, y = uvtl.y };

            Layers[(int)TargetLayer].DecalInstance.Add(di);
        }

        /// <summary>
        /// Draw a decal with arbitrary bounds.
        /// </summary>
        /// <param name="decal"></param>
        /// <param name="pos">Must be an array of 4 elements (x1, y1, x2, y2)</param>
        /// <param name="tint">If null, will use Pixel.WHITE</param>
        public void DrawWarpedDecal(Decal decal, vec2d_f[] pos, Pixel? tint = null)
        {
            if (!tint.HasValue) tint = Pixel.WHITE;

            // TODO: Finish...
        }

        public void DrawRotatedDecal(vec2d_f pos, Decal decal, float fAngle, vec2d_f center, vec2d_f? scale = null, Pixel? tint = null)
        {
            DecalInstance di = new DecalInstance();
            di.decal = decal;
            di.tint = tint ?? Pixel.WHITE;
            di.pos[0] = (new vec2d_f(0.0f, 0.0f) - center) * (scale ?? vec2d_f.UNIT);
            di.pos[1] = (new vec2d_f(0.0f, decal.sprite.Height) - center) * (scale ?? vec2d_f.UNIT);
            di.pos[2] = (new vec2d_f(decal.sprite.Width, decal.sprite.Height) - center) * (scale ?? vec2d_f.UNIT);
            di.pos[3] = (new vec2d_f(decal.sprite.Width, 0.0f) - center) * (scale ?? vec2d_f.UNIT);
            float c = (float)Math.Cos(fAngle);
            float s = (float)Math.Sin(fAngle);
            for (int i = 0; i < 4; i++)
            {
                di.pos[i] = pos + new vec2d_f(di.pos[i].x * c - di.pos[i].y * s, di.pos[i].x * s + di.pos[i].y * c);
                di.pos[i] = di.pos[i] * invScreenSize * 2.0f - vec2d_f.UNIT;
                di.pos[i].y *= -1.0f;
            }
            Layers[(int)TargetLayer].DecalInstance.Add(di);
        }

        public void DrawStringDecal(int x, int y, string sText, Pixel? col = null)
        {
            DrawStringDecal(new vec2d_f { x = x, y = y }, sText, col);
        }

		public void DrawStringDecal(vec2d_f pos, string sText, Pixel? col = null, vec2d_f? scale = null)
        {
            vec2d_f spos = new vec2d_f();
            scale = scale ?? vec2d_f.UNIT;

            foreach (var c in sText)
            {
                if (c == '\n')
                {
                    spos.x = 0;
                    spos.y += 8.0f * scale.Value.y;
                }
                else
                {
                    int ox = (c - 32) % 16;
                    int oy = (c - 32) / 16;
                    DrawPartialDecal(pos + spos, fontDecal, new vec2d_f { x = ox * 8.0f, y = oy * 8.0f }, new vec2d_f(8.0f, 8.0f), scale, col);
                    spos.x += 8.0f * scale.Value.x;
                }
            }
        }

		public void DrawPartialRotatedDecal(vec2d_f pos, Decal decal, float fAngle, vec2d_f center, vec2d_f source_pos, vec2d_f source_size, vec2d_f? scale = null, Pixel? tint = null)
        {

        }

		public void DrawPartialWarpedDecal(Decal decal, vec2d_f[] pos, vec2d_f source_pos, vec2d_f source_size, Pixel? tint = null)
        {

        }

        #endregion // Drawing Methods

        #region Layer Methods
        public uint CreateLayer()
        {
            var ld = new LayerDesc();
            ld.DrawTarget = new Sprite((uint)ScreenSize.x, (uint)ScreenSize.y);
            ld.ResID = renderer.CreateTexture((uint)ScreenSize.x, (uint)ScreenSize.y);
            renderer.UpdateTexture(ld.ResID, ld.DrawTarget);

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
                DrawTarget = Layers[0].DrawTarget;
            }
        }

        public void SetDrawTarget(uint layer)
        {
            if (layer < Layers.Count)
            {
                DrawTarget = Layers[(int)layer].DrawTarget;
                Layers[(int)layer].bUpdate = true;
                TargetLayer = layer;
            }
        }

        public void EnableLayer(uint layer, bool enabled)
        {
            if (layer < Layers.Count)
            {
                Layers[(int)layer].bShow = enabled;
            }
        }

        public void SetLayerOffset(uint layer, vec2d_f offset)
        {
            SetLayerOffset(layer, offset.x, offset.y);
        }

        public void SetLayerOffset(uint layer, float xOffset, float yOffset)
        {
            if (layer < Layers.Count)
            {
                Layers[(int)layer].vOffset = new vec2d_f { x = xOffset, y = yOffset };
            }
        }

        public void SetLayerScale(uint layer, vec2d_f scale)
        {
            SetLayerScale(layer, scale.x, scale.y);
        }

        public void SetLayerScale(uint layer, float xScale, float yScale)
        {
            if (layer < Layers.Count)
            {
                Layers[(int)layer].vScale = new vec2d_f { x = xScale, y = yScale };
            }
        }

        public void SetLayerTint(uint layer, Pixel tint)
        {
            if (layer < Layers.Count)
            {
                Layers[(int)layer].Tint = tint;
            }
        }

        public void SetLayerCustomRenderFunction(uint layer, Action func)
        {
            if (layer < Layers.Count)
            {
                Layers[(int)layer].funcHook = func;
            }
        }
        #endregion // Layer Methods

        #region Overrideable methods you should not override

        // I would not recommend doing things this way, but if you must (cause you want to do things the OLC way)
        // then I have provided them here. I would consider using the events instead, when possible.

        /// <summary>
        /// Override if you must do it the OLC way
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnUserCreate()
        {
            return false;
        }

        /// <summary>
        /// Override if you must do it the OLC way
        /// </summary>
        /// <param name="fElapsedTime"></param>
        /// <returns></returns>
        protected virtual bool OnUserUpdate(float fElapsedTime)
        {
            return false;
        }

        /// <summary>
        /// Override if you must do it the OLC way
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnUserDestroy()
        {
            return true;
        }
        #endregion // Overrideable methods you should not override
    }
}
