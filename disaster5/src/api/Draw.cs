using OpenGL;
using Jurassic;
using Jurassic.Library;
using System.Numerics;

namespace DisasterAPI
{
    
    public class Draw : ObjectInstance{

        public Draw(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "loadFont")]
        [FunctionDescription("Load a font for the software renderer. All future Draw.text calls will use the specified font.")]
        [ArgumentDescription("fontPath", "Path to the font texture. Fonts are 2-color images where pixels with a red value above zero are considered filled.")]
        public static void LoadFont(string fontPath) {
            if (!Disaster.Assets.LoadPath(fontPath, out string fontAssetPath))
            {
                return;
            }
            Disaster.SoftwareCanvas.LoadFont(fontAssetPath);
        }

        [JSFunction(Name = "clear")]
        [FunctionDescription("Clear the 2D canvas.")]
        public static void Clear() {
            Disaster.SoftwareCanvas.Clear();
        }

        [JSProperty(Name = "fontHeight")] public static int fontHeight { get { return Disaster.SoftwareCanvas.fontHeight; } }
        [JSProperty(Name = "fontWidth")] public static int fontWidth { get { return Disaster.SoftwareCanvas.fontWidth; } }
        [JSProperty(Name = "screenWidth")] public static int screenWidth { get { return Disaster.ScreenController.screenWidth; } }
        [JSProperty(Name = "screenHeight")] public static int screenHeight { get { return Disaster.ScreenController.screenHeight; } }
        
        [JSFunction(Name = "setFog")]
        [FunctionDescription("Sets fog properties.")]
        [ArgumentDescription("color", "Fog color", "Color32 { r, g, b, a }")]
        [ArgumentDescription("fogStart", "Distance at which the fog starts")]
        [ArgumentDescription("fogDistance", "Distance after fog start when the fog will be 100% dense")]
        public static void SetFog(ObjectInstance color, double fogStart, double fogDistance)
        {
            var clr = Disaster.TypeInterface.Color32(color);
            Disaster.ObjRenderer.SetFogProperties(clr, (float)fogStart, (float)fogDistance);
        }
        
        [JSFunction(Name = "enableFog")]
        [FunctionDescription("Enable 3D fog. See also: setFog, disableFog")]
        public static void EnableFog()
        {
            Disaster.ObjRenderer.SetFogEnabled(true);
        }
        
        [JSFunction(Name = "disableFog")]
        [FunctionDescription("Disable 3D fog. See also: setFog, enableFog")]
        public static void DisableFog()
        {
            Disaster.ObjRenderer.SetFogEnabled(false);
        }
        
        [JSFunction(Name = "setClearColor")]
        public static void SetClearColor(ObjectInstance color, double fogStart, double fogDistance)
        {
            var clr = Disaster.TypeInterface.Color32(color);
            Disaster.ScreenController.SetClearColor(clr);
        }

        [JSFunction(Name = "offset")]
        [FunctionDescription("Set a global offset for 2D rendering.")]
        [ArgumentDescription("x", "Pixels in the x axis to offset by")]
        [ArgumentDescription("y", "Pixels in the y axis to offset by")]
        public static void Offset(int x, int y)
        {
            Disaster.SoftwareCanvas.offsetX = x;
            Disaster.SoftwareCanvas.offsetY = y;
        }


        [JSFunction(Name = "strokeRect")]
        [FunctionDescription("Draw a rectangle outline. See also: fillRect")]
        [ArgumentDescription("x", "x position of the rectangle")]
        [ArgumentDescription("y", "y position of the rectangle")]
        [ArgumentDescription("width", "width of the rectangle")]
        [ArgumentDescription("height", "height of the rectangle")]
        [ArgumentDescription("color", "Rectangle color", "Color32 { r, g, b, a }")]
        public static void StrokeRect(int x, int y, int width, int height, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.DrawRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
        }

        [JSFunction(Name = "fillRect")]
        [FunctionDescription("Draw a filled rectangle. See also: strokeRect")]
        [ArgumentDescription("x", "x position of the rectangle")]
        [ArgumentDescription("y", "y position of the rectangle")]
        [ArgumentDescription("width", "width of the rectangle")]
        [ArgumentDescription("height", "height of the rectangle")]
        [ArgumentDescription("color", "Rectangle color", "Color32 { r, g, b, a }")]
        public static void FillRect(int x, int y, int width, int height, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.FillRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
        }

        [JSFunction(Name ="line")]
        [FunctionDescription("Draw a 2d line.")]
        [ArgumentDescription("x1", "starting x position")]
        [ArgumentDescription("y1", "starting y position")]
        [ArgumentDescription("x2", "ending x position")]
        [ArgumentDescription("y2", "ending y position")]
        [ArgumentDescription("color", "line color", "Color32 { r, g, b, a }")]
        public static void Line(int x1, int y1, int x2, int y2, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(color));
        }

        //public static void Pixel(int x, int y, Color32 color) {
        //    Disaster.Draw.Pixel(x, y, color);
        //}

        [JSFunction(Name = "text")]
        [FunctionDescription("Draw a line of text.")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        [ArgumentDescription("text", "the text content to draw")]
        [ArgumentDescription("color", "text color", "Color32 { r, g, b, a }")]
        public static void Text(int x, int y, string text, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.Text(x, y, Disaster.TypeInterface.Color32(color), text);
        }

        [JSFunction(Name = "model")]
        [FunctionDescription("Draw a line of text.")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        [ArgumentDescription("text", "the text content to draw")]
        [ArgumentDescription("color", "text color", "Color32 { r, g, b, a }")]
        public static void Model(ObjectInstance position, ObjectInstance rotation, string modelPath, string texturePath)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            Matrix4x4 mat = Matrix4x4.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z) * Matrix4x4.CreateTranslation(pos);
            Matrix4 matrix = new Matrix4(new float[] { mat.M11, mat.M12, mat.M13, mat.M14, mat.M21, mat.M22, mat.M23, mat.M24, mat.M31, mat.M32, mat.M33, mat.M34, mat.M41, mat.M42, mat.M43, mat.M44 });
            var texture = Disaster.Assets.Texture(texturePath);
            var model = Disaster.Assets.ObjModel(modelPath);
            Disaster.ObjRenderer.EnqueueRender(model, Disaster.Assets.defaultShader, texture, matrix);
        }

        [JSFunction(Name = "texture")]
        [FunctionDescription("Draw an image to the software canvas.")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void Texture(int x, int y, string texturePath)
        {
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, Disaster.Transform2D.identity);
        }

        [JSFunction(Name = "textureTransformed")]
        [FunctionDescription("Draw an image to the software canvas, with scaling, rotation and origin offset.")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("transformation", "scaling, rotation and origin properties", "Transform2D { originX, originY, rotation, scaleX, scaleY }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TextureTransformed(int x, int y, ObjectInstance transformation, string texturePath)
        {
            var trans = Disaster.TypeInterface.Transform2d(transformation);
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, trans);
        }

        [JSFunction(Name = "texturePart")]
        [FunctionDescription("Draw a part of an image to the software canvas")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("rectangle", "rectangle defining the portion of the image to draw", "Rectangle { x, y, w, h }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TexturePart(int x, int y, ObjectInstance rectangle, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, rect);
        }

        [JSFunction(Name = "texturePartTransformed")]
        [FunctionDescription("Draw a part of an image to the software canvas, with transformations")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("rectangle", "rectangle defining the portion of the image to draw", "Rectangle { x, y, w, h }")]
        [ArgumentDescription("transformation", "scaling, rotation and origin properties", "Transform2D { originX, originY, rotation, scaleX, scaleY }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TexturePartTransformed(int x, int y, ObjectInstance rectangle, ObjectInstance transformation, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);
            var trans = Disaster.TypeInterface.Transform2d(transformation);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, rect, trans);
        }

        //public void TexturePart() {}
        // [JSFunction(Name = "reset")] public void Reset() {}
        // [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}


    }
}