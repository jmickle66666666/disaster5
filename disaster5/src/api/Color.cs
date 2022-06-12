using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    [ClassDescription("A list of handy colors to use!")]
    public class Color : ObjectInstance
    {
        public Color(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSProperty(Name = "black")] 
        [PropertyDescription("black", "{r, g, b, a}")] 
        public ObjectInstance black { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0, 0, 0)); } }

        [JSProperty(Name = "gray")] 
        [PropertyDescription("gray", "{r, g, b, a}")] 
        public ObjectInstance gray { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x9d, 0x9d, 0x9d)); } }

        [JSProperty(Name = "darkgray")]
        [PropertyDescription("dark gray", "{r, g, b, a}")]
        public ObjectInstance darkgray { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x24, 0x24, 0x20)); } }

        [JSProperty(Name = "white")] 
        [PropertyDescription("white", "{r, g, b, a}")] 
        public ObjectInstance white { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xff, 0xff, 0xff)); } }

        [JSProperty(Name = "red")] 
        [PropertyDescription("red", "{r, g, b, a}")] 
        public ObjectInstance red { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xbe, 0x26, 0x33)); } }

        [JSProperty(Name = "meat")] 
        [PropertyDescription("meat", "{r, g, b, a}")] 
        public ObjectInstance meat { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xe0, 0x6f, 0x8b)); } }

        [JSProperty(Name = "darkbrown")] 
        [PropertyDescription("dark brown", "{r, g, b, a}")] 
        public ObjectInstance darkbrown { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x49, 0x3c, 0x2b)); } }

        [JSProperty(Name = "brown")] 
        [PropertyDescription("brown", "{r, g, b, a}")] 
        public ObjectInstance brown { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xa4, 0x64, 0x22)); } }

        [JSProperty(Name = "orange")] 
        [PropertyDescription("orange", "{r, g, b, a}")] 
        public ObjectInstance orange { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xeb, 0x89, 0x31)); } }

        [JSProperty(Name = "yellow")] 
        [PropertyDescription("yellow", "{r, g, b, a}")] 
        public ObjectInstance yellow { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xf7, 0xe2, 0x6b)); } }

        [JSProperty(Name = "darkgreen")] 
        [PropertyDescription("dark green", "{r, g, b, a}")] 
        public ObjectInstance darkgreen { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x2f, 0x48, 0x4e)); } }

        [JSProperty(Name = "green")] 
        [PropertyDescription("green", "{r, g, b, a}")] 
        public ObjectInstance green { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x44, 0x89, 0x1a)); } }

        [JSProperty(Name = "slimegreen")] 
        [PropertyDescription("slime green", "{r, g, b, a}")] 
        public ObjectInstance slimegreen { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xa3, 0xce, 0x27)); } }

        [JSProperty(Name = "nightblue")] 
        [PropertyDescription("night blue", "{r, g, b, a}")] 
        public ObjectInstance nightblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x1b, 0x26, 0x32)); } }

        [JSProperty(Name = "seablue")] 
        [PropertyDescription("sea blue", "{r, g, b, a}")] 
        public ObjectInstance seablue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x00, 0x57, 0x84)); } }

        [JSProperty(Name = "skyblue")] 
        [PropertyDescription("sky blue", "{r, g, b, a}")] 
        public ObjectInstance skyblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x31, 0xa2, 0xf2)); } }

        [JSProperty(Name = "cloudblue")] 
        [PropertyDescription("cloud blue", "{r, g, b, a}")] 
        public ObjectInstance cloudblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xb2, 0xdc, 0xef)); } }

        [JSProperty(Name = "disaster")]
        [PropertyDescription("disaster engine brand yellow", "{r, g, b, a}")]
        public ObjectInstance disaster { get { return Disaster.TypeInterface.Object(new Disaster.Color32(255, 180, 0x0)); } }

    }
}