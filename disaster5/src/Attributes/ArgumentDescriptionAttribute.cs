using System;
using System.Collections.Generic;
using System.Text;

namespace Disaster
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgumentDescriptionAttribute : Attribute
    {
        public string ArgumentName
        {
            get;
            private set;
        }
        public string Description
        {
            get;
            private set;
        }
        public string TypeOverride
        {
            get;
            private set;
        }
        public ArgumentDescriptionAttribute(string argName, string description, string typeOverride = "")
        {
            ArgumentName = argName;
            Description = description;
            TypeOverride = typeOverride;
        }
    }
}
