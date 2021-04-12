using System;
using System.Collections.Generic;
using System.Text;
using Jurassic.Library;
using Jurassic;

namespace DisasterAPI
{
    class Engine : ObjectInstance
    {
        public Engine(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "reset")]
        public void Reset()
        {
            Disaster.Assets.UnloadAll();
            Disaster.JS.instance.Reset();
        }

        [JSFunction(Name = "quit")]
        public void Quit()
        {
            Disaster.Program.running = false;
        }
    }
}