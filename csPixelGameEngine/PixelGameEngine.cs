using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csPixelGameEngine.enums;
using static csPixelGameEngine.Sprite;

namespace csPixelGameEngine
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
            get { return drawTarget; }

            set
            {
                if (value == null)
                    drawTarget = DefaultDrawTarget;
                else
                    drawTarget = value;
            }
        }

        private float blendFactor;
        public float BlendFactor
        {
            get { return blendFactor; }

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

        private float subPixelOffsetX;
        public float SubPixelOffsetX
        {
            get
            {
                return subPixelOffsetX;
            }

            set
            {
                subPixelOffsetX = value * PixelX;
            }
        }

        private float subPixelOffsetY;
        public float SubPixelOffsetY
        {
            get
            {
                return subPixelOffsetY;
            }

            set
            {
                subPixelOffsetY = value * PixelY;
            }
        }

        private Pixel.BlendMode pixelBlendMode;
        public Pixel.BlendMode PixelBlendMode
        {
            get { return this.pixelBlendMode; }

            set
            {
                // Don't allow custom blender if no blender function defined
                if (funcPixelBlender == null && value == Pixel.BlendMode.CUSTOM)
                    return;

                this.pixelBlendMode = value;
            }
        }

        private PixelBlender funcPixelBlender;
        public PixelBlender CustomPixelBlender
        {
            get { return funcPixelBlender; }

            set
            {
                // Reset blend mode to normal if blender function is removed
                if (value == null)
                    this.PixelBlendMode = Pixel.BlendMode.NORMAL;
                else
                    this.PixelBlendMode = Pixel.BlendMode.CUSTOM;

                this.funcPixelBlender = value;
            }
        }

        private Sprite fontSprite;

        public PixelGameEngine(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                this.AppName = "Undefined";
            else
                this.AppName = appName;
        }

        public rcode Construct(uint pixel_w, uint pixel_h, GLWindow window)
        {
            this.Window = window;
            this.ScreenWidth = (uint)this.Window.Width;
            this.ScreenHeight = (uint)this.Window.Height;
            this.PixelWidth = pixel_w;
            this.PixelHeight = pixel_h;
            this.FullScreen = (window.WindowState == OpenTK.WindowState.Fullscreen);
            this.EnableVSYNC = (window.VSync == OpenTK.VSyncMode.On);
            this.PixelX = 2.0f / (float)this.ScreenWidth;
            this.PixelY = 2.0f / (float)this.ScreenHeight;

            if (this.PixelWidth == 0 || this.PixelHeight == 0 ||
                this.ScreenWidth == 0 || this.ScreenHeight == 0)
                return rcode.FAIL;

            // Load the default font sheet
            construct_fontSheet();

            // Create a sprite that represents the primary drawing target
            this.DefaultDrawTarget = new Sprite(ScreenWidth, ScreenHeight);
            this.drawTarget = this.DefaultDrawTarget;

            return rcode.OK;
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

        public rcode Start()
        {
            return rcode.OK;
        }

        #region Overridable Interfaces

        /// <summary>
        /// Called once on application startup. Use to load resources
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUserCreate()
        {
            return false;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public virtual bool OnUserUpdate(float elapsedTime)
        {
            return false;
        }

        /// <summary>
        /// Called once on application termination.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUserDestroy()
        {
            return true;
        }

        #endregion // Overridable Interfaces

        /// <summary>
        /// Check if this window is currently in focus
        /// </summary>
        /// <returns>True if window has focus</returns>
        public bool IsFocused()
        {
            throw new NotImplementedException();
        }

        #region Input Methods

        /// <summary>
        /// Get the state of a key
        /// </summary>
        /// <param name="k">Key to query state of</param>
        /// <returns></returns>
        public HWButton GetKey(Key k)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the state of a mouse button
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public HWButton GetMouse(uint button)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get mouse X coordinate in "pixel" space
        /// </summary>
        /// <returns></returns>
        public int GetMouseX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get mouse Y coordinate in "pixel" space
        /// </summary>
        /// <returns></returns>
        public int GetMouseY()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get mouse wheel delta
        /// </summary>
        /// <returns></returns>
        public int GetMouseWheel()
        {
            throw new NotImplementedException();
        }

        #endregion // Input Methods

        #region Drawing Methods

        /// <summary>
        /// Draws a single Pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="p">Pixel color (leave null for white)</param>
        /// <returns></returns>
        public virtual bool Draw(int x, int y, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Draws a line from (x1,y1) to (x2,y2)
        public void DrawLine(int x1, int y1, int x2, int y2, Pixel p = null, uint pattern = 0xFFFFFFFF)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Draws a circle located at (x,y) with radius
        public void DrawCircle(int x, int y, int radius, Pixel p = null, byte mask = 0xFF)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Fills a circle located at (x,y) with radius
        public void FillCircle(int x, int y, int radius, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Draws a rectangle at (x,y) to (x+w,y+h)
        public void DrawRect(int x, int y, int w, int h, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Fills a rectangle at (x,y) to (x+w,y+h)
        public void FillRect(int x, int y, int w, int h, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Draws a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Flat fills a triangle between points (x1,y1), (x2,y2) and (x3,y3)
        public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p = null)
        {
            if (p == null)
                p = Pixel.WHITE;

            throw new NotImplementedException();
        }

        // Draws an entire sprite at location (x,y)
        public void DrawSprite(int x, int y, Sprite sprite, uint scale = 1)
        {
            throw new NotImplementedException();
        }

        // Draws an area of a sprite at location (x,y), where the
        // selected area is (ox,oy) to (ox+w,oy+h)
        public void DrawPartialSprite(int x, int y, Sprite sprite, int ox, int oy, int w, int h, uint scale = 1)
        {
            throw new NotImplementedException();
        }

        // Draws a single line of text
        public void DrawString(int x, int y, string sText, Pixel col = null, uint scale = 1)
        {
            if (col == null)
                col = Pixel.WHITE;

            int sx = 0;
            int sy = 0;
            Pixel.BlendMode m = this.PixelBlendMode;
            if (col.a != 255)
                this.PixelBlendMode = Pixel.BlendMode.ALPHA;
            else
                this.PixelBlendMode = Pixel.BlendMode.MASK;

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
                                    for (uint is_ = 0; is_ < scale; is_ ++)
                                        for (uint js = 0; js < scale; js++)
                                            Draw((int)(x + sx + (i * scale) + is_), (int)(y + sy + (j * scale) + js), col);
                    }
                    else
                    {
                        for (uint i = 0; i < 8; i++)
                            for (uint j = 0; j < 8; j++)
                                if (fontSprite.GetPixel((uint)(i + ox * 8), (uint)(j + oy * 8)).r > 0)
                                    Draw((int)(x + sx + i), (int)(y + sy + j), col);
                    }
                    sx += (int)(8 * scale);
                }
            }
            PixelBlendMode = m;
        }

        // Clears entire draw target to Pixel
        public void Clear(Pixel p)
        {
            throw new NotImplementedException();
        }

        // Resize the primary screen sprite
        public void SetScreenSize(int w, int h)
        {
            throw new NotImplementedException();
        }

        #endregion // Drawing Methods

    }
}
