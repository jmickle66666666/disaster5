using System;
using System.Collections.Generic;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace DisasterAPI
{
    public class ShaderSystem : ObjectInstance 
    {
        public ShaderSystem(ScriptEngine engine)
            : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "RegisterShader")]
        public static void RegisterShader(string Name, FunctionInstance InitFunc, FunctionInstance FallbackFunc, FunctionInstance DrawFunc)
        {
            throw new NotImplementedException();
            //Disaster.ShaderSystem.RegisterShaderJS(Name, InitFunc, FallbackFunc, DrawFunc);
        }

        [JSFunction(Name = "RegisterBasicShader")]
        public static void RegisterBasicShader(string Name, string VertexShaderPath, string PixelShaderPath)
        {
            Disaster.ShaderSystem.RegisterBasicShaderJS(Name, VertexShaderPath, PixelShaderPath);
        }
    }
}
