using Jurassic.Library;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Raylib_cs;

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
            return new Color32(R, G, B, A);
        }

        public static Vector4 Vector4(ObjectInstance input)
        {
            float X = 0, Y = 0, Z = 0, W = 0;

            if (input.TryGetPropertyValue("x", out object x)) { X = GetFloat(x); }
            if (input.TryGetPropertyValue("y", out object y)) { Y = GetFloat(y); }
            if (input.TryGetPropertyValue("z", out object z)) { Z = GetFloat(z); }
            if (input.TryGetPropertyValue("w", out object w)) { W = GetFloat(w); }

            return new Vector4(X, Y, Z, W);
        }

        public static ObjectInstance Object(Vector4 input)
        {
            var output = JS.instance.engine.Object.Construct();
            output.SetPropertyValue("x", (double)input.X, true);
            output.SetPropertyValue("y", (double)input.Y, true);
            output.SetPropertyValue("z", (double)input.Z, true);
            output.SetPropertyValue("w", (double)input.W, true);
            return output;
        }

        public static Vector3 Vector3(ObjectInstance input)
        {
            float X = 0, Y = 0, Z = 0;

            if (input.TryGetPropertyValue("x", out object x)) { X = GetFloat(x); }
            if (input.TryGetPropertyValue("y", out object y)) { Y = GetFloat(y); }
            if (input.TryGetPropertyValue("z", out object z)) { Z = GetFloat(z); }
            
            return new Vector3(X, Y ,Z);
        }

        public static ObjectInstance Object(Vector3 input)
        {
            var output = JS.instance.engine.Object.Construct();
            output.SetPropertyValue("x", (double) input.X, true);
            output.SetPropertyValue("y", (double) input.Y, true);
            output.SetPropertyValue("z", (double) input.Z, true);
            return output;
        }

        public static Vector2 Vector2(ObjectInstance input)
        {
            float X = 0, Y = 0;

            if (input.TryGetPropertyValue("x", out object x)) { X = GetFloat(x); }
            if (input.TryGetPropertyValue("y", out object y)) { Y = GetFloat(y); }

            return new Vector2(X, Y);
        }

        public static ObjectInstance Object(Vector2 input)
        {
            var output = JS.instance.engine.Object.Construct();
            output["x"] = (double)input.X;
            output["y"] = (double)input.Y;
            return output;
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

        public static Plane Plane(ObjectInstance input)
        {
            Vector3 position = Vector3((ObjectInstance) input.GetPropertyValue("position"));
            Vector3 normal = Vector3((ObjectInstance) input.GetPropertyValue("normal"));
            return Util.CreatePlaneFromPositionNormal(position, normal);
        }

        public static Raylib_cs.Ray Ray(ObjectInstance input)
        {
            Vector3 position = new Vector3();
            Vector3 direction = new Vector3();
            if (input.TryGetPropertyValue("position", out object pos)) { position = Vector3((ObjectInstance)pos); }
            if (input.TryGetPropertyValue("direction", out object dir)) { direction = Vector3((ObjectInstance)dir); }
            return new Ray(
                position, direction
            );
        }

        public static ObjectInstance Object(Ray input)
        {
            var output = JS.instance.engine.Object.Construct();
            output.SetPropertyValue("position", Object(input.position), true);
            output.SetPropertyValue("direction", Object(input.direction), true);
            return output;
        }

        public static ObjectInstance Object(RayHitInfo input)
        {
            var output = JS.instance.engine.Object.Construct();
            output["hit"] = input.hit != 0;
            output["distance"] = (double) input.distance;
            output["position"] = Object(input.position);
            output["normal"] = Object(input.normal);
            return output;
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

        public static ShaderParameter[] ShaderParameters(ObjectInstance input)
        {
            List<ShaderParameter> output = new List<ShaderParameter>();
            foreach (var value in input.Properties)
            {
                ShaderUniformDataType type = ShaderUniformDataType.SHADER_UNIFORM_FLOAT;
                object property = value.Value;
                switch (value.Value)
                {
                    case double _:
                        type = ShaderUniformDataType.SHADER_UNIFORM_FLOAT;
                        property = (float) ((double) property);
                        break;
                    case int _:
                        type = ShaderUniformDataType.SHADER_UNIFORM_FLOAT;
                        property = (float)((int)property);
                        break;
                    case ObjectInstance _:
                        type = ShaderUniformDataType.SHADER_UNIFORM_VEC4;
                        var vec4 = Vector4((ObjectInstance) property);
                        var floatsize = Marshal.SizeOf(typeof(float));
                        var ptr = Marshal.AllocHGlobal(floatsize * 4);
                        Marshal.StructureToPtr(vec4, ptr, false);
                        property = ptr;
                        break;
                    case string _:
                        type = ShaderUniformDataType.SHADER_UNIFORM_SAMPLER2D;
                        property = Assets.PixelBuffer((string)property).texture;
                        break;
                }

                output.Add(new ShaderParameter(
                    (string) value.Key,
                    property,
                    type
                ));
            }
            
            return output.ToArray();
        }

        //public unsafe static ObjectInstance Object(Model model)
        //{
            //var mesh = ((Mesh*)model.meshes)[0];
            //var output = JS.instance.engine.Object.Construct();
            //output.SetPropertyValue("position", Object(model.position), true);
            //output.SetPropertyValue("direction", Object(model.direction), true);
            //return output;
        //}

        public static Model Model(ObjectInstance input)
        {
            Mesh output = new Mesh();
            
            // necessary stuff
            var vertices = Vector3Array((ObjectInstance)input.GetPropertyValue("vertices"));

            output.vertexCount = vertices.Length;
            output.vertices = ArrayPointer(vertices);
            
            // optionals
            if (input.HasProperty("indices"))
            {
                var indices = ShortArray((ObjectInstance)input.GetPropertyValue("indices"));
                output.triangleCount = indices.Length / 3;
                output.indices = ArrayPointer(indices);
            }

            if (input.TryGetPropertyValue("uvs", out object uvs)) { output.texcoords = ArrayPointer(Vector2Array((ObjectInstance)uvs)); }
            if (input.TryGetPropertyValue("uv2s", out object uv2s)) { output.texcoords2 = ArrayPointer(Vector2Array((ObjectInstance)uv2s)); }
            if (input.TryGetPropertyValue("normals", out object normals)) { output.normals = ArrayPointer(Vector3Array((ObjectInstance)normals)); }
            if (input.TryGetPropertyValue("tangents", out object tangents)) { output.tangents = ArrayPointer(Vector3Array((ObjectInstance)tangents)); }
            if (input.TryGetPropertyValue("colors", out object colors)) { output.colors = ArrayPointer(Color32Array((ObjectInstance)colors)); }

            Raylib.UploadMesh(ref output, false);

            Model model = Raylib.LoadModelFromMesh(output);

            if (input.TryGetPropertyValue("texture", out object texturePath))
            {
                Texture2D texture;
                if (!Assets.LoadPath((string)texturePath, out _))
                {
                    texture = PixelBuffer.missing.texture;
                } else
                {
                    texture = Assets.PixelBuffer((string)texturePath).texture;
                }
                SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_DIFFUSE, ref texture);
            } else
            {
                var texture = PixelBuffer.missing.texture;
                SetMaterialTexture(ref model, 0, MaterialMapIndex.MATERIAL_MAP_DIFFUSE, ref texture);
            }
            
            return model;
        }

        public unsafe static void SetMaterialTexture(ref Model model, int materialIndex, MaterialMapIndex mapIndex, ref Texture2D texture)
        {
            Material* materials = (Material*)model.materials.ToPointer();
            Raylib.SetMaterialTexture(ref materials[materialIndex], (int)mapIndex, texture);
        }

        public static IntPtr ArrayPointer<T>(T[] array) where T : unmanaged
        {
            int length = Marshal.SizeOf(typeof(T)) * array.Length;
            
            IntPtr output = Marshal.AllocHGlobal(length);

            unsafe
            {
                array.AsSpan().CopyTo(new Span<T>((void*)output, length));
            }

            return output;
        }

        public static short[] ShortArray(ObjectInstance input)
        {
            short[] output = new short[(uint)input.GetPropertyValue("length")];
            for (uint i = 0; i < output.Length; i++)
            {
                output[i] = (short)GetInt(input.GetPropertyValue(i));
            }
            return output;
        }

        public static int[] IntArray(ObjectInstance input)
        {
            int[] output = new int[(uint) input.GetPropertyValue("length")];
            for (var i = 0; i < output.Length; i++)
            {
                output[i] = (int) input.GetPropertyValue((uint) i);
            }
            return output;
        }

        public static Vector3[] Vector3Array(ObjectInstance input)
        {
            var length = (uint)input.GetPropertyValue("length");
            Vector3[] output = new Vector3[length];
            for (var i = 0; i < output.Length; i++)
            {
                output[i] = Vector3((ObjectInstance) input.GetPropertyValue((uint) i));
            }
            return output;
        }

        public static Vector2[] Vector2Array(ObjectInstance input)
        {
            Vector2[] output = new Vector2[(uint)input.GetPropertyValue("length")];
            for (var i = 0; i < output.Length; i++)
            {
                output[i] = Vector2((ObjectInstance)input.GetPropertyValue((uint) i));
            }
            return output;
        }

        public static Color32[] Color32Array(ObjectInstance input)
        {
            Color32[] output = new Color32[(uint)input.GetPropertyValue("length")];
            for (var i = 0; i < output.Length; i++)
            {
                output[i] = Color32((ObjectInstance)input.GetPropertyValue((uint)i));
            }
            return output;
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
            // sorry about this
            try
            {
                if (value.GetType() == typeof(Jurassic.ConcatenatedString))
                {
                    return int.Parse(((Jurassic.ConcatenatedString) value).ToString());
                }

                IConvertible val = (IConvertible)value;
                int cast = val.ToInt32(null);
                return cast;

            } 
            catch
            {
                return 0;
            }
        }

        static float GetFloat(object value) {
            try
            {
                if (value.GetType() == typeof(Jurassic.ConcatenatedString))
                {
                    return float.Parse(((Jurassic.ConcatenatedString)value).ToString());
                }

                IConvertible val = (IConvertible)value;
                float cast = val.ToSingle(null);
                return cast;

            }
            catch
            {
                return 0;
            }
        }
    }
}