// js api stuff
using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    public class System : ObjectInstance {
        public System(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "unlockMouse")] public void UnlockMouse() {}
        [JSFunction(Name = "reset")] public void Reset() {}
        [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}
    }
}