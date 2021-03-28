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
            LoadConfig();
            LoadScripts();
        }

        public void Reset()
        {
            LoadConfig();
            LoadScripts();
        }

        public void Update(double deltaTime)
        {
            system.SetPropertyValue("deltaTime", deltaTime, false);
            
            updateFunction.Call(null);
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
                File.ReadAllText(Path.Combine(basedir, "main.js"))
            );

            updateFunction = engine.GetGlobalValue<Jurassic.Library.FunctionInstance>("update");
            system = engine.GetGlobalValue<Jurassic.Library.ObjectInstance>("System");
        }

        void LoadConfig()
        {
            string[] lines = File.ReadAllLines("disaster.cfg");

            foreach (var line in lines)
            {
                string[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "basedir":
                        if (tokens.Length != 2) {
                            Console.WriteLine($"Unexpected number of tokens: {line}");
                            break;
                        }
                        basedir = tokens[1];
                        break;
                }
            }

            Assets.basePath = basedir;
        }
    }

}