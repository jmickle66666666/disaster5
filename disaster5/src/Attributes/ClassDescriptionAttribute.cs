using System;
using System.Collections.Generic;
using System.Text;

namespace DisasterAPI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClassDescriptionAttribute : Attribute
    {
        public string Description
        {
            get;
            private set;
        }
        public ClassDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
