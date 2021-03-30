using System.IO;
using System;
using Jurassic;
using System.Collections.Generic;

namespace Disaster {

    public class JS
    {
        public static JS instance;
        public string basedir;
        public ScriptEngine engine;
        public Dictionary<string, Jurassic.Library.GlobalObject> cachedScripts;
        public List<string> currentlyLoadingScripts;
        Jurassic.Library.FunctionInstance updateFunction;
        Jurassic.Library.ObjectInstance system;
        
        public JS()
        {
            instance = this;
            LoadScripts();


        }

        public void Reset()
        {
            LoadScripts();
        }

        public void Update(double deltaTime)
        {
            system.SetPropertyValue("deltaTime", deltaTime, false);

            //updateFunction.Call(null);
            engine.CallGlobalFunction("update");
            
        }

        public static void LoadStandardFunctions(ScriptEngine engine)
        {
            engine.SetGlobalFunction("load", new System.Func<string, object>((string path) => {
                return Assets.Script(path);
            }));

            engine.SetGlobalFunction("log", new Action<string>((string message) => { Console.WriteLine(message); }));
        }

        void LoadScripts()
        {
            cachedScripts = new Dictionary<string, Jurassic.Library.GlobalObject>();
            currentlyLoadingScripts = new List<string>();
            engine = new ScriptEngine();
            
            engine.SetGlobalValue("System", new DisasterAPI.System(engine));
            engine.SetGlobalValue("Draw", new DisasterAPI.Draw(engine));
            
            LoadStandardFunctions(engine);
        
            engine.Execute("var System = {}");
            engine.Execute(
                File.ReadAllText(Path.Combine(Assets.basePath, "main.js"))
            );

            updateFunction = engine.GetGlobalValue<Jurassic.Library.FunctionInstance>("update");

            system = engine.GetGlobalValue<Jurassic.Library.ObjectInstance>("System");
        }

    }

}