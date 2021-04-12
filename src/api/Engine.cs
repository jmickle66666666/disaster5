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


        static double _timescale = 1;
        [JSProperty(Name = "timescale")]
        public static double timescale
        {
            get
            {
                return _timescale;
            }
            set
            {
                _timescale = value < 0 ? 0 : value;
            }
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