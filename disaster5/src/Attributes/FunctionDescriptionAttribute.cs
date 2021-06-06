using System;
using System.Collections.Generic;
using System.Text;

namespace DisasterAPI
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FunctionDescriptionAttribute : Attribute
    {
        public string Description
        {
            get;
            private set;
        }
        public string ReturnTypeOverride
        {
            get;
            private set;
        }
        public FunctionDescriptionAttribute(string description, string returnType = "")
        {
            Description = description;
            ReturnTypeOverride = returnType;
        }
    }
}
