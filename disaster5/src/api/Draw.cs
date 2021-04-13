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
        public static void LoadFont(string fontPath) {
            Disaster.Draw.LoadFont(Disaster.Assets.LoadPath(fontPath));
        }

        [JSFunction(Name = "clear")]
        public static void Clear() {
            Disaster.Draw.Clear();
        }

        [JSProperty(Name = "fontHeight")] public static int fontHeight { get { return Disaster.Draw.fontHeight; } }
        [JSProperty(Name = "fontWidth")] public static int fontWidth { get { return Disaster.Draw.fontWidth; } }
        [JSProperty(Name = "screenWidth")] public static int screenWidth { get { return Disaster.ScreenController.screenWidth; } }
        [JSProperty(Name = "screenHeight")] public static int screenHeight { get { return Disaster.ScreenController.screenHeight; } }
        
        [JSFunction(Name = "setFog")]
        public static void SetFog(ObjectInstance color, double fogStart, double fogDistance)
        {
            var clr = Disaster.TypeInterface.Color32(color);
            Disaster.ObjRenderer.SetFogProperties(clr, (float)fogStart, (float)fogDistance);
        }
        
        [JSFunction(Name = "enableFog")]
        public static void EnableFog()
        {
            Disaster.ObjRenderer.SetFogEnabled(true);
        }
        
        [JSFunction(Name = "disableFog")]
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
        public static void Offset(int x, int y)
        {
            Disaster.Draw.offsetX = x;
            Disaster.Draw.offsetY = y;
        }


        [JSFunction(Name = "strokeRect")]
        public static void StrokeRect(int x, int y, int width, int height, ObjectInstance color)
        {
            Disaster.Draw.DrawRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
        }

        [JSFunction(Name = "fillRect")]
        public static void FillRect(int x, int y, int width, int height, ObjectInstance color)
        {
            Disaster.Draw.FillRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
        }

        [JSFunction(Name ="line")]
        public static void Line(int x1, int y1, int x2, int y2, ObjectInstance color)
        {
            Disaster.Draw.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(color));
        }

        //public static void Pixel(int x, int y, Color32 color) {
        //    Disaster.Draw.Pixel(x, y, color);
        //}

        [JSFunction(Name = "text")]
        public static void Text(int x, int y, string text, ObjectInstance color)
        {
            Disaster.Draw.Text(x, y, Disaster.TypeInterface.Color32(color), text);
        }

        [JSFunction(Name = "model")]
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
        public static void Texture(int x, int y, string texturePath)
        {
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.Draw.PixelBuffer(pixelBuffer, x, y, Disaster.Transform2D.identity);
        }

        [JSFunction(Name = "textureTransformed")]
        public static void TextureTransformed(int x, int y, ObjectInstance transformation, string texturePath)
        {
            var trans = Disaster.TypeInterface.Transform2d(transformation);
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.Draw.PixelBuffer(pixelBuffer, x, y, trans);
        }

        [JSFunction(Name = "texturePart")]
        public static void TexturePart(int x, int y, ObjectInstance rectangle, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.Draw.PixelBuffer(pixelBuffer, x, y, rect, Disaster.Transform2D.identity);
        }

        [JSFunction(Name = "texturePartTransformed")]
        public static void TexturePartTransformed(int x, int y, ObjectInstance rectangle, ObjectInstance transformation, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);
            var trans = Disaster.TypeInterface.Transform2d(transformation);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.Draw.PixelBuffer(pixelBuffer, x, y, rect, trans);
        }

        //public void TexturePart() {}
        // [JSFunction(Name = "reset")] public void Reset() {}
        // [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}


    }
}