//#define GET_JURASSIC_FUNCS

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Jurassic;
using Newtonsoft.Json;

namespace ScriptingDocGenerator
{
    class Program
    {
        static List<FunctionDefinition> Functions = new List<FunctionDefinition>();
        static void Main(string[] args)
        {
            Assembly asm = Assembly.LoadFrom("disaster5");

            foreach (var type in asm.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    Attribute attr = method.GetCustomAttribute(typeof(Jurassic.Library.JSFunctionAttribute));

                    if (attr != null)
                    {
                        var ja = ((Jurassic.Library.JSFunctionAttribute)attr);
                        Functions.Add(new FunctionDefinition(type, method));
                    }
                }
            }

#if GET_JURASSIC_FUNCS

            asm = Assembly.LoadFrom("Jurassic");

            foreach (var type in asm.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    Attribute attr = method.GetCustomAttribute(typeof(Jurassic.Library.JSFunctionAttribute));

                    if (attr != null)
                    {
                        var ja = ((Jurassic.Library.JSFunctionAttribute)attr);
                        Functions.Add(new FunctionDefinition(type, method));
                    }
                }
            }
#endif

            string ScriptDocJSON = JsonConvert.SerializeObject(Functions, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            HTMLOutput.OutputHTML(Functions);
            MarkdownOutput.OutputHTML(Functions);
            File.WriteAllText("ScriptDoc.json", ScriptDocJSON);
                

            Console.WriteLine("Finished processing");
        }
    }
}
