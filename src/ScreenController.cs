
using System;
using System.Collections.Generic;
using System.IO;
using SDL2;
using OpenGL;
using System.Numerics;

namespace Disaster {
    public class ScreenController {
        IntPtr window;

        Renderer drawScreen;
        uint framebuffer;

        VBO<Vector3> vertices;
        VBO<Vector2> uvs;
        VBO<uint> triangles;
        uint texture;
        ShaderProgram shader;

        public static int screenWidth = 320;
        public static int screenHeight = 240;

        public static int windowWidth = 640;
        public static int windowHeight = 480;

        public ScreenController(IntPtr window) {
            this.window = window;

            var glcontext = SDL.SDL_GL_CreateContext(window);

            shader = new ShaderProgram(
                    File.ReadAllText(Assets.LoadPath("outputvert.glsl")),
                    File.ReadAllText(Assets.LoadPath("outputfrag.glsl"))
                );
            framebuffer = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);

            texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2D, texture);
            Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, screenWidth, screenHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);

            var renderBuffer = Gl.GenRenderbuffer();
            Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBuffer);

            Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, screenWidth, screenHeight);
            Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, renderBuffer);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            Gl.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var error = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            switch (error)
            {
                case FramebufferErrorCode.FramebufferComplete:
                    break;
                default:
                    Console.WriteLine($"fbo error: {error}");
                    break;
            }

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
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                }
            );

            triangles = new VBO<uint>(
                new uint[] {
                    0, 1, 2, 0, 2, 3
                }, BufferTarget.ElementArrayBuffer
            );

            

            drawScreen = new DrawRenderer(
                new ShaderProgram(
                    File.ReadAllText(Assets.LoadPath("screenvert.glsl")),
                    File.ReadAllText(Assets.LoadPath("screenfrag.glsl"))
                )
            );

        }

        public void Update() {
            // render software texture to opengl
            Draw.CreateOGLTexture();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            Gl.Viewport(0, 0, screenWidth, screenHeight);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ObjRenderer.RenderQueue();
            drawScreen.Render();

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.Viewport(0, 0, windowWidth, windowHeight);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Gl.Disable(EnableCap.DepthTest);
            Gl.UseProgram(shader);
            Gl.BindBufferToShaderAttribute(vertices, shader, "pos");
            Gl.BindBufferToShaderAttribute(uvs, shader, "uv");
            Gl.BindBuffer(triangles);
            
            Gl.BindTexture(TextureTarget.Texture2D, texture);
            Gl.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            SDL.SDL_GL_SwapWindow(window);
        }

        public void Done() {
            drawScreen.Dispose();
            
        }
    }
}
