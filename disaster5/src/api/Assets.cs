using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    [ClassDescription("Functions for managing assets.")]
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
            return Disaster.Assets.PathExists(path);
        }

        [JSFunction(Name = "loaded")]
        [FunctionDescription("Check if asset is loaded")]
        [ArgumentDescription("path", "path for asset")]
        public bool Loaded(string path)
        {
            return Disaster.Assets.Loaded(path);
        }

        [JSFunction(Name = "loadedAssetCount")]
        [FunctionDescription("Get the number of loaded assets")]
        public int LoadedAssetCount()
        {
            return Disaster.Assets.TotalLoaded();
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
        [FunctionDescription("Returns a list of the paths to all assets", "String[]")]
        public ObjectInstance List()
        {
            return Disaster.TypeInterface.Object(Disaster.Assets.GetAllPaths());
        }

        [JSFunction(Name = "readText")]
        [FunctionDescription("Returns a string containing the text of the file at the given path.")]
        [ArgumentDescription("path", "Path of the asset to read")]
        public string ReadText(string path)
        {
            var output = Disaster.Assets.Text(path);
            if (output.succeeded) return output.text;
            return "";
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
        [FunctionDescription("Returns the size of a specified texture.", "{w, h}")]
        [ArgumentDescription("path", "Path of the texture")]
        public ObjectInstance GetTextureSize(string path)
        {
            var texture = Disaster.Assets.PixelBuffer(path);
            if (texture.succeeded)
            {
                var output = Disaster.JS.instance.engine.Object.Construct();
                output["w"] = texture.pixelBuffer.width;
                output["h"] = texture.pixelBuffer.height;
                return output;
            } else
            {
                System.Console.WriteLine($"No texture: {path}");
                var output = Disaster.JS.instance.engine.Object.Construct();
                output["w"] = 0;
                output["h"] = 0;
                return output;
            }
        }

        [JSFunction(Name ="getTexture")]
        [FunctionDescription("Returns a Texture object from the given asset path. The Texture section below outlines the texture API")]
        [ArgumentDescription("path", "Path of the texture")]
        public Texture GetTexture(string path)
        {   
            if (!Disaster.Assets.PixelBuffer(path).succeeded)
            {
                System.Console.WriteLine($"No texture: {path}");
                return null;
            }
            return new Texture(Disaster.JS.instance.engine, path);
        }

        [JSFunction(Name = "createTexture")]
        [FunctionDescription("Create a new, blank texture object.")]
        [ArgumentDescription("width", "width of the texture, in pixels")]
        [ArgumentDescription("height", "height of the texture, in pixels")]
        public Texture CreateTexture(int width, int height)
        {
            Disaster.BufferRenderer.StartBuffer(width, height);
            var asset = Disaster.BufferRenderer.EndBuffer();
            return GetTexture(asset);
        }

        [JSFunction(Name = "createMesh")]
        [FunctionDescription("Creates a new mesh object and returns a reference for it.")]
        [ArgumentDescription("meshData", "Data to create the mesh with, all parameters other than vertices are optional.", "{vertices, indices?, uvs?, uv2s?, normals?, tangents?, colors?, texture?}")]
        public string CreateMesh(ObjectInstance meshData)
        {
            Raylib_cs.Model model = Disaster.TypeInterface.Model(meshData);
            
            int hash = model.GetHashCode();
            while (Disaster.Assets.models.ContainsKey(hash.ToString())) {
                hash += 1;
            }
            Disaster.Assets.models.Add(hash.ToString(), model);
            return hash.ToString();
        }

        [JSFunction(Name = "createVoxelMesh")]
        [FunctionDescription("Creates a new mesh object from voxel data returns a reference for it.")]
        [ArgumentDescription("chunkSize", "Dimensions of voxel area", "{x, y, z}")]
        [ArgumentDescription("texturePath", "Texture to use for the voxels. This should be a map of smaller textures, like a tileset.")]
        [ArgumentDescription("textureSize", "How many individual textures along each axis of your texture map", "{x, y}")]
        [ArgumentDescription("data", "Integer array defining the voxels. -1 means empty, any other number specifies a texture in the texture map", "int[]")]
        
        public string CreateVoxelMesh(ObjectInstance chunkSize, string texturePath, ObjectInstance textureSize, ObjectInstance data)
        {
            var voxelModel = Disaster.VoxelMeshGenerator.Generate(
                (Disaster.Vector3Int)Disaster.TypeInterface.Vector3(chunkSize),
                Disaster.Assets.PixelBuffer(texturePath).pixelBuffer.texture,
                (Disaster.Vector2Int)Disaster.TypeInterface.Vector2(textureSize),
                Disaster.TypeInterface.IntArray(data),
                new Disaster.MultiTextureVoxel[] { }
            );

            int hash = voxelModel.GetHashCode();
            while (Disaster.Assets.models.ContainsKey(hash.ToString()))
            {
                hash += 1;
            }
            Disaster.Assets.models.Add(hash.ToString(), voxelModel);
            return hash.ToString();
        }

        //[JSFunction(Name = "getMeshData")]
        //[FunctionDescription("Returns an object containing a meshes data.")]
        //[ArgumentDescription("meshReference", "Path to the mesh to retrieve")]
        //public ObjectInstance GetMeshData(string meshReference)
        //{

        //}
    }
}
