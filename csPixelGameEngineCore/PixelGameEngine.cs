using System;
using System.Collections.Generic;
using System.Text;
using csPixelGameEngineCore.Enums;

namespace csPixelGameEngineCore
{
    public delegate void PixelBlender(int x, int y, Pixel pSrc, Pixel pDst);

    public class PixelGameEngine
    {
        public string   AppName             { get; private set; }
        public GLWindow Window              { get; private set; }
        public bool     FullScreen          { get; private set; }
        public bool     EnableVSYNC         { get; private set; }
        public uint     ScreenWidth         { get; private set; }
        public uint     ScreenHeight        { get; private set; }
        public int      DrawTargetWidth     { get; private set; }
        public int      DrawTagetHeight     { get; private set; }
        public Sprite   DefaultDrawTarget   { get; private set; }
        public uint     PixelWidth          { get; private set; }
        public uint     PixelHeight         { get; private set; }
        public int      MousePosX           { get; private set; }
        public int      MousePosY           { get; private set; }
        public int      MouseWheelDelta     { get; private set; }
        public float    PixelX              { get; set; }
        public float    PixelY              { get; set; }

        private Sprite drawTarget;
        public Sprite DrawTarget
        {
            get => drawTarget;
            set => drawTarget = value ?? DefaultDrawTarget;
        }

        private float blendFactor;
        public float BlendFactor
        {
            get => blendFactor;

            set
            {
                if (value < 0.0f)
                    blendFactor = 0.0f;
                else if (value > 1.0f)
                    blendFactor = 1.0f;
                else
                    blendFactor = value;
            }
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

        private Sprite fontSprite;

        #region Events

        /// <summary>
        /// This event fires once per frame
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

        public PixelGameEngine(string appName)
        {
            AppName = string.IsNullOrWhiteSpace(appName) ? "Undefined" : appName;
        }

        public int Construct(uint screen_w, uint screen_h, GLWindow window)
        {
            Window          = window;
            ScreenWidth     = screen_w;
            ScreenHeight    = screen_h;
            FullScreen      = (window.WindowState == OpenTK.WindowState.Fullscreen);
            EnableVSYNC     = (window.VSync == OpenTK.VSyncMode.On);
            PixelWidth      = window.PixelWidth;
            PixelHeight     = window.PixelHeight;
            PixelX          = 2.0f / ScreenWidth;
            PixelY          = 2.0f / ScreenHeight;

            // Load the default font sheet
            construct_fontSheet();

            // Create a sprite that represents the primary drawing target
            DefaultDrawTarget = window.DrawTarget;
            drawTarget = DefaultDrawTarget;

            return 0;
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
        }

        public int Start()
        {
            OnCreate?.Invoke(this, new EventArgs());

            Window.Closed += (sender, cancelEventArgs) =>
            {
                OnDestroy?.Invoke(sender, EventArgs.Empty);
            };
            // Since Window already has an update loop with events, lets tap into that
            Window.UpdateFrame += (sender, frameEventArgs) =>
            {
                OnFrameUpdate?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.Time));
            };
            Window.RenderFrame += (sender, frameEventArgs) =>
            {
                OnFrameRender?.Invoke(sender, new FrameUpdateEventArgs(frameEventArgs.Time));
            };
            Window.Run(120, 60);

            return 0;
        }

        #region Drawing Methods

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

            if (p == default)
                p = Pixel.WHITE;

            return PixelBlendMode switch
            {
                BlendMode.NORMAL => DrawTarget.SetPixel(x, y, p),
                BlendMode.MASK when p.a == 255 => DrawTarget.SetPixel(x, y, p),
                _ => false
            };
        }

        private void swap<T>(ref T v1, ref T v2)
        {
            T temp = v1;
            v1 = v2;
            v2 = temp;
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
                if (y2 < y1) swap(ref y1, ref y2);
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
                if (x2 < x1) swap(ref x1, ref x2);
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

        // Draws a circle located at (x,y) with radius
        public void DrawCircle(int x, int y, int radius, Pixel p, byte mask = 0xFF)
        {
            if (p == default)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Fills a circle located at (x,y) with radius
        public void FillCircle(int x, int y, int radius, Pixel p)
        {
            if (p == default)
                p = Pixel.WHITE;

            throw new NotImplementedException();
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

        // Fills a rectangle at (x,y) to (x+w,y+h)
        public void FillRect(int x, int y, int w, int h, Pixel p)
        {
            // drawTarget.FillRect((uint)x, (uint)y, (uint)w, (uint)h, p);

            if (p == default)
                p = Pixel.WHITE;

            int x2 = x + w;
            int y2 = y + h;

            if (x < 0)
                x = 0;
            if (x >= (int)ScreenWidth)
                x = (int)ScreenWidth;
            if (y < 0)
                y = 0;
            if (y >= (int)ScreenHeight)
                y = (int)ScreenHeight;

            if (x2 < 0)
                x2 = 0;
            if (x2 >= (int)ScreenWidth)
                x2 = (int)ScreenWidth;
            if (y2 < 0)
                y2 = 0;
            if (y2 >= (int)ScreenHeight)
                y2 = (int)ScreenHeight;

            for (int i = x; i < x2; i++)
                for (int j = y; j < y2; j++)
                    Draw((uint)i, (uint)j, p);
        }

        // Draws a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            if (p == default)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Flat fills a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
        {
            if (p == default)
                p = Pixel.WHITE;

            throw new NotImplementedException();
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

        // Draws a single line of text
        public void DrawString(int x, int y, string sText, Pixel col, uint scale = 1)
        {
            if (col == default)
                col = Pixel.WHITE;

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

        // Resize the primary screen sprite
        public void SetScreenSize(int w, int h)
        {
            throw new NotImplementedException();
        }

        #endregion // Drawing Methods
    }
}
