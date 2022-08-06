using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptingDocGenerator
{
    public static class TypeScriptDeclOutput
    {
        public static void Output(
            List<FunctionDefinition> functions, List<PropertyDefinition> properties, List<ClassDefinition> classes, string baseDir)
        {
            var classLists = new Dictionary<string, ClassList>();
            
            foreach (var function in functions)
            {
                if (!classLists.ContainsKey(function.Class))
                {
                    classLists.Add(function.Class, new ClassList(function.Class));
                }
                classLists[function.Class].functions.Add(function);
            }
            
            foreach (var property in properties)
            {
                if (!classLists.ContainsKey(property.Class))
                {
                    classLists.Add(property.Class, new ClassList(property.Class));
                }
                classLists[property.Class].properties.Add(property);
            }
            
            List<string> output = new List<string>();
            foreach (var classDef in classes)
            {
                if (!classLists.ContainsKey(classDef.Name)) continue;
                
                var classList = classLists[classDef.Name];
                var funcList = classList.functions;
                var propList = classList.properties;
                
                output.Add($"declare namespace {classDef.Name} {{");
                foreach (var function in funcList)
                {
                    if (function.Name == "toLocaleString") continue;
                    if (function.Name == "valueOf") continue;
                    output.Add(WriteFunction(function));
                }
                foreach (var property in propList)
                    output.Add(WriteProperty(property));
                output.Add("}\n");
            }

            File.WriteAllLines(Path.Combine(baseDir, "disaster5.d.ts"), output);
        }

        static string WriteProperty(PropertyDefinition prop)
        {
            string output = String.Empty;
            
            // JSDoc
            output += $"    /**\n";
            output += $"     * {prop.Description}\n";
            output += $"     */\n";
            
            // Declaration
            output += $"    export var {prop.Name}: {prop.PropertyType};\n";
            
            return output;
        }

        static string WriteFunction(FunctionDefinition func)
        {
            string output = string.Empty;
            
            // JSDoc
            output += $"    /**\n";
            output += $"     * {func.Description}\n";
            foreach (var param in func.Parameters)
                output += $"     * @param {{{param.ArgType}}} {param.Name} - {param.Description}\n";
            output += $"     */\n";
            
            // Declaration
            output += $"    export function {func.Name}(";
            for (var i = 0; i < func.Parameters.Count; i++)
            {
                var param = func.Parameters[i];
                output += $"{param.Name}: {param.ArgType}";
                if (i != func.Parameters.Count - 1)
                    output += ", ";
                    
            }

            output += $"): {func.ReturnType};\n";
            return output;
        }
    }
}