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

        //static Dictionary<int, ShaderProgram> shaderCache;
        static int currentShader = -1;

        public ModelRenderer(Model model, Shader shader, Texture2D texture)
        {
            this.model = model;
            this.shader = shader;
            this.texture = texture;

            transform = Matrix4x4.CreateTranslation(new Vector3(0, 0, -5));
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
                }

                Raylib.DrawModelEx(
                    renderQueue[i].model,
                    renderQueue[i].transform.position,
                    renderQueue[i].transform.rotationAxis,
                    renderQueue[i].transform.rotationAngle,
                    renderQueue[i].transform.scale,
                    new Color(255, 255, 255, 255)
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