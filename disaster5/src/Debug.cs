using System;
using System.Collections.Generic;
using System.Text;

namespace Disaster
{

    class Debug
    {
        public static bool enabled = false;
        static long frameStart;
        static long frameEnd;
        static Dictionary<string, long> labels;
        static List<int[]> frameHistory;
        static List<string> names;
        
        public static void FrameStart()
        {
            if (!enabled) return;
            if (labels == null)
            {
                labels = new Dictionary<string, long>();
            } else
            {
                labels.Clear();
            }
            frameStart = DateTime.UtcNow.Ticks;
        }

        public static void Label(string name)
        {
            if (!enabled) return;
            if (labels == null) return;
            labels.Add(name, DateTime.UtcNow.Ticks);
        }

        public static long FrameEnd()
        {
            if (!enabled) return 0;
            frameEnd = DateTime.UtcNow.Ticks - frameStart;
            return frameEnd;
        }

        public static Dictionary<string, double> GetFrameMSData()
        {
            if (!enabled) return null;
            if (labels == null) return null;

            if (names == null)
            {
                names = new List<string>();
            } else
            {
                names.Clear();
            }

            if (frameHistory == null)
            {
                frameHistory = new List<int[]>();
            }

            var output = new Dictionary<string, double>();
            output.Add("start", 0);

            List<int> framePart = new List<int>();

            foreach (var kvp in labels)
            {
                names.Add(kvp.Key);
                double ms = (kvp.Value - frameStart) / 10_000.0;
                framePart.Add((int)Math.Round(ms));
                output.Add(kvp.Key, ms);
            }
            framePart.Add((int)Math.Round(frameEnd / 10_000.0));
            output.Add("end", frameEnd / 10_000.0);

            frameHistory.Insert(0, framePart.ToArray());

            return output;
        }

        public static void DrawGraph()
        {
            if (!enabled) return;
            if (frameHistory == null) return;
            for (int i = 0; i < names.Count; i++)
            {
                SoftwareCanvas.FillRect(0, SoftwareCanvas.fontHeight * i, SoftwareCanvas.fontWidth * names[i].Length, SoftwareCanvas.fontHeight, new Color32(0, 0, 0));
                SoftwareCanvas.Text(0, SoftwareCanvas.fontHeight * i, colors[i % colors.Length], names[i]);
            }

            for (int i = 0; i < frameHistory.Count; i++)
            {
                int y = 240;
                for (int j = 0; j < frameHistory[i].Length; j++)
                {
                    int last;
                    if (j == 0) {
                        last = 0;
                    } else
                    {
                        last = frameHistory[i][j - 1];
                    }

                    if (Math.Abs(last - frameHistory[i][j]) < 2) continue;

                    SoftwareCanvas.Line(new System.Numerics.Vector2(i, y - last), new System.Numerics.Vector2(i, y - frameHistory[i][j]), colors[j % colors.Length]);
                }
            }
        }

        static Color32[] colors = new Color32[]
        {
            new Color32(255, 128, 0),
            new Color32(255, 128, 255),
            new Color32(128, 128, 128),
            new Color32(128, 255, 0),
            new Color32(64, 64, 64),
            new Color32(0, 255, 128),
            new Color32(128, 0, 255),
            new Color32(0, 128, 255),
            new Color32(255, 0, 128),
        };
    }
}
