// renderer for obj files

using OpenGL;
using System;
using SDL2;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Disaster {
    public class ObjRenderer : Renderer {
        public ObjFile model;
        public ShaderProgram shader;
        public Texture texture;
        public Matrix4 transform;

        public ObjRenderer(ObjFile model, ShaderProgram shader, string texturePath) {
            this.model = model;
            this.shader = shader;
            LoadTexture(texturePath);

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

        public void LoadTexture(string path)
        {
            SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(SDL_image.IMG_Load(path));
            texture = new Texture(surface.pixels, surface.w, surface.h);
        }

        public void Dispose()
        {
            model.Dispose();
            texture.Dispose();
            shader.Dispose();
        }
    }

}