using Jurassic;
using Jurassic.Library;
namespace DisasterAPI
{
    public class Color : ObjectInstance
    {
        public Color(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSProperty(Name = "black")] 
        [PropertyDescription("black")] 
        public ObjectInstance black { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0, 0, 0)); } }

        [JSProperty(Name = "gray")] 
        [PropertyDescription("gray")] 
        public ObjectInstance gray { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x9d, 0x9d, 0x9d)); } }

        [JSProperty(Name = "white")] 
        [PropertyDescription("white")] 
        public ObjectInstance white { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xff, 0xff, 0xff)); } }

        [JSProperty(Name = "red")] 
        [PropertyDescription("red")] 
        public ObjectInstance red { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xbe, 0x26, 0x33)); } }

        [JSProperty(Name = "meat")] 
        [PropertyDescription("meat")] 
        public ObjectInstance meat { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xe0, 0x6f, 0x8b)); } }

        [JSProperty(Name = "darkbrown")] 
        [PropertyDescription("darkbrown")] 
        public ObjectInstance darkbrown { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x49, 0x3c, 0x2b)); } }

        [JSProperty(Name = "brown")] 
        [PropertyDescription("brown")] 
        public ObjectInstance brown { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xa4, 0x64, 0x22)); } }

        [JSProperty(Name = "orange")] 
        [PropertyDescription("orange")] 
        public ObjectInstance orange { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xeb, 0x89, 0x31)); } }

        [JSProperty(Name = "yellow")] 
        [PropertyDescription("yellow")] 
        public ObjectInstance yellow { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xf7, 0xe2, 0x6b)); } }

        [JSProperty(Name = "darkgreen")] 
        [PropertyDescription("darkgreen")] 
        public ObjectInstance darkgreen { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x2f, 0x48, 0x4e)); } }

        [JSProperty(Name = "green")] 
        [PropertyDescription("green")] 
        public ObjectInstance green { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x44, 0x89, 0x1a)); } }

        [JSProperty(Name = "slimegreen")] 
        [PropertyDescription("slimegreen")] 
        public ObjectInstance slimegreen { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xa3, 0xce, 0x27)); } }

        [JSProperty(Name = "nightblue")] 
        [PropertyDescription("nightblue")] 
        public ObjectInstance nightblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x1b, 0x26, 0x32)); } }

        [JSProperty(Name = "seablue")] 
        [PropertyDescription("seablue")] 
        public ObjectInstance seablue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x00, 0x57, 0x84)); } }

        [JSProperty(Name = "skyblue")] 
        [PropertyDescription("skyblue")] 
        public ObjectInstance skyblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0x31, 0xa2, 0xf2)); } }

        [JSProperty(Name = "cloudblue")] 
        [PropertyDescription("cloudblue")] 
        public ObjectInstance cloudblue { get { return Disaster.TypeInterface.Object(new Disaster.Color32(0xb2, 0xdc, 0xef)); } }

        [JSProperty(Name = "disaster")]
        [PropertyDescription("disaster engine brand yellow")]
        public ObjectInstance disaster { get { return Disaster.TypeInterface.Object(new Disaster.Color32(255, 180, 0x0)); } }

    }
}