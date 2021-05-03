using System.IO;
using System;
using Jurassic;
using System.Collections.Generic;
using Raylib_cs;

namespace Disaster {

    public class JS
    {
        public static JS instance;
        public string basedir;
        public ScriptEngine engine;
        public Dictionary<string, Jurassic.Library.GlobalObject> cachedScripts;
        public List<string> currentlyLoadingScripts;
        Jurassic.Library.FunctionInstance updateFunction;
        
        public JS()
        {
            instance = this;
            LoadScripts();
        }

        public void Reset()
        {
            currentlyLoadingScripts.Clear();
            cachedScripts.Clear();
            LoadScripts();
        }

        bool stopped = false;

        public void Update(double deltaTime)
        {
            if (stopped)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                {
                    Assets.UnloadAll();
                    Reset();
                    stopped = false;
                }
                return;
            }

            try
            {
                DisasterAPI.Input.Update();
                
                updateFunction.Call(null, deltaTime);
            }
            catch (JavaScriptException e)
            {
                if (Assets.LoadPath("fontsmall.png", out string fontPath))
                {
                    SoftwareCanvas.LoadFont(fontPath);
                }
                string message = $"line:{e.LineNumber} {e.Message}";
                Program.LoadingMessage(message, new Color32(255, 50, 0));
                Program.LoadingMessage("press R to reload", new Color32(255, 50, 0));
                stopped = true;
            }
            
        }

        public static void LoadStandardFunctions(ScriptEngine engine)
        {
            engine.SetGlobalFunction("load", new System.Func<string, object>((string path) => {
                return Assets.Script(path);
            }));

            engine.SetGlobalFunction("log", new Action<string>((string message) => { Console.WriteLine(message); }));

            engine.SetGlobalValue("Draw", new DisasterAPI.Draw(engine));
            engine.SetGlobalValue("Input", new DisasterAPI.Input(engine));
            engine.SetGlobalValue("Audio", new DisasterAPI.Audio(engine));
            engine.SetGlobalValue("Debug", new DisasterAPI.Debug(engine));
            engine.SetGlobalValue("Engine", new DisasterAPI.Engine(engine));
            engine.SetGlobalValue("Assets", new DisasterAPI.Assets(engine));
        }

        void LoadScripts()
        {
            cachedScripts = new Dictionary<string, Jurassic.Library.GlobalObject>();
            currentlyLoadingScripts = new List<string>();
            engine = new ScriptEngine();

            LoadStandardFunctions(engine);
        
            try
            {
                engine.Execute(
                    File.ReadAllText(Path.Combine(Assets.basePath, "main.js"))
                );
            } catch (Exception e)
            {
                Program.LoadingMessage(e.Message);
            }

            updateFunction = engine.GetGlobalValue<Jurassic.Library.FunctionInstance>("update");
        }

    }

}