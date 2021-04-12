using System;
using System.Collections.Generic;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace Disaster
{
    public class ShaderSystem
    {
        static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();

        public static void Init()
        {
        
        }
        public static void RegisterBasicShaderJS(string Name, string VertexShaderPath, string PixelShaderPath)
        {
            Shader newShader = new Shader(Name, VertexShaderPath, PixelShaderPath);

            newShader.Compile();

            Shaders.Add(Name, newShader);
        }
    }
}
