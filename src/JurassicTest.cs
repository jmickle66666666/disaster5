// js scripting test

using Jurassic;
namespace Disaster {
    public class JurassicTest {

        ScriptEngine engine;
        // string test;

        public JurassicTest() {
            engine = new ScriptEngine();
            
            engine.SetGlobalValue("fart", 5);

            // engine.SetGlobalFunction("welp", new System.Action<string>((a) => { System.Console.WriteLine(a); }));
            engine.SetGlobalFunction("text", new System.Action<string>((a) => { Text(a); }));
            // engine.Evaluate("welp('wow');");
            
        }

        void Text(string text)
        {
            Draw.Paragraph(5, 5 + Draw.fontHeight, new Color32(255, 128, 0), text);
        }

        public void Update() {
            Draw.Text(5, 5, new Color32(255, 128, 0), "JURASSIC TEST");
            engine.Evaluate("text('HELLO FROM JS');");
        }
    }
}