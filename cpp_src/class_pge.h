class PixelGameEngine
{
	public:
		PixelGameEngine();
		virtual ~PixelGameEngine();
	public:
		olc::rcode Construct(int32_t screen_w, int32_t screen_h, int32_t pixel_w, int32_t pixel_h,
			bool full_screen = false, bool vsync = false, bool cohesion = false, bool realwindow = false);
		olc::rcode Start();

	public: // User Override Interfaces
		// Called once on application startup, use to load your resources
		virtual bool OnUserCreate();
		// Called every frame, and provides you with a time per frame value
		virtual bool OnUserUpdate(float fElapsedTime);
		// Called once on application termination, so you can be one clean coder
		virtual bool OnUserDestroy();

		// Called when a text entry is confirmed with "enter" key
		virtual void OnTextEntryComplete(const std::string& sText);
		// Called when a console command is executed
		virtual bool OnConsoleCommand(const std::string& sCommand);


	public: // Hardware Interfaces
		// Returns true if window is currently in focus
		bool IsFocused() const;
		// Get the state of a specific keyboard button
		HWButton GetKey(Key k) const;
		// Get the state of a specific mouse button
		HWButton GetMouse(uint32_t b) const;
		// Get Mouse X coordinate in "pixel" space
		int32_t GetMouseX() const;
		// Get Mouse Y coordinate in "pixel" space
		int32_t GetMouseY() const;
		// Get Mouse Wheel Delta
		int32_t GetMouseWheel() const;
		// Get the mouse in window space
		const olc::vi2d& GetWindowMouse() const;
		// Gets the mouse as a vector to keep Tarriest happy
		const olc::vi2d& GetMousePos() const;

		static const std::map<size_t, uint8_t>& GetKeyMap() { return mapKeys; }

		// Muck about with the GUI
		olc::rcode SetWindowSize(const olc::vi2d& vPos, const olc::vi2d& vSize);
		olc::rcode ShowWindowFrame(const bool bShowFrame);

	public: // Utility
		// Returns the width of the screen in "pixels"
		int32_t ScreenWidth() const;
		// Returns the height of the screen in "pixels"
		int32_t ScreenHeight() const;
		// Returns the width of the currently selected drawing target in "pixels"
		int32_t GetDrawTargetWidth() const;
		// Returns the height of the currently selected drawing target in "pixels"
		int32_t GetDrawTargetHeight() const;
		// Returns the currently active draw target
		olc::Sprite* GetDrawTarget() const;
		// Resize the primary screen sprite
		void SetScreenSize(int w, int h);
		// Specify which Sprite should be the target of drawing functions, use nullptr
		// to specify the primary screen
		void SetDrawTarget(Sprite* target);
		// Gets the current Frames Per Second
		uint32_t GetFPS() const;
		// Gets last update of elapsed time
		float GetElapsedTime() const;
		// Gets Actual Window size
		const olc::vi2d& GetWindowSize() const;
		// Gets Actual Window position
		const olc::vi2d& GetWindowPos() const;
		// Gets pixel scale
		const olc::vi2d& GetPixelSize() const;
		// Gets actual pixel scale
		const olc::vi2d& GetScreenPixelSize() const;
		// Gets "screen" size
		const olc::vi2d& GetScreenSize() const;
		// Gets any files dropped this frame
		const std::vector<std::string>& GetDroppedFiles() const;
		const olc::vi2d& GetDroppedFilesPoint() const;

	public: // CONFIGURATION ROUTINES
		// Layer targeting functions
		void SetDrawTarget(uint8_t layer, bool bDirty = true);
		void EnableLayer(uint8_t layer, bool b);
		void SetLayerOffset(uint8_t layer, const olc::vf2d& offset);
		void SetLayerOffset(uint8_t layer, float x, float y);
		void SetLayerScale(uint8_t layer, const olc::vf2d& scale);
		void SetLayerScale(uint8_t layer, float x, float y);
		void SetLayerTint(uint8_t layer, const olc::Pixel& tint);
		void SetLayerCustomRenderFunction(uint8_t layer, std::function<void()> f);

		std::vector<LayerDesc>& GetLayers();
		uint32_t CreateLayer();

		// Change the pixel mode for different optimisations
		// olc::Pixel::NORMAL = No transparency
		// olc::Pixel::MASK   = Transparent if alpha is < 255
		// olc::Pixel::ALPHA  = Full transparency
		void SetPixelMode(Pixel::Mode m);
		Pixel::Mode GetPixelMode();
		// Use a custom blend function
		void SetPixelMode(std::function<olc::Pixel(const int x, const int y, const olc::Pixel& pSource, const olc::Pixel& pDest)> pixelMode);
		// Change the blend factor from between 0.0f to 1.0f;
		void SetPixelBlend(float fBlend);

		// [ADVANCED] For those that really want to dick about with PGE :P
		// Note: Normal use of olc::PGE does not require you use these functions
		void adv_ManualRenderEnable(const bool bEnable);
		void adv_HardwareClip(const bool bScale, const olc::vi2d& viewPos, const olc::vi2d& viewSize, const bool bClear = false);
		void adv_FlushLayer(const size_t nLayerID);
		void adv_FlushLayerDecals(const size_t nLayerID);

	public: // DRAWING ROUTINES
		// Draws a single Pixel
		virtual bool Draw(int32_t x, int32_t y, Pixel p = olc::WHITE);
		bool Draw(const olc::vi2d& pos, Pixel p = olc::WHITE);
		// Draws a line from (x1,y1) to (x2,y2)
		void DrawLine(int32_t x1, int32_t y1, int32_t x2, int32_t y2, Pixel p = olc::WHITE, uint32_t pattern = 0xFFFFFFFF);
		void DrawLine(const olc::vi2d& pos1, const olc::vi2d& pos2, Pixel p = olc::WHITE, uint32_t pattern = 0xFFFFFFFF);
		// Draws a circle located at (x,y) with radius
		void DrawCircle(int32_t x, int32_t y, int32_t radius, Pixel p = olc::WHITE, uint8_t mask = 0xFF);
		void DrawCircle(const olc::vi2d& pos, int32_t radius, Pixel p = olc::WHITE, uint8_t mask = 0xFF);
		// Fills a circle located at (x,y) with radius
		void FillCircle(int32_t x, int32_t y, int32_t radius, Pixel p = olc::WHITE);
		void FillCircle(const olc::vi2d& pos, int32_t radius, Pixel p = olc::WHITE);
		// Draws a rectangle at (x,y) to (x+w,y+h)
		void DrawRect(int32_t x, int32_t y, int32_t w, int32_t h, Pixel p = olc::WHITE);
		void DrawRect(const olc::vi2d& pos, const olc::vi2d& size, Pixel p = olc::WHITE);
		// Fills a rectangle at (x,y) to (x+w,y+h)
		void FillRect(int32_t x, int32_t y, int32_t w, int32_t h, Pixel p = olc::WHITE);
		void FillRect(const olc::vi2d& pos, const olc::vi2d& size, Pixel p = olc::WHITE);
		// Draws a triangle between points (x1,y1), (x2,y2) and (x3,y3)
		void DrawTriangle(int32_t x1, int32_t y1, int32_t x2, int32_t y2, int32_t x3, int32_t y3, Pixel p = olc::WHITE);
		void DrawTriangle(const olc::vi2d& pos1, const olc::vi2d& pos2, const olc::vi2d& pos3, Pixel p = olc::WHITE);
		// Flat fills a triangle between points (x1,y1), (x2,y2) and (x3,y3)
		void FillTriangle(int32_t x1, int32_t y1, int32_t x2, int32_t y2, int32_t x3, int32_t y3, Pixel p = olc::WHITE);
		void FillTriangle(const olc::vi2d& pos1, const olc::vi2d& pos2, const olc::vi2d& pos3, Pixel p = olc::WHITE);
		// Fill a textured and coloured triangle
		void FillTexturedTriangle(std::vector<olc::vf2d> vPoints, std::vector<olc::vf2d> vTex, std::vector<olc::Pixel> vColour, olc::Sprite* sprTex);
		void FillTexturedPolygon(const std::vector<olc::vf2d>& vPoints, const std::vector<olc::vf2d>& vTex, const std::vector<olc::Pixel>& vColour, olc::Sprite* sprTex, olc::DecalStructure structure = olc::DecalStructure::LIST);
		// Draws an entire sprite at location (x,y)
		void DrawSprite(int32_t x, int32_t y, Sprite* sprite, uint32_t scale = 1, uint8_t flip = olc::Sprite::NONE);
		void DrawSprite(const olc::vi2d& pos, Sprite* sprite, uint32_t scale = 1, uint8_t flip = olc::Sprite::NONE);
		// Draws an area of a sprite at location (x,y), where the
		// selected area is (ox,oy) to (ox+w,oy+h)
		void DrawPartialSprite(int32_t x, int32_t y, Sprite* sprite, int32_t ox, int32_t oy, int32_t w, int32_t h, uint32_t scale = 1, uint8_t flip = olc::Sprite::NONE);
		void DrawPartialSprite(const olc::vi2d& pos, Sprite* sprite, const olc::vi2d& sourcepos, const olc::vi2d& size, uint32_t scale = 1, uint8_t flip = olc::Sprite::NONE);
		// Draws a single line of text - traditional monospaced
		void DrawString(int32_t x, int32_t y, const std::string& sText, Pixel col = olc::WHITE, uint32_t scale = 1);
		void DrawString(const olc::vi2d& pos, const std::string& sText, Pixel col = olc::WHITE, uint32_t scale = 1);
		olc::vi2d GetTextSize(const std::string& s);
		// Draws a single line of text - non-monospaced
		void DrawStringProp(int32_t x, int32_t y, const std::string& sText, Pixel col = olc::WHITE, uint32_t scale = 1);
		void DrawStringProp(const olc::vi2d& pos, const std::string& sText, Pixel col = olc::WHITE, uint32_t scale = 1);
		olc::vi2d GetTextSizeProp(const std::string& s);

		// Decal Quad functions
		void SetDecalMode(const olc::DecalMode& mode);
		void SetDecalStructure(const olc::DecalStructure& structure);
		// Draws a whole decal, with optional scale and tinting
		void DrawDecal(const olc::vf2d& pos, olc::Decal* decal, const olc::vf2d& scale = { 1.0f,1.0f }, const olc::Pixel& tint = olc::WHITE);
		// Draws a region of a decal, with optional scale and tinting
		void DrawPartialDecal(const olc::vf2d& pos, olc::Decal* decal, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::vf2d& scale = { 1.0f,1.0f }, const olc::Pixel& tint = olc::WHITE);
		void DrawPartialDecal(const olc::vf2d& pos, const olc::vf2d& size, olc::Decal* decal, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint = olc::WHITE);
		// Draws fully user controlled 4 vertices, pos(pixels), uv(pixels), colours
		void DrawExplicitDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::vf2d* uv, const olc::Pixel* col, uint32_t elements = 4);
		// Draws a decal with 4 arbitrary points, warping the texture to look "correct"
		void DrawWarpedDecal(olc::Decal* decal, const olc::vf2d(&pos)[4], const olc::Pixel& tint = olc::WHITE);
		void DrawWarpedDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::Pixel& tint = olc::WHITE);
		void DrawWarpedDecal(olc::Decal* decal, const std::array<olc::vf2d, 4>& pos, const olc::Pixel& tint = olc::WHITE);
		// As above, but you can specify a region of a decal source sprite
		void DrawPartialWarpedDecal(olc::Decal* decal, const olc::vf2d(&pos)[4], const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint = olc::WHITE);
		void DrawPartialWarpedDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint = olc::WHITE);
		void DrawPartialWarpedDecal(olc::Decal* decal, const std::array<olc::vf2d, 4>& pos, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint = olc::WHITE);
		// Draws a decal rotated to specified angle, wit point of rotation offset
		void DrawRotatedDecal(const olc::vf2d& pos, olc::Decal* decal, const float fAngle, const olc::vf2d& center = { 0.0f, 0.0f }, const olc::vf2d& scale = { 1.0f,1.0f }, const olc::Pixel& tint = olc::WHITE);
		void DrawPartialRotatedDecal(const olc::vf2d& pos, olc::Decal* decal, const float fAngle, const olc::vf2d& center, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::vf2d& scale = { 1.0f, 1.0f }, const olc::Pixel& tint = olc::WHITE);
		// Draws a multiline string as a decal, with tiniting and scaling
		void DrawStringDecal(const olc::vf2d& pos, const std::string& sText, const Pixel col = olc::WHITE, const olc::vf2d& scale = { 1.0f, 1.0f });
		void DrawStringPropDecal(const olc::vf2d& pos, const std::string& sText, const Pixel col = olc::WHITE, const olc::vf2d& scale = { 1.0f, 1.0f });
		// Draws a single shaded filled rectangle as a decal
		void DrawRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel col = olc::WHITE);
		void FillRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel col = olc::WHITE);
		// Draws a corner shaded rectangle as a decal
		void GradientFillRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel colTL, const olc::Pixel colBL, const olc::Pixel colBR, const olc::Pixel colTR);		
		// Draws a single shaded filled triangle as a decal
		void FillTriangleDecal(const olc::vf2d& p0, const olc::vf2d& p1, const olc::vf2d& p2, const olc::Pixel col = olc::WHITE);
		// Draws a corner shaded triangle as a decal
		void GradientTriangleDecal(const olc::vf2d& p0, const olc::vf2d& p1, const olc::vf2d& p2, const olc::Pixel c0, const olc::Pixel c1, const olc::Pixel c2);
		// Draws an arbitrary convex textured polygon using GPU
		void DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const olc::Pixel tint = olc::WHITE);
		void DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<float>& depth, const std::vector<olc::vf2d>& uv, const olc::Pixel tint = olc::WHITE);
		void DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel>& tint);
		void DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel>& colours, const olc::Pixel tint);
		void DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<float>& depth, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel>& colours, const olc::Pixel tint);

		// Draws a line in Decal Space
		void DrawLineDecal(const olc::vf2d& pos1, const olc::vf2d& pos2, Pixel p = olc::WHITE);
		void DrawRotatedStringDecal(const olc::vf2d& pos, const std::string& sText, const float fAngle, const olc::vf2d& center = { 0.0f, 0.0f }, const olc::Pixel col = olc::WHITE, const olc::vf2d& scale = { 1.0f, 1.0f });
		void DrawRotatedStringPropDecal(const olc::vf2d& pos, const std::string& sText, const float fAngle, const olc::vf2d& center = { 0.0f, 0.0f }, const olc::Pixel col = olc::WHITE, const olc::vf2d& scale = { 1.0f, 1.0f });
		// Clears entire draw target to Pixel
		void Clear(Pixel p);
		// Clears the rendering back buffer
		void ClearBuffer(Pixel p, bool bDepth = true);
		// Returns the font image
		olc::Sprite* GetFontSprite();

		// Clip a line segment to visible area
		bool ClipLineToScreen(olc::vi2d& in_p1, olc::vi2d& in_p2);

		// Dont allow PGE to mark layers as dirty, so pixel graphics don't update
		void EnablePixelTransfer(const bool bEnable = true);

		// Command Console Routines
		void ConsoleShow(const olc::Key &keyExit, bool bSuspendTime = true);
		bool IsConsoleShowing() const;
		void ConsoleClear();
		std::stringstream& ConsoleOut();
		void ConsoleCaptureStdOut(const bool bCapture);

		// Text Entry Routines
		void TextEntryEnable(const bool bEnable, const std::string& sText = "");
		std::string TextEntryGetString() const;
		int32_t TextEntryGetCursor() const;
		bool IsTextEntryEnabled() const;



	private:
		void UpdateTextEntry();
		void UpdateConsole();

	public:

		// Experimental Lightweight 3D Routines ================
#ifdef OLC_ENABLE_EXPERIMENTAL
		// Set Manual View Matrix
		void LW3D_View(const std::array<float, 16>& m);
		// Set Manual World Matrix
		void LW3D_World(const std::array<float, 16>& m);
		// Set Manual Projection Matrix
		void LW3D_Projection(const std::array<float, 16>& m);
		
		// Draws a vector of vertices, interprted as individual triangles
		void LW3D_DrawTriangles(olc::Decal* decal, const std::vector<std::array<float,3>>& pos, const std::vector<olc::vf2d>& tex, const std::vector<olc::Pixel>& col);
		void LW3D_DrawWarpedDecal(olc::Decal* decal, const std::vector<std::array<float, 3>>& pos, const olc::Pixel& tint);

		void LW3D_ModelTranslate(const float x, const float y, const float z);
		
		// Camera convenience functions
		void LW3D_SetCameraAtTarget(const float fEyeX, const float fEyeY, const float fEyeZ,
			const float fTargetX, const float fTargetY, const float fTargetZ,
			const float fUpX = 0.0f, const float fUpY = 1.0f, const float fUpZ = 0.0f);
		void LW3D_SetCameraAlongDirection(const float fEyeX, const float fEyeY, const float fEyeZ,
			const float fDirX, const float fDirY, const float fDirZ,
			const float fUpX = 0.0f, const float fUpY = 1.0f, const float fUpZ = 0.0f);

		// 3D Rendering Flags
		void LW3D_EnableDepthTest(const bool bEnableDepth);
		void LW3D_EnableBackfaceCulling(const bool bEnableCull);
#endif
	public: // Branding
		std::string sAppName;

	private: // Inner mysterious workings
		olc::Sprite*     pDrawTarget = nullptr;
		Pixel::Mode	nPixelMode = Pixel::NORMAL;
		float		fBlendFactor = 1.0f;
		olc::vi2d	vScreenSize = { 256, 240 };
		olc::vf2d	vInvScreenSize = { 1.0f / 256.0f, 1.0f / 240.0f };
		olc::vi2d	vPixelSize = { 4, 4 };
		olc::vi2d   vScreenPixelSize = { 4, 4 };
		olc::vi2d	vMousePos = { 0, 0 };
		int32_t		nMouseWheelDelta = 0;
		olc::vi2d	vMousePosCache = { 0, 0 };
		olc::vi2d   vMouseWindowPos = { 0, 0 };
		int32_t		nMouseWheelDeltaCache = 0;
		olc::vi2d	vWindowPos = { 0, 0 };
		olc::vi2d	vWindowSize = { 0, 0 };
		olc::vi2d	vViewPos = { 0, 0 };
		olc::vi2d	vViewSize = { 0,0 };
		bool		bFullScreen = false;
		olc::vf2d	vPixel = { 1.0f, 1.0f };
		bool		bHasInputFocus = false;
		bool		bHasMouseFocus = false;
		bool		bEnableVSYNC = false;
		bool		bRealWindowMode = false;
		bool		bResizeRequested = false;
		olc::vi2d	vResizeRequested = { 0, 0 };
		float		fFrameTimer = 1.0f;
		float		fLastElapsed = 0.0f;
		int			nFrameCount = 0;		
		bool        bSuspendTextureTransfer = false;
		Renderable  fontRenderable;
		std::vector<LayerDesc> vLayers;
		uint8_t		nTargetLayer = 0;
		uint32_t	nLastFPS = 0;
		bool		bManualRenderEnable = false;
		bool        bPixelCohesion = false;
		DecalMode   nDecalMode = DecalMode::NORMAL;
		DecalStructure nDecalStructure = DecalStructure::FAN;
		std::function<olc::Pixel(const int x, const int y, const olc::Pixel&, const olc::Pixel&)> funcPixelMode;
		std::chrono::time_point<std::chrono::system_clock> m_tp1, m_tp2;
		std::vector<olc::vi2d> vFontSpacing;
		std::vector<std::string> vDroppedFiles;
		std::vector<std::string> vDroppedFilesCache;
		olc::vi2d vDroppedFilesPoint;
		olc::vi2d vDroppedFilesPointCache;

		// Command Console Specific
		bool bConsoleShow = false;
		bool bConsoleSuspendTime = false;
		olc::Key keyConsoleExit = olc::Key::F1;
		std::stringstream ssConsoleOutput;
		std::streambuf* sbufOldCout = nullptr;
		olc::vi2d vConsoleSize;
		olc::vi2d vConsoleCursor = { 0,0 };
		olc::vf2d vConsoleCharacterScale = { 1.0f, 2.0f };
		std::vector<std::string> sConsoleLines;
		std::list<std::string> sCommandHistory;
		std::list<std::string>::iterator sCommandHistoryIt;

		// Text Entry Specific
		bool bTextEntryEnable = false;
		std::string sTextEntryString = "";
		int32_t nTextEntryCursor = 0;
		std::vector<std::tuple<olc::Key, std::string, std::string>> vKeyboardMap;



		// State of keyboard		
		bool		pKeyNewState[256] = { 0 };
		bool		pKeyOldState[256] = { 0 };
		HWButton	pKeyboardState[256] = { 0 };

		// State of mouse
		bool		pMouseNewState[nMouseButtons] = { 0 };
		bool		pMouseOldState[nMouseButtons] = { 0 };
		HWButton	pMouseState[nMouseButtons] = { 0 };

		// The main engine thread
		void		EngineThread();


		// If anything sets this flag to false, the engine
		// "should" shut down gracefully
		static std::atomic<bool> bAtomActive;

	public:
		// "Break In" Functions
		void olc_UpdateMouse(int32_t x, int32_t y);
		void olc_UpdateMouseWheel(int32_t delta);
		void olc_UpdateWindowPos(int32_t x, int32_t y);
		void olc_UpdateWindowSize(int32_t x, int32_t y);
		void olc_UpdateViewport();
		void olc_ConstructFontSheet();
		void olc_CoreUpdate();
		void olc_PrepareEngine();
		void olc_UpdateMouseState(int32_t button, bool state);
		void olc_UpdateKeyState(int32_t key, bool state);
		void olc_UpdateMouseFocus(bool state);
		void olc_UpdateKeyFocus(bool state);
		void olc_Terminate();
		void olc_DropFiles(int32_t x, int32_t y, const std::vector<std::string>& vFiles);
		void olc_Reanimate();
		bool olc_IsRunning();

		// At the very end of this file, chooses which
		// components to compile
		virtual void olc_ConfigureSystem();

		// NOTE: Items Here are to be deprecated, I have left them in for now
		// in case you are using them, but they will be removed.
		// olc::vf2d	vSubPixelOffset = { 0.0f, 0.0f };

	public: // PGEX Stuff
		friend class PGEX;
		void pgex_Register(olc::PGEX* pgex);

	private:
		std::vector<olc::PGEX*> vExtensions;
	};



	// O------------------------------------------------------------------------------O
	// | PGE EXTENSION BASE CLASS - Permits access to PGE functions from extension    |
	// O------------------------------------------------------------------------------O
	class PGEX
	{
		friend class olc::PixelGameEngine;
	public:
		PGEX(bool bHook = false);

	protected:
		virtual void OnBeforeUserCreate();
		virtual void OnAfterUserCreate();
		virtual bool OnBeforeUserUpdate(float &fElapsedTime);
		virtual void OnAfterUserUpdate(float fElapsedTime);

	protected:
		static PixelGameEngine* pge;
	};
}
	
	PixelGameEngine::PixelGameEngine()
	{
		sAppName = "Undefined";
		olc::PGEX::pge = this;

		// Bring in relevant Platform & Rendering systems depending
		// on compiler parameters
		olc_ConfigureSystem();
	}

	olc::rcode PixelGameEngine::Construct(int32_t screen_w, int32_t screen_h, int32_t pixel_w, int32_t pixel_h, bool full_screen, bool vsync, bool cohesion, bool realwindow)
	{
		bPixelCohesion = cohesion;
		bRealWindowMode = realwindow;
		vScreenSize = { screen_w, screen_h };
		vInvScreenSize = { 1.0f / float(screen_w), 1.0f / float(screen_h) };
		vPixelSize = { pixel_w, pixel_h };
		vWindowSize = vScreenSize * vPixelSize;
		bFullScreen = full_screen;
		bEnableVSYNC = vsync;
		vPixel = 2.0f / vScreenSize;

		if (vPixelSize.x <= 0 || vPixelSize.y <= 0 || vScreenSize.x <= 0 || vScreenSize.y <= 0)
			return olc::FAIL;
		return olc::OK;
	}


	void PixelGameEngine::SetScreenSize(int w, int h)
	{
		vScreenSize = { w, h };
		vInvScreenSize = { 1.0f / float(w), 1.0f / float(h) };
		for (auto& layer : vLayers)
		{
			layer.pDrawTarget.Create(vScreenSize.x, vScreenSize.y);
			layer.bUpdate = true;
		}
		SetDrawTarget(nullptr);
		if (!bRealWindowMode)
		{
			// Flush backbuffer
			renderer->ClearBuffer(olc::BLACK, true);
			renderer->DisplayFrame();
			renderer->ClearBuffer(olc::BLACK, true);
		}
		renderer->UpdateViewport(vViewPos, vViewSize);
	}

#if !defined(PGE_USE_CUSTOM_START)
	olc::rcode PixelGameEngine::Start()
	{
		if (platform->ApplicationStartUp() != olc::OK) return olc::FAIL;

		// Construct the window
		if (platform->CreateWindowPane({ 30,30 }, vWindowSize, bFullScreen) != olc::OK) return olc::FAIL;
		olc_UpdateWindowSize(vWindowSize.x, vWindowSize.y);

		// Start the thread
		bAtomActive = true;
		std::thread t = std::thread(&PixelGameEngine::EngineThread, this);

		// Some implementations may form an event loop here
		platform->StartSystemEventLoop();

		// Wait for thread to be exited
		t.join();

		if (platform->ApplicationCleanUp() != olc::OK) return olc::FAIL;

		return olc::OK;
	}
#endif

	void PixelGameEngine::SetDrawTarget(Sprite* target)
	{
		if (target)
		{
			pDrawTarget = target;
		}
		else
		{
			nTargetLayer = 0;
			if(!vLayers.empty())
				pDrawTarget = vLayers[0].pDrawTarget.Sprite();
		}
	}

	void PixelGameEngine::SetDrawTarget(uint8_t layer, bool bDirty)
	{
		if (layer < vLayers.size())
		{
			pDrawTarget = vLayers[layer].pDrawTarget.Sprite();
			vLayers[layer].bUpdate = bDirty;
			nTargetLayer = layer;
		}
	}

	void PixelGameEngine::EnableLayer(uint8_t layer, bool b)
	{ if (layer < vLayers.size()) vLayers[layer].bShow = b; }

	void PixelGameEngine::SetLayerOffset(uint8_t layer, const olc::vf2d& offset)
	{ SetLayerOffset(layer, offset.x, offset.y); }

	void PixelGameEngine::SetLayerOffset(uint8_t layer, float x, float y)
	{ if (layer < vLayers.size()) vLayers[layer].vOffset = { x, y }; }

	void PixelGameEngine::SetLayerScale(uint8_t layer, const olc::vf2d& scale)
	{ SetLayerScale(layer, scale.x, scale.y); }

	void PixelGameEngine::SetLayerScale(uint8_t layer, float x, float y)
	{ if (layer < vLayers.size()) vLayers[layer].vScale = { x, y }; }

	void PixelGameEngine::SetLayerTint(uint8_t layer, const olc::Pixel& tint)
	{ if (layer < vLayers.size()) vLayers[layer].tint = tint; }

	void PixelGameEngine::SetLayerCustomRenderFunction(uint8_t layer, std::function<void()> f)
	{ if (layer < vLayers.size()) vLayers[layer].funcHook = f; }

	std::vector<LayerDesc>& PixelGameEngine::GetLayers()
	{ return vLayers; }

	uint32_t PixelGameEngine::CreateLayer()
	{
		LayerDesc ld;
		ld.pDrawTarget.Create(vScreenSize.x, vScreenSize.y);
		vLayers.push_back(std::move(ld));
		return uint32_t(vLayers.size()) - 1;
	}

	Sprite* PixelGameEngine::GetDrawTarget() const
	{ return pDrawTarget; }

	int32_t PixelGameEngine::GetDrawTargetWidth() const
	{
		if (pDrawTarget)
			return pDrawTarget->width;
		else
			return 0;
	}

	int32_t PixelGameEngine::GetDrawTargetHeight() const
	{
		if (pDrawTarget)
			return pDrawTarget->height;
		else
			return 0;
	}

	uint32_t PixelGameEngine::GetFPS() const
	{ return nLastFPS; }

	bool PixelGameEngine::IsFocused() const
	{ return bHasInputFocus; }

	HWButton PixelGameEngine::GetKey(Key k) const
	{ return pKeyboardState[k];	}

	HWButton PixelGameEngine::GetMouse(uint32_t b) const
	{ return pMouseState[b]; }

	int32_t PixelGameEngine::GetMouseX() const
	{ return vMousePos.x; }

	int32_t PixelGameEngine::GetMouseY() const
	{ return vMousePos.y; }

	const olc::vi2d& PixelGameEngine::GetMousePos() const
	{ return vMousePos; }

	int32_t PixelGameEngine::GetMouseWheel() const
	{ return nMouseWheelDelta; }

	int32_t PixelGameEngine::ScreenWidth() const
	{ return vScreenSize.x; }

	int32_t PixelGameEngine::ScreenHeight() const
	{ return vScreenSize.y; }

	float PixelGameEngine::GetElapsedTime() const
	{ return fLastElapsed; }

	const olc::vi2d& PixelGameEngine::GetWindowSize() const
	{ return vWindowSize; }

	const olc::vi2d& PixelGameEngine::GetWindowPos() const
	{ return vWindowPos; }

	const olc::vi2d& PixelGameEngine::GetPixelSize() const
	{ return vPixelSize; }

	const olc::vi2d& PixelGameEngine::GetScreenPixelSize() const
	{ return vScreenPixelSize; }

	const olc::vi2d& PixelGameEngine::GetScreenSize() const
	{ return vScreenSize;	}

	const olc::vi2d& PixelGameEngine::GetWindowMouse() const
	{ return vMouseWindowPos; }

	bool PixelGameEngine::Draw(const olc::vi2d& pos, Pixel p)
	{ return Draw(pos.x, pos.y, p); }

	// This is it, the critical function that plots a pixel
	bool PixelGameEngine::Draw(int32_t x, int32_t y, Pixel p)
	{
		if (!pDrawTarget) return false;

		if (nPixelMode == Pixel::NORMAL)
		{
			return pDrawTarget->SetPixel(x, y, p);
		}

		if (nPixelMode == Pixel::MASK)
		{
			if (p.a == 255)
				return pDrawTarget->SetPixel(x, y, p);
		}

		if (nPixelMode == Pixel::ALPHA)
		{
			Pixel d = pDrawTarget->GetPixel(x, y);
			float a = (float)(p.a / 255.0f) * fBlendFactor;
			float c = 1.0f - a;
			float r = a * (float)p.r + c * (float)d.r;
			float g = a * (float)p.g + c * (float)d.g;
			float b = a * (float)p.b + c * (float)d.b;
			return pDrawTarget->SetPixel(x, y, Pixel((uint8_t)r, (uint8_t)g, (uint8_t)b/*, (uint8_t)(p.a * fBlendFactor)*/));
		}

		if (nPixelMode == Pixel::CUSTOM)
		{
			return pDrawTarget->SetPixel(x, y, funcPixelMode(x, y, p, pDrawTarget->GetPixel(x, y)));
		}

		return false;
	}


	void PixelGameEngine::DrawLine(const olc::vi2d& pos1, const olc::vi2d& pos2, Pixel p, uint32_t pattern)
	{ DrawLine(pos1.x, pos1.y, pos2.x, pos2.y, p, pattern); }

	void PixelGameEngine::DrawLine(int32_t x1, int32_t y1, int32_t x2, int32_t y2, Pixel p, uint32_t pattern)
	{
		int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
		dx = x2 - x1; dy = y2 - y1;

		auto rol = [&](void) { pattern = (pattern << 1) | (pattern >> 31); return pattern & 1; };

		olc::vi2d p1(x1, y1), p2(x2, y2);
		if (!ClipLineToScreen(p1, p2))
			return;
		x1 = p1.x; y1 = p1.y;
		x2 = p2.x; y2 = p2.y;

		// straight lines idea by gurkanctn
		if (dx == 0) // Line is vertical
		{
			if (y2 < y1) std::swap(y1, y2);
			for (y = y1; y <= y2; y++) if (rol()) Draw(x1, y, p);
			return;
		}

		if (dy == 0) // Line is horizontal
		{
			if (x2 < x1) std::swap(x1, x2);
			for (x = x1; x <= x2; x++) if (rol()) Draw(x, y1, p);
			return;
		}

		// Line is Funk-aye
		dx1 = abs(dx); dy1 = abs(dy);
		px = 2 * dy1 - dx1;	py = 2 * dx1 - dy1;
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

			if (rol()) Draw(x, y, p);

			for (i = 0; x < xe; i++)
			{
				x = x + 1;
				if (px < 0)
					px = px + 2 * dy1;
				else
				{
					if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
					px = px + 2 * (dy1 - dx1);
				}
				if (rol()) Draw(x, y, p);
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

			if (rol()) Draw(x, y, p);

			for (i = 0; y < ye; i++)
			{
				y = y + 1;
				if (py <= 0)
					py = py + 2 * dx1;
				else
				{
					if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
					py = py + 2 * (dx1 - dy1);
				}
				if (rol()) Draw(x, y, p);
			}
		}
	}

	void PixelGameEngine::DrawCircle(const olc::vi2d& pos, int32_t radius, Pixel p, uint8_t mask)
	{ DrawCircle(pos.x, pos.y, radius, p, mask); }

	void PixelGameEngine::DrawCircle(int32_t x, int32_t y, int32_t radius, Pixel p, uint8_t mask)
	{ // Thanks to IanM-Matrix1 #PR121
		if (radius < 0 || x < -radius || y < -radius || x - GetDrawTargetWidth() > radius || y - GetDrawTargetHeight() > radius)
			return;

		if (radius > 0)
		{
			int x0 = 0;
			int y0 = radius;
			int d = 3 - 2 * radius;

			while (y0 >= x0) // only formulate 1/8 of circle
			{
				// Draw even octants
				if (mask & 0x01) Draw(x + x0, y - y0, p);// Q6 - upper right right
				if (mask & 0x04) Draw(x + y0, y + x0, p);// Q4 - lower lower right
				if (mask & 0x10) Draw(x - x0, y + y0, p);// Q2 - lower left left
				if (mask & 0x40) Draw(x - y0, y - x0, p);// Q0 - upper upper left
				if (x0 != 0 && x0 != y0)
				{
					if (mask & 0x02) Draw(x + y0, y - x0, p);// Q7 - upper upper right
					if (mask & 0x08) Draw(x + x0, y + y0, p);// Q5 - lower right right
					if (mask & 0x20) Draw(x - y0, y + x0, p);// Q3 - lower lower left
					if (mask & 0x80) Draw(x - x0, y - y0, p);// Q1 - upper left left
				}

				if (d < 0)
					d += 4 * x0++ + 6;
				else
					d += 4 * (x0++ - y0--) + 10;
			}
		}
		else
			Draw(x, y, p);
	}

	void PixelGameEngine::FillCircle(const olc::vi2d& pos, int32_t radius, Pixel p)
	{ FillCircle(pos.x, pos.y, radius, p); }

	void PixelGameEngine::FillCircle(int32_t x, int32_t y, int32_t radius, Pixel p)
	{ // Thanks to IanM-Matrix1 #PR121
		if (radius < 0 || x < -radius || y < -radius || x - GetDrawTargetWidth() > radius || y - GetDrawTargetHeight() > radius)
			return;

		if (radius > 0)
		{
			int x0 = 0;
			int y0 = radius;
			int d = 3 - 2 * radius;

			auto drawline = [&](int sx, int ex, int y)
			{
				for (int x = sx; x <= ex; x++)
					Draw(x, y, p);
			};

			while (y0 >= x0)
			{
				drawline(x - y0, x + y0, y - x0);
				if (x0 > 0)	drawline(x - y0, x + y0, y + x0);

				if (d < 0)
					d += 4 * x0++ + 6;
				else
				{
					if (x0 != y0)
					{
						drawline(x - x0, x + x0, y - y0);
						drawline(x - x0, x + x0, y + y0);
					}
					d += 4 * (x0++ - y0--) + 10;
				}
			}
		}
		else
			Draw(x, y, p);
	}

	void PixelGameEngine::DrawRect(const olc::vi2d& pos, const olc::vi2d& size, Pixel p)
	{ DrawRect(pos.x, pos.y, size.x, size.y, p); }

	void PixelGameEngine::DrawRect(int32_t x, int32_t y, int32_t w, int32_t h, Pixel p)
	{
		DrawLine(x, y, x + w, y, p);
		DrawLine(x + w, y, x + w, y + h, p);
		DrawLine(x + w, y + h, x, y + h, p);
		DrawLine(x, y + h, x, y, p);
	}

	void PixelGameEngine::Clear(Pixel p)
	{
		int pixels = GetDrawTargetWidth() * GetDrawTargetHeight();
		Pixel* m = GetDrawTarget()->GetData();
		for (int i = 0; i < pixels; i++) m[i] = p;
	}

	void PixelGameEngine::ClearBuffer(Pixel p, bool bDepth)
	{ renderer->ClearBuffer(p, bDepth);	}

	olc::Sprite* PixelGameEngine::GetFontSprite()
	{ return fontRenderable.Sprite(); }

	bool PixelGameEngine::ClipLineToScreen(olc::vi2d& in_p1, olc::vi2d& in_p2)
	{
		// https://en.wikipedia.org/wiki/Cohen%E2%80%93Sutherland_algorithm
		static constexpr int SEG_I = 0b0000, SEG_L = 0b0001, SEG_R = 0b0010, SEG_B = 0b0100, SEG_T = 0b1000;
		auto Segment = [&vScreenSize = vScreenSize](const olc::vi2d& v)
		{
			int i = SEG_I;
			if (v.x < 0) i |= SEG_L; else if (v.x > vScreenSize.x) i |= SEG_R;
			if (v.y < 0) i |= SEG_B; else if (v.y > vScreenSize.y) i |= SEG_T;
			return i;
		};

		int s1 = Segment(in_p1), s2 = Segment(in_p2);

		while (true)
		{
			if (!(s1 | s2))	  return true;
			else if (s1 & s2) return false;
			else
			{
				int s3 = s2 > s1 ? s2 : s1;
				olc::vi2d n;
				if (s3 & SEG_T) { n.x = in_p1.x + (in_p2.x - in_p1.x) * (vScreenSize.y - in_p1.y) / (in_p2.y - in_p1.y); n.y = vScreenSize.y; }
				else if (s3 & SEG_B) { n.x = in_p1.x + (in_p2.x - in_p1.x) * (0 - in_p1.y) / (in_p2.y - in_p1.y); n.y = 0; }
				else if (s3 & SEG_R) { n.x = vScreenSize.x; n.y = in_p1.y + (in_p2.y - in_p1.y) * (vScreenSize.x - in_p1.x) / (in_p2.x - in_p1.x); }
				else if (s3 & SEG_L) { n.x = 0; n.y = in_p1.y + (in_p2.y - in_p1.y) * (0 - in_p1.x) / (in_p2.x - in_p1.x); }
				if (s3 == s1) { in_p1 = n; s1 = Segment(in_p1); }
				else { in_p2 = n; s2 = Segment(in_p2); }
			}
		}
		return true;
	}

	void PixelGameEngine::EnablePixelTransfer(const bool bEnable)
	{
		bSuspendTextureTransfer = !bEnable;
	}


	void PixelGameEngine::FillRect(const olc::vi2d& pos, const olc::vi2d& size, Pixel p)
	{ FillRect(pos.x, pos.y, size.x, size.y, p); }

	void PixelGameEngine::FillRect(int32_t x, int32_t y, int32_t w, int32_t h, Pixel p)
	{
		int32_t x2 = x + w;
		int32_t y2 = y + h;

		if (x < 0) x = 0;
		if (x >= (int32_t)GetDrawTargetWidth()) x = (int32_t)GetDrawTargetWidth();
		if (y < 0) y = 0;
		if (y >= (int32_t)GetDrawTargetHeight()) y = (int32_t)GetDrawTargetHeight();

		if (x2 < 0) x2 = 0;
		if (x2 >= (int32_t)GetDrawTargetWidth()) x2 = (int32_t)GetDrawTargetWidth();
		if (y2 < 0) y2 = 0;
		if (y2 >= (int32_t)GetDrawTargetHeight()) y2 = (int32_t)GetDrawTargetHeight();

		for (int i = x; i < x2; i++)
			for (int j = y; j < y2; j++)
				Draw(i, j, p);
	}

	void PixelGameEngine::DrawTriangle(const olc::vi2d& pos1, const olc::vi2d& pos2, const olc::vi2d& pos3, Pixel p)
	{ DrawTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p); }

	void PixelGameEngine::DrawTriangle(int32_t x1, int32_t y1, int32_t x2, int32_t y2, int32_t x3, int32_t y3, Pixel p)
	{
		DrawLine(x1, y1, x2, y2, p);
		DrawLine(x2, y2, x3, y3, p);
		DrawLine(x3, y3, x1, y1, p);
	}

	void PixelGameEngine::FillTriangle(const olc::vi2d& pos1, const olc::vi2d& pos2, const olc::vi2d& pos3, Pixel p)
	{ FillTriangle(pos1.x, pos1.y, pos2.x, pos2.y, pos3.x, pos3.y, p); }

	// https://www.avrfreaks.net/sites/default/files/triangles.c
	void PixelGameEngine::FillTriangle(int32_t x1, int32_t y1, int32_t x2, int32_t y2, int32_t x3, int32_t y3, Pixel p)
	{
		auto drawline = [&](int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) Draw(i, ny, p); };

		int t1x, t2x, y, minx, maxx, t1xp, t2xp;
		bool changed1 = false;
		bool changed2 = false;
		int signx1, signx2, dx1, dy1, dx2, dy2;
		int e1, e2;
		// Sort vertices
		if (y1 > y2) { std::swap(y1, y2); std::swap(x1, x2); }
		if (y1 > y3) { std::swap(y1, y3); std::swap(x1, x3); }
		if (y2 > y3) { std::swap(y2, y3); std::swap(x2, x3); }

		t1x = t2x = x1; y = y1;   // Starting points
		dx1 = (int)(x2 - x1);
		if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
		else signx1 = 1;
		dy1 = (int)(y2 - y1);

		dx2 = (int)(x3 - x1);
		if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
		else signx2 = 1;
		dy2 = (int)(y3 - y1);

		if (dy1 > dx1) { std::swap(dx1, dy1); changed1 = true; }
		if (dy2 > dx2) { std::swap(dy2, dx2); changed2 = true; }

		e2 = (int)(dx2 >> 1);
		// Flat top, just process the second half
		if (y1 == y2) goto next;
		e1 = (int)(dx1 >> 1);

		for (int i = 0; i < dx1;) {
			t1xp = 0; t2xp = 0;
			if (t1x < t2x) { minx = t1x; maxx = t2x; }
			else { minx = t2x; maxx = t1x; }
			// process first line until y value is about to change
			while (i < dx1) {
				i++;
				e1 += dy1;
				while (e1 >= dx1) {
					e1 -= dx1;
					if (changed1) t1xp = signx1;//t1x += signx1;
					else          goto next1;
				}
				if (changed1) break;
				else t1x += signx1;
			}
			// Move line
		next1:
			// process second line until y value is about to change
			while (1) {
				e2 += dy2;
				while (e2 >= dx2) {
					e2 -= dx2;
					if (changed2) t2xp = signx2;//t2x += signx2;
					else          goto next2;
				}
				if (changed2)     break;
				else              t2x += signx2;
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
	next:
		// Second half
		dx1 = (int)(x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
		else signx1 = 1;
		dy1 = (int)(y3 - y2);
		t1x = x2;

		if (dy1 > dx1) {   // swap values
			std::swap(dy1, dx1);
			changed1 = true;
		}
		else changed1 = false;

		e1 = (int)(dx1 >> 1);

		for (int i = 0; i <= dx1; i++) {
			t1xp = 0; t2xp = 0;
			if (t1x < t2x) { minx = t1x; maxx = t2x; }
			else { minx = t2x; maxx = t1x; }
			// process first line until y value is about to change
			while (i < dx1) {
				e1 += dy1;
				while (e1 >= dx1) {
					e1 -= dx1;
					if (changed1) { t1xp = signx1; break; }//t1x += signx1;
					else          goto next3;
				}
				if (changed1) break;
				else   	   	  t1x += signx1;
				if (i < dx1) i++;
			}
		next3:
			// process second line until y value is about to change
			while (t2x != x3) {
				e2 += dy2;
				while (e2 >= dx2) {
					e2 -= dx2;
					if (changed2) t2xp = signx2;
					else          goto next4;
				}
				if (changed2)     break;
				else              t2x += signx2;
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

	void PixelGameEngine::FillTexturedTriangle(std::vector<olc::vf2d> vPoints, std::vector<olc::vf2d> vTex, std::vector<olc::Pixel> vColour, olc::Sprite* sprTex)
	{
		olc::vi2d p1 = vPoints[0];
		olc::vi2d p2 = vPoints[1];
		olc::vi2d p3 = vPoints[2];

		if (p2.y < p1.y){std::swap(p1.y, p2.y); std::swap(p1.x, p2.x); std::swap(vTex[0].x, vTex[1].x); std::swap(vTex[0].y, vTex[1].y); std::swap(vColour[0], vColour[1]);}
		if (p3.y < p1.y){std::swap(p1.y, p3.y); std::swap(p1.x, p3.x); std::swap(vTex[0].x, vTex[2].x); std::swap(vTex[0].y, vTex[2].y); std::swap(vColour[0], vColour[2]);}
		if (p3.y < p2.y){std::swap(p2.y, p3.y); std::swap(p2.x, p3.x); std::swap(vTex[1].x, vTex[2].x); std::swap(vTex[1].y, vTex[2].y); std::swap(vColour[1], vColour[2]);}

		olc::vi2d dPos1 = p2 - p1;
		olc::vf2d dTex1 = vTex[1] - vTex[0];
		int dcr1 = vColour[1].r - vColour[0].r;
		int dcg1 = vColour[1].g - vColour[0].g;
		int dcb1 = vColour[1].b - vColour[0].b;
		int dca1 = vColour[1].a - vColour[0].a;

		olc::vi2d dPos2 = p3 - p1;
		olc::vf2d dTex2 = vTex[2] - vTex[0];
		int dcr2 = vColour[2].r - vColour[0].r;
		int dcg2 = vColour[2].g - vColour[0].g;
		int dcb2 = vColour[2].b - vColour[0].b;
		int dca2 = vColour[2].a - vColour[0].a;

		float dax_step = 0, dbx_step = 0, dcr1_step = 0, dcr2_step = 0,	dcg1_step = 0, dcg2_step = 0, dcb1_step = 0, dcb2_step = 0,	dca1_step = 0, dca2_step = 0;
		olc::vf2d vTex1Step, vTex2Step;

		if (dPos1.y)
		{
			dax_step = dPos1.x / (float)abs(dPos1.y);
			vTex1Step = dTex1 / (float)abs(dPos1.y);
			dcr1_step = dcr1 / (float)abs(dPos1.y);
			dcg1_step = dcg1 / (float)abs(dPos1.y);
			dcb1_step = dcb1 / (float)abs(dPos1.y);
			dca1_step = dca1 / (float)abs(dPos1.y);
		}

		if (dPos2.y)
		{
			dbx_step = dPos2.x / (float)abs(dPos2.y);
			vTex2Step = dTex2 / (float)abs(dPos2.y);
			dcr2_step = dcr2 / (float)abs(dPos2.y);
			dcg2_step = dcg2 / (float)abs(dPos2.y);
			dcb2_step = dcb2 / (float)abs(dPos2.y);
			dca2_step = dca2 / (float)abs(dPos2.y);
		}

		olc::vi2d vStart;
		olc::vi2d vEnd;
		int vStartIdx;

		for (int pass = 0; pass < 2; pass++)
		{
			if (pass == 0)
			{
				vStart = p1; vEnd = p2;	vStartIdx = 0;
			}
			else
			{
				dPos1 = p3 - p2;
				dTex1 = vTex[2] - vTex[1];
				dcr1 = vColour[2].r - vColour[1].r;
				dcg1 = vColour[2].g - vColour[1].g;
				dcb1 = vColour[2].b - vColour[1].b;
				dca1 = vColour[2].a - vColour[1].a;
				dcr1_step = 0; dcg1_step = 0; dcb1_step = 0; dca1_step = 0;

				if (dPos2.y) dbx_step = dPos2.x / (float)abs(dPos2.y);
				if (dPos1.y)
				{
					dax_step = dPos1.x / (float)abs(dPos1.y);
					vTex1Step = dTex1 / (float)abs(dPos1.y);
					dcr1_step = dcr1 / (float)abs(dPos1.y);
					dcg1_step = dcg1 / (float)abs(dPos1.y);
					dcb1_step = dcb1 / (float)abs(dPos1.y);
					dca1_step = dca1 / (float)abs(dPos1.y);
				}

				vStart = p2; vEnd = p3; vStartIdx = 1;
			}

			if (dPos1.y)
			{
				for (int i = vStart.y; i <= vEnd.y; i++)
				{
					int ax = int(vStart.x + (float)(i - vStart.y) * dax_step);
					int bx = int(p1.x + (float)(i - p1.y) * dbx_step);

					olc::vf2d tex_s(vTex[vStartIdx].x + (float)(i - vStart.y) * vTex1Step.x, vTex[vStartIdx].y + (float)(i - vStart.y) * vTex1Step.y);
					olc::vf2d tex_e(vTex[0].x + (float)(i - p1.y) * vTex2Step.x, vTex[0].y + (float)(i - p1.y) * vTex2Step.y);

					olc::Pixel col_s(vColour[vStartIdx].r + uint8_t((float)(i - vStart.y) * dcr1_step), vColour[vStartIdx].g + uint8_t((float)(i - vStart.y) * dcg1_step),
						vColour[vStartIdx].b + uint8_t((float)(i - vStart.y) * dcb1_step), vColour[vStartIdx].a + uint8_t((float)(i - vStart.y) * dca1_step));

					olc::Pixel col_e(vColour[0].r + uint8_t((float)(i - p1.y) * dcr2_step), vColour[0].g + uint8_t((float)(i - p1.y) * dcg2_step),
						vColour[0].b + uint8_t((float)(i - p1.y) * dcb2_step), vColour[0].a + uint8_t((float)(i - p1.y) * dca2_step));

					if (ax > bx) { std::swap(ax, bx); std::swap(tex_s, tex_e); std::swap(col_s, col_e); }

					float tstep = 1.0f / ((float)(bx - ax));
					float t = 0.0f;

					for (int j = ax; j < bx; j++)
					{
						olc::Pixel pixel = PixelLerp(col_s, col_e, t);
						if (sprTex != nullptr) pixel *= sprTex->Sample(tex_s.lerp(tex_e, t));
						Draw(j, i, pixel);
						t += tstep;
					}
				}
			}
		}			
	}

	void PixelGameEngine::FillTexturedPolygon(const std::vector<olc::vf2d>& vPoints, const std::vector<olc::vf2d>& vTex, const std::vector<olc::Pixel>& vColour, olc::Sprite* sprTex, olc::DecalStructure structure)
	{
		if (structure == olc::DecalStructure::LINE)
		{
			return; // Meaningless, so do nothing
		}

		if (vPoints.size() < 3 || vTex.size() < 3 || vColour.size() < 3)
			return;

		if (structure == olc::DecalStructure::LIST)
		{			
			for (size_t tri = 0; tri < vPoints.size() / 3; tri++)
			{
				std::vector<olc::vf2d> vP = { vPoints[tri * 3 + 0], vPoints[tri * 3 + 1], vPoints[tri * 3 + 2] };
				std::vector<olc::vf2d> vT = { vTex[tri * 3 + 0], vTex[tri * 3 + 1], vTex[tri * 3 + 2] };
				std::vector<olc::Pixel> vC = { vColour[tri * 3 + 0], vColour[tri * 3 + 1], vColour[tri * 3 + 2] };
				FillTexturedTriangle(vP, vT, vC, sprTex);
			}
			return;
		}

		if (structure == olc::DecalStructure::STRIP)
		{
			for (size_t tri = 2; tri < vPoints.size(); tri++)
			{
				std::vector<olc::vf2d> vP = { vPoints[tri - 2], vPoints[tri-1], vPoints[tri] };
				std::vector<olc::vf2d> vT = { vTex[tri - 2], vTex[tri - 1], vTex[tri] };
				std::vector<olc::Pixel> vC = { vColour[tri - 2], vColour[tri - 1], vColour[tri] };
				FillTexturedTriangle(vP, vT, vC, sprTex);
			}
			return;
		}

		if (structure == olc::DecalStructure::FAN)
		{
			for (size_t tri = 2; tri < vPoints.size(); tri++)
			{
				std::vector<olc::vf2d> vP = { vPoints[0], vPoints[tri - 1], vPoints[tri] };
				std::vector<olc::vf2d> vT = { vTex[0], vTex[tri - 1], vTex[tri] };
				std::vector<olc::Pixel> vC = { vColour[0], vColour[tri - 1], vColour[tri] };
				FillTexturedTriangle(vP, vT, vC, sprTex);
			}
			return;
		}
	}


	void PixelGameEngine::DrawSprite(const olc::vi2d& pos, Sprite* sprite, uint32_t scale, uint8_t flip)
	{ DrawSprite(pos.x, pos.y, sprite, scale, flip); }

	void PixelGameEngine::DrawSprite(int32_t x, int32_t y, Sprite* sprite, uint32_t scale, uint8_t flip)
	{
		if (sprite == nullptr)
			return;

		int32_t fxs = 0, fxm = 1, fx = 0;
		int32_t fys = 0, fym = 1, fy = 0;
		if (flip & olc::Sprite::Flip::HORIZ) { fxs = sprite->width - 1; fxm = -1; }
		if (flip & olc::Sprite::Flip::VERT) { fys = sprite->height - 1; fym = -1; }

		if (scale > 1)
		{
			fx = fxs;
			for (int32_t i = 0; i < sprite->width; i++, fx += fxm)
			{
				fy = fys;
				for (int32_t j = 0; j < sprite->height; j++, fy += fym)
					for (uint32_t is = 0; is < scale; is++)
						for (uint32_t js = 0; js < scale; js++)
							Draw(x + (i * scale) + is, y + (j * scale) + js, sprite->GetPixel(fx, fy));
			}
		}
		else
		{
			fx = fxs;
			for (int32_t i = 0; i < sprite->width; i++, fx += fxm)
			{
				fy = fys;
				for (int32_t j = 0; j < sprite->height; j++, fy += fym)
					Draw(x + i, y + j, sprite->GetPixel(fx, fy));
			}
		}
	}

	void PixelGameEngine::DrawPartialSprite(const olc::vi2d& pos, Sprite* sprite, const olc::vi2d& sourcepos, const olc::vi2d& size, uint32_t scale, uint8_t flip)
	{ DrawPartialSprite(pos.x, pos.y, sprite, sourcepos.x, sourcepos.y, size.x, size.y, scale, flip); }

	void PixelGameEngine::DrawPartialSprite(int32_t x, int32_t y, Sprite* sprite, int32_t ox, int32_t oy, int32_t w, int32_t h, uint32_t scale, uint8_t flip)
	{
		if (sprite == nullptr)
			return;

		int32_t fxs = 0, fxm = 1, fx = 0;
		int32_t fys = 0, fym = 1, fy = 0;
		if (flip & olc::Sprite::Flip::HORIZ) { fxs = w - 1; fxm = -1; }
		if (flip & olc::Sprite::Flip::VERT) { fys = h - 1; fym = -1; }

		if (scale > 1)
		{
			fx = fxs;
			for (int32_t i = 0; i < w; i++, fx += fxm)
			{
				fy = fys;
				for (int32_t j = 0; j < h; j++, fy += fym)
					for (uint32_t is = 0; is < scale; is++)
						for (uint32_t js = 0; js < scale; js++)
							Draw(x + (i * scale) + is, y + (j * scale) + js, sprite->GetPixel(fx + ox, fy + oy));
			}
		}
		else
		{
			fx = fxs;
			for (int32_t i = 0; i < w; i++, fx += fxm)
			{
				fy = fys;
				for (int32_t j = 0; j < h; j++, fy += fym)
					Draw(x + i, y + j, sprite->GetPixel(fx + ox, fy + oy));
			}
		}
	}

	void PixelGameEngine::SetDecalMode(const olc::DecalMode& mode)
	{ nDecalMode = mode; }

	void PixelGameEngine::SetDecalStructure(const olc::DecalStructure& structure)
	{ nDecalStructure = structure; }

	void PixelGameEngine::DrawPartialDecal(const olc::vf2d& pos, olc::Decal* decal, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::vf2d& scale, const olc::Pixel& tint)
	{
		olc::vf2d vScreenSpacePos =
		{
			  (pos.x * vInvScreenSize.x) * 2.0f - 1.0f,
			-((pos.y * vInvScreenSize.y) * 2.0f - 1.0f)
		};

		
		olc::vf2d vScreenSpaceDim =
		{
			  ((pos.x + source_size.x * scale.x) * vInvScreenSize.x) * 2.0f - 1.0f,
			-(((pos.y + source_size.y * scale.y) * vInvScreenSize.y) * 2.0f - 1.0f)
		};

		olc::vf2d vWindow = olc::vf2d(vViewSize);
		olc::vf2d vQuantisedPos = ((vScreenSpacePos * vWindow) + olc::vf2d(0.5f, 0.5f)).floor() / vWindow;
		olc::vf2d vQuantisedDim = ((vScreenSpaceDim * vWindow) + olc::vf2d(0.5f, -0.5f)).ceil() / vWindow;

		DecalInstance di;
		di.points = 4;
		di.decal = decal;
		di.tint = { tint, tint, tint, tint };
		di.pos = { { vQuantisedPos.x, vQuantisedPos.y }, { vQuantisedPos.x, vQuantisedDim.y }, { vQuantisedDim.x, vQuantisedDim.y }, { vQuantisedDim.x, vQuantisedPos.y } };
		olc::vf2d uvtl = (source_pos + olc::vf2d(0.0001f, 0.0001f)) * decal->vUVScale;
		olc::vf2d uvbr = (source_pos + source_size - olc::vf2d(0.0001f, 0.0001f)) * decal->vUVScale;
		di.uv = { { uvtl.x, uvtl.y }, { uvtl.x, uvbr.y }, { uvbr.x, uvbr.y }, { uvbr.x, uvtl.y } };
		di.w = { 1,1,1,1 };
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPartialDecal(const olc::vf2d& pos, const olc::vf2d& size, olc::Decal* decal, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint)
	{
		olc::vf2d vScreenSpacePos =
		{
			(pos.x * vInvScreenSize.x) * 2.0f - 1.0f,
			((pos.y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f
		};

		olc::vf2d vScreenSpaceDim =
		{
			vScreenSpacePos.x + (2.0f * size.x * vInvScreenSize.x),
			vScreenSpacePos.y - (2.0f * size.y * vInvScreenSize.y)
		};

		DecalInstance di;
		di.points = 4;
		di.decal = decal;
		di.tint = { tint, tint, tint, tint };
		di.pos = { { vScreenSpacePos.x, vScreenSpacePos.y }, { vScreenSpacePos.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpacePos.y } };
		olc::vf2d uvtl = (source_pos) * decal->vUVScale;
		olc::vf2d uvbr = uvtl + ((source_size) * decal->vUVScale);
		di.uv = { { uvtl.x, uvtl.y }, { uvtl.x, uvbr.y }, { uvbr.x, uvbr.y }, { uvbr.x, uvtl.y } };
		di.w = { 1,1,1,1 };
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}


	void PixelGameEngine::DrawDecal(const olc::vf2d& pos, olc::Decal* decal, const olc::vf2d& scale, const olc::Pixel& tint)
	{
		olc::vf2d vScreenSpacePos =
		{
			(pos.x * vInvScreenSize.x) * 2.0f - 1.0f,
			((pos.y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f
		};

		olc::vf2d vScreenSpaceDim =
		{
			vScreenSpacePos.x + (2.0f * (float(decal->sprite->width) * vInvScreenSize.x)) * scale.x,
			vScreenSpacePos.y - (2.0f * (float(decal->sprite->height) * vInvScreenSize.y)) * scale.y
		};

		DecalInstance di;
		di.decal = decal;
		di.points = 4;
		di.tint = { tint, tint, tint, tint };
		di.pos = { { vScreenSpacePos.x, vScreenSpacePos.y }, { vScreenSpacePos.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpacePos.y } };
		di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
		di.w = { 1, 1, 1, 1 };
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawExplicitDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::vf2d* uv, const olc::Pixel* col, uint32_t elements)
	{
		DecalInstance di;
		di.decal = decal;
		di.pos.resize(elements);
		di.uv.resize(elements);
		di.w.resize(elements);
		di.tint.resize(elements);
		di.points = elements;
		for (uint32_t i = 0; i < elements; i++)
		{
			di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			di.uv[i] = uv[i];
			di.tint[i] = col[i];
			di.w[i] = 1.0f;
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const olc::Pixel tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = uint32_t(pos.size());
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.tint.resize(di.points);
		for (uint32_t i = 0; i < di.points; i++)
		{
			di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			di.uv[i] = uv[i];
			di.tint[i] = tint;
			di.w[i] = 1.0f;
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel> &tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = uint32_t(pos.size());
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.tint.resize(di.points);
		for (uint32_t i = 0; i < di.points; i++)
		{
			di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			di.uv[i] = uv[i];
			di.tint[i] = tint[i];
			di.w[i] = 1.0f;
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel>& colours, const olc::Pixel tint)
	{
		std::vector<olc::Pixel> newColours(colours.size(), olc::WHITE);
		std::transform(colours.begin(), colours.end(), newColours.begin(),
			[&tint](const olc::Pixel pin) {	return pin * tint; });
		DrawPolygonDecal(decal, pos, uv, newColours);
	}


	void PixelGameEngine::DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<float>& depth, const std::vector<olc::vf2d>& uv, const olc::Pixel tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = uint32_t(pos.size());
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.tint.resize(di.points);
		for (uint32_t i = 0; i < di.points; i++)
		{
			di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			di.uv[i] = uv[i];
			di.tint[i] = tint;
			di.w[i] = depth[i];
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPolygonDecal(olc::Decal* decal, const std::vector<olc::vf2d>& pos, const std::vector<float>& depth, const std::vector<olc::vf2d>& uv, const std::vector<olc::Pixel>& colours, const olc::Pixel tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = uint32_t(pos.size());
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.tint.resize(di.points);
		for (uint32_t i = 0; i < di.points; i++)
		{
			di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			di.uv[i] = uv[i];
			di.tint[i] = colours[i] * tint;
			di.w[i] = depth[i];
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

#ifdef OLC_ENABLE_EXPERIMENTAL
	// Lightweight 3D
	void PixelGameEngine::LW3D_DrawTriangles(olc::Decal* decal, const std::vector<std::array<float, 3>>& pos, const std::vector<olc::vf2d>& tex, const std::vector<olc::Pixel>& col)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = uint32_t(pos.size());
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.z.resize(di.points);
		di.tint.resize(di.points);
		for (uint32_t i = 0; i < di.points; i++)
		{
			di.pos[i] = { pos[i][0], pos[i][1] };
			di.w[i] = pos[i][2];
			di.z[i] = pos[i][2];
			di.uv[i] = tex[i];
			di.tint[i] = col[i];			
		}
		di.mode = nDecalMode;
		di.structure = DecalStructure::LIST;
		di.depth = true;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::LW3D_DrawWarpedDecal(olc::Decal* decal, const std::vector<std::array<float, 3>>& pos, const olc::Pixel& tint)
	{
		// Thanks Nathan Reed, a brilliant article explaining whats going on here
		// http://www.reedbeta.com/blog/quadrilateral-interpolation-part-1/
		DecalInstance di;
		di.points = 4;
		di.decal = decal;
		di.tint = { tint, tint, tint, tint };
		di.w = { 1, 1, 1, 1 };
		di.z = { 1, 1, 1, 1 };
		di.pos.resize(4);
		di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
		olc::vf2d center;
		float rd = ((pos[2][0] - pos[0][0]) * (pos[3][1] - pos[1][1]) - (pos[3][0] - pos[1][0]) * (pos[2][1] - pos[0][1]));
		if (rd != 0)
		{
			rd = 1.0f / rd;
			float rn = ((pos[3][0] - pos[1][0]) * (pos[0][1] - pos[1][1]) - (pos[3][1] - pos[1][1]) * (pos[0][0] - pos[1][0])) * rd;
			float sn = ((pos[2][0] - pos[0][0]) * (pos[0][1] - pos[1][1]) - (pos[2][1] - pos[0][1]) * (pos[0][0] - pos[1][0])) * rd;
			if (!(rn < 0.f || rn > 1.f || sn < 0.f || sn > 1.f))
			{
				center.x = pos[0][0] + rn * (pos[2][0] - pos[0][0]);
				center.y = pos[0][1] + rn * (pos[2][1] - pos[0][1]);
			}
			float d[4];
			for (int i = 0; i < 4; i++)
				d[i] = std::sqrt((pos[i][0] - center.x) * (pos[i][0] - center.x) + (pos[i][1] - center.y) * (pos[i][1] - center.y));

			for (int i = 0; i < 4; i++)
			{
				float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
				di.uv[i] *= q; 
				di.w[i] *= q;
				di.z[i] = pos[i][2];
				di.pos[i] = { (pos[i][0] * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i][1] * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			}
			di.mode = nDecalMode;
			di.structure = nDecalStructure;
			di.depth = true;
			vLayers[nTargetLayer].vecDecalInstance.push_back(di);
		}
	}
#endif

	void PixelGameEngine::DrawLineDecal(const olc::vf2d& pos1, const olc::vf2d& pos2, Pixel p)
	{
		auto m = nDecalMode;
		nDecalMode = olc::DecalMode::WIREFRAME;
		DrawPolygonDecal(nullptr, { pos1, pos2 }, { {0, 0}, {0,0} }, p);
		nDecalMode = m;

		/*DecalInstance di;
		di.decal = nullptr;
		di.points = uint32_t(2);
		di.pos.resize(di.points);
		di.uv.resize(di.points);
		di.w.resize(di.points);
		di.tint.resize(di.points);
		di.pos[0] = { (pos1.x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos1.y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
		di.uv[0] = { 0.0f, 0.0f };
		di.tint[0] = p;
		di.w[0] = 1.0f;
		di.pos[1] = { (pos2.x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos2.y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
		di.uv[1] = { 0.0f, 0.0f };
		di.tint[1] = p;
		di.w[1] = 1.0f;
		di.mode = olc::DecalMode::WIREFRAME;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);*/
	}

	void PixelGameEngine::DrawRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel col)
	{
		auto m = nDecalMode;
		SetDecalMode(olc::DecalMode::WIREFRAME);
		olc::vf2d vNewSize = size;// (size - olc::vf2d(0.375f, 0.375f)).ceil();
		std::array<olc::vf2d, 4> points = { { {pos}, {pos.x, pos.y + vNewSize.y}, {pos + vNewSize}, {pos.x + vNewSize.x, pos.y} } };
		std::array<olc::vf2d, 4> uvs = { {{0,0},{0,0},{0,0},{0,0}} };
		std::array<olc::Pixel, 4> cols = { {col, col, col, col} };
		DrawExplicitDecal(nullptr, points.data(), uvs.data(), cols.data(), 4);
		SetDecalMode(m);

	}

	void PixelGameEngine::FillRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel col)
	{
		olc::vf2d vNewSize = size;// (size - olc::vf2d(0.375f, 0.375f)).ceil();
		std::array<olc::vf2d, 4> points = { { {pos}, {pos.x, pos.y + vNewSize.y}, {pos + vNewSize}, {pos.x + vNewSize.x, pos.y} } };
		std::array<olc::vf2d, 4> uvs = { {{0,0},{0,0},{0,0},{0,0}} };
		std::array<olc::Pixel, 4> cols = { {col, col, col, col} };
		DrawExplicitDecal(nullptr, points.data(), uvs.data(), cols.data(), 4);
	}

	void PixelGameEngine::GradientFillRectDecal(const olc::vf2d& pos, const olc::vf2d& size, const olc::Pixel colTL, const olc::Pixel colBL, const olc::Pixel colBR, const olc::Pixel colTR)
	{
		std::array<olc::vf2d, 4> points = { { {pos}, {pos.x, pos.y + size.y}, {pos + size}, {pos.x + size.x, pos.y} } };
		std::array<olc::vf2d, 4> uvs = { {{0,0},{0,0},{0,0},{0,0}} };
		std::array<olc::Pixel, 4> cols = { {colTL, colBL, colBR, colTR} };
		DrawExplicitDecal(nullptr, points.data(), uvs.data(), cols.data(), 4);
	}

	void PixelGameEngine::FillTriangleDecal(const olc::vf2d& p0, const olc::vf2d& p1, const olc::vf2d& p2, const olc::Pixel col)
	{		
		std::array<olc::vf2d, 4> points = { { p0, p1, p2 } };
		std::array<olc::vf2d, 4> uvs = { {{0,0},{0,0},{0,0}} };
		std::array<olc::Pixel, 4> cols = { {col, col, col} };
		DrawExplicitDecal(nullptr, points.data(), uvs.data(), cols.data(), 3);
	}

	void PixelGameEngine::GradientTriangleDecal(const olc::vf2d& p0, const olc::vf2d& p1, const olc::vf2d& p2, const olc::Pixel c0, const olc::Pixel c1, const olc::Pixel c2)
	{
		std::array<olc::vf2d, 4> points = { { p0, p1, p2 } };
		std::array<olc::vf2d, 4> uvs = { {{0,0},{0,0},{0,0}} };
		std::array<olc::Pixel, 4> cols = { {c0, c1, c2} };
		DrawExplicitDecal(nullptr, points.data(), uvs.data(), cols.data(), 3);
	}

	void PixelGameEngine::DrawRotatedDecal(const olc::vf2d& pos, olc::Decal* decal, const float fAngle, const olc::vf2d& center, const olc::vf2d& scale, const olc::Pixel& tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.pos.resize(4);
		di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
		di.w = { 1, 1, 1, 1 };
		di.tint = { tint, tint, tint, tint };
		di.points = 4;
		di.pos[0] = (olc::vf2d(0.0f, 0.0f) - center) * scale;
		di.pos[1] = (olc::vf2d(0.0f, float(decal->sprite->height)) - center) * scale;
		di.pos[2] = (olc::vf2d(float(decal->sprite->width), float(decal->sprite->height)) - center) * scale;
		di.pos[3] = (olc::vf2d(float(decal->sprite->width), 0.0f) - center) * scale;
		float c = cos(fAngle), s = sin(fAngle);
		for (int i = 0; i < 4; i++)
		{
			di.pos[i] = pos + olc::vf2d(di.pos[i].x * c - di.pos[i].y * s, di.pos[i].x * s + di.pos[i].y * c);
			di.pos[i] = di.pos[i] * vInvScreenSize * 2.0f - olc::vf2d(1.0f, 1.0f);
			di.pos[i].y *= -1.0f;
			di.w[i] = 1;
		}
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}


	void PixelGameEngine::DrawPartialRotatedDecal(const olc::vf2d& pos, olc::Decal* decal, const float fAngle, const olc::vf2d& center, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::vf2d& scale, const olc::Pixel& tint)
	{
		DecalInstance di;
		di.decal = decal;
		di.points = 4;
		di.tint = { tint, tint, tint, tint };
		di.w = { 1, 1, 1, 1 };
		di.pos.resize(4);
		di.pos[0] = (olc::vf2d(0.0f, 0.0f) - center) * scale;
		di.pos[1] = (olc::vf2d(0.0f, source_size.y) - center) * scale;
		di.pos[2] = (olc::vf2d(source_size.x, source_size.y) - center) * scale;
		di.pos[3] = (olc::vf2d(source_size.x, 0.0f) - center) * scale;
		float c = cos(fAngle), s = sin(fAngle);
		for (int i = 0; i < 4; i++)
		{
			di.pos[i] = pos + olc::vf2d(di.pos[i].x * c - di.pos[i].y * s, di.pos[i].x * s + di.pos[i].y * c);
			di.pos[i] = di.pos[i] * vInvScreenSize * 2.0f - olc::vf2d(1.0f, 1.0f);
			di.pos[i].y *= -1.0f;
		}

		olc::vf2d uvtl = source_pos * decal->vUVScale;
		olc::vf2d uvbr = uvtl + (source_size * decal->vUVScale);
		di.uv = { { uvtl.x, uvtl.y }, { uvtl.x, uvbr.y }, { uvbr.x, uvbr.y }, { uvbr.x, uvtl.y } };
		di.mode = nDecalMode;
		di.structure = nDecalStructure;
		vLayers[nTargetLayer].vecDecalInstance.push_back(di);
	}

	void PixelGameEngine::DrawPartialWarpedDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint)
	{
		DecalInstance di;
		di.points = 4;
		di.decal = decal;
		di.tint = { tint, tint, tint, tint };
		di.w = { 1, 1, 1, 1 };
		di.pos.resize(4);
		di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
		olc::vf2d center;
		float rd = ((pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y));
		if (rd != 0)
		{
			olc::vf2d uvtl = source_pos * decal->vUVScale;
			olc::vf2d uvbr = uvtl + (source_size * decal->vUVScale);
			di.uv = { { uvtl.x, uvtl.y }, { uvtl.x, uvbr.y }, { uvbr.x, uvbr.y }, { uvbr.x, uvtl.y } };

			rd = 1.0f / rd;
			float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
			float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
			if (!(rn < 0.f || rn > 1.f || sn < 0.f || sn > 1.f)) center = pos[0] + rn * (pos[2] - pos[0]);
			float d[4];	for (int i = 0; i < 4; i++)	d[i] = (pos[i] - center).mag();
			for (int i = 0; i < 4; i++)
			{
				float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
				di.uv[i] *= q; di.w[i] *= q;
				di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			}
			di.mode = nDecalMode;
			di.structure = nDecalStructure;
			vLayers[nTargetLayer].vecDecalInstance.push_back(di);
		}
	}

	void PixelGameEngine::DrawWarpedDecal(olc::Decal* decal, const olc::vf2d* pos, const olc::Pixel& tint)
	{
		// Thanks Nathan Reed, a brilliant article explaining whats going on here
		// http://www.reedbeta.com/blog/quadrilateral-interpolation-part-1/
		DecalInstance di;
		di.points = 4;
		di.decal = decal;
		di.tint = { tint, tint, tint, tint };
		di.w = { 1, 1, 1, 1 };
		di.pos.resize(4);
		di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
		olc::vf2d center;
		float rd = ((pos[2].x - pos[0].x) * (pos[3].y - pos[1].y) - (pos[3].x - pos[1].x) * (pos[2].y - pos[0].y));
		if (rd != 0)
		{
			rd = 1.0f / rd;
			float rn = ((pos[3].x - pos[1].x) * (pos[0].y - pos[1].y) - (pos[3].y - pos[1].y) * (pos[0].x - pos[1].x)) * rd;
			float sn = ((pos[2].x - pos[0].x) * (pos[0].y - pos[1].y) - (pos[2].y - pos[0].y) * (pos[0].x - pos[1].x)) * rd;
			if (!(rn < 0.f || rn > 1.f || sn < 0.f || sn > 1.f)) center = pos[0] + rn * (pos[2] - pos[0]);
			float d[4];	for (int i = 0; i < 4; i++)	d[i] = (pos[i] - center).mag();
			for (int i = 0; i < 4; i++)
			{
				float q = d[i] == 0.0f ? 1.0f : (d[i] + d[(i + 2) & 3]) / d[(i + 2) & 3];
				di.uv[i] *= q; di.w[i] *= q;
				di.pos[i] = { (pos[i].x * vInvScreenSize.x) * 2.0f - 1.0f, ((pos[i].y * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f };
			}
			di.mode = nDecalMode;
			di.structure = nDecalStructure;
			vLayers[nTargetLayer].vecDecalInstance.push_back(di);
		}
	}

	void PixelGameEngine::DrawWarpedDecal(olc::Decal* decal, const std::array<olc::vf2d, 4>& pos, const olc::Pixel& tint)
	{ DrawWarpedDecal(decal, pos.data(), tint); }

	void PixelGameEngine::DrawWarpedDecal(olc::Decal* decal, const olc::vf2d(&pos)[4], const olc::Pixel& tint)
	{ DrawWarpedDecal(decal, &pos[0], tint); }

	void PixelGameEngine::DrawPartialWarpedDecal(olc::Decal* decal, const std::array<olc::vf2d, 4>& pos, const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint)
	{ DrawPartialWarpedDecal(decal, pos.data(), source_pos, source_size, tint); }

	void PixelGameEngine::DrawPartialWarpedDecal(olc::Decal* decal, const olc::vf2d(&pos)[4], const olc::vf2d& source_pos, const olc::vf2d& source_size, const olc::Pixel& tint)
	{ DrawPartialWarpedDecal(decal, &pos[0], source_pos, source_size, tint); }

	void PixelGameEngine::DrawStringDecal(const olc::vf2d& pos, const std::string& sText, const Pixel col, const olc::vf2d& scale)
	{
		olc::vf2d spos = { 0.0f, 0.0f };
		for (auto c : sText)
		{
			if (c == '\n')
			{
				spos.x = 0; spos.y += 8.0f * scale.y;
			}
			else if (c == '\t')
			{
				spos.x += 8.0f * float(nTabSizeInSpaces) * scale.x;
			}
			else
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;
				DrawPartialDecal(pos + spos, fontRenderable.Decal(), {float(ox) * 8.0f, float(oy) * 8.0f}, {8.0f, 8.0f}, scale, col);
				spos.x += 8.0f * scale.x;
			}
		}
	}

	void PixelGameEngine::DrawStringPropDecal(const olc::vf2d& pos, const std::string& sText, const Pixel col, const olc::vf2d& scale)
	{
		olc::vf2d spos = { 0.0f, 0.0f };
		for (auto c : sText)
		{
			if (c == '\n')
			{
				spos.x = 0; spos.y += 8.0f * scale.y;
			}
			else if (c == '\t')
			{
				spos.x += 8.0f * float(nTabSizeInSpaces) * scale.x;
			}
			else
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;
				DrawPartialDecal(pos + spos, fontRenderable.Decal(), { float(ox) * 8.0f + float(vFontSpacing[c - 32].x), float(oy) * 8.0f }, { float(vFontSpacing[c - 32].y), 8.0f }, scale, col);
				spos.x += float(vFontSpacing[c - 32].y) * scale.x;
			}
		}
	}
	// Thanks Oso-Grande/Sopadeoso For these awesom and stupidly clever Text Rotation routines... duh XD
	void PixelGameEngine::DrawRotatedStringDecal(const olc::vf2d& pos, const std::string& sText, const float fAngle, const olc::vf2d& center, const Pixel col, const olc::vf2d& scale)
	{
		olc::vf2d spos = center;
		for (auto c : sText)
		{
			if (c == '\n')
			{
				spos.x = center.x; spos.y -= 8.0f;
			}
			else if (c == '\t')
			{
				spos.x += 8.0f * float(nTabSizeInSpaces) * scale.x;
			}
			else
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;
				DrawPartialRotatedDecal(pos, fontRenderable.Decal(), fAngle, spos, { float(ox) * 8.0f, float(oy) * 8.0f }, { 8.0f, 8.0f }, scale, col);
				spos.x -= 8.0f;
			}
		}
	}

	void PixelGameEngine::DrawRotatedStringPropDecal(const olc::vf2d& pos, const std::string& sText, const float fAngle, const olc::vf2d& center, const Pixel col, const olc::vf2d& scale)
	{
		olc::vf2d spos = center;
		for (auto c : sText)
		{
			if (c == '\n')
			{
				spos.x = center.x; spos.y -= 8.0f;
			}
			else if (c == '\t')
			{
				spos.x += 8.0f * float(nTabSizeInSpaces) * scale.x;
			}
			else
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;
				DrawPartialRotatedDecal(pos, fontRenderable.Decal(), fAngle, spos, { float(ox) * 8.0f + float(vFontSpacing[c - 32].x), float(oy) * 8.0f }, { float(vFontSpacing[c - 32].y), 8.0f }, scale, col);
				spos.x -= float(vFontSpacing[c - 32].y);
			}
		}
	}

	olc::vi2d PixelGameEngine::GetTextSize(const std::string& s)
	{
		olc::vi2d size = { 0,1 };
		olc::vi2d pos = { 0,1 };
		for (auto c : s)
		{
			if (c == '\n') { pos.y++;  pos.x = 0; }
			else if (c == '\t') { pos.x += nTabSizeInSpaces; }
			else pos.x++;
			size.x = std::max(size.x, pos.x);
			size.y = std::max(size.y, pos.y);
		}
		return size * 8;
	}

	void PixelGameEngine::DrawString(const olc::vi2d& pos, const std::string& sText, Pixel col, uint32_t scale)
	{ DrawString(pos.x, pos.y, sText, col, scale); }

	void PixelGameEngine::DrawString(int32_t x, int32_t y, const std::string& sText, Pixel col, uint32_t scale)
	{
		int32_t sx = 0;
		int32_t sy = 0;
		Pixel::Mode m = nPixelMode;
		// Thanks @tucna, spotted bug with col.ALPHA :P
		if (m != Pixel::CUSTOM) // Thanks @Megarev, required for "shaders"
		{
			if (col.a != 255)		SetPixelMode(Pixel::ALPHA);
			else					SetPixelMode(Pixel::MASK);
		}
		for (auto c : sText)
		{
			if (c == '\n')
			{
				sx = 0; sy += 8 * scale;
			}
			else if (c == '\t')
			{
				sx += 8 * nTabSizeInSpaces * scale;
			}
			else			
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;

				if (scale > 1)
				{
					for (uint32_t i = 0; i < 8; i++)
						for (uint32_t j = 0; j < 8; j++)
							if (fontRenderable.Sprite()->GetPixel(i + ox * 8, j + oy * 8).r > 0)
								for (uint32_t is = 0; is < scale; is++)
									for (uint32_t js = 0; js < scale; js++)
										Draw(x + sx + (i * scale) + is, y + sy + (j * scale) + js, col);
				}
				else
				{
					for (uint32_t i = 0; i < 8; i++)
						for (uint32_t j = 0; j < 8; j++)
							if (fontRenderable.Sprite()->GetPixel(i + ox * 8, j + oy * 8).r > 0)
								Draw(x + sx + i, y + sy + j, col);
				}
				sx += 8 * scale;
			}
		}
		SetPixelMode(m);
	}

	olc::vi2d PixelGameEngine::GetTextSizeProp(const std::string& s)
	{
		olc::vi2d size = { 0,1 };
		olc::vi2d pos = { 0,1 };
		for (auto c : s)
		{
			if (c == '\n') { pos.y += 1;  pos.x = 0; }
			else if (c == '\t') { pos.x += nTabSizeInSpaces * 8; }
			else pos.x += vFontSpacing[c - 32].y;
			size.x = std::max(size.x, pos.x);
			size.y = std::max(size.y, pos.y);
		}

		size.y *= 8;
		return size;
	}

	void PixelGameEngine::DrawStringProp(const olc::vi2d& pos, const std::string& sText, Pixel col, uint32_t scale)
	{ DrawStringProp(pos.x, pos.y, sText, col, scale); }

	void PixelGameEngine::DrawStringProp(int32_t x, int32_t y, const std::string& sText, Pixel col, uint32_t scale)
	{
		int32_t sx = 0;
		int32_t sy = 0;
		Pixel::Mode m = nPixelMode;

		if (m != Pixel::CUSTOM)
		{
			if (col.a != 255)		SetPixelMode(Pixel::ALPHA);
			else					SetPixelMode(Pixel::MASK);
		}
		for (auto c : sText)
		{
			if (c == '\n')
			{
				sx = 0; sy += 8 * scale;
			}
			else if (c == '\t')
			{
				sx += 8 * nTabSizeInSpaces * scale;
			}
			else
			{
				int32_t ox = (c - 32) % 16;
				int32_t oy = (c - 32) / 16;

				if (scale > 1)
				{
					for (int32_t i = 0; i < vFontSpacing[c - 32].y; i++)
						for (int32_t j = 0; j < 8; j++)
							if (fontRenderable.Sprite()->GetPixel(i + ox * 8 + vFontSpacing[c - 32].x, j + oy * 8).r > 0)
								for (int32_t is = 0; is < int(scale); is++)
									for (int32_t js = 0; js < int(scale); js++)
										Draw(x + sx + (i * scale) + is, y + sy + (j * scale) + js, col);
				}
				else
				{
					for (int32_t i = 0; i < vFontSpacing[c - 32].y; i++)
						for (int32_t j = 0; j < 8; j++)
							if (fontRenderable.Sprite()->GetPixel(i + ox * 8 + vFontSpacing[c - 32].x, j + oy * 8).r > 0)
								Draw(x + sx + i, y + sy + j, col);
				}
				sx += vFontSpacing[c - 32].y * scale;
			}
		}
		SetPixelMode(m);
	}

	void PixelGameEngine::SetPixelMode(Pixel::Mode m)
	{ nPixelMode = m; }

	Pixel::Mode PixelGameEngine::GetPixelMode()
	{ return nPixelMode; }

	void PixelGameEngine::SetPixelMode(std::function<olc::Pixel(const int x, const int y, const olc::Pixel&, const olc::Pixel&)> pixelMode)
	{
		funcPixelMode = pixelMode;
		nPixelMode = Pixel::Mode::CUSTOM;
	}

	void PixelGameEngine::SetPixelBlend(float fBlend)
	{
		fBlendFactor = fBlend;
		if (fBlendFactor < 0.0f) fBlendFactor = 0.0f;
		if (fBlendFactor > 1.0f) fBlendFactor = 1.0f;
	}

	std::stringstream& PixelGameEngine::ConsoleOut()
	{ return ssConsoleOutput; }

	bool PixelGameEngine::IsConsoleShowing() const
	{ return bConsoleShow; }

	void PixelGameEngine::ConsoleShow(const olc::Key& keyExit, bool bSuspendTime)
	{
		if (bConsoleShow)
			return;

		bConsoleShow = true;		
		bConsoleSuspendTime = bSuspendTime;
		TextEntryEnable(true);
		keyConsoleExit = keyExit;
		pKeyboardState[keyConsoleExit].bHeld = false;
		pKeyboardState[keyConsoleExit].bPressed = false;
		pKeyboardState[keyConsoleExit].bReleased = true;
	}
	
	void PixelGameEngine::ConsoleClear()
	{ sConsoleLines.clear(); }

	void PixelGameEngine::ConsoleCaptureStdOut(const bool bCapture)
	{
		if(bCapture)
			sbufOldCout = std::cout.rdbuf(ssConsoleOutput.rdbuf());
		else
			std::cout.rdbuf(sbufOldCout);
	}

	void PixelGameEngine::UpdateConsole()
	{
		if (GetKey(keyConsoleExit).bPressed)
		{
			TextEntryEnable(false);
			bConsoleSuspendTime = false;
			bConsoleShow = false;
			return;
		}

		// Keep Console sizes based in real screen dimensions
		vConsoleCharacterScale = olc::vf2d(1.0f, 2.0f) / (olc::vf2d(vViewSize) * vInvScreenSize);
		vConsoleSize = (vViewSize / olc::vi2d(8, 16)) - olc::vi2d(2, 4);

		// If console has changed size, simply reset it
		if (vConsoleSize.y != sConsoleLines.size())
		{
			vConsoleCursor = { 0,0 };
			sConsoleLines.clear();
			sConsoleLines.resize(vConsoleSize.y);
		}

		auto TypeCharacter = [&](const char c)
		{
			if (c >= 32 && c < 127)
			{
				sConsoleLines[vConsoleCursor.y].append(1, c);
				vConsoleCursor.x++;
			}

			if( c == '\n' || vConsoleCursor.x >= vConsoleSize.x)
			{
				vConsoleCursor.y++; vConsoleCursor.x = 0;				
			}			

			if (vConsoleCursor.y >= vConsoleSize.y)
			{
				vConsoleCursor.y = vConsoleSize.y - 1;
				for (int i = 1; i < vConsoleSize.y; i++)
					sConsoleLines[i - 1] = sConsoleLines[i];
				sConsoleLines[vConsoleCursor.y].clear();
			}
		};

		// Empty out "std::cout", parsing as we go
		while (ssConsoleOutput.rdbuf()->sgetc() != -1)
		{
			char c = ssConsoleOutput.rdbuf()->sbumpc();
			TypeCharacter(c);
		}

		// Draw Shadow
		GradientFillRectDecal({ 0,0 }, olc::vf2d(vScreenSize), olc::PixelF(0, 0, 0.5f, 0.5f), olc::PixelF(0, 0, 0.25f, 0.5f), olc::PixelF(0, 0, 0.25f, 0.5f), olc::PixelF(0, 0, 0.25f, 0.5f));
				
		// Draw the console buffer
		SetDecalMode(olc::DecalMode::NORMAL);
		for (int32_t nLine = 0; nLine < vConsoleSize.y; nLine++)
			DrawStringDecal(olc::vf2d( 1, 1 + float(nLine) ) * vConsoleCharacterScale * 8.0f, sConsoleLines[nLine], olc::WHITE, vConsoleCharacterScale);

		// Draw Input State
		FillRectDecal(olc::vf2d(1 + float((TextEntryGetCursor() + 1)), 1 + float((vConsoleSize.y - 1))) * vConsoleCharacterScale * 8.0f, olc::vf2d(8, 8) * vConsoleCharacterScale, olc::DARK_CYAN);
		DrawStringDecal(olc::vf2d(1, 1 + float((vConsoleSize.y - 1))) * vConsoleCharacterScale * 8.0f, std::string(">") + TextEntryGetString(), olc::YELLOW, vConsoleCharacterScale);		
	}


	const std::vector<std::string>& PixelGameEngine::GetDroppedFiles() const
	{ return vDroppedFiles;	}

	const olc::vi2d& PixelGameEngine::GetDroppedFilesPoint() const
	{ return vDroppedFilesPoint; }


	void PixelGameEngine::TextEntryEnable(const bool bEnable, const std::string& sText)
	{
		if (bEnable)
		{
			nTextEntryCursor = int32_t(sText.size());
			sTextEntryString = sText;
			bTextEntryEnable = true;
		}
		else
		{
			bTextEntryEnable = false;
		}
	}

	std::string PixelGameEngine::TextEntryGetString() const
	{ return sTextEntryString; }

	int32_t PixelGameEngine::TextEntryGetCursor() const
	{ return nTextEntryCursor; }

	bool PixelGameEngine::IsTextEntryEnabled() const
	{ return bTextEntryEnable; }


	void PixelGameEngine::UpdateTextEntry()
	{
		// Check for typed characters
		for (const auto& key : vKeyboardMap)
			if (GetKey(std::get<0>(key)).bPressed)
			{
				sTextEntryString.insert(nTextEntryCursor, GetKey(olc::Key::SHIFT).bHeld ? std::get<2>(key) : std::get<1>(key));
				nTextEntryCursor++;
			}

		// Check for command characters
		if (GetKey(olc::Key::LEFT).bPressed)
			nTextEntryCursor = std::max(0, nTextEntryCursor - 1);
		if (GetKey(olc::Key::RIGHT).bPressed)
			nTextEntryCursor = std::min(int32_t(sTextEntryString.size()), nTextEntryCursor + 1);
		if (GetKey(olc::Key::BACK).bPressed && nTextEntryCursor > 0)
		{
			sTextEntryString.erase(nTextEntryCursor-1, 1);
			nTextEntryCursor = std::max(0, nTextEntryCursor - 1);
		}
		if (GetKey(olc::Key::DEL).bPressed && size_t(nTextEntryCursor) < sTextEntryString.size())
			sTextEntryString.erase(nTextEntryCursor, 1);	

		if (GetKey(olc::Key::UP).bPressed)
		{
			if (!sCommandHistory.empty())
			{
				if (sCommandHistoryIt != sCommandHistory.begin())
					sCommandHistoryIt--;

				nTextEntryCursor = int32_t(sCommandHistoryIt->size());
				sTextEntryString = *sCommandHistoryIt;
			}
		}

		if (GetKey(olc::Key::DOWN).bPressed)
		{	
			if (!sCommandHistory.empty())
			{
				if (sCommandHistoryIt != sCommandHistory.end())
				{
					sCommandHistoryIt++;
					if (sCommandHistoryIt != sCommandHistory.end())
					{
						nTextEntryCursor = int32_t(sCommandHistoryIt->size());
						sTextEntryString = *sCommandHistoryIt;
					}
					else
					{
						nTextEntryCursor = 0;
						sTextEntryString = "";
					}
				}
			}
		}

		if (GetKey(olc::Key::ENTER).bPressed)
		{
			if (bConsoleShow)
			{
				std::cout << ">" + sTextEntryString + "\n";
				if (OnConsoleCommand(sTextEntryString))
				{
					sCommandHistory.push_back(sTextEntryString);
					sCommandHistoryIt = sCommandHistory.end();
				}
				sTextEntryString.clear();
				nTextEntryCursor = 0;
			}
			else
			{
				OnTextEntryComplete(sTextEntryString);
				TextEntryEnable(false);
			}
		}
	}

	// User must override these functions as required. I have not made
	// them abstract because I do need a default behaviour to occur if
	// they are not overwritten

	bool PixelGameEngine::OnUserCreate()
	{ return false;	}

	bool PixelGameEngine::OnUserUpdate(float fElapsedTime)
	{ UNUSED(fElapsedTime);  return false; }

	bool PixelGameEngine::OnUserDestroy()
	{ return true; }

	void PixelGameEngine::OnTextEntryComplete(const std::string& sText) { UNUSED(sText); }
	bool PixelGameEngine::OnConsoleCommand(const std::string& sCommand) { UNUSED(sCommand); return false; }
	

	olc::rcode PixelGameEngine::SetWindowSize(const olc::vi2d& vPos, const olc::vi2d& vSize)
	{
		if (platform)
			return platform->SetWindowSize(vPos, vSize);
		else
			return olc::FAIL;
	}
	
	olc::rcode PixelGameEngine::ShowWindowFrame(const bool bShowFrame)
	{
		if (platform)
			return platform->ShowWindowFrame(bShowFrame);
		else
			return olc::FAIL;
	}


	// Externalised API
	void PixelGameEngine::olc_UpdateViewport()
	{
		if (bRealWindowMode)
		{
			vPixelSize = { 1,1 };
			vViewSize = vScreenSize;
			vViewPos = { 0,0 };
			return;
		}

		int32_t ww = vScreenSize.x * vPixelSize.x;
		int32_t wh = vScreenSize.y * vPixelSize.y;
		float wasp = (float)ww / (float)wh;

		if (bPixelCohesion)
		{
			vScreenPixelSize = (vWindowSize / vScreenSize);
			vViewSize = (vWindowSize / vScreenSize) * vScreenSize;
		}
		else
		{
			vViewSize.x = (int32_t)vWindowSize.x;
			vViewSize.y = (int32_t)((float)vViewSize.x / wasp);

			if (vViewSize.y > vWindowSize.y)
			{
				vViewSize.y = vWindowSize.y;
				vViewSize.x = (int32_t)((float)vViewSize.y * wasp);
			}
		}

		vViewPos = (vWindowSize - vViewSize) / 2;
	}

	void PixelGameEngine::olc_UpdateWindowPos(int32_t x, int32_t y)
	{
		vWindowPos = { x, y };	
		olc_UpdateViewport();
	}

	void PixelGameEngine::olc_UpdateWindowSize(int32_t x, int32_t y)
	{
		vWindowSize = { x, y };

		if (bRealWindowMode)
		{
			vResizeRequested = vWindowSize;
			bResizeRequested = true;			
		}

		olc_UpdateViewport();
	}

	void PixelGameEngine::olc_UpdateMouseWheel(int32_t delta)
	{ nMouseWheelDeltaCache += delta; }

	void PixelGameEngine::olc_UpdateMouse(int32_t x, int32_t y)
	{
		// Mouse coords come in screen space
		// But leave in pixel space
		bHasMouseFocus = true;
		vMouseWindowPos = { x, y };
		// Full Screen mode may have a weird viewport we must clamp to
		x -= vViewPos.x;
		y -= vViewPos.y;
		vMousePosCache.x = (int32_t)(((float)x / (float)(vWindowSize.x - (vViewPos.x * 2)) * (float)vScreenSize.x));
		vMousePosCache.y = (int32_t)(((float)y / (float)(vWindowSize.y - (vViewPos.y * 2)) * (float)vScreenSize.y));
		if (vMousePosCache.x >= (int32_t)vScreenSize.x)	vMousePosCache.x = vScreenSize.x - 1;
		if (vMousePosCache.y >= (int32_t)vScreenSize.y)	vMousePosCache.y = vScreenSize.y - 1;
		if (vMousePosCache.x < 0) vMousePosCache.x = 0;
		if (vMousePosCache.y < 0) vMousePosCache.y = 0;
	}

	void PixelGameEngine::olc_UpdateMouseState(int32_t button, bool state)
	{ pMouseNewState[button] = state; }

	void PixelGameEngine::olc_UpdateKeyState(int32_t key, bool state)
	{ pKeyNewState[key] = state; }

	void PixelGameEngine::olc_UpdateMouseFocus(bool state)
	{ bHasMouseFocus = state; }

	void PixelGameEngine::olc_UpdateKeyFocus(bool state)
	{ bHasInputFocus = state; }

	void PixelGameEngine::olc_DropFiles(int32_t x, int32_t y, const std::vector<std::string>& vFiles)
	{ 
		x -= vViewPos.x;
		y -= vViewPos.y;
		vDroppedFilesPointCache.x = (int32_t)(((float)x / (float)(vWindowSize.x - (vViewPos.x * 2)) * (float)vScreenSize.x));
		vDroppedFilesPointCache.y = (int32_t)(((float)y / (float)(vWindowSize.y - (vViewPos.y * 2)) * (float)vScreenSize.y));
		if (vDroppedFilesPointCache.x >= (int32_t)vScreenSize.x)	vDroppedFilesPointCache.x = vScreenSize.x - 1;
		if (vDroppedFilesPointCache.y >= (int32_t)vScreenSize.y)	vDroppedFilesPointCache.y = vScreenSize.y - 1;
		if (vDroppedFilesPointCache.x < 0) vDroppedFilesPointCache.x = 0;
		if (vDroppedFilesPointCache.y < 0) vDroppedFilesPointCache.y = 0;
		vDroppedFilesCache = vFiles; 
	}

	void PixelGameEngine::olc_Reanimate()
	{ bAtomActive = true; }

	bool PixelGameEngine::olc_IsRunning()
	{ return bAtomActive; }

	void PixelGameEngine::olc_Terminate()
	{ bAtomActive = false; }

	void PixelGameEngine::EngineThread()
	{
		// Allow platform to do stuff here if needed, since its now in the
		// context of this thread
		if (platform->ThreadStartUp() == olc::FAIL)	return;

		// Do engine context specific initialisation
		olc_PrepareEngine();

		// Create user resources as part of this thread
		for (auto& ext : vExtensions) ext->OnBeforeUserCreate();
		if (!OnUserCreate()) bAtomActive = false;
		for (auto& ext : vExtensions) ext->OnAfterUserCreate();

		while (bAtomActive)
		{
			// Run as fast as possible
			while (bAtomActive) { olc_CoreUpdate(); }

			// Allow the user to free resources if they have overrided the destroy function
			if (!OnUserDestroy())
			{
				// User denied destroy for some reason, so continue running
				bAtomActive = true;
			}
		}

		platform->ThreadCleanUp();
	}

	void PixelGameEngine::olc_PrepareEngine()
	{
		// Start OpenGL, the context is owned by the game thread
		if (platform->CreateGraphics(bFullScreen, bEnableVSYNC, vViewPos, vViewSize) == olc::FAIL) return;

		// Construct default font sheet
		olc_ConstructFontSheet();

		// Create Primary Layer "0"
		CreateLayer();
		vLayers[0].bUpdate = true;
		vLayers[0].bShow = true;
		SetDrawTarget(nullptr);

		m_tp1 = std::chrono::system_clock::now();
		m_tp2 = std::chrono::system_clock::now();
	}


	void PixelGameEngine::adv_ManualRenderEnable(const bool bEnable)
	{
		bManualRenderEnable = bEnable;
	}

	void PixelGameEngine::adv_HardwareClip(const bool bClipAndScale, const olc::vi2d & viewPos, const olc::vi2d & viewSize, const bool bClear)
	{
		olc::vf2d vNewSize = olc::vf2d(viewSize) / olc::vf2d(vScreenSize);
		olc::vf2d vNewPos = olc::vf2d(viewPos) / olc::vf2d(vScreenSize);
		renderer->UpdateViewport(vViewPos + vNewPos * vViewSize, vNewSize * vViewSize);

		if (bClear)
			renderer->ClearBuffer(olc::BLACK, true);

		SetDecalMode(DecalMode::NORMAL);
		renderer->PrepareDrawing();

		if(!bClipAndScale)
			vInvScreenSize = 1.0f / olc::vf2d(viewSize);
		else
			vInvScreenSize = 1.0f / olc::vf2d(vScreenSize);
	}

	void PixelGameEngine::adv_FlushLayer(const size_t nLayerID)
	{
		auto& layer = vLayers[nLayerID];

		if (layer.bShow)
		{
			if (layer.funcHook == nullptr)
			{
				renderer->ApplyTexture(layer.pDrawTarget.Decal()->id);
				if (!bSuspendTextureTransfer)
				{
					layer.pDrawTarget.Decal()->Update();
					layer.bUpdate = false;
				}

				// Can't use this as it assumes full screen coords
				// renderer->DrawLayerQuad(layer.vOffset, layer.vScale, layer.tint);			
				// Instead, render a textured decal

				olc::vf2d vScreenSpacePos =
				{
					(layer.vOffset.x  * vInvScreenSize.x) * 2.0f - 1.0f,
					((layer.vOffset.y  * vInvScreenSize.y) * 2.0f - 1.0f) * -1.0f
				};

				olc::vf2d vScreenSpaceDim =
				{
					vScreenSpacePos.x + (2.0f * (float(layer.pDrawTarget.Sprite()->width) * vInvScreenSize.x)) * layer.vScale.x,
					vScreenSpacePos.y - (2.0f * (float(layer.pDrawTarget.Sprite()->height) * vInvScreenSize.y)) * layer.vScale.y
				};

				DecalInstance di;
				di.decal = layer.pDrawTarget.Decal();
				di.points = 4;
				di.tint = { olc::WHITE, olc::WHITE, olc::WHITE, olc::WHITE };
				di.pos = { { vScreenSpacePos.x, vScreenSpacePos.y }, { vScreenSpacePos.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpaceDim.y }, { vScreenSpaceDim.x, vScreenSpacePos.y } };
				di.uv = { { 0.0f, 0.0f}, {0.0f, 1.0f}, {1.0f, 1.0f}, {1.0f, 0.0f} };
				di.w = { 1, 1, 1, 1 };
				di.mode = DecalMode::NORMAL;
				di.structure = DecalStructure::FAN;
				renderer->DrawDecal(di);
			}
			else
			{
				// Mwa ha ha.... Have Fun!!!
				layer.funcHook();
			}
		}
	}

	void PixelGameEngine::adv_FlushLayerDecals(const size_t nLayerID)
	{
		// Display Decals in order for this layer
		auto& layer = vLayers[nLayerID];
		for (auto& decal : layer.vecDecalInstance)
			renderer->DrawDecal(decal);
		layer.vecDecalInstance.clear();
	}



	void PixelGameEngine::olc_CoreUpdate()
	{
		// Handle Timing
		m_tp2 = std::chrono::system_clock::now();
		std::chrono::duration<float> elapsedTime = m_tp2 - m_tp1;
		m_tp1 = m_tp2;

		// Our time per frame coefficient
		float fElapsedTime = elapsedTime.count();
		fLastElapsed = fElapsedTime;

		if (bConsoleSuspendTime)
			fElapsedTime = 0.0f;

		// Some platforms will need to check for events
		platform->HandleSystemEvent();

		// Compare hardware input states from previous frame
		auto ScanHardware = [&](HWButton* pKeys, bool* pStateOld, bool* pStateNew, uint32_t nKeyCount)
		{
			for (uint32_t i = 0; i < nKeyCount; i++)
			{
				pKeys[i].bPressed = false;
				pKeys[i].bReleased = false;
				if (pStateNew[i] != pStateOld[i])
				{
					if (pStateNew[i])
					{
						pKeys[i].bPressed = !pKeys[i].bHeld;
						pKeys[i].bHeld = true;
					}
					else
					{
						pKeys[i].bReleased = true;
						pKeys[i].bHeld = false;
					}
				}
				pStateOld[i] = pStateNew[i];
			}
		};

		ScanHardware(pKeyboardState, pKeyOldState, pKeyNewState, 256);
		ScanHardware(pMouseState, pMouseOldState, pMouseNewState, nMouseButtons);

		// Cache mouse coordinates so they remain consistent during frame
		vMousePos = vMousePosCache;
		nMouseWheelDelta = nMouseWheelDeltaCache;
		nMouseWheelDeltaCache = 0;

		vDroppedFiles = vDroppedFilesCache;
		vDroppedFilesPoint = vDroppedFilesPointCache;
		vDroppedFilesCache.clear();

		if (bTextEntryEnable)
		{
			UpdateTextEntry();
		}

		// Handle Frame Update
		bool bExtensionBlockFrame = false;		
		for (auto& ext : vExtensions) bExtensionBlockFrame |= ext->OnBeforeUserUpdate(fElapsedTime);
		if (!bExtensionBlockFrame)
		{
			if (!OnUserUpdate(fElapsedTime)) bAtomActive = false;
			
		}
		for (auto& ext : vExtensions) ext->OnAfterUserUpdate(fElapsedTime);

		

		if (bRealWindowMode)
		{
			vPixelSize = { 1,1 };
			vViewSize = vScreenSize;
			vViewPos = { 0,0 };
		}

		if (!bManualRenderEnable)
		{
			if (bConsoleShow)
			{
				SetDrawTarget((uint8_t)0);
				UpdateConsole();
			}

			// Display Frame
			renderer->UpdateViewport(vViewPos, vViewSize);
			renderer->ClearBuffer(olc::BLACK, true);

			// Layer 0 must always exist
			vLayers[0].bUpdate = true;
			vLayers[0].bShow = true;
			SetDecalMode(DecalMode::NORMAL);
			renderer->PrepareDrawing();

			for (auto layer = vLayers.rbegin(); layer != vLayers.rend(); ++layer)
			{
				if (layer->bShow)
				{
					if (layer->funcHook == nullptr)
					{
						renderer->ApplyTexture(layer->pDrawTarget.Decal()->id);
						if (!bSuspendTextureTransfer && layer->bUpdate)
						{
							layer->pDrawTarget.Decal()->Update();
							layer->bUpdate = false;
						}

						renderer->DrawLayerQuad(layer->vOffset, layer->vScale, layer->tint);

						// Display Decals in order for this layer
						for (auto& decal : layer->vecDecalInstance)
							renderer->DrawDecal(decal);
						layer->vecDecalInstance.clear();
					}
					else
					{
						// Mwa ha ha.... Have Fun!!!
						layer->funcHook();
					}
				}
			}
		}	

		// Present Graphics to screen
		renderer->DisplayFrame();

		if (bResizeRequested)
		{
			bResizeRequested = false;
			SetScreenSize(vWindowSize.x, vWindowSize.y);
			renderer->UpdateViewport({ 0,0 }, vWindowSize);
		}

		// Update Title Bar
		fFrameTimer += fElapsedTime;
		nFrameCount++;
		if (fFrameTimer >= 1.0f)
		{
			nLastFPS = nFrameCount;
			fFrameTimer -= 1.0f;
			std::string sTitle = "OneLoneCoder.com - Pixel Game Engine - " + sAppName + " - FPS: " + std::to_string(nFrameCount);
			platform->SetWindowTitle(sTitle);
			nFrameCount = 0;
		}
	}

	void PixelGameEngine::olc_ConstructFontSheet()
	{
		std::string data;
		data += "?Q`0001oOch0o01o@F40o0<AGD4090LAGD<090@A7ch0?00O7Q`0600>00000000";
		data += "O000000nOT0063Qo4d8>?7a14Gno94AA4gno94AaOT0>o3`oO400o7QN00000400";
		data += "Of80001oOg<7O7moBGT7O7lABET024@aBEd714AiOdl717a_=TH013Q>00000000";
		data += "720D000V?V5oB3Q_HdUoE7a9@DdDE4A9@DmoE4A;Hg]oM4Aj8S4D84@`00000000";
		data += "OaPT1000Oa`^13P1@AI[?g`1@A=[OdAoHgljA4Ao?WlBA7l1710007l100000000";
		data += "ObM6000oOfMV?3QoBDD`O7a0BDDH@5A0BDD<@5A0BGeVO5ao@CQR?5Po00000000";
		data += "Oc``000?Ogij70PO2D]??0Ph2DUM@7i`2DTg@7lh2GUj?0TO0C1870T?00000000";
		data += "70<4001o?P<7?1QoHg43O;`h@GT0@:@LB@d0>:@hN@L0@?aoN@<0O7ao0000?000";
		data += "OcH0001SOglLA7mg24TnK7ln24US>0PL24U140PnOgl0>7QgOcH0K71S0000A000";
		data += "00H00000@Dm1S007@DUSg00?OdTnH7YhOfTL<7Yh@Cl0700?@Ah0300700000000";
		data += "<008001QL00ZA41a@6HnI<1i@FHLM81M@@0LG81?O`0nC?Y7?`0ZA7Y300080000";
		data += "O`082000Oh0827mo6>Hn?Wmo?6HnMb11MP08@C11H`08@FP0@@0004@000000000";
		data += "00P00001Oab00003OcKP0006@6=PMgl<@440MglH@000000`@000001P00000000";
		data += "Ob@8@@00Ob@8@Ga13R@8Mga172@8?PAo3R@827QoOb@820@0O`0007`0000007P0";
		data += "O`000P08Od400g`<3V=P0G`673IP0`@3>1`00P@6O`P00g`<O`000GP800000000";
		data += "?P9PL020O`<`N3R0@E4HC7b0@ET<ATB0@@l6C4B0O`H3N7b0?P01L3R000000020";

		fontRenderable.Create(128, 48);

		int px = 0, py = 0;
		for (size_t b = 0; b < 1024; b += 4)
		{
			uint32_t sym1 = (uint32_t)data[b + 0] - 48;
			uint32_t sym2 = (uint32_t)data[b + 1] - 48;
			uint32_t sym3 = (uint32_t)data[b + 2] - 48;
			uint32_t sym4 = (uint32_t)data[b + 3] - 48;
			uint32_t r = sym1 << 18 | sym2 << 12 | sym3 << 6 | sym4;

			for (int i = 0; i < 24; i++)
			{
				int k = r & (1 << i) ? 255 : 0;
				fontRenderable.Sprite()->SetPixel(px, py, olc::Pixel(k, k, k, k));
				if (++py == 48) { px++; py = 0; }
			}
		}

		fontRenderable.Decal()->Update();

		constexpr std::array<uint8_t, 96> vSpacing = { {
			0x03,0x25,0x16,0x08,0x07,0x08,0x08,0x04,0x15,0x15,0x08,0x07,0x15,0x07,0x24,0x08,
			0x08,0x17,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x24,0x15,0x06,0x07,0x16,0x17,
			0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x17,0x08,0x08,0x17,0x08,0x08,0x08,
			0x08,0x08,0x08,0x08,0x17,0x08,0x08,0x08,0x08,0x17,0x08,0x15,0x08,0x15,0x08,0x08,
			0x24,0x18,0x17,0x17,0x17,0x17,0x17,0x17,0x17,0x33,0x17,0x17,0x33,0x18,0x17,0x17,
			0x17,0x17,0x17,0x17,0x07,0x17,0x17,0x18,0x18,0x17,0x17,0x07,0x33,0x07,0x08,0x00, } };

		for (auto c : vSpacing) vFontSpacing.push_back({ c >> 4, c & 15 });

		// UK Standard Layout
#ifdef OLC_KEYBOARD_UK
		vKeyboardMap =
		{
			{olc::Key::A, "a", "A"}, {olc::Key::B, "b", "B"}, {olc::Key::C, "c", "C"}, {olc::Key::D, "d", "D"}, {olc::Key::E, "e", "E"},
			{olc::Key::F, "f", "F"}, {olc::Key::G, "g", "G"}, {olc::Key::H, "h", "H"}, {olc::Key::I, "i", "I"}, {olc::Key::J, "j", "J"},
			{olc::Key::K, "k", "K"}, {olc::Key::L, "l", "L"}, {olc::Key::M, "m", "M"}, {olc::Key::N, "n", "N"}, {olc::Key::O, "o", "O"},
			{olc::Key::P, "p", "P"}, {olc::Key::Q, "q", "Q"}, {olc::Key::R, "r", "R"}, {olc::Key::S, "s", "S"}, {olc::Key::T, "t", "T"},
			{olc::Key::U, "u", "U"}, {olc::Key::V, "v", "V"}, {olc::Key::W, "w", "W"}, {olc::Key::X, "x", "X"}, {olc::Key::Y, "y", "Y"},
			{olc::Key::Z, "z", "Z"},

			{olc::Key::K0, "0", ")"}, {olc::Key::K1, "1", "!"}, {olc::Key::K2, "2", "\""}, {olc::Key::K3, "3", "#"},	{olc::Key::K4, "4", "$"},
			{olc::Key::K5, "5", "%"}, {olc::Key::K6, "6", "^"}, {olc::Key::K7, "7", "&"}, {olc::Key::K8, "8", "*"},	{olc::Key::K9, "9", "("},

			{olc::Key::NP0, "0", "0"}, {olc::Key::NP1, "1", "1"}, {olc::Key::NP2, "2", "2"}, {olc::Key::NP3, "3", "3"},	{olc::Key::NP4, "4", "4"},
			{olc::Key::NP5, "5", "5"}, {olc::Key::NP6, "6", "6"}, {olc::Key::NP7, "7", "7"}, {olc::Key::NP8, "8", "8"},	{olc::Key::NP9, "9", "9"},
			{olc::Key::NP_MUL, "*", "*"}, {olc::Key::NP_DIV, "/", "/"}, {olc::Key::NP_ADD, "+", "+"}, {olc::Key::NP_SUB, "-", "-"},	{olc::Key::NP_DECIMAL, ".", "."},

			{olc::Key::PERIOD, ".", ">"}, {olc::Key::EQUALS, "=", "+"}, {olc::Key::COMMA, ",", "<"}, {olc::Key::MINUS, "-", "_"}, {olc::Key::SPACE, " ", " "},

			{olc::Key::OEM_1, ";", ":"}, {olc::Key::OEM_2, "/", "?"}, {olc::Key::OEM_3, "\'", "@"}, {olc::Key::OEM_4, "[", "{"},
			{olc::Key::OEM_5, "\\", "|"}, {olc::Key::OEM_6, "]", "}"}, {olc::Key::OEM_7, "#", "~"}, 
			
			// {olc::Key::TAB, "\t", "\t"}
		};
#endif
	}

	void PixelGameEngine::pgex_Register(olc::PGEX* pgex)
	{
		if (std::find(vExtensions.begin(), vExtensions.end(), pgex) == vExtensions.end())
			vExtensions.push_back(pgex);			
	}
	

	PGEX::PGEX(bool bHook) { if(bHook) pge->pgex_Register(this); }
	void PGEX::OnBeforeUserCreate() {}
	void PGEX::OnAfterUserCreate()	{}
	bool PGEX::OnBeforeUserUpdate(float& fElapsedTime) { return false; }
	void PGEX::OnAfterUserUpdate(float fElapsedTime) {}

	// Need a couple of statics as these are singleton instances
	// read from multiple locations
	std::atomic<bool> PixelGameEngine::bAtomActive{ false };
	olc::PixelGameEngine* olc::PGEX::pge = nullptr;
	olc::PixelGameEngine* olc::Platform::ptrPGE = nullptr;
	olc::PixelGameEngine* olc::Renderer::ptrPGE = nullptr;
	std::unique_ptr<ImageLoader> olc::Sprite::loader = nullptr;
};