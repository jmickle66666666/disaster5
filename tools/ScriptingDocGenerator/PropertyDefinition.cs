using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptingDocGenerator
{

    [Serializable]
    public class PropertyDefinition
    {
        public string PropertyType;
        public string Class;
        public string Name;
        public string Description;

        public PropertyDefinition(Type BaseClass, PropertyInfo Property)
        {
            PropertyType = Property.PropertyType.Name;
            if (Property.GetCustomAttribute<DisasterAPI.PropertyDescriptionAttribute>()?.PropertyTypeOverride != "")
            {
                PropertyType = Property.GetCustomAttribute<DisasterAPI.PropertyDescriptionAttribute>()?.PropertyTypeOverride;
            }
            Class = BaseClass.Name;
            Name = Property.GetCustomAttribute<Jurassic.Library.JSPropertyAttribute>().Name;
            Description = Property.GetCustomAttribute<DisasterAPI.PropertyDescriptionAttribute>()?.Description;
        }
    }
}
