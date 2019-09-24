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
        public String   AppName             { get; private set; }
        public int      ScreenWidth         { get; private set; }
        public int      ScreenHeight        { get; private set; }
        public int      DrawTargetWidth     { get; private set; }
        public int      DrawTagetHeight     { get; private set; }
        public Sprite   DefaultDrawTarget   { get; private set; }
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

        public PixelGameEngine()
        {
            this.AppName = "Undefined";

        }

        public rcode Construct(uint screen_w, uint screen_h, uint pixel_w, uint pixel_h, bool full_screen = false, bool vsync = false)
        {

            // Load the default font sheet
            // ConstructFontSheet();

            // Create a sprite that represents the primary drawing target
            DefaultDrawTarget = new Sprite(ScreenWidth, ScreenHeight);
            drawTarget = DefaultDrawTarget;

            return rcode.OK;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        public virtual bool OnUserUpdate(float elapsedTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called once on application termination.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnUserDestroy()
        {
            throw new NotImplementedException();
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
        /// Specify which Sprite should be the target of drawing functions, use null
        /// to specify the primary screen
        /// </summary>
        public void SetDrawTarget(Sprite target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the pixel mode for different optimisations
        /// olc::Pixel::NORMAL = No transparency
        /// olc::Pixel::MASK   = Transparent if alpha is < 255
        /// olc::Pixel::ALPHA  = Full transparency
        /// </summary>
        /// <param name="m">Pixel mode</param>
        public void SetPixelMode(Mode m)
        {
            throw new NotImplementedException();
        }

        public Mode GetPixelMode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use a custom blend function
        /// </summary>
        public void SetPixelMode(PixelBlender blendFn)
        {
            throw new NotImplementedException();
        }

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

            throw new NotImplementedException();
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
