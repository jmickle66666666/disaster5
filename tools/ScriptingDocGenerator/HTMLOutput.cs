using System.Collections.Generic;
using System.IO;

namespace ScriptingDocGenerator
{
    class ClassList
    {
        public string className;
        public List<FunctionDefinition> functions;
        public List<PropertyDefinition> properties;

        public ClassList(string className)
        {
            this.className = className;
            functions = new List<FunctionDefinition>();
            properties = new List<PropertyDefinition>();
        }
    }
    class HTMLOutput
    {
        public static void OutputHTML(List<FunctionDefinition> functions, List<PropertyDefinition> properties, List<ClassDefinition> classes)
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
            output.Add("<html><head><link rel=\"stylesheet\" href=\"style.css\"></head><body>");
            foreach (var classDef in classes)
            {
                if (!classLists.ContainsKey(classDef.Name)) {
                    continue;
                }
                output.Add($"<div class=\"class\"><div class=\"classname\">{classDef.Name}</div><div class=\"classdescription\">{classDef.Description}</div>");
                var funcList = classLists[classDef.Name].functions;
                foreach (var function in funcList)
                {
                    if (function.Name == "toLocaleString") continue;
                    if (function.Name == "valueOf") continue;
                    output.Add(WriteFunction(function));
                }
                var propList = classLists[classDef.Name].properties;
                foreach (var property in propList)
                {
                    //if (function.Name == "toLocaleString") continue;
                    //if (function.Name == "valueOf") continue;
                    output.Add(WriteProperty(property));
                }
                output.Add($"</div>");
            }
            output.Add("</body></html>");

            File.WriteAllLines("docs.html", output);
        }

        static string WriteProperty(PropertyDefinition prop)
        {
            return $"<div class=\"function\"><div class=\"data\"><span class=\"type\">{prop.PropertyType}</span> {prop.Class}.{prop.Name};</div> <span class=\"description\">// {prop.Description}</span></div>";
        }

        static string WriteFunction(FunctionDefinition func)
        {
            string output = $"<details><summary><div class=\"function\"><div class=\"data\"><span class=\"type\">{func.ReturnType}</span> {func.Class}.{func.Name}(";
            for (var i = 0; i < func.Parameters.Count; i++)
            {
                //output += $"<span class=\"type\">{func.Parameters[i].ArgType}</span> {func.Parameters[i].Name}";
                output += $"{func.Parameters[i].Name}";
                if (i != func.Parameters.Count - 1)
                {
                    output += ", ";
                }
            }
            output += $");</div>";
            if (func.Description != null) { output += $"<div class=\"description\"> // {func.Description}</div>"; }
            output += $"</div></summary><div class=\"parameterDescriptions\">";
            for (var i = 0; i < func.Parameters.Count; i++)
            {
                output += $"<span class = \"parameter\"><span class=\"parameterInfo\"><span class=\"type\">{func.Parameters[i].ArgType}</span> {func.Parameters[i].Name}</span> <span class=\"description\">// {func.Parameters[i].Description}</span></span>";
                if (i != func.Parameters.Count - 1)
                {
                    output += "<br>";
                }
            }
            output += "</div></details>";
            return output;
        }
    }
}
