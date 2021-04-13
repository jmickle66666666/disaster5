// opengl renderer for software renderer

using OpenGL;
using System.Numerics;
using System;
namespace Disaster{
    public class DrawRenderer : Renderer 
    {
        VBO<Vector3> vertices;
        VBO<Vector2> uvs;
        VBO<uint> triangles;
        ShaderProgram shader;
        public DrawRenderer(ShaderProgram shader)
        {
            this.shader = shader;

            vertices = new VBO<Vector3>(
                new Vector3[] {
                    new Vector3(-1, -1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(-1, 1, 0),
                }
            );

            uvs = new VBO<Vector2>(
                new Vector2[] {
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                }
            );

            triangles = new VBO<uint>(
                new uint[] {
                    0, 1, 2, 0, 2, 3
                }, BufferTarget.ElementArrayBuffer
            );
        }

        public void Render()
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.UseProgram(shader);
            Gl.BindBufferToShaderAttribute(vertices, shader, "pos");
            Gl.BindBufferToShaderAttribute(uvs, shader, "uv");

            Debug.Label("soft render setup");
            Draw.CreateOGLTexture();
            Debug.Label("create ogl texture");
            Gl.BindBuffer(triangles);
            Gl.DrawElements(BeginMode.Triangles, triangles.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void Dispose()
        {
            vertices.Dispose();
            uvs.Dispose();
            triangles.Dispose();
            shader.Dispose();
        }
    }
}