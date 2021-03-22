// opengl test scene

using System;
using System.Collections.Generic;
using System.IO;
using SDL2;
using OpenGL;
using System.Numerics;

namespace Disaster {
    public class OGLTest {
        IntPtr window;

        List<ObjRenderer> renderers;
        ShaderProgram shader;

        Renderer drawScreen;

        public OGLTest(IntPtr window) {
            this.window = window;

            var glcontext = SDL.SDL_GL_CreateContext(window);

            var vertShader = File.ReadAllText("res/vert.glsl");
            var fragShader = File.ReadAllText("res/frag.glsl");
            shader = new ShaderProgram(vertShader, fragShader);

            ObjRenderer radio = new ObjRenderer(ObjFile.Parse("res/tec1.obj"), shader, "res/radio.png");
            ObjRenderer laptop = new ObjRenderer(ObjFile.Parse("res/laptop.obj"), shader, "res/laptop.png");

            drawScreen = new DrawRenderer(
                new ShaderProgram(
                    File.ReadAllText("res/screenvert.glsl"),
                    File.ReadAllText("res/screenfrag.glsl")
                )
            );

            renderers = new List<ObjRenderer>();
            renderers.Add(radio);
            renderers.Add(laptop);

            test = new Test();
        }

        Test test;
        public void Update() {

            Draw.FillRect(0, 0, 320, 240, new Color32(100, 100, 100, 0));

            // software render test drawing
            test.Update();

            // render software texture to opengl
            Draw.CreateOGLTexture();

            shader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(1f, (float)320 / 240, 0.1f, 1000f));

            SDL.SDL_GetMouseState(out int mouseX, out int mouseY);

            // rotate the radio with mouse position
            renderers[0].transform = 
                Matrix4.CreateRotation(new Vector3(1, 0, 0), (mouseY-240) / -100f) * 
                Matrix4.CreateRotation(new Vector3(0, 1, 0), (mouseX-320) / -100f) * 
                Matrix4.CreateTranslation(new Vector3(0, 0, -5));
                
            Gl.Viewport(0, 0, 640, 480);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // draw software renderer to the screen
            drawScreen.Render();
            
            // then draw everything else
            foreach (var r in renderers)
            {
                r.Render();
            }

            SDL.SDL_GL_SwapWindow(window);
        }

        public void Done() {
            foreach (var r in renderers)
            {
                r.Dispose();
            }
            drawScreen.Dispose();
        }
    }
}