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
            var frameStart = DateTime.Now.Ticks;

            // software renderer initialisation
            Draw.InitTexture(renderer, 320, 240);
            Draw.LoadFont(renderer, "./res/font1b.png");

            // var test = new Test();
            var test = new OGLTest(window);
            while (running)
            {
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            running = false;
                            break;
                    }
                }

                test.Update();

                // Draw.ApplyColorBuffer(renderer);
                // SDL.SDL_RenderPresent(renderer);

                long t = DateTime.Now.Ticks - frameStart;
                double ms = t / 10000.0;
                uint delayTime = (uint) (16 - ms);
                double fps = 1000.0 / ms;
                SDL.SDL_SetWindowTitle(window, $"DISASTER ENGINE 5 -- MS: {ms} -- FPS: {fps} -- delay: {delayTime}");
                // if (delayTime > 0 && delayTime < 16) SDL.SDL_Delay((uint) delayTime);

                frameStart = DateTime.Now.Ticks;
            }

            test.Done();

            // Clean up the resources that were created.
            SDL.SDL_DestroyRenderer(renderer);
            SDL.SDL_DestroyTexture(Draw.drawTexture);
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
