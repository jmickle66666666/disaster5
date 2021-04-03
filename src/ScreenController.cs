
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

        public ScreenController(IntPtr window) {
            this.window = window;

            var glcontext = SDL.SDL_GL_CreateContext(window);

            drawScreen = new DrawRenderer(
                new ShaderProgram(
                    File.ReadAllText("res/screenvert.glsl"),
                    File.ReadAllText("res/screenfrag.glsl")
                )
            );

        }

        public void Update() {
            // render software texture to opengl
            Draw.CreateOGLTexture();
            Gl.Viewport(0, 0, 640, 480);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                
            ObjRenderer.RenderQueue();
            drawScreen.Render();

            SDL.SDL_GL_SwapWindow(window);
        }

        public void Done() {
            drawScreen.Dispose();
        }
    }
}
