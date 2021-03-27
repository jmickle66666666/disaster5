// entry point

using System;
using SDL2;

namespace Disaster
{
    
    class Program
    {
        static void Main(string[] args)
        {

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
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
            Draw.LoadFont("./res/fontsmall.png");

            // scripting engine initialisation
            var js = new JS();
            double ms = 0;

            var test = new ScreenController(window);
            while (running)
            {
                long t = DateTime.UtcNow.Ticks - frameStart;
                ms = t / 10000.0;
                uint delayTime = (uint) (16 - ms);
                double fps = 1000.0 / ms;
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

                js.Update(ms);
                test.Update();

                SDL.SDL_SetWindowTitle(window, $"DISASTER ENGINE 5 -- MS: {ms} -- FPS: {fps} -- delay: {delayTime}");
                if (delayTime > 0 && delayTime < 16) SDL.SDL_Delay((uint) delayTime);

            }

            test.Done();

            // Clean up the resources that were created.
            Assets.Dispose();
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyTexture(Draw.drawTexture);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
