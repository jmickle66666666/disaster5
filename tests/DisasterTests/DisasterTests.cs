using System;
using System.Collections.Generic;
using System.Text;
using SDL2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DisasterTests
{
    public static class DisasterTests
    {
        private static IntPtr _renderer;
        private static IntPtr _window;

        public static void Init()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO) < 0)
            {
                Assert.Fail($"SDL failed to initialize.  {SDL.SDL_GetError()}");
                SDL.SDL_Quit();
                return;
            }

            // LUNA: Initialize with OpenGL 3.2, so we can debug graphics with RenderDoc
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_COMPATIBILITY);

            _window = SDL.SDL_CreateWindow(
                "Disaster Engine Test",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                320, 240,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
            );

            if (_window == IntPtr.Zero)
            {
                Assert.Fail($"There was an issue creating the window. {SDL.SDL_GetError()}");
            }

            _renderer = SDL.SDL_CreateRenderer(
                _window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );

            if (_renderer == IntPtr.Zero)
            {
                Assert.Fail($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
            }
        }

        public static void Quit()
        {
            SDL.SDL_DestroyRenderer(_renderer);
            SDL.SDL_DestroyWindow(_window);
            SDL.SDL_Quit();
        }
    }
}
