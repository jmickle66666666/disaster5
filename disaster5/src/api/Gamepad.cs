using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    [ClassDescription("A list of gamepad values for use with Input.getGamepadButton etc")]
    public class Gamepad : ObjectInstance
    {
        public Gamepad(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }
        [JSProperty(Name = "up")] [PropertyDescription("DPad up")] public int up { get { return 0; } }
        [JSProperty(Name = "down")] [PropertyDescription("DPad down")] public int down { get { return 1; } }
        [JSProperty(Name = "left")] [PropertyDescription("DPad left")] public int left { get { return 2; } }
        [JSProperty(Name = "right")] [PropertyDescription("DPad right")] public int right { get { return 3; } }
        
        [JSProperty(Name = "y")] [PropertyDescription("Xbox: Y")] public int y { get { return 4; } }
        [JSProperty(Name = "a")] [PropertyDescription("Xbox: A")] public int a { get { return 5; } }
        [JSProperty(Name = "x")] [PropertyDescription("Xbox: X")] public int x { get { return 6; } }
        [JSProperty(Name = "b")] [PropertyDescription("Xbox: B")] public int b { get { return 7; } }
        [JSProperty(Name = "triangle")] [PropertyDescription("PlayStation: △")] public int triangle { get { return 4; } }
        [JSProperty(Name = "square")] [PropertyDescription("PlayStation: □")] public int square { get { return 5; } }
        [JSProperty(Name = "cross")] [PropertyDescription("PlayStation: X")] public int cross { get { return 6; } }
        [JSProperty(Name = "circle")] [PropertyDescription("PlayStation: ○")] public int circle { get { return 7; } }

        [JSProperty(Name = "leftbumper")] [PropertyDescription("Left bumper")] public int leftbumper { get { return 8; } }
        [JSProperty(Name = "rightbumper")] [PropertyDescription("Left bumper")] public int rightbumper { get { return 9; } }
        [JSProperty(Name = "lefttrigger")] [PropertyDescription("Left bumper")] public int lefttrigger { get { return 10; } }
        [JSProperty(Name = "righttrigger")] [PropertyDescription("Left bumper")] public int righttrigger { get { return 11; } }

        [JSProperty(Name = "select")] [PropertyDescription("Select")] public int select { get { return 12; } }
        [JSProperty(Name = "start")] [PropertyDescription("Start")] public int start { get { return 13; } }

        [JSProperty(Name = "leftthumbstick")] [PropertyDescription("Left thumbstick")] public int leftthumbstick { get { return 14; } }
        [JSProperty(Name = "rightthumbstick")] [PropertyDescription("Right thumbstick")] public int rightthumbstick { get { return 15; } }
    }
}