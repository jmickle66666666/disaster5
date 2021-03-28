using Jurassic.Library;
using System;
using System.Numerics;
namespace Disaster
{
    /**
        this is for converting objects (safely?) from js to c#
    */
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

        public static Vector3 Vector3(ObjectInstance input)
        {
            float X = 0, Y = 0, Z = 0;

            if (input.TryGetPropertyValue("x", out object x)) { X = GetFloat(x); }
            if (input.TryGetPropertyValue("y", out object y)) { Y = GetFloat(y); }
            if (input.TryGetPropertyValue("z", out object z)) { Z = GetFloat(z); }
            
            return new System.Numerics.Vector3(X, Y ,Z);
        }

        static float GetFloat(object value) {
            IConvertible val = (IConvertible) value;
            float cast = val.ToSingle(null);
            return cast;
        }
    }
}