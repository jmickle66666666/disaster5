using Jurassic.Library;
using System;
namespace Disaster
{
    public class TypeInterface
    {
        public static Color32 Color32(ObjectInstance input)
        {
            byte R = 0, G = 0, B = 0;
            byte A = 255;
            if (input.TryGetPropertyValue("r", out object r)) { R = Convert.ToByte(Math.Clamp((int) r, 0, 255)); }
            if (input.TryGetPropertyValue("g", out object g)) { G = Convert.ToByte(Math.Clamp((int) g, 0, 255)); }
            if (input.TryGetPropertyValue("b", out object b)) { B = Convert.ToByte(Math.Clamp((int) b, 0, 255)); }
            if (input.TryGetPropertyValue("a", out object a)) { A = Convert.ToByte(Math.Clamp((int) a, 0, 255)); }
            return new Disaster.Color32(R, G, B, A);
        }
    }
}