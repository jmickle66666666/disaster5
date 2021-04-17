using System;
using System.Collections.Generic;
using System.Text;
using Jurassic.Library;
using Jurassic;

namespace DisasterAPI
{
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
    }
}
