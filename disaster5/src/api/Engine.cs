﻿using Jurassic.Library;
using Jurassic;
using Raylib_cs;

namespace DisasterAPI
{
    class Engine : ObjectInstance
    {
        public Engine(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }


        [JSProperty(Name = "timescale")]
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
            Disaster.Assets.UnloadAll();
            Disaster.Assets.assignedDefaultShader = false;
            Disaster.ScreenController.instance.ReloadShader();
            Disaster.JS.instance.Reset();
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
            Disaster.SoftwareCanvas.slowDrawPixels = frameSkip;
            Disaster.SoftwareCanvas.slowDraw = true;
        }

        [JSFunction(Name = "toggleOverdraw")]
        [FunctionDescription("Toggle overdraw debug visualisation. Brighter pixels are being drawn more times")]
        public static void ToggleOverdraw()
        {
            Disaster.SoftwareCanvas.overdraw =! Disaster.SoftwareCanvas.overdraw;
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

        [JSFunction(Name = "setResolution")]
        [FunctionDescription("Set the screen resolution")]
        [ArgumentDescription("width", "sometimes i don't know what to write in here cause the variable name says everything already")]
        [ArgumentDescription("height", "i do'nt think it a PROBLEM to have a redundant description but its difficult to write them")]
        [ArgumentDescription("scale", "integer scaling for the screen")]
        public static void SetResolution(int width, int height, int scale)
        {
            Disaster.ScreenController.instance.Resize(width, height, scale);
        }
    }

}
