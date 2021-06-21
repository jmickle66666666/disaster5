
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace Disaster
{
    public class ScreenController
    {
        SoftwareCanvasRenderer drawScreen;

        public static int screenWidth = 320;
        public static int screenHeight = 240;

        public static int windowWidth = 640;
        public static int windowHeight = 480;

        static RenderTexture2D renderTexture;
        static int scale = 2;
        public static Camera3D camera;

        public ScreenController()
        {

            Raylib.InitWindow(640, 480, "disaster engine 5.0");
            renderTexture = Raylib.LoadRenderTexture(640 / scale, 480 / scale);
            Raylib.SetTargetFPS(60);
            camera = new Camera3D(
                new Vector3(0, 0f, 0f),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                45f
            );

            Assets.LoadPath("shaders/screenfrag.glsl", out string fragPath);
            Assets.LoadPath("shaders/screenvert.glsl", out string vertPath);

            var shader = Raylib.LoadShader(
                vertPath,
                fragPath
            );

            drawScreen = new SoftwareCanvasRenderer(shader);

        }

        public void Update()
        {
            drawScreen.Update();

            Raylib.BeginDrawing();
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.BLACK);

            Raylib.BeginMode3D(camera);

            ModelRenderer.RenderQueue();

            Raylib.EndMode3D();
            Raylib.EndTextureMode();

            Raylib.ClearBackground(Color.BLACK);


            Raylib.DrawTexturePro(
                renderTexture.texture,
                new Rectangle(0, 0, renderTexture.texture.width, -renderTexture.texture.height),
                new Rectangle(0, 0, 640, 480),
                Vector2.Zero,
                0,
                Color.RAYWHITE
            );

            drawScreen.Render();
            Raylib.EndDrawing();

            Debug.Label("swap window");
        }

        public void Done()
        {
            drawScreen.Dispose();
            Raylib.CloseWindow();
            Raylib.UnloadRenderTexture(renderTexture);
        }
    }
}
