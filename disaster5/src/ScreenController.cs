using System;
using System.Numerics;
using System.Threading;
using Raylib_cs;

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

        // Framerate control
        private static double previousTime;
        private static double currentTime;
        private static double updateDrawTime;
        private static double waitTime;
        public static float deltaTime = 0.0f;
        public static int targetFPS = 60;

        public ScreenController(int width, int height, int scale)
        {
            screenWidth = width;
            screenHeight = height;
            windowWidth = width * scale;
            windowHeight = height * scale;
            ScreenController.scale = scale;

            instance = this;
            Raylib.InitWindow(windowWidth, windowHeight, "disaster engine 5.0");
            renderTexture = Raylib.LoadRenderTexture(screenWidth, screenHeight);
            renderTextureTTF = Raylib.LoadRenderTexture(windowWidth, windowHeight);
            camera = new Camera3D(
                new Vector3(0, 0f, 0f),
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0),
                45f,
                CameraProjection.CAMERA_PERSPECTIVE
            );

            ReloadShader();

            previousTime = Raylib.GetTime();
            currentTime = 0.0;
            updateDrawTime = 0.0;
            waitTime = 0.0;
            deltaTime = 0.0f;
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
            renderTexture = Raylib.LoadRenderTexture(screenWidth, screenHeight);

            Raylib.UnloadRenderTexture(renderTextureTTF);
            renderTextureTTF = Raylib.LoadRenderTexture(windowWidth, windowHeight);

            ReloadShader();
        }

        public void Update()
        {
            Draw();
            ControlFramerate();
        }

        private void ControlFramerate()
        {
            currentTime = Raylib.GetTime();
            updateDrawTime = currentTime - previousTime;
            waitTime = (1.0f / targetFPS) - updateDrawTime;
            
            if (waitTime > 0)
            {
                // Perform an initial wait. This waits for the number of milliseconds rounded down.
                Thread.Sleep((int) (waitTime * 1000));
                currentTime = Raylib.GetTime();

                // There's typically a decent amount of a millisecond left over to wait
                // but C# only supports millisecond wait times. So we instead just chuck
                // it over to the OS to do a bunch of little sub-millisecond waits to get
                // a more accurate and consistent framerate.
                while (currentTime < previousTime + (1.0f / targetFPS))
                {
                    Thread.Sleep(0);
                    currentTime = Raylib.GetTime();
                }
            }            

            deltaTime = (float)(currentTime - previousTime);
            previousTime = currentTime;

            Debug.Label("wait time");
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            
            Raylib.BeginTextureMode(renderTexture);
            // TODO: Screen hader makes non texture/buffer draws white (because the frag shader assumes a texture)
            // Raylib.BeginShaderMode(screenShader);
            ShapeRenderer.RenderQueue();
            // Raylib.EndShaderMode();
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
            Raylib.UnloadRenderTexture(renderTextureTTF);
        }
    }
}
