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
        public void ToggleProfileGraph()
        {
            Disaster.Debug.enabled = !Disaster.Debug.enabled;
        }
    }
}
