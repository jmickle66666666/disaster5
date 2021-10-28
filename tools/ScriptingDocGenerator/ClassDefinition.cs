using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptingDocGenerator
{

    [Serializable]
    public class ClassDefinition
    {
        public string Name;
        public string Description;

        public ClassDefinition(Type BaseClass)
        {
            Name = BaseClass.Name;
            if (BaseClass.GetCustomAttribute<DisasterAPI.ClassDescriptionAttribute>() != null)
            {
                Description = BaseClass.GetCustomAttribute<DisasterAPI.ClassDescriptionAttribute>().Description;
            }
        }
    }
}
