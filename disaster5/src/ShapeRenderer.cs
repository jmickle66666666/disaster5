using System;
using System.Collections.Generic;

namespace Disaster
{
    class ShapeRenderer
    {
        private static List<Action> drawQueue;

        public static void EnqueueRender(Action renderAction)
        {
            if (drawQueue == null) drawQueue = new List<Action>();
            drawQueue.Add(renderAction);
        }

        public static void RenderQueue()
        {
            for (var i = 0; i < drawQueue.Count; i++)
            {
                drawQueue[i].Invoke();
            }
            drawQueue.Clear();
        }
    }
}
