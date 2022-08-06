using System;
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
    
    public static class ShapeRenderer
    {
        public static BlendMode blendMode { get; set; }
        private static List<Action> drawQueue;

        public static void EnqueueRender(Action renderAction)
        {
            drawQueue ??= new List<Action>();
            drawQueue.Add(renderAction);
        }

        public static void RenderQueue()
        {
            Raylib.BeginBlendMode(blendMode);
            drawQueue ??= new List<Action>();
            foreach (var action in drawQueue)
                action.Invoke();
            drawQueue.Clear();
            Raylib.EndBlendMode();
        }
    }
}
