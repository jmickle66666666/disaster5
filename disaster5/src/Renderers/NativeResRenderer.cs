using System;
using System.Collections.Generic;

namespace Disaster
{
    public static class NativeResRenderer
    {
        private static List<Action> drawQueue;

        public static void Enqueue(Action renderAction)
        {
            drawQueue ??= new List<Action>();
            drawQueue.Add(renderAction);
        }

        public static void RenderQueue()
        {
            drawQueue ??= new List<Action>();
            foreach (var t in drawQueue)
                t.Invoke();
            drawQueue.Clear();
        }
    }
}
