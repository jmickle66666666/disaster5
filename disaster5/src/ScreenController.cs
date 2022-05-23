
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
        public static ScreenController instance;
        private Shader screenShader;
        private Shader postProcessShader;

        public static int screenWidth = 320;
        public static int screenHeight = 240;

        public static int windowWidth = 640;
        public static int windowHeight = 480;

        public static Vector2Int offset;
        public static Camera3D camera;
        private static RenderTexture2D renderTexture;
        private static RenderTexture2D renderTextureTTF;
        private static int scale = 2;

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
            renderTextureTTF = Util.LoadRenderTexture(windowWidth, windowHeight);
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
                screenShader = Assets.Shader("shaders/screen").shader;
            }
            else
            {
                Console.WriteLine("loading backup software shader");
                screenShader = Raylib.LoadMaterialDefault().shader;
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
            Raylib.SetWindowSize(windowWidth, windowHeight);

            Raylib.UnloadRenderTexture(renderTexture);
            renderTexture = Util.LoadRenderTexture(screenWidth, screenHeight);

            Raylib.UnloadRenderTexture(renderTextureTTF);
            renderTextureTTF = Util.LoadRenderTexture(windowWidth, windowHeight);

            ReloadShader();
        }

        public void Update()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            
            Raylib.BeginTextureMode(renderTexture);
            ModelRenderer.RenderQueue();
            Raylib.BeginShaderMode(screenShader);
            ShapeRenderer.RenderQueue();
            Raylib.EndShaderMode();
            Raylib.EndTextureMode();

            Raylib.BeginTextureMode(renderTextureTTF);
            NativeResRenderer.RenderQueue();
            Raylib.EndTextureMode();

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
            Raylib.DrawTexturePro(
                renderTextureTTF.texture,
                new Rectangle(0, 0, renderTextureTTF.texture.width, -renderTextureTTF.texture.height),
                new Rectangle(0, 0, windowWidth, windowHeight),
                Vector2.Zero,
                0,
                Color.WHITE
            );
            Raylib.EndShaderMode();
            Rlgl.rlDrawRenderBatchActive();

            Raylib.EndDrawing();

            Debug.Label("swap window");
        }

        public void Done()
        {
            Raylib.CloseWindow();
            Raylib.UnloadRenderTexture(renderTexture);
        }
    }
}
