using System;
using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    [ClassDescription("The object returned by Assets.getTexture(path). Can be used to modify existing assets.")]
    public class Texture : ObjectInstance
    {
        public Texture(ScriptEngine engine, string assetID) : base(engine)
        {
            this.pixelBuffer = Disaster.Assets.PixelBuffer(assetID).pixelBuffer;
            this.PopulateFunctions();
            this.SetPropertyValue("assetID", assetID, true);
            this.SetPropertyValue("width", pixelBuffer.width, true);
            this.SetPropertyValue("height", pixelBuffer.height, true);
            this._assetID = assetID;
        }

        private string _assetID;
        Disaster.PixelBuffer pixelBuffer;

        [JSProperty(Name = "assetID")]
        [PropertyDescription("the asset path for the texture object. use this to draw it")]
        public string ignored { get { return this._assetID; } set { this._assetID = value; } }

        [JSFunction(Name="getPixel")]
        [FunctionDescription("Get a single pixel", "{r, g, b, a}")]
        [ArgumentDescription("x", "x coordinate of the pixel")]
        [ArgumentDescription("y", "y coordinate of the pixel")]
        public ObjectInstance GetPixel(int x, int y)
        {
            // TODO: Check that (x,y) is within the pixelbuffer
            return Disaster.TypeInterface.Object(pixelBuffer.pixels[y * pixelBuffer.width + x]);
        }

        [JSFunction(Name="getPixels")]
        [FunctionDescription("Get an array of pixels for the image", "{r, g, b, a}[]")]
        public ObjectInstance GetPixels()
        {
            // TODO: Add option to define the rect of pixels you want
            return Disaster.TypeInterface.Object(pixelBuffer.pixels);
        }

        [JSFunction(Name ="setPixels")]
        [FunctionDescription("Set the pixels in the texture. Array length must be width * height.")]
        [ArgumentDescription("colors", "Color array defining the new pixels. Laid out horizontally, so the first N pixels are like. the top row", "{r, g, b, a}[]")]
        public void SetPixels(ArrayInstance colors)
        {
            pixelBuffer.SetPixels(Disaster.TypeInterface.Color32Array(colors));
            Disaster.Assets.pixelBuffers[_assetID] = pixelBuffer;
        }

        [JSFunction(Name ="startBuffer")]
        [FunctionDescription("Start drawing to this texture instead of the screen. If you don't call endBuffer() afterwards things will break")]
        public void StartBuffer()
        {
            if (!Disaster.BufferRenderer.StartBuffer(_assetID))
                Console.WriteLine("In Buffer!!");
        }

        [JSFunction(Name = "endBuffer")]
        [FunctionDescription("Stop drawing to the texture and update it")]
        public void EndBuffer()
        {
            // AssetID *shouldn't* ever change but just in case
            _assetID = Disaster.BufferRenderer.EndBuffer();
            pixelBuffer = Disaster.Assets.PixelBuffer(_assetID).pixelBuffer;
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