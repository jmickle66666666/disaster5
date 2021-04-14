using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptingDocGenerator
{
    [Serializable]
    public class FunctionArgumentDefinition
    {
        public string ArgType;
        public string Name;
        public string Description;
    }

    [Serializable]
    public class FunctionDefinition
    {
        public string ReturnType;
        public string Class;
        public string Name;
        public string Description;
        public List<FunctionArgumentDefinition> Parameters = new List<FunctionArgumentDefinition>();

        public FunctionDefinition(Type BaseClass, MethodInfo Method)
        {
            ReturnType = Method.ReturnType.Name;
            Class = BaseClass.Name;
            Name = Method.GetCustomAttribute<Jurassic.Library.JSFunctionAttribute>().Name;
            Description = Method.GetCustomAttribute<DisasterAPI.FunctionDescriptionAttribute>()?.Description;

            var argAttr = Method.GetCustomAttributes<DisasterAPI.ArgumentDescriptionAttribute>();

            foreach (var param in Method.GetParameters())
            {
                FunctionArgumentDefinition fad = new FunctionArgumentDefinition();
                fad.ArgType = param.ParameterType.Name;
                fad.Name = param.Name;

                foreach (var attr in argAttr)
                {
                    if (attr.ArgumentName == param.Name)
                    {
                        fad.Description = attr.Description;
                        if (attr.TypeOverride != "")
                        {
                            fad.ArgType = attr.TypeOverride;
                        }
                        break;
                    }
                }

                Parameters.Add(fad);
            }
        }
    }
}
