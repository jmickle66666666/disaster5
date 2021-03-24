// js api stuff
using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    public class Draw : ObjectInstance {
        public Draw(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "clear")] public void Clear() {
            Disaster.Draw.Clear();
        }

        [JSFunction(Name = "rect")] public void Rect(int x, int y, int width, int height, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.FillRect(x, y, width, height, color32);
        }

        [JSFunction(Name = "text")] public void Text(int x, int y, string text, ObjectInstance color) {
            var color32 = Disaster.TypeInterface.Color32(color);
            Disaster.Draw.Text(x, y, color32, text);
        }

        [JSFunction(Name = "texturePart")] public void TexturePart() {}
        // [JSFunction(Name = "reset")] public void Reset() {}
        // [JSFunction(Name = "fogColor")] public void FogColor(double r, double g, double b) {}
    }
}