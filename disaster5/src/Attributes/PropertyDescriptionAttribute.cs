using System;
using System.Collections.Generic;
using System.Text;

namespace DisasterAPI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyDescriptionAttribute : Attribute
    {
        public string Description
        {
            get;
            private set;
        }
        public string PropertyTypeOverride
        {
            get;
            private set;
        }
        public PropertyDescriptionAttribute(string description, string propertyType = "")
        {
            Description = description;
            PropertyTypeOverride = propertyType;
        }
    }
}
