using System;
using System.Collections.Generic;
using System.Text;
using Jurassic.Library;
using Jurassic;

namespace DisasterAPI
{
    [ClassDescription("Various debugging tools.")]
    class Debug : ObjectInstance
    {
        public Debug(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "toggleProfileGraph")]
        [FunctionDescription("Toggle the debug profiler graph. This shows how long each frame took to compute.")]
        public void ToggleProfileGraph()
        {
            Disaster.Debug.enabled = !Disaster.Debug.enabled;
        }

        [JSFunction(Name = "log")]
        [FunctionDescription("Print a message to the system console")]
        [ArgumentDescription("message", "The message to print")]
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        [JSProperty(Name = "scriptTime")]
        [PropertyDescription("Time taken to run all the scripts last frame, in seconds! 0.016s == 60fps")]
        public static double scriptTime { get { return Disaster.Program.scriptTime;  } }

        [JSFunction(Name = "ilAnalysis")]
        [FunctionDescription("Return the disassembled IL of a script after compilation. Useful for debugging hopefully")]
        [ArgumentDescription("sourcePath", "path to the script to analyze")]
        public string ILAnalysis(string sourcePath)
        {
            var options = new Jurassic.Compiler.CompilerOptions();
            options.EnableILAnalysis = true;

            var text = Disaster.Assets.Text(sourcePath);
            if (text.succeeded)
            {
                var script = CompiledScript.Compile(new StringScriptSource(text.text), options);
                return script.DisassembledIL;
            }

            return "";
        }
    }
}
