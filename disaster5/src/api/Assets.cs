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
        [FunctionDescription("Returns a list of the paths to all assets, delimited with a comma `,`")]
        public string List()
        {
            return string.Join(',', Disaster.Assets.GetAllPaths());
        }

        [JSFunction(Name = "readText")]
        [FunctionDescription("Returns a string containing the text of the file at the given path.")]
        [ArgumentDescription("path", "Path of the asset to read")]
        public string ReadText(string path)
        {
            return Disaster.Assets.Text(path);
        }

        [JSFunction(Name = "writeText")]
        [FunctionDescription("Creates or overwrites a file in the assets directory.")]
        [ArgumentDescription("path", "Path of the asset to create or overwrite")]
        [ArgumentDescription("data", "A string containing the data to write")]
        public void WriteText(string path, string data)
        {
            Disaster.Assets.LoadPath(path, out string assetPath);
            System.IO.File.WriteAllText(assetPath, data);
        }
    }
}
