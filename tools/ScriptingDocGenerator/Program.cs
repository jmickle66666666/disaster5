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
        static List<PropertyDefinition> Properties = new List<PropertyDefinition>();
        static List<ClassDefinition> Classes = new List<ClassDefinition>();
        static void Main(string[] args)
        {
            Assembly asm = Assembly.LoadFrom("disaster5.dll");

            foreach (var type in asm.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    Attribute attr = method.GetCustomAttribute(typeof(Jurassic.Library.JSFunctionAttribute));

                    if (attr != null)
                    {
                        Functions.Add(new FunctionDefinition(type, method));
                    }
                }

                foreach (var property in type.GetProperties())
                {
                    Attribute attr = property.GetCustomAttribute<Jurassic.Library.JSPropertyAttribute>();
                    if (attr != null)
                    {
                        Properties.Add(new PropertyDefinition(type, property));
                    }
                }

                Attribute classattr = type.GetCustomAttribute<DisasterAPI.ClassDescriptionAttribute>();
                if (classattr != null)
                {
                    Classes.Add(new ClassDefinition(type));
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

            // Setup output directory
            string baseDir = "docs";
            if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
            
            HTMLOutput.OutputHTML(Functions, Properties, Classes, baseDir);
            TypeScriptDeclOutput.Output(Functions, Properties, Classes, baseDir);
            MarkdownOutput.OutputHTML(Functions, baseDir);
            File.WriteAllText("ScriptDoc.json", ScriptDocJSON);

            Console.WriteLine("Finished processing");
        }
    }
}
