// entry point

//#define JS
//#define localcs

using System;
using SDL2;
using System.IO;

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
        static void Main(string[] args)
        {

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0) {
                Console.WriteLine("Error initializing SDL");
                SDL.SDL_Quit();
                return;
            }

            var window = SDL.SDL_CreateWindow(
                "Disaster Engine Again",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                640, 480,
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

            var running = true;
            var frameStart = DateTime.UtcNow.Ticks;

            // software renderer initialisation

            Draw.InitTexture(renderer, 320, 240);
            //var path = System.IO.Path.Combine("base/fontsmall.png");
            LoadConfig();
            Draw.LoadFont(Assets.LoadPath("fontsmall.png"));
            // scripting engine initialisation

#if JS
            var js = new JS();
#elif localcs
            var testy = new Testy();
            testy.Init();
#else
            var sr = new ScriptRunner();
#endif
            double ms = 0;
            int frame = 0;




            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 1024);


            var wav = SDL_mixer.Mix_LoadMUS(Assets.LoadPath("wove.ogg"));
            // Console.WriteLine(System.IO.File.Exists("base/wove.mp3"));
            if (wav == IntPtr.Zero) Console.WriteLine($"problam {SDL.SDL_GetError()}");
            SDL_mixer.Mix_PlayMusic(wav, 1);

            var test = new ScreenController(window);
            while (running)
            {
                frame += 1;
                long t = DateTime.UtcNow.Ticks - frameStart;
                ms = t / 10000.0;
                //uint delayTime = (uint) (16 - ms);
                //double fps = 1000.0 / ms;
                frameStart = DateTime.UtcNow.Ticks;

                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            running = false;
                            break;
                    }
                }

#if JS
                js.Update(ms);
#elif localcs
                testy.Update((float) ms);
#else
                sr.Update((float)ms);

#endif
                test.Update();

                SDL.SDL_SetWindowTitle(window, $"DISASTER ENGINE 5 -- MS: {Math.Floor(ms)}");// -- FPS: {fps} -- delay: {delayTime}");
                // if (delayTime > 0 && delayTime < 16) SDL.SDL_Delay((uint) delayTime);

                if (frame % 60 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            test.Done();

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
