using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    public class Texture : ObjectInstance
    {
        public Texture(ScriptEngine engine, string assetID) : base(engine)
        {
            this.pixelBuffer = Disaster.Assets.PixelBuffer(assetID);
            this.PopulateFunctions();
            this.SetPropertyValue("assetID", assetID, true);
            this.SetPropertyValue("width", pixelBuffer.width, true);
            this.SetPropertyValue("height", pixelBuffer.height, true);
            this.assetID = assetID;
        }

        private string assetID;
        Disaster.PixelBuffer pixelBuffer;

        [JSFunction(Name="getPixels")]
        [FunctionDescription("Get an array of pixels for the image", "{r, g, b, a}[]")]
        public ObjectInstance GetPixels()
        {
            return Disaster.TypeInterface.Object(pixelBuffer.pixels);
        }

        [JSFunction(Name ="setPixels")]
        [FunctionDescription("Set the pixels in the texture. Array length must be width * height.")]
        [ArgumentDescription("colors", "Color array defining the new pixels. Laid out horizontally, so the first N pixels are like. the top row", "{r, g, b, a}[]")]
        public void SetPixels(ArrayInstance colors)
        {
            pixelBuffer.SetPixels(Disaster.TypeInterface.Color32Array(colors));
            Disaster.Assets.pixelBuffers[assetID] = pixelBuffer;
        }

        [JSFunction(Name ="startBuffer")]
        [FunctionDescription("Start drawing to this texture instead of the screen. If you don't call endBuffer() afterwards things will break")]
        public void StartBuffer()
        {
            Disaster.SoftwareCanvas.StartBuffer(pixelBuffer);
        }

        [JSFunction(Name = "endBuffer")]
        [FunctionDescription("Stop drawing to the texture and update it")]
        public void EndBuffer()
        {
            pixelBuffer.SetPixels(Disaster.SoftwareCanvas.colorBuffer);
            Disaster.SoftwareCanvas.EndBuffer();
            Disaster.Assets.pixelBuffers[assetID] = pixelBuffer;
        }

        [JSFunction(Name = "save")]
        [FunctionDescription("Save the texture to a file")]
        [ArgumentDescription("path", "Path to save the texture to. i think you can pick the format but best stick to .png")]
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