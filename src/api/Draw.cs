// js api stuff
using Jurassic;
using Jurassic.Library;
using OpenGL;

namespace DisasterAPI
{
    public class Draw : ObjectInstance {
        public Draw(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "loadFont")] public void LoadFont(string fontPath) {
            Disaster.Draw.LoadFont(fontPath);
        }

        [JSFunction(Name = "clear")] public void Clear() {
            Disaster.Draw.Clear();
        }

        [JSFunction(Name = "strokeRect")] public void StrokeRect(int x, int y, int width, int height, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.DrawRect(x, y, width, height, color32);
        }

        [JSFunction(Name = "fillRect")] public void FillRect(int x, int y, int width, int height, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.FillRect(x, y, width, height, color32);
        }

        [JSFunction(Name = "line")] public void Line(int x1, int y1, int x2, int y2, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.Line(x1, y1, x2, y2, color32);
        }

        [JSFunction(Name = "pixel")] public void Pixel(int x, int y, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.Pixel(x, y, color32);
        }

        [JSFunction(Name = "text")] public void Text(int x, int y, string text, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.Text(x, y, color32, text);
        }

        [JSFunction(Name = "model")] public void Model(ObjectInstance position, ObjectInstance rotation, string modelPath, string texturePath)
        {
            var pos = Disaster.TypeInterface.Vector3(position);
            var rot = Disaster.TypeInterface.Vector3(rotation);
            Matrix4 matrix = 
                Matrix4.CreateRotationX(rot.X) *
                Matrix4.CreateRotationY(rot.Y) *
                Matrix4.CreateRotationZ(rot.Z) *
                Matrix4.CreateTranslation(pos);
            var texture = Disaster.Assets.Texture(texturePath);
            var model = Disaster.Assets.ObjModel(modelPath);
            Disaster.ObjRenderer.EnqueueRender(model, Disaster.Assets.defaultShader, texture, matrix);
        }



        [JSFunction(Name = "texturePart")] public void TexturePart() {}
        // [JSFunction(Name = "reset")] public void Reset() {}
        // [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}
    }
}