
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;
using System.Runtime.InteropServices;

namespace Disaster
{
    public class ScreenController
    {
        SoftwareCanvasRenderer softwareCanvasRenderer;
        public static ScreenController instance;
        Shader postProcessShader;

        public static int screenWidth = 320;
        public static int screenHeight = 240;

        public static int windowWidth = 640;
        public static int windowHeight = 480;

        static RenderTexture2D renderTexture;
        static int scale = 2;
        public static Camera3D camera;

        public ScreenController(int width, int height, int scale)
        {
            screenWidth = width;
            screenHeight = height;
            windowWidth = width * scale;
            windowHeight = height * scale;
            ScreenController.scale = scale;

            instance = this;
            Raylib.InitWindow(windowWidth, windowHeight, "disaster engine 5.0");
            renderTexture = Util.LoadRenderTexture(screenWidth, screenHeight);
            Raylib.SetTargetFPS(60);
            camera = new Camera3D(
                new Vector3(0, 0f, 0f),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                45f,
                CameraProjection.CAMERA_PERSPECTIVE
            );

            ReloadShader();
        }

        public void ReloadShader()
        {
            if (Assets.PathExists("shaders/screen.vert") && Assets.PathExists("shaders/screen.frag"))
            {
                softwareCanvasRenderer = new SoftwareCanvasRenderer(Assets.Shader("shaders/screen").shader);
            }
            else
            {
                Console.WriteLine("loading backup software shader");
                softwareCanvasRenderer = new SoftwareCanvasRenderer(Raylib.LoadMaterialDefault().shader);
            }

            if (Assets.PathExists("shaders/postprocess.vert") && Assets.PathExists("shaders/postprocess.frag"))
            {
                postProcessShader = Assets.Shader("shaders/postprocess").shader;
            }
            else
            {
                Console.WriteLine("loading backup postprocess shader");
                postProcessShader = Raylib.LoadMaterialDefault().shader;
            }
        }

        public void Resize(int width, int height, int scale)
        {
            screenWidth = width;
            screenHeight = height;
            windowWidth = width * scale;
            windowHeight = height * scale;
            ScreenController.scale = scale;
            Raylib.SetWindowSize(width * scale, height * scale);

            SoftwareCanvas.InitTexture(width, height);

            Raylib.UnloadRenderTexture(renderTexture);
            renderTexture = Util.LoadRenderTexture(screenWidth, screenHeight);
            softwareCanvasRenderer = new SoftwareCanvasRenderer(Assets.Shader("shaders/screen").shader);
            ReloadShader();
        }

        public void Update()
        {
            softwareCanvasRenderer.Update();

            Raylib.BeginDrawing();

            Raylib.BeginTextureMode(renderTexture);
            ModelRenderer.RenderQueue();
            ShapeRenderer.RenderQueue();
            Raylib.EndTextureMode();

            Raylib.ClearBackground(Color.BLACK);
            //Console.WriteLine(renderTexture.depth.id);

            Raylib.BeginShaderMode(postProcessShader);
            Raylib.SetShaderValueTexture(postProcessShader, Raylib.GetShaderLocation(postProcessShader, "depthTexture"), renderTexture.depth);
            var t = (float)Raylib.GetTime();
            Raylib.SetShaderValue(postProcessShader, Raylib.GetShaderLocation(postProcessShader, "time"), ref t, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
            unsafe
            {
                Vector4 screenDims = new Vector4(screenWidth, screenHeight, windowWidth, windowHeight);
                IntPtr pointer = new IntPtr(&screenDims);
                Raylib.SetShaderValue(
                    postProcessShader, 
                    Raylib.GetShaderLocation(postProcessShader, "screenSize"),
                    pointer,
                    ShaderUniformDataType.SHADER_UNIFORM_VEC4
                );
            }
            
            Raylib.DrawTexturePro(
                renderTexture.texture,
                new Rectangle(0, 0, renderTexture.texture.width, -renderTexture.texture.height),
                new Rectangle(0, 0, windowWidth, windowHeight),
                Vector2.Zero,
                0,
                Color.WHITE
            );
            Raylib.EndShaderMode();
            softwareCanvasRenderer.Render();
            Rlgl.rlDrawRenderBatchActive();

            Raylib.EndDrawing();

            Debug.Label("swap window");
        }

        public void Done()
        {
            softwareCanvasRenderer.Dispose();
            Raylib.CloseWindow();
            Raylib.UnloadRenderTexture(renderTexture);
        }
    }
}
