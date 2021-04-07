using SDL2;
using Jurassic;
using Jurassic.Library;
using System.Collections.Generic;
namespace DisasterAPI
{
    public class Input : ObjectInstance
    {
        public Input(ScriptEngine engine) : base (engine)
        {
            this.PopulateFunctions();
        }

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

        [JSFunction(Name = "getKey")]
        public static bool GetKey(int key) { return GetKey(keyCodes[key]); }
        [JSFunction(Name = "getKeyDown")]
        public static bool GetKeyDown(int key) { return GetKeyDown(keyCodes[key]); }
        [JSFunction(Name = "getKeyUp")]
        public static bool GetKeyUp(int key) { return GetKeyUp(keyCodes[key]); }

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

        public static SDL.SDL_Keycode[] keyCodes = new SDL.SDL_Keycode[]
        {
            SDL.SDL_Keycode.SDLK_UP,
            SDL.SDL_Keycode.SDLK_DOWN,
            SDL.SDL_Keycode.SDLK_LEFT,
            SDL.SDL_Keycode.SDLK_RIGHT,

            SDL.SDL_Keycode.SDLK_a,
            SDL.SDL_Keycode.SDLK_b,
            SDL.SDL_Keycode.SDLK_c,
            SDL.SDL_Keycode.SDLK_d,
            SDL.SDL_Keycode.SDLK_e,
            SDL.SDL_Keycode.SDLK_f,
            SDL.SDL_Keycode.SDLK_g,
            SDL.SDL_Keycode.SDLK_h,
            SDL.SDL_Keycode.SDLK_i,
            SDL.SDL_Keycode.SDLK_j,
            SDL.SDL_Keycode.SDLK_k,
            SDL.SDL_Keycode.SDLK_l,
            SDL.SDL_Keycode.SDLK_m,
            SDL.SDL_Keycode.SDLK_n,
            SDL.SDL_Keycode.SDLK_o,
            SDL.SDL_Keycode.SDLK_p,
            SDL.SDL_Keycode.SDLK_q,
            SDL.SDL_Keycode.SDLK_r,
            SDL.SDL_Keycode.SDLK_s,
            SDL.SDL_Keycode.SDLK_t,
            SDL.SDL_Keycode.SDLK_u,
            SDL.SDL_Keycode.SDLK_v,
            SDL.SDL_Keycode.SDLK_w,
            SDL.SDL_Keycode.SDLK_x,
            SDL.SDL_Keycode.SDLK_y,
            SDL.SDL_Keycode.SDLK_z,

            SDL.SDL_Keycode.SDLK_ESCAPE,
        };

    }

    
}