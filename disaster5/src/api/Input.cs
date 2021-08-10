//using SDL2;
using Jurassic;
using Jurassic.Library;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
namespace DisasterAPI
{
    public class Input : ObjectInstance
    {
        public Input(ScriptEngine engine) : base (engine)
        {
            this.PopulateFunctions();
        }

        [JSProperty(Name = "mouseX")]
        [PropertyDescription("X position of the mouse on the screen")]
        public static int mouseX
        {
            get { return (int) mousePosition.X; }
        }

        [JSProperty(Name = "mouseY")]
        [PropertyDescription("Y position of the mouse on the screen")]
        public static int mouseY
        {
            get { return (int)mousePosition.Y; }
        }

        [JSFunction(Name = "lockMouse")]
        [FunctionDescription("Lock the mouse and hide it")]
        public void LockMouse()
        {
            Raylib_cs.Raylib.DisableCursor();
        }

        [JSFunction(Name = "unlockMouse")]
        [FunctionDescription("Unlock the mouse and show it")]
        public void UnlockMouse()
        {
            Raylib_cs.Raylib.EnableCursor();
        }

        [JSProperty(Name = "mouseLeft")]
        [PropertyDescription("Whether the left mouse button is currently pressed.")]
        public static bool mouseLeft { get { return Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON); } }
        [JSProperty(Name = "mouseLeftDown")]
        [PropertyDescription("Whether the left mouse button was pressed on this frame.")]
        public static bool mouseLeftDown { get { return Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON); } }
        [JSProperty(Name = "mouseLeftUp")]
        [PropertyDescription("Whether the left mouse button was released on this frame.")] 
        public static bool mouseLeftUp { get { return Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON); } }
        
        [JSProperty(Name = "mouseRight")]
        [PropertyDescription("Whether the right mouse button is currently pressed.")] 
        public static bool mouseRight { get { return Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON); } }
        [JSProperty(Name = "mouseRightDown")]
        [PropertyDescription("Whether the right mouse button was pressed on this frame.")] 
        public static bool mouseRightDown { get { return Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON); } }
        [JSProperty(Name = "mouseRightUp")]
        [PropertyDescription("Whether the right mouse button was released on this frame.")] 
        public static bool mouseRightUp { get { return Raylib.IsMouseButtonReleased(MouseButton.MOUSE_RIGHT_BUTTON); } }

        [JSProperty(Name = "mouseWheel")]
        [PropertyDescription("How much has the mousewheel been moved this frame.")]
        public static double mouseWheel { get { return Raylib.GetMouseWheelMove(); } }

        public static void Update()
        {
            float ratioW = (float)Disaster.ScreenController.screenWidth / (float)Disaster.ScreenController.windowWidth;
            float ratioH = (float)Disaster.ScreenController.screenHeight / (float)Disaster.ScreenController.windowHeight;

            var x = (int)(Raylib.GetMouseX() * ratioW);
            var y = (int)(Raylib.GetMouseY() * ratioH);

            

            mousePosition.X = x;
            mousePosition.Y = y;

            _anyKeyDown = false;
            while (Raylib.GetKeyPressed() != 0)
            {
                _anyKeyDown = true;
            }

            _inputString = "";
            var c = Raylib.GetCharPressed();
            while (c != 0)
            {
                _inputString += ((char)c).ToString();
                c = Raylib.GetCharPressed();
                _anyKeyDown = true;
            }

        }

        public static Vector2 mousePosition = new Vector2();

        [JSFunction(Name = "getKey")]
        [FunctionDescription("Check if a key is held")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
        public static bool GetKey(int key) { return Raylib.IsKeyDown(keyCodes[key]); }
        [JSFunction(Name = "getKeyDown")]
        [FunctionDescription("Check if a key has been pressed this frame")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
        public static bool GetKeyDown(int key) { return Raylib.IsKeyPressed(keyCodes[key]); }
        [JSFunction(Name = "getKeyUp")]
        [FunctionDescription("Check if a key has been released this frame")]
        [ArgumentDescription("key", "key code to test (see keycodes.js)")]
        public static bool GetKeyUp(int key) { return Raylib.IsKeyReleased(keyCodes[key]); }

        [JSProperty(Name = "inputString")]
        [PropertyDescription("Alpha-numeric characters that have been typed this frame.")]
        public static string InputString
        {
            get { return _inputString; }
        }

        public static string _inputString;

        public static bool _anyKeyDown = false;
        [JSProperty(Name = "anyKeyDown")]
        [PropertyDescription("Whether any key has been pressed this frame.")]
        public static bool anyKeyDown
        {
            get
            {
                return _anyKeyDown;
            }
        }

        public static KeyboardKey[] keyCodes = new KeyboardKey[]
        {
            KeyboardKey.KEY_UP,
            KeyboardKey.KEY_DOWN,
            KeyboardKey.KEY_LEFT,
            KeyboardKey.KEY_RIGHT,

            KeyboardKey.KEY_A,
            KeyboardKey.KEY_B,
            KeyboardKey.KEY_C,
            KeyboardKey.KEY_D,
            KeyboardKey.KEY_E,
            KeyboardKey.KEY_F,
            KeyboardKey.KEY_G,
            KeyboardKey.KEY_H,
            KeyboardKey.KEY_I,
            KeyboardKey.KEY_J,
            KeyboardKey.KEY_K,
            KeyboardKey.KEY_L,
            KeyboardKey.KEY_M,
            KeyboardKey.KEY_N,
            KeyboardKey.KEY_O,
            KeyboardKey.KEY_P,
            KeyboardKey.KEY_Q,
            KeyboardKey.KEY_R,
            KeyboardKey.KEY_S,
            KeyboardKey.KEY_T,
            KeyboardKey.KEY_U,
            KeyboardKey.KEY_V,
            KeyboardKey.KEY_W,
            KeyboardKey.KEY_X,
            KeyboardKey.KEY_Y,
            KeyboardKey.KEY_Z,

            KeyboardKey.KEY_ZERO,
            KeyboardKey.KEY_ONE,
            KeyboardKey.KEY_TWO,
            KeyboardKey.KEY_THREE,
            KeyboardKey.KEY_FOUR,
            KeyboardKey.KEY_FIVE,
            KeyboardKey.KEY_SIX,
            KeyboardKey.KEY_SEVEN,
            KeyboardKey.KEY_EIGHT,
            KeyboardKey.KEY_NINE,

            KeyboardKey.KEY_F1,
            KeyboardKey.KEY_F2,
            KeyboardKey.KEY_F3,
            KeyboardKey.KEY_F4,
            KeyboardKey.KEY_F5,
            KeyboardKey.KEY_F6,
            KeyboardKey.KEY_F7,
            KeyboardKey.KEY_F8,
            KeyboardKey.KEY_F9,
            KeyboardKey.KEY_F10,
            KeyboardKey.KEY_F11,
            KeyboardKey.KEY_F12,

            KeyboardKey.KEY_ESCAPE,
            KeyboardKey.KEY_ENTER,
            KeyboardKey.KEY_BACKSPACE,
            KeyboardKey.KEY_TAB,
            KeyboardKey.KEY_SPACE,
            KeyboardKey.KEY_LEFT_BRACKET,
            KeyboardKey.KEY_RIGHT_BRACKET,

            KeyboardKey.KEY_PRINT_SCREEN,
            KeyboardKey.KEY_SCROLL_LOCK,
            KeyboardKey.KEY_PAUSE,
            KeyboardKey.KEY_INSERT,
            KeyboardKey.KEY_HOME,
            KeyboardKey.KEY_PAGE_UP,
            KeyboardKey.KEY_DELETE,
            KeyboardKey.KEY_END,
            KeyboardKey.KEY_PAGE_DOWN,

            KeyboardKey.KEY_LEFT_CONTROL,
            KeyboardKey.KEY_RIGHT_CONTROL,
            KeyboardKey.KEY_LEFT_ALT,
            KeyboardKey.KEY_RIGHT_ALT,
            KeyboardKey.KEY_LEFT_SHIFT,
            KeyboardKey.KEY_RIGHT_SHIFT,
        };

    }


}