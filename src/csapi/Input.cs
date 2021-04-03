using SDL2;
using System.Collections.Generic;
namespace DisasterEngine
{
    public class Input
    {
        public static Dictionary<SDL.SDL_Keycode, (bool down, bool held, bool up)> keyState;

        public static void Clear()
        {
            Dictionary<SDL.SDL_Keycode, (bool down, bool held, bool up)> nextState = new Dictionary<SDL.SDL_Keycode, (bool down, bool held, bool up)>();
            foreach (var kvp in keyState)
            {
                nextState[kvp.Key] = (false, kvp.Value.held, false);
            }
            keyState = nextState;
        }

        public static bool GetKey(KeyCode key) { return GetKey((SDL.SDL_Keycode)key); }
        public static bool GetKeyDown(KeyCode key) { return GetKeyDown((SDL.SDL_Keycode)key); }
        public static bool GetKeyUp(KeyCode key) { return GetKeyUp((SDL.SDL_Keycode)key); }

        public static bool GetKey(SDL.SDL_Keycode key)
        {
            if (!keyState.ContainsKey(key)) return false;
            return keyState[key].held;
        }

        public static bool GetKeyDown(SDL.SDL_Keycode key)
        {
            if (!keyState.ContainsKey(key)) return false;
            return keyState[key].down;
        }

        public static bool GetKeyUp(SDL.SDL_Keycode key)
        {
            if (!keyState.ContainsKey(key)) return false;
            return keyState[key].up;
        }

    }

    public enum KeyCode
    {
        UP = SDL.SDL_Keycode.SDLK_UP,
        DOWN = SDL.SDL_Keycode.SDLK_DOWN,
        LEFT = SDL.SDL_Keycode.SDLK_LEFT,
        RIGHT = SDL.SDL_Keycode.SDLK_RIGHT,
        ESCAPE = SDL.SDL_Keycode.SDLK_ESCAPE,
    }
}