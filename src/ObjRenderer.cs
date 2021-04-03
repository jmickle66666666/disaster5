// renderer for obj files

using OpenGL;
using System;
using SDL2;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Collections.Generic;

namespace Disaster {
    public struct ObjRenderer : Renderer {
        public ObjModel model;
        public ShaderProgram shader;
        public Texture texture;
        public Matrix4 transform;

        //static Dictionary<int, ShaderProgram> shaderCache;
        static int currentShader = -1;

        public ObjRenderer(ObjModel model, ShaderProgram shader, Texture texture) {
            this.model = model;
            this.shader = shader;
            this.texture = texture;

            transform = Matrix4.CreateTranslation(new Vector3(0, 0, -5));
        }

        public void Render()
        {
            Gl.UseProgram(shader);
            shader["modelview_matrix"].SetValue(transform);
            Gl.BindBufferToShaderAttribute(model.vertices, shader, "pos");
            Gl.BindBufferToShaderAttribute(model.uvs, shader, "uv");
            Gl.BindBuffer(model.triangles);
            Gl.BindTexture(texture);
            Gl.DrawElements(BeginMode.Triangles, model.triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public static (ObjModel objFile, ShaderProgram shader, Texture texture, Matrix4 transform)[] renderQueue;
        public static int renderQueueLength;

        public static void EnqueueRender(ObjModel objFile, ShaderProgram shader, Texture texture, Matrix4 transform)
        {
            
            if (renderQueue == null) {
                renderQueue = new (ObjModel objFile, ShaderProgram shader, Texture texture, Matrix4 transform)[16];
            }
            renderQueue[renderQueueLength] = (objFile, shader, texture, transform);
            renderQueueLength += 1;
            if (renderQueueLength >= renderQueue.Length) {
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
            int currentModelHash = -1;
            for (int i = 0; i < renderQueueLength; i++)
            {
                int shaderHash = renderQueue[i].shader.GetHashCode();
                if (currentShader != shaderHash)
                {
                    Gl.UseProgram(renderQueue[i].shader);
                    currentShader = shaderHash;
                    renderQueue[i].shader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(1f, (float)320 / 240, 0.1f, 1000f));
                }

                int modelHash = renderQueue[i].objFile.hash;
                if (currentModelHash != modelHash)
                {
                    currentModelHash = modelHash;
                    Gl.BindBufferToShaderAttribute(renderQueue[i].objFile.vertices, renderQueue[i].shader, "pos");
                    Gl.BindBufferToShaderAttribute(renderQueue[i].objFile.uvs, renderQueue[i].shader, "uv");
                    Gl.BindBuffer(renderQueue[i].objFile.triangles);
                }

                renderQueue[i].shader["modelview_matrix"].SetValue(renderQueue[i].transform);
                Gl.BindTexture(renderQueue[i].texture);

                Gl.DrawElements(BeginMode.Triangles, renderQueue[i].objFile.triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }
            renderQueueLength = 0;
            currentShader = -1;
        }

        public static void Render(ObjModel objFile, ShaderProgram shader, Texture texture, Matrix4 transform)
        {
            Gl.Enable(EnableCap.DepthTest);
            int shaderHash = shader.GetHashCode();
            if (currentShader != shaderHash)
            {
                Gl.UseProgram(shader);
                currentShader = shaderHash;
            }
            shader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(1f, (float)320 / 240, 0.1f, 1000f));
            shader["modelview_matrix"].SetValue(transform);
            Gl.BindBufferToShaderAttribute(objFile.vertices, shader, "pos");
            Gl.BindBufferToShaderAttribute(objFile.uvs, shader, "uv");
            Gl.BindBuffer(objFile.triangles);
            Gl.BindTexture(texture);
            Gl.DrawElements(BeginMode.Triangles, objFile.triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Dispose()
        {
            model.Dispose();
            texture.Dispose();
            shader.Dispose();
        }
    }

}