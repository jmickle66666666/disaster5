using System.IO;
using System;
using Jurassic;
using System.Collections.Generic;
using Raylib_cs;
using System.Linq;

namespace Disaster {

    public class JS
    {
        public static JS instance;
        public string basedir;
        public ScriptEngine engine;
        public Dictionary<string, Jurassic.Library.GlobalObject> cachedScripts;
        public List<string> currentlyLoadingScripts;
        Jurassic.Library.FunctionInstance updateFunction;

        public static KeyboardKey resetKey = KeyboardKey.KEY_F2;
        public JS()
        {
            instance = this;
            LoadScripts();
        }

        public void Reset()
        {
            Assets.UnloadAll();
            Assets.assignedDefaultShader = false;
            ScreenController.instance.ReloadShader();
            ScreenController.camera.position = System.Numerics.Vector3.Zero;
            ScreenController.camera.target = -System.Numerics.Vector3.UnitZ;
            AudioController.StopMusic();
            AudioController.StopAllSound();
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
                    Reset();
                    stopped = false;
                }
                return;
            }

            try
            {
                if (Raylib.IsKeyPressed(resetKey))
                {
                    Assets.UnloadAll();
                    Assets.assignedDefaultShader = false;
                    ScreenController.instance.ReloadShader();
                    Reset();
                } else
                {
                    DisasterAPI.Input.Update();
                
                    if (updateFunction != null)
                    {
                        updateFunction.Call(engine.Global, deltaTime);
                    }
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
            
        }

        void ShowException(Exception e)
        {
            TextController.LoadDefaultFont();

            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Source);

            string message = $"{e.Message}\nPress R to restart";
            if (e is JavaScriptException)
            {
                message = $"line:{((JavaScriptException)e).LineNumber} {e.Message}\nPress R to restart";
            }

            int x = 32;
            int y = 32;
            int chars = 32;
            int lines = ((message.Length / chars) + message.Count(f => f == '\n')) + 2;

            ShapeRenderer.EnqueueRender(() =>
            {
                Raylib.DrawRectangle(x - 4, y - 4, chars * TextController.fontWidth + 8, lines * TextController.fontHeight + 8, new Color32(255, 180, 0));
                
                for (int i = x - 4; i < x + chars * TextController.fontWidth + 4; i++)
                {
                    for (int j = y - 4; j < y + lines * TextController.fontHeight + 4; j++)
                    {
                        if ((i + j) % 6 < 3)
                        {
                            Raylib.DrawPixel(i, j, Color.BLACK);
                        }
                    }
                }
                
                Raylib.DrawRectangle(x - 1, y - 1, chars * TextController.fontWidth + 2, lines * TextController.fontHeight + 2, new Color32(12, 12, 12));
            });

            TextController.Paragraph(x, y, new Color32(255, 50, 0), message, chars);
            stopped = true;
        }

        public static void LoadStandardFunctions(ScriptEngine engine)
        {
            engine.SetGlobalFunction("load", new System.Func<string, object>((string path) => {
                return Assets.Script(path).script;
            }));

            engine.SetGlobalFunction("create", new Func<string, object>((string path) =>
            {
                return Assets.LoadScript(path).script;
            }));

            engine.SetGlobalFunction("log", new Action<string>((string message) => { Console.WriteLine(message); }));

            engine.SetGlobalValue("Draw", new DisasterAPI.Draw(engine));
            engine.SetGlobalValue("Input", new DisasterAPI.Input(engine));
            engine.SetGlobalValue("Audio", new DisasterAPI.Audio(engine));
            engine.SetGlobalValue("Debug", new DisasterAPI.Debug(engine));
            engine.SetGlobalValue("Engine", new DisasterAPI.Engine(engine));
            engine.SetGlobalValue("Assets", new DisasterAPI.Assets(engine));
            engine.SetGlobalValue("Physics", new DisasterAPI.Physics(engine));
            engine.SetGlobalValue("Key", new DisasterAPI.Key(engine));
            engine.SetGlobalValue("Gamepad", new DisasterAPI.Gamepad(engine));
            engine.SetGlobalValue("Color", new DisasterAPI.Color(engine));
            engine.SetGlobalValue("VMath", new DisasterAPI.VMath(engine));
        }

        void LoadScripts()
        {
            cachedScripts = new Dictionary<string, Jurassic.Library.GlobalObject>();
            currentlyLoadingScripts = new List<string>();
            engine = new ScriptEngine();

            LoadStandardFunctions(engine);

            if (Assets.PathExists("main.js")) { 
                try
                {
                    engine.Execute(
                        File.ReadAllText(Path.Combine(Assets.basePath, "main.js"))
                    );
                    updateFunction = engine.GetGlobalValue<Jurassic.Library.FunctionInstance>("update");
                } catch (Exception e)
                {
                    ShowException(e);
                    //Program.LoadingMessage($"{e.Message}");
                    //Console.WriteLine(e.StackTrace);
                }
            } 
            else
            {
                // load default script here
                try
                {
                    engine.Execute(defaultScript);
                }
                catch (Exception e)
                {
                    ShowException(e);
                    //Program.LoadingMessage($"{e.Message}");
                    //Console.WriteLine(e.StackTrace);
                }
                updateFunction = engine.GetGlobalValue<Jurassic.Library.FunctionInstance>("update");
            }

        }

        static string defaultScript = @"
var x = 10;
var y = 0;
var xmov = 1;
var ymov = 1;
var speed = 35;
Draw.clear();
function update(dt) {
    Draw.text('no data loaded!', x, y, {r:Math.floor(Math.random()*255), g:Math.floor(Math.random()*255), b:Math.floor(Math.random()*255)});
    x += dt * speed * xmov;
    y += dt * speed * ymov;
if (x > 232) xmov = -1;
if (x < 0) xmov = 1;
if (y > 232) ymov = -1;
if (y < 0) ymov = 1;
}
";

    }

}