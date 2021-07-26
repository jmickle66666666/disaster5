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

        [JSFunction(Name = "exists")]
        [FunctionDescription("Check if asset exists")]
        [ArgumentDescription("path", "path for asset")]
        public bool Exists(string path)
        {
            return Disaster.Assets.LoadPath(path, out _);
        }

        [JSFunction(Name = "loaded")]
        [FunctionDescription("Check if asset is loaded")]
        [ArgumentDescription("path", "path for asset")]
        public bool Loaded(string path)
        {
            return Disaster.Assets.Loaded(path);
        }

        [JSFunction(Name = "unload")]
        [FunctionDescription("Unload an asset")]
        [ArgumentDescription("path", "path to the asset")]
        public void Unload(string path)
        {
            Disaster.Assets.Unload(path);
        }

        [JSFunction(Name = "unloadAll")]
        [FunctionDescription("Unload all assets")]
        public void UnloadAll()
        {
            Disaster.Assets.UnloadAll();
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
            string dir = System.IO.Path.GetDirectoryName(assetPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            System.IO.File.WriteAllText(assetPath, data);
        }

        [JSFunction(Name = "getTextureSize")]
        [FunctionDescription("Returns the size of a specified texture.", "{int w, int h}")]
        [ArgumentDescription("path", "Path of the texture")]
        public ObjectInstance GetTextureSize(string path)
        {
            var texture = Disaster.Assets.PixelBuffer(path);
            var output = Disaster.JS.instance.engine.Object.Construct();
            output["w"] = texture.width;
            output["h"] = texture.height;
            return output;
        }

        [JSFunction(Name = "createMesh")]
        [FunctionDescription("Creates a new mesh object and returns a reference for it.")]
        [ArgumentDescription("meshData", "Data to create the mesh with, all parameters other than vertices are optional.", "{vertices [, indices, uvs, uv2s, normals, tangents, colors]}")]
        public string CreateMesh(ObjectInstance meshData)
        {
            Raylib_cs.Model model = Disaster.TypeInterface.Model(meshData);
            
            int hash = model.GetHashCode();
            if (Disaster.Assets.models == null)
            {
                Disaster.Assets.models = new System.Collections.Generic.Dictionary<string, Raylib_cs.Model>();
            }
            while (Disaster.Assets.models.ContainsKey(hash.ToString())) {
                hash += 1;
            }
            Disaster.Assets.models.Add(hash.ToString(), model);
            return hash.ToString();
        }
    }
}
