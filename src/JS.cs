using System.IO;
using System;
using Jurassic;
using System.Collections.Generic;
using SDL2;

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
            LoadScripts();
        }

        bool stopped = false;

        public void Update(double deltaTime)
        {
            if (stopped) return;

            try
            {
                DisasterAPI.Input.UpdateMouse();
                
                updateFunction.Call(null, deltaTime);
            }
            catch (JavaScriptException e)
            {
                string message = $"path: {e.SourcePath}:{e.LineNumber} message: {e.Message}";
                Program.LoadingMessage(message, new Color32(255, 50, 0));
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