// renderer for obj files

using System;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace Disaster
{
    public struct ModelRenderer : Renderer
    {
        public Model model;
        public Shader shader;
        public Texture2D texture;
        public Matrix4x4 transform;

        public static Color32 fogColor;
        public static float fogStart = 16.0f;
        public static float fogDistance = 128.0f;
        public static bool fogEnabled = false;

        public static void SetFogProperties(Color32 clr, float startDist, float dist)
        {
            fogColor = clr;
            fogStart = startDist;
            fogDistance = dist;
        }

        public static void SetFogEnabled(bool enabled)
        {
            fogEnabled = enabled;
        }

        //static Dictionary<int, ShaderProgram> shaderCache;
        static int currentShader = -1;

        public ModelRenderer(Model model, Shader shader, Texture2D texture)
        {
            this.model = model;
            this.shader = shader;
            this.texture = texture;

            transform = Matrix4x4.CreateTranslation(new Vector3(0, 0, -5));

            fogColor = new Color32(0, 0, 0, 255);
            fogStart = 32.0f;
            fogDistance = 96.0f;
        }

        public void Render()
        {
            
        }

        public static (Model model, Shader shader, Transformation transform)[] renderQueue;
        public static int renderQueueLength;

        public static void EnqueueRender(Model model, Shader shader, Transformation transform)
        {

            if (renderQueue == null)
            {
                renderQueue = new (Model model, Shader shader, Transformation transform)[16];
            }
            renderQueue[renderQueueLength] = (model, shader, transform);
            renderQueueLength += 1;
            if (renderQueueLength >= renderQueue.Length)
            {
                Array.Resize(ref renderQueue, renderQueue.Length + 16);
            }
        }

        public static void RenderQueue()
        {
            //Array.Sort(renderQueue, (a, b) =>
            //{
            //    int shaderHash1 = a.shader.GetHashCode();
            //    int shaderHash2 = b.shader.GetHashCode();
            //    if (shaderHash1 == shaderHash2)
            //    {
            //        return a.objFile.hash - b.objFile.hash;
            //    }
            //    return shaderHash1 - shaderHash2;
            //});
            //int currentModelHash = -1;
            Raylib.BeginShaderMode(Raylib.GetShaderDefault());
            for (int i = 0; i < renderQueueLength; i++)
            {
                int shaderHash = renderQueue[i].shader.GetHashCode();
                if (currentShader != shaderHash)
                {
                    Raylib.EndShaderMode();
                    Raylib.BeginShaderMode(renderQueue[i].shader);
                    currentShader = shaderHash;
                    Raylib.SetShaderValueMatrix(renderQueue[i].shader, 0, Matrix4x4.CreatePerspectiveFieldOfView(1f, (float)320 / 240, 0.1f, 1000f));
                    //renderQueue[i].shader["projection_matrix"].SetValue();

                    //renderQueue[i].shader["Use_Fog"]?.SetValue(fogEnabled);

                    //System.Numerics.Vector3 fogColorV3 =
                    //    new Vector3(fogColor.r / 255.0f, fogColor.g / 255.0f, fogColor.b / 255.0f);

                    //renderQueue[i].shader["Fog_Color"]?.SetValue(fogColorV3);
                    //renderQueue[i].shader["Fog_Start"]?.SetValue(fogStart);
                    //renderQueue[i].shader["Fog_Distance"]?.SetValue(fogDistance);
                }

                Raylib.DrawModelEx(
                    renderQueue[i].model,
                    renderQueue[i].transform.position,
                    renderQueue[i].transform.rotationAxis,
                    renderQueue[i].transform.rotationAngle,
                    renderQueue[i].transform.scale,
                    Color.RAYWHITE
                );
            }
            Raylib.EndShaderMode();
            renderQueueLength = 0;
            currentShader = -1;
        }

        public void Dispose()
        {

        }
    }

}