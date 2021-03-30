using OpenGL;
using System.Numerics;
namespace DisasterEngine
{
    
    public class Draw{

        public static void LoadFont(string fontPath) {
            Disaster.Draw.LoadFont(Disaster.Assets.LoadPath(fontPath));
        }

        public static void Clear() {
            Disaster.Draw.Clear();
        }

        public static void StrokeRect(int x, int y, int width, int height, Color32 color) {
            Disaster.Draw.DrawRect(x, y, width, height, color);
        }

        public static void FillRect(int x, int y, int width, int height, Color32 color) {
            Disaster.Draw.FillRect(x, y, width, height, color);
        }

        public static void Line(int x1, int y1, int x2, int y2, Color32 color) {
            Disaster.Draw.Line(x1, y1, x2, y2, color);
        }

        public static void Pixel(int x, int y, Color32 color) {
            Disaster.Draw.Pixel(x, y, color);
        }

        public static void Text(int x, int y, string text, Color32 color)
        {
            Disaster.Draw.Text(x, y, color, text);
        }

        public static void Model(Vector3 position, Vector3 rotation, string modelPath, string texturePath)
        {
            Matrix4x4 mat = Matrix4x4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4x4.CreateTranslation(position);
            Matrix4 matrix = new Matrix4(new float[] { mat.M11, mat.M12, mat.M13, mat.M14, mat.M21, mat.M22, mat.M23, mat.M24, mat.M31, mat.M32, mat.M33, mat.M34, mat.M41, mat.M42, mat.M43, mat.M44 });
            var texture = Disaster.Assets.Texture(texturePath);
            var model = Disaster.Assets.ObjModel(modelPath);
            Disaster.ObjRenderer.EnqueueRender(model, Disaster.Assets.defaultShader, texture, matrix);
        }

        public void TexturePart() {}
        // [JSFunction(Name = "reset")] public void Reset() {}
        // [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}

        
    }
}