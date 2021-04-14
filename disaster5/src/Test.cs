// software renderer test

using System;
using System.Numerics;
using SDL2;
namespace Disaster {
    public class Test {
        Vector2 center = new Vector2(160, 120);
        Vector2 rotation = new Vector2(0, 0);
        Vector2 offset = new Vector2(0, 0);

        Vector2[][] shapes;

        string[] message = new string[] {
            "WELCOME TO ZONE",
            "INITIALISING [...........]",
            "C# ENGINE REWRITE IN PROGRESS",
            "FUCKING AROUND WITH SDL 2.0"
        };


        public Test() {
            // Vector2[] outline = 

            shapes = new Vector2[][] {
                new Vector2[] {
                    new Vector2(6.28f * 0.75f, 48),
                    new Vector2(6.28f * (0.75f + 0.33f), 48),
                    new Vector2(6.28f * (0.75f + 0.67f), 48)
                },

                new Vector2[] {
                    new Vector2(6.28f * 0.75f, 40),
                    new Vector2(6.28f * (0.75f + 0.33f), 40),
                    new Vector2(6.28f * (0.75f + 0.67f), 40)
                },

                new Vector2[] {
                    new Vector2(6.28f * 0.28f, 14),
                    new Vector2(6.28f * 0.22f, 14),
                    new Vector2(6.28f * 0.2f, 8),
                    new Vector2(6.28f * 0.3f, 8)
                },

                new Vector2[] {
                    new Vector2(6.28f * 0.1f, 3),
                    new Vector2(6.28f * 0.4f, 3),
                    new Vector2(6.28f * 0.725f, 24),
                    new Vector2(6.28f * 0.775f, 24),
                },
            };
        }

        static Vector2 AngleOffset(float angle, float offset)
        {
            return new Vector2(
                MathF.Cos(angle) * offset,
                MathF.Sin(angle) * offset
            );
        }

        static Vector2 AngleOffset(Vector2 ang)
        {
            return AngleOffset(ang.X, ang.Y);
        }

        uint lastFrame;
        public void Update()
        {
            uint dt = lastFrame - SDL.SDL_GetTicks();
            
            rotation.X = SDL.SDL_GetTicks() / 300f;

            offset.X = MathF.Sin(SDL.SDL_GetTicks() / 300f) * 20;
            offset.Y = MathF.Sin(100 + SDL.SDL_GetTicks() / 500f) * 20;

            for (int i = 0; i < shapes.Length; i++) {
                for (int j = 0; j < shapes[i].Length; j++) {
                    int j2 = (j + 1) % shapes[i].Length;
                    Draw.Line(offset + center + AngleOffset(shapes[i][j] + rotation), offset + center + AngleOffset(shapes[i][j2] + rotation), new Color32(255,180,0));
                }
            }

            Draw.Text(5, 5, new Color32(255,180,0), "/!\\ DISASTER ENGINE /!\\");

            for (int i = 0; i < message.Length; i++)
            {
                Draw.Text(
                    5, 
                    5 + (0 + Draw.fontHeight) * (1 + i), 
                    new Color32((byte)(100 - (i * 20)), (byte)(20 + (i * 20)), 160),
                    message[i]
                );
            }

            lastFrame = SDL.SDL_GetTicks();
        }

        public void Done() {}
    }
}