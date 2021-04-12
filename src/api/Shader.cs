using System;
using System.Collections.Generic;
using System.Text;
using Jurassic;
using Jurassic.Library;
using System.IO;


namespace Disaster
{

    public class Shader
    {
        public OpenGL.ShaderProgram ShaderProgram;

        List<string> ShaderParams = new List<string>();

        string Name = "None";

        bool IsSimple = true;

        // DO NOT USE
        public Shader(string name, FunctionInstance InitFunc, FunctionInstance FallbackFunc, FunctionInstance DrawFunc)
        {
            throw new NotImplementedException();
            /*
            Name = name;
            Init = InitFunc;
            Fallback = FallbackFunc;
            Draw = DrawFunc;

            IsSimple = false;
            */
        }

        public Shader(string name, string vertexShaderPath, string pixelShaderPath)
        {
            Name = name;
            VertexShaderPath = Assets.LoadPath(vertexShaderPath);
            PixelShaderPath = Assets.LoadPath(pixelShaderPath);
        }

        public bool Compile()
        {
            if (!File.Exists(VertexShaderPath))
            {
                Console.WriteLine("ERROR!!!");
                Console.WriteLine(String.Format("Vertex Shader {0} in {1} does not exist", VertexShaderPath, Name));
                return false;
            }

            if (!File.Exists(PixelShaderPath))
            {
                Console.WriteLine("ERROR!!!");
                Console.WriteLine(String.Format("Pixel Shader {0} in {1} does not exist", PixelShaderPath, Name));
                return false;
            }

            string VSource = File.ReadAllText(VertexShaderPath);
            string PSource = File.ReadAllText(PixelShaderPath);

            try
            {
                ShaderProgram = new OpenGL.ShaderProgram(VSource, PSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR!!!");
                Console.WriteLine(e.Message);

                Console.WriteLine(String.Format("The {0} shader has failed to compile", Name));
                Console.WriteLine(String.Format("Vertex: {0}", VertexShaderPath));
                Console.WriteLine(String.Format("Pixel: {0}", PixelShaderPath));

                Environment.FailFast("Failed Shader Compile", e);
            }

            return false;
        }

        string VertexShaderPath;
        string PixelShaderPath;

        Jurassic.Library.FunctionInstance Init;
        Jurassic.Library.FunctionInstance Fallback;
        Jurassic.Library.FunctionInstance Draw;

    }
}
