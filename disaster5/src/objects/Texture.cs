using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    public class Texture : ObjectInstance
    {
        public Texture(ScriptEngine engine, string assetID) : base(engine)
        {
            //this.pixelBuffer = Disaster.Assets.PixelBuffer(assetID);
            this.PopulateFunctions();
            this.SetPropertyValue("assetID", assetID, true);
            this.SetPropertyValue("width", pixelBuffer.width, true);
            this.SetPropertyValue("height", pixelBuffer.height, true);
            this.assetID = assetID;
        }

        private string assetID;

        Disaster.PixelBuffer pixelBuffer { get { return Disaster.Assets.pixelBuffers[assetID]; } }

        [JSFunction(Name="getPixels")]
        public ObjectInstance GetPixels()
        {
            return Disaster.TypeInterface.Object(pixelBuffer.pixels);
        }

        [JSFunction(Name ="setPixels")]
        public void SetPixels(ArrayInstance colors)
        {
            pixelBuffer.SetPixels(Disaster.TypeInterface.Color32Array(colors));
        }

        [JSFunction(Name ="startBuffer")]
        public void StartBuffer()
        {
            Disaster.SoftwareCanvas.StartBuffer(pixelBuffer);
        }

        [JSFunction(Name = "endBuffer")]
        public void EndBuffer()
        {
            pixelBuffer.SetPixels(Disaster.SoftwareCanvas.colorBuffer);
            Disaster.SoftwareCanvas.EndBuffer();
        }

        [JSFunction(Name = "save")]
        public void Save(string path)
        {
            Disaster.Assets.LoadPath(path, out string assetPath);
            string dir = System.IO.Path.GetDirectoryName(assetPath);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            Raylib_cs.Raylib.ExportImage(Raylib_cs.Raylib.GetTextureData(pixelBuffer.texture), assetPath);
        }

    }
}