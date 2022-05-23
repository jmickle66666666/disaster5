using Jurassic.Library;
using Jurassic;
using Raylib_cs;

namespace DisasterAPI
{
    [ClassDescription("Functions for engine-level functionality.")]
    class Engine : ObjectInstance
    {
        public Engine(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }


        [JSProperty(Name = "timescale")]
        [PropertyDescription("Global game speed modifier. 1.0 = Default. 0.5 = half speed")]
        public static double timescale
        {
            get
            {
                return Disaster.Program.timescale;
            }
            set
            {
                Disaster.Program.timescale = value < 0 ? 0 : value;
            }
        }


        [JSFunction(Name = "reset")]
        [FunctionDescription("Unload all assets and reset the engine.")]
        public static void Reset()
        {
            Disaster.JS.instance.Reset();
        }

        [JSFunction(Name = "setResetHotkey")]
        [FunctionDescription("Set a global hotkey for resetting the engine, default F2.")]
        [ArgumentDescription("keyCode", "The keycode to set the hotkey to. Check Key documentation for options.")]
        public static void SetResetHotkey(int keyCode)
        {
            Disaster.JS.resetKey = Input.keyCodes[keyCode];
        }

        [JSFunction(Name ="reloadShaders")]
        [FunctionDescription("Reload all shaders without resetting the engine.")]
        public static void ReloadShaders()
        {
            Disaster.Assets.shaders.Clear();
            Disaster.Assets.assignedDefaultShader = false;
            Disaster.ScreenController.instance.ReloadShader();
        }

        [JSFunction(Name = "quit")]
        [FunctionDescription("Quit the game.")]
        public static void Quit()
        {
            Disaster.Program.running = false;
        }

        [JSFunction(Name = "redraw")]
        [FunctionDescription("Force a render mid-frame.")]
        public static void Redraw()
        {
            Disaster.Program.screen.Update();
        }

        [JSFunction(Name = "preload")]
        [FunctionDescription("Preload an asset.")]
        [ArgumentDescription("path", "Path to the asset, asset type is determined by the extension")]
        public static void Preload(string path)
        {
            Disaster.Assets.Preload(path);
        }

        [JSFunction(Name ="slowDrawFrame")]
        [FunctionDescription("Draw the next frame pixel by pixel")]
        [ArgumentDescription("frameSkip", "How many pixels to draw per frame of slow draw")]
        public static void SlowDrawFrame(int frameSkip)
        {
            // TODO: Re-add slowdraw support
        }

        [JSFunction(Name = "toggleOverdraw")]
        [FunctionDescription("Toggle overdraw debug visualisation. Brighter pixels are being drawn more times")]
        public static void ToggleOverdraw()
        {
            // TODO: Re-add overdraw support
        }

        [JSFunction(Name = "setMouseVisible")]
        [FunctionDescription("Show or hide the mouse cursor")]
        [ArgumentDescription("visible", "true: show mouse, false: hide mouse")]
        public static void SetMouseVisible(bool visible)
        {
            if (visible)
            {
                Raylib.ShowCursor();
            } else
            {
                Raylib.HideCursor();
            }
        }

        [JSFunction(Name = "setTitle")]
        [FunctionDescription("Set the window title")]
        [ArgumentDescription("title", "The new title for the window")]
        public static void SetTitle(string title)
        {
            Raylib.SetWindowTitle(title);
        }

        [JSFunction(Name = "setResolution")]
        [FunctionDescription("Set the screen resolution")]
        [ArgumentDescription("width", "sometimes i don't know what to write in here cause the variable name says everything already")]
        [ArgumentDescription("height", "i do'nt think it a PROBLEM to have a redundant description but its difficult to write them")]
        [ArgumentDescription("scale", "integer scaling for the screen")]
        public static void SetResolution(int width, int height, int scale)
        {
            Disaster.ScreenController.instance.Resize(width, height, scale);
        }

        [JSFunction(Name = "getTime")]
        [FunctionDescription("Return current system time.")]
        public static double GetTime()
        {
            return (double)System.DateTime.Now.Ticks/ (double)System.TimeSpan.TicksPerSecond;
        }

        [JSFunction(Name = "setTargetFPS")]
        [FunctionDescription("Set target maximum FPS.")]
        public static void SetTargetFPS(int fps)
        {
            Raylib.SetTargetFPS(fps);
        }
    }

}
