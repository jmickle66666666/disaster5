using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    public class Audio : ObjectInstance {
        public Audio(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "play")] public void Play(string audioPath)
        {
            
        }
    }
}