using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptingDocGenerator
{
    class MarkdownOutput
    {
        public static void OutputHTML(List<FunctionDefinition> scriptdoc)
        {
            // Get list of all classes and the methods they use
            Dictionary<string, List<FunctionDefinition>> functions = new Dictionary<string, List<FunctionDefinition>>();
            foreach (var func in scriptdoc)
            {
                if (!functions.ContainsKey(func.Class))
                {
                    functions.Add(func.Class, new List<FunctionDefinition>() { func });
                }
                else
                {
                    functions[func.Class].Add(func);
                }
            }

            // Generate class pages
            var page_classes = new StringBuilder("---\nlayout: default\ntitle: Disaster Engine API\n---\n# Disaster Engine API\n");
            var pages = new Dictionary<string, StringBuilder>();
            foreach (var k in functions.Keys)
            {
                page_classes.AppendLine($"- [{ k }](class_{ k.ToLower() }.html)");

                // Create class page
                var page_content = new StringBuilder($"---\nlayout: default\ntitle: { k }\n---\n# { k }\n[Back](index.html)\n");
                var funs = functions[k];
                foreach (var fun in funs)
                {
                    page_content.Append($"## **{ fun.ReturnType }** { fun.Name }(");
                    var has_params = fun.Parameters.Count > 0;
                    if (has_params)
                    {
                        var first = true;
                        foreach (var param in fun.Parameters)
                        {
                            if (!first)
                            {
                                page_content.Append(", ");
                            }
                            else
                            {
                                first = false;
                            }
                            page_content.Append(param.Name);
                        }
                    }
                    page_content.AppendLine(")");

                    if (!String.IsNullOrWhiteSpace(fun.Description))
                    {
                        page_content.AppendLine($"*{ fun.Description }*");
                    }
                    if (has_params)
                    {
                        foreach (var param in fun.Parameters)
                        {
                            page_content.Append($"- { param.ArgType } **{ param.Name }**");
                            if (!String.IsNullOrWhiteSpace(param.Description))
                            {
                                page_content.Append($" - { param.Description }");
                            }
                            page_content.AppendLine();
                        }
                    }
                    page_content.AppendLine();
                }
                pages.Add(k, page_content);
            }

            //Save to files
            string baseDir = "docs";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            File.WriteAllText(Path.Combine(baseDir, "index.md"), page_classes.ToString());
            foreach (var page in pages)
            {
                Console.WriteLine($"Creating page {page.Key}");
                File.WriteAllText(Path.Combine(baseDir, $"class_{page.Key.ToLower()}.md"), page.Value.ToString());
            }
        }
    }
}