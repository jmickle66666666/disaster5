using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    public class Assets : ObjectInstance
    {
        public Assets(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "list")]
        [FunctionDescription("Returns a list of the paths to all assets.")]
        public string[] List()
        {
            return Disaster.Assets.GetAllPaths();
        }

        [JSFunction(Name = "readText")]
        [FunctionDescription("Returns a string containing the text of the file at the given path.")]
        [ArgumentDescription("path", "Path of the asset to read")]
        public string ReadText(string path)
        {
            return Disaster.Assets.Text(path);
        }
    }
}
