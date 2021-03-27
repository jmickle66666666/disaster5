// renderer for obj files

using OpenGL;
using System;
using SDL2;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Disaster {
    public class ObjRenderer : Renderer {
        public ObjModel model;
        public ShaderProgram shader;
        public Texture texture;
        public Matrix4 transform;

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
            for (int i = 0; i < renderQueueLength; i++)
            {
                Render(
                    renderQueue[i].objFile,
                    renderQueue[i].shader,
                    renderQueue[i].texture,
                    renderQueue[i].transform
                );
            }
            renderQueueLength = 0;
        }

        public static void Render(ObjModel objFile, ShaderProgram shader, Texture texture, Matrix4 transform)
        {
            
            Gl.Enable(EnableCap.DepthTest);
            shader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(1f, (float)320 / 240, 0.1f, 1000f));
            Gl.UseProgram(shader);
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