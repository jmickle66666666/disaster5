using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    public class Key : ObjectInstance
    {
        public Key(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }
        [JSProperty(Name ="up")] [PropertyDescription("up")] public int up { get { return 0; } }
        [JSProperty(Name = "down")] [PropertyDescription("down")] public int down { get { return 1; } }
        [JSProperty(Name = "left")] [PropertyDescription("left")] public int left { get { return 2; } }
        [JSProperty(Name = "right")] [PropertyDescription("right")] public int right { get { return 3; } }
        [JSProperty(Name = "a")] [PropertyDescription("a")] public int a { get { return 4; } }
        [JSProperty(Name = "b")] [PropertyDescription("b")] public int b { get { return 5; } }
        [JSProperty(Name = "c")] [PropertyDescription("c")] public int c { get { return 6; } }
        [JSProperty(Name = "d")] [PropertyDescription("d")] public int d { get { return 7; } }
        [JSProperty(Name = "e")] [PropertyDescription("e")] public int e { get { return 8; } }
        [JSProperty(Name = "f")] [PropertyDescription("f")] public int f { get { return 9; } }
        [JSProperty(Name = "g")] [PropertyDescription("g")] public int g { get { return 10; } }
        [JSProperty(Name = "h")] [PropertyDescription("h")] public int h { get { return 11; } }
        [JSProperty(Name = "i")] [PropertyDescription("i")] public int i { get { return 12; } }
        [JSProperty(Name = "j")] [PropertyDescription("j")] public int j { get { return 13; } }
        [JSProperty(Name = "k")] [PropertyDescription("k")] public int k { get { return 14; } }
        [JSProperty(Name = "l")] [PropertyDescription("l")] public int l { get { return 15; } }
        [JSProperty(Name = "m")] [PropertyDescription("m")] public int m { get { return 16; } }
        [JSProperty(Name = "n")] [PropertyDescription("n")] public int n { get { return 17; } }
        [JSProperty(Name = "o")] [PropertyDescription("o")] public int o { get { return 18; } }
        [JSProperty(Name = "p")] [PropertyDescription("p")] public int p { get { return 19; } }
        [JSProperty(Name = "q")] [PropertyDescription("q")] public int q { get { return 20; } }
        [JSProperty(Name = "r")] [PropertyDescription("r")] public int r { get { return 21; } }
        [JSProperty(Name = "s")] [PropertyDescription("s")] public int s { get { return 22; } }
        [JSProperty(Name = "t")] [PropertyDescription("t")] public int t { get { return 23; } }
        [JSProperty(Name = "u")] [PropertyDescription("u")] public int u { get { return 24; } }
        [JSProperty(Name = "v")] [PropertyDescription("v")] public int v { get { return 25; } }
        [JSProperty(Name = "w")] [PropertyDescription("w")] public int w { get { return 26; } }
        [JSProperty(Name = "x")] [PropertyDescription("x")] public int x { get { return 27; } }
        [JSProperty(Name = "y")] [PropertyDescription("y")] public int y { get { return 28; } }
        [JSProperty(Name = "z")] [PropertyDescription("z")] public int z { get { return 29; } }
        [JSProperty(Name = "key0")] [PropertyDescription("key0")] public int key0 { get { return 30; } }
        [JSProperty(Name = "key1")] [PropertyDescription("key1")] public int key1 { get { return 31; } }
        [JSProperty(Name = "key2")] [PropertyDescription("key2")] public int key2 { get { return 32; } }
        [JSProperty(Name = "key3")] [PropertyDescription("key3")] public int key3 { get { return 33; } }
        [JSProperty(Name = "key4")] [PropertyDescription("key4")] public int key4 { get { return 34; } }
        [JSProperty(Name = "key5")] [PropertyDescription("key5")] public int key5 { get { return 35; } }
        [JSProperty(Name = "key6")] [PropertyDescription("key6")] public int key6 { get { return 36; } }
        [JSProperty(Name = "key7")] [PropertyDescription("key7")] public int key7 { get { return 37; } }
        [JSProperty(Name = "key8")] [PropertyDescription("key8")] public int key8 { get { return 38; } }
        [JSProperty(Name = "key9")] [PropertyDescription("key9")] public int key9 { get { return 39; } }
        [JSProperty(Name = "f1")] [PropertyDescription("f1")] public int f1 { get { return 40; } }
        [JSProperty(Name = "f2")] [PropertyDescription("f2")] public int f2 { get { return 41; } }
        [JSProperty(Name = "f3")] [PropertyDescription("f3")] public int f3 { get { return 42; } }
        [JSProperty(Name = "f4")] [PropertyDescription("f4")] public int f4 { get { return 43; } }
        [JSProperty(Name = "f5")] [PropertyDescription("f5")] public int f5 { get { return 44; } }
        [JSProperty(Name = "f6")] [PropertyDescription("f6")] public int f6 { get { return 45; } }
        [JSProperty(Name = "f7")] [PropertyDescription("f7")] public int f7 { get { return 46; } }
        [JSProperty(Name = "f8")] [PropertyDescription("f8")] public int f8 { get { return 47; } }
        [JSProperty(Name = "f9")] [PropertyDescription("f9")] public int f9 { get { return 48; } }
        [JSProperty(Name = "f10")] [PropertyDescription("f10")] public int f10 { get { return 49; } }
        [JSProperty(Name = "f11")] [PropertyDescription("f11")] public int f11 { get { return 50; } }
        [JSProperty(Name = "f12")] [PropertyDescription("f12")] public int f12 { get { return 51; } }
        [JSProperty(Name = "escape")] [PropertyDescription("escape")] public int escape { get { return 52; } }
        [JSProperty(Name = "return")] [PropertyDescription("return")] public int Return { get { return 53; } }
        [JSProperty(Name = "backspace")] [PropertyDescription("backspace")] public int backspace { get { return 54; } }
        [JSProperty(Name = "tab")] [PropertyDescription("tab")] public int tab { get { return 55; } }
        [JSProperty(Name = "space")] [PropertyDescription("space")] public int space { get { return 56; } }
        [JSProperty(Name = "leftbracket")] [PropertyDescription("leftbracket")] public int leftbracket { get { return 57; } }
        [JSProperty(Name = "rightbracket")] [PropertyDescription("rightbracket")] public int rightbracket { get { return 58; } }
        [JSProperty(Name = "printscreen")] [PropertyDescription("printscreen")] public int printscreen { get { return 59; } }
        [JSProperty(Name = "scrollock")] [PropertyDescription("scrollock")] public int scrollock { get { return 60; } }
        [JSProperty(Name = "pause")] [PropertyDescription("pause")] public int pause { get { return 61; } }
        [JSProperty(Name = "insert")] [PropertyDescription("insert")] public int insert { get { return 62; } }
        [JSProperty(Name = "home")] [PropertyDescription("home")] public int home { get { return 63; } }
        [JSProperty(Name = "pageup")] [PropertyDescription("pageup")] public int pageup { get { return 64; } }
        [JSProperty(Name = "delete")] [PropertyDescription("delete")] public int delete { get { return 65; } }
        [JSProperty(Name = "end")] [PropertyDescription("end")] public int end { get { return 66; } }
        [JSProperty(Name = "pagedown")] [PropertyDescription("pagedown")] public int pagedown { get { return 67; } }
        [JSProperty(Name = "leftcontrol")] [PropertyDescription("leftcontrol")] public int leftcontrol { get { return 68; } }
        [JSProperty(Name = "rightcontrol")] [PropertyDescription("rightcontrol")] public int rightcontrol { get { return 69; } }
        [JSProperty(Name = "leftalt")] [PropertyDescription("leftalt")] public int leftalt { get { return 70; } }
        [JSProperty(Name = "rightalt")] [PropertyDescription("rightalt")] public int rightalt { get { return 71; } }
        [JSProperty(Name = "leftshift")] [PropertyDescription("leftshift")] public int leftshift { get { return 72; } }
        [JSProperty(Name = "rightshift")] [PropertyDescription("rightshift")] public int rightshift { get { return 73; } }
    }
}