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
                output.Add($"<div class=\"class\"><div class=\"classname\">{className}</div>");
                var funcList = classLists[className];
                foreach (var function in funcList)
                {
                    if (function.Name == "toLocaleString") continue;
                    if (function.Name == "valueOf") continue;
                    output.Add(WriteFunction(function));
                }
                output.Add($"</div>");
            }
            output.Add("</body></html>");

            File.WriteAllLines("docs.html", output);
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
