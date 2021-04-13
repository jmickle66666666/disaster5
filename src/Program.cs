// entry point

using System;
using SDL2;
using System.IO;
using System.Collections.Generic;

namespace Disaster
{
    
    class Program
    {

        static void LoadConfig()
        {
            string basedir = "";
            string[] lines = File.ReadAllLines("disaster.cfg");

            foreach (var line in lines)
            {
                string[] tokens = line.Split(' ');
                switch (tokens[0])
                {
                    case "basedir":
                        if (tokens.Length != 2)
                        {
                            Console.WriteLine($"Unexpected number of tokens: {line}");
                            break;
                        }
                        basedir = tokens[1];
                        break;
                }
            }

            Assets.basePath = basedir;
        }

        static ScreenController screen;
        static int loadingScreenPosition = 0;

        public static bool running = true;
        public static double timescale = 1;

        public static void LoadingMessage(string message)
        {
            LoadingMessage(message, new Color32(255, 140, 0));
        }

        public static void LoadingMessage(string message, Color32 color)
        {
            Console.WriteLine(message);
            string[] lines = Draw.SplitLineToFitScreen(message);
            foreach (var l in lines)
            {
                if (l == "") continue;
                Draw.Text(0, loadingScreenPosition, color, l);
                loadingScreenPosition += Draw.fontHeight;
            }
            screen.Update();
        }
        static void Main(string[] args)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0) {
                Console.WriteLine("Error initializing SDL");
                SDL.SDL_Quit();
                return;
            }

            // LUNA: Initialize with OpenGL 3.2, so we can debug graphics with RenderDoc
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_COMPATIBILITY);

            var window = SDL.SDL_CreateWindow(
                "Disaster Engine Again",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                ScreenController.windowWidth, ScreenController.windowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
            );

            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
            }

            var renderer = SDL.SDL_CreateRenderer(
                window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
            }

            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "0");

            Console.WriteLine($"Welcome to disaster engine");

            var frameStart = DateTime.UtcNow.Ticks;

            // software renderer initialisation
            Draw.InitTexture(renderer, 320, 240);
            LoadConfig();
            Draw.LoadFont(Assets.LoadPath("fontsmall.png"));

            screen = new ScreenController(window);
            LoadingMessage("disaster engine 5.0");
            LoadingMessage("(c) jazz mickle ultramegacorp 2021");
            LoadingMessage("initialised screen");
            var js = new JS();
            
            double ms = 0;
            int frame = 0;

            LoadingMessage("building input collection");
            DisasterAPI.Input.keyState = new System.Collections.Generic.Dictionary<SDL.SDL_Keycode, (bool down, bool held, bool up)>();

            LoadingMessage("opening audio");
            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 1024);

            LoadingMessage("complete");

            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;

            long ticks;
            long t;
            
            Debug.enabled = false;

            while (running)
            {
                frame += 1;

                Debug.FrameStart();

                ticks = DateTime.UtcNow.Ticks;
                t = ticks - frameStart;
                ms = t / 10000.0;
                frameStart = ticks;

                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            running = false;
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            DisasterAPI.Input.keyState[e.key.keysym.sym] = (true, true, DisasterAPI.Input.GetKeyUp(e.key.keysym.sym));
                            break;
                        case SDL.SDL_EventType.SDL_KEYUP:
                            DisasterAPI.Input.keyState[e.key.keysym.sym] = (DisasterAPI.Input.GetKeyDown(e.key.keysym.sym), false, true);
                            break;
                    }
                }

                Debug.Label("sdl events");

                double delta = ms * .001 * timescale;
                js.Update(delta);

                Debug.Label("js update");
                Debug.DrawGraph();
                screen.Update();

                Debug.Label("render update");

                DisasterAPI.Input.Clear();

                SDL.SDL_SetWindowTitle(window, $"DISASTER ENGINE 5 -- MS: {Math.Floor(ms)}");
                                                                                             
                if (ms > 20)
                {
                    //Console.WriteLine("HElo");
                    System.Diagnostics.Debugger.Log(0, "hitch", $"took {ms} this frame\n");
                }
                //if (frame % 30 == 0)

                if (GC.GetTotalMemory(false) > 10000000)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                Debug.Label("gc");

                Debug.FrameEnd();
                Debug.GetFrameMSData();
            }

            screen.Done();

            // Clean up the resources that were created.
            Assets.Dispose();
            
            SDL_mixer.Mix_CloseAudio();
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyTexture(Draw.drawTexture);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
