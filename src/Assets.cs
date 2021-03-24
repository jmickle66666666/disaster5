using System.Collections.Generic;
using Jurassic;
using Jurassic.Library;
using System.IO;
using System;

namespace Disaster 
{
    public class Assets {
        public static string basePath;

        public static Dictionary<string, ObjectInstance> scripts;
        public static List<string> currentlyLoadingScripts;

        public static string LoadPath(string path)
        {
            return Path.Combine(basePath, path);
        }

        public static ObjectInstance LoadScript(string path)
        {
            if (scripts == null) scripts = new Dictionary<string, ObjectInstance>();
            if (!scripts.ContainsKey(path)) {
                var scriptPath = LoadPath(path);

                if (currentlyLoadingScripts == null) currentlyLoadingScripts = new List<string>();

                if (currentlyLoadingScripts.Contains(scriptPath)) {
                    Console.WriteLine($"Circular dependency: {scriptPath}");
                    return null;
                }

                if (!File.Exists(scriptPath)) {
                    Console.WriteLine($"Cannot find script: {scriptPath}");
                    return null;
                }

                currentlyLoadingScripts.Add(scriptPath);

                var newEngine = new ScriptEngine();
                JS.LoadStandardFunctions(newEngine);
                newEngine.Execute(File.ReadAllText(scriptPath));
                scripts.Add(path, newEngine.Global);
                
                currentlyLoadingScripts.Remove(scriptPath);
            }

            return scripts[path];
        }
    }
}