using SDL2;
using Jurassic;
using Jurassic.Library;
using System.Collections.Generic;
using System.Numerics;
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

        [JSProperty(Name = "mouseX")]
        public static int mouseX
        {
            get { return (int) mousePosition.X; }
        }

        [JSProperty(Name = "mouseY")]
        public static int mouseY
        {
            get { return (int)mousePosition.Y; }
        }

        [JSProperty(Name = "mouseLeft")] public static bool mouseLeft { get { return _mouseLeft; } }
        [JSProperty(Name = "mouseLeftDown")] public static bool mouseLeftDown { get { return _mouseLeftDown; } }
        [JSProperty(Name = "mouseLeftUp")] public static bool mouseLeftUp { get { return _mouseLeftUp; } }

        static bool _mouseLeft;
        static bool _mouseLeftDown;
        static bool _mouseLeftUp;
        
        [JSProperty(Name = "mouseRight")] public static bool mouseRight { get { return _mouseRight; } }
        [JSProperty(Name = "mouseRightDown")] public static bool mouseRightDown { get { return _mouseRightDown; } }
        [JSProperty(Name = "mouseRightUp")] public static bool mouseRightUp { get { return _mouseRightUp; } }

        static bool _mouseRight;
        static bool _mouseRightDown;
        static bool _mouseRightUp;

        public static void UpdateMouse()
        {
            uint mouseState = SDL.SDL_GetMouseState(out int x, out int y);

            float ratioW = (float)Disaster.ScreenController.screenWidth / (float)Disaster.ScreenController.windowWidth;
            float ratioH = (float)Disaster.ScreenController.screenHeight / (float)Disaster.ScreenController.windowHeight;

            x = (int)(x * ratioW);
            y = (int)(y * ratioH);

            _mouseLeftDown = false;
            _mouseLeftUp = false;
            _mouseRightDown = false;
            _mouseRightUp = false;
            
            mousePosition.X = x;
            mousePosition.Y = y;

            bool currentLeft = (mouseState & SDL.SDL_BUTTON(SDL.SDL_BUTTON_LEFT)) == SDL.SDL_BUTTON(SDL.SDL_BUTTON_LEFT);
            bool currentRight = (mouseState & SDL.SDL_BUTTON(SDL.SDL_BUTTON_RIGHT)) == SDL.SDL_BUTTON(SDL.SDL_BUTTON_RIGHT);

            if (_mouseLeft && !currentLeft) { _mouseLeftUp = true; }
            if (!_mouseLeft && currentLeft) { _mouseLeftDown = true; }
            _mouseLeft = currentLeft;

            if (_mouseRight && !currentRight) { _mouseRightUp = true; }
            if (!_mouseRight && currentRight) { _mouseRightDown = true; }
            _mouseRight = currentRight;
        }

        public static Vector2 mousePosition = new Vector2();

        [JSFunction(Name = "getKey")]
        [FunctionDescription("Check if a key is held")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
        public static bool GetKey(int key) { return GetKey(keyCodes[key]); }
        [JSFunction(Name = "getKeyDown")]
        [FunctionDescription("Check if a key has been pressed this frame")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
        public static bool GetKeyDown(int key) { return GetKeyDown(keyCodes[key]); }
        [JSFunction(Name = "getKeyUp")]
        [FunctionDescription("Check if a key has been released this frame")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
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
            
            SDL.SDL_Keycode.SDLK_0,
            SDL.SDL_Keycode.SDLK_1,
            SDL.SDL_Keycode.SDLK_2,
            SDL.SDL_Keycode.SDLK_3,
            SDL.SDL_Keycode.SDLK_4,
            SDL.SDL_Keycode.SDLK_5,
            SDL.SDL_Keycode.SDLK_6,
            SDL.SDL_Keycode.SDLK_7,
            SDL.SDL_Keycode.SDLK_8,
            SDL.SDL_Keycode.SDLK_9,

            SDL.SDL_Keycode.SDLK_F1,
            SDL.SDL_Keycode.SDLK_F2,
            SDL.SDL_Keycode.SDLK_F3,
            SDL.SDL_Keycode.SDLK_F4,
            SDL.SDL_Keycode.SDLK_F5,
            SDL.SDL_Keycode.SDLK_F6,
            SDL.SDL_Keycode.SDLK_F7,
            SDL.SDL_Keycode.SDLK_F8,
            SDL.SDL_Keycode.SDLK_F9,
            SDL.SDL_Keycode.SDLK_F10,
            SDL.SDL_Keycode.SDLK_F11,
            SDL.SDL_Keycode.SDLK_F12,

            SDL.SDL_Keycode.SDLK_ESCAPE,
            SDL.SDL_Keycode.SDLK_RETURN,
            SDL.SDL_Keycode.SDLK_BACKSPACE,
            SDL.SDL_Keycode.SDLK_TAB,
            SDL.SDL_Keycode.SDLK_SPACE,
            SDL.SDL_Keycode.SDLK_LEFTBRACKET,
            SDL.SDL_Keycode.SDLK_RIGHTBRACKET,

            SDL.SDL_Keycode.SDLK_PRINTSCREEN,
            SDL.SDL_Keycode.SDLK_SCROLLLOCK,
            SDL.SDL_Keycode.SDLK_PAUSE,
            SDL.SDL_Keycode.SDLK_INSERT,
            SDL.SDL_Keycode.SDLK_HOME,
            SDL.SDL_Keycode.SDLK_PAGEUP,
            SDL.SDL_Keycode.SDLK_DELETE,
            SDL.SDL_Keycode.SDLK_END,
            SDL.SDL_Keycode.SDLK_PAGEDOWN,
        };

    }

    
}