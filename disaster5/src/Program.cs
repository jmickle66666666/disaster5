// entry point

using System;
using System.IO;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

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

        public static ScreenController screen;
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
            string[] lines = SoftwareCanvas.SplitLineToFitScreen(message);
            foreach (var l in lines)
            {
                if (l == "") continue;
                SoftwareCanvas.Text(0, loadingScreenPosition, color, l);
                loadingScreenPosition += SoftwareCanvas.fontHeight;
            }
            screen.Update();
        }
        static void Main(string[] args)
        {
            Raylib.SetTraceLogLevel(TraceLogType.LOG_WARNING);
            Raylib.InitAudioDevice();
            
            Console.WriteLine($"Welcome to disaster engine");

            // software renderer initialisation
            SoftwareCanvas.InitTexture(320, 240);
            LoadConfig();
            // TODO: bake in a default font! so you can't end up with no font at all
            if (Assets.LoadPath("fontsmall.png", out string fontPath))
            {
                SoftwareCanvas.LoadFont(fontPath);
            }

            screen = new ScreenController();
            LoadingMessage("disaster engine 5.0");
            LoadingMessage("(c) jazz mickle ultramegacorp 2021");
            LoadingMessage("initialised screen");

            var js = new JS();

            while (!Raylib.WindowShouldClose())
            {
                js.Update(Raylib.GetFrameTime());
                screen.Update();
            }

            screen.Done();
            Raylib.CloseAudioDevice();





            //double ms = 0;
            //int frame = 0;

            //LoadingMessage("building input collection");
            //DisasterAPI.Input.keyState = new System.Collections.Generic.Dictionary<SDL.SDL_Keycode, (bool down, bool held, bool up)>();

            //LoadingMessage("opening audio");
            //SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 1024);
            //SDL_mixer.Mix_AllocateChannels(DisasterAPI.Audio.maxChannels);
            //LoadingMessage("complete");

            //System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;

            //long ticks;
            //long t;

            //Debug.enabled = false;

            //while (running)
            //{
            //    frame += 1;

            //    Debug.FrameStart();

            //    ticks = DateTime.UtcNow.Ticks;
            //    t = ticks - frameStart;
            //    ms = t / 10000.0;
            //    frameStart = ticks;

            //    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            //    {
            //        switch (e.type)
            //        {
            //            case SDL.SDL_EventType.SDL_QUIT:
            //                running = false;
            //                break;
            //            case SDL.SDL_EventType.SDL_TEXTINPUT:
            //                //DisasterAPI.Input._lastChar = e.text.text;
            //                break;
            //            case SDL.SDL_EventType.SDL_KEYDOWN:
            //                DisasterAPI.Input._anyKeyDown = true;
            //                //DisasterAPI.Input._lastChar = ((char)e.key.keysym.sym).ToString();
            //                DisasterAPI.Input.keyState[e.key.keysym.sym] = (true, true, DisasterAPI.Input.GetKeyUp(e.key.keysym.sym));
            //                break;
            //            case SDL.SDL_EventType.SDL_KEYUP:
            //                DisasterAPI.Input.keyState[e.key.keysym.sym] = (DisasterAPI.Input.GetKeyDown(e.key.keysym.sym), false, true);
            //                break;
            //        }
            //    }

            //    //Debug.Label("sdl events");

            //    double delta = ms * .001 * timescale;
            //    js.Update(delta);

            //    Debug.Label("js update");
            //    Debug.DrawGraph();
            //    screen.Update();

            //    //Debug.Label("render update");

            //    DisasterAPI.Input.Clear();

            //    SDL.SDL_SetWindowTitle(window, $"DISASTER ENGINE 5 -- MS: {Math.Floor(ms)}");

            //    if (ms > 20)
            //    {
            //        //Console.WriteLine("HElo");
            //        System.Diagnostics.Debugger.Log(0, "hitch", $"took {ms} this frame\n");
            //    }
            //    //if (frame % 30 == 0)

            //    //if (GC.GetTotalMemory(false) > 10000000)
            //    //{
            //    //    GC.Collect();
            //    //    GC.WaitForPendingFinalizers();
            //    //}

            //    //Debug.Label("gc");

            //    Debug.FrameEnd();
            //    Debug.GetFrameMSData();
            //}

            //screen.Done();

            //// Clean up the resources that were created.
            //Assets.Dispose();

            //SDL_mixer.Mix_CloseAudio();
            //SDL.SDL_DestroyRenderer(renderer);
            //SDL.SDL_DestroyTexture(SoftwareCanvas.drawTexture);
            //SDL.SDL_DestroyWindow(window);
            //SDL.SDL_Quit();
        }
    }
}
