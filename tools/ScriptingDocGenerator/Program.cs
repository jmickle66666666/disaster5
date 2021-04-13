using System;
using System.Reflection;
using Jurassic;

namespace ScriptingDocGenerator
{
    class Program
    {
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
                        Console.WriteLine($"Script Func: {method.ReturnType.Name} { type.Name }.{ ja.Name }");
                        
                        foreach (var arg in method.GetParameters())
                        {
                            Console.WriteLine($"    {arg.ParameterType.Name} {arg.Name}");
                        }
                    }
                }
            }
                

            Console.WriteLine("Hello World!");
        }
    }
}
