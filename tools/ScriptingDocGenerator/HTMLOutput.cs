using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptingDocGenerator
{
    class HTMLOutput
    {
        public static void OutputHTML(List<FunctionDefinition> functions)
        {
            var classLists = new Dictionary<string, List<FunctionDefinition>>();

            foreach (var function in functions)
            {
                if (!classLists.ContainsKey(function.Class))
                {
                    classLists.Add(function.Class, new List<FunctionDefinition>());
                }
                classLists[function.Class].Add(function);
            }

            List<string> output = new List<string>();
            output.Add("<html><head><link rel=\"stylesheet\" href=\"style.css\"></head><body>");
            foreach (var className in classLists.Keys)
            {
                output.Add($"<div class=\"class\">{className}");
                var funcList = classLists[className];
                foreach (var function in funcList)
                {
                    if (function.Name == "toLocaleString") continue;
                    output.Add($"<div class=\"function\">");
                    output.Add($"<div class=\"function_name\">{function.Name}</div>");
                    output.Add($"<div class=\"description\">{function.Description}</div>");
                    output.Add($"<div class=\"parameters\">");
                    foreach (var param in function.Parameters)
                    {
                        output.Add($"<div class=\"parameter_name\">{param.Name}</div>");
                        output.Add($"<div class=\"parameter_type\">{param.ArgType}</div>");
                        output.Add($"<div class=\"parameter_description\">{param.Description}</div>");
                    }
                    output.Add($"</div>");
                    output.Add($"<div class=\"return_type\">{function.ReturnType}</div>");
                    output.Add($"</div>");
                }
                output.Add($"</div>");
            }
            output.Add("</body></html>");

            File.WriteAllLines("docs.html", output);
        }
    }
}
