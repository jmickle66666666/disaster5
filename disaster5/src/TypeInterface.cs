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
            if (input.TryGetPropertyValue("r", out object r)) { R = GetByteSafe(r); }
            if (input.TryGetPropertyValue("g", out object g)) { G = GetByteSafe(g); }
            if (input.TryGetPropertyValue("b", out object b)) { B = GetByteSafe(b); }
            if (input.TryGetPropertyValue("a", out object a)) { A = GetByteSafe(a); }
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

        public static Rect Rect(ObjectInstance input)
        {
            float X = 0, Y = 0, W = 0, H = 0;

            if (input.TryGetPropertyValue("x", out object x)) { X = GetFloat(x); };
            if (input.TryGetPropertyValue("y", out object y)) { Y = GetFloat(y); };
            if (input.TryGetPropertyValue("w", out object w)) { W = GetFloat(w); };
            if (input.TryGetPropertyValue("h", out object h)) { H = GetFloat(h); };

            return new Rect(X, Y, W, H);
        }

        public static Transform2D Transform2d(ObjectInstance input)
        {
            Vector2 origin = new Vector2(0, 0);
            Vector2 scale = new Vector2(1, 1);
            float rotation = 0f;

            if (input.TryGetPropertyValue("originX", out object ox)) { origin.X = GetFloat(ox); };
            if (input.TryGetPropertyValue("originY", out object oy)) { origin.Y = GetFloat(oy); };
            if (input.TryGetPropertyValue("scaleX", out object sx)) { scale.X = GetFloat(sx); };
            if (input.TryGetPropertyValue("scaleY", out object sy)) { scale.Y = GetFloat(sy); };
            if (input.TryGetPropertyValue("rotation", out object rot)) { rotation = GetFloat(rot); };

            return new Transform2D(origin, scale, rotation);
        }

        static byte GetByte(object value) {
            IConvertible val = (IConvertible) value;
            byte cast = val.ToByte(null);
            return cast;
        }

        static byte GetByteSafe(object value) {
            int val = GetInt(value);
            val = Math.Clamp(val, 0, 255);
            return (byte) val;
        }

        static int GetInt(object value) {
            IConvertible val = (IConvertible) value;
            int cast = val.ToInt32(null);
            return cast;
        }

        static float GetFloat(object value) {
            IConvertible val = (IConvertible) value;
            float cast = val.ToSingle(null);
            return cast;
        }
    }
}