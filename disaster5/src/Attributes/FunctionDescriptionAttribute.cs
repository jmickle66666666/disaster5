using System;
using System.Collections.Generic;
using System.Text;

namespace Disaster
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionDescriptionAttribute : Attribute
    {
        public string Description
        {
            get;
            private set;
        }
        public FunctionDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
