using System;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace Disaster
{
    public struct ShaderParameter
    {
        public ShaderUniformDataType dataType;
        public string uniformName;
        public object uniformValue;
        public ShaderParameter(string name, object value, ShaderUniformDataType type)
        {
            dataType = type;
            uniformName = name;
            uniformValue = value;
        }
    }

    public struct RenderQueueElement
    {
        public Model model;
        public Shader shader;
        public Transformation transform;
        public ShaderParameter[] shaderParameters;
        public bool wireframe;
    }

    public struct ModelRenderer : Renderer
    {
        public Model model;
        public Shader shader;
        public Texture2D texture;
        public Matrix4x4 transform;

        public ModelRenderer(Model model, Shader shader, Texture2D texture)
        {
            this.model = model;
            this.shader = shader;
            this.texture = texture;

            transform = Matrix4x4.CreateTranslation(new Vector3(0, 0, -5));
        }

        public void Render()
        {
            
        }

        public static RenderQueueElement[] renderQueue;
        public static int renderQueueLength;

        public static void EnqueueRender(Model model, Shader shader, Transformation transform)
        {
            EnqueueRender(model, shader, transform, new ShaderParameter[0]);
        }

        public static void EnqueueRender(Model model, Shader shader, Transformation transform, ShaderParameter[] shaderParameters)
        {
            if (renderQueue == null)
            {
                renderQueue = new RenderQueueElement[16];
            }

            renderQueue[renderQueueLength] = new RenderQueueElement() { model = model, shader = shader, transform = transform, shaderParameters = shaderParameters};
            renderQueueLength += 1;
            if (renderQueueLength >= renderQueue.Length)
            {
                Array.Resize(ref renderQueue, renderQueue.Length + 16);
            }
        }

        public static void RenderQueue()
        {
            Raylib.BeginMode3D(ScreenController.camera);
            for (int i = 0; i < renderQueueLength; i++)
            {
                var t = (float) Raylib.GetTime();
                Raylib.SetShaderValue(renderQueue[i].shader, Raylib.GetShaderLocation(renderQueue[i].shader, "time"), ref t, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

                for (int j = 0; j < renderQueue[i].shaderParameters.Length; j++)
                {
                    var shaderParam = renderQueue[i].shaderParameters[j];
                    switch (shaderParam.dataType)
                    {
                        case ShaderUniformDataType.SHADER_UNIFORM_FLOAT:
                            float valuef = (float) shaderParam.uniformValue;
                            Raylib.SetShaderValue(renderQueue[i].shader, Raylib.GetShaderLocation(renderQueue[i].shader, shaderParam.uniformName), ref valuef, shaderParam.dataType);
                            break;
                        case ShaderUniformDataType.SHADER_UNIFORM_SAMPLER2D:
                            Texture2D valuet = (Texture2D)shaderParam.uniformValue;
                            Raylib.SetShaderValueTexture(renderQueue[i].shader, Raylib.GetShaderLocation(renderQueue[i].shader, shaderParam.uniformName), valuet);
                            break;
                        // TODO:
                        //case ShaderUniformDataType.SHADER_UNIFORM_VEC4:
                        //    IntPtr valuev = (IntPtr)shaderParam.uniformValue;
                        //    Raylib.SetShaderValueV(renderQueue[i].shader, Raylib.GetShaderLocation(renderQueue[i].shader, shaderParam.uniformName), valuev, shaderParam.dataType, 4);
                        //    break;
                    }

                }

                SetMaterialShader(ref renderQueue[i].model, 0, ref renderQueue[i].shader);

                Raylib.DrawModelEx(
                    renderQueue[i].model,
                    renderQueue[i].transform.position,
                    renderQueue[i].transform.rotationAxis,
                    renderQueue[i].transform.rotationAngle,
                    renderQueue[i].transform.scale,
                    new Color(255, 255, 255, 255)
                );
            }
            //Raylib.EndShaderMode();
            renderQueueLength = 0;
            Raylib.EndMode3D();
        }
        
        public void Dispose()
        {
            
        }

        public unsafe static void SetMaterialShader(ref Model model, int materialIndex, ref Shader shader)
        {
            Material* materials = (Material*)model.materials.ToPointer();
            materials[materialIndex].shader = shader;
        }
    }

}