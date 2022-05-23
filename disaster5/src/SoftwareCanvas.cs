// software renderer. commented out blocks are to be implemented

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Numerics;
using Raylib_cs;

namespace Disaster
{

    public class SoftwareCanvas
    {
        public static int textureWidth;
        public static int textureHeight;
        public static Color32[] colorBuffer;
        public static Vector2Int offset;
        public static Color32 clear = new Color32 { r = 0, g = 0, b = 0, a = 0 };

        public static bool slowDraw = false;
        public static int slowDrawPixels = 1;
        static int slowDrawCount = 0;

        public static bool overdraw = false;
        static int[] overdrawBuffer;
        static int maxOverdraw = 0;

        public enum BlendMode
        {
            Normal,
            Add,
            Noise,
            Dither,
            Subtract
        }

        public static BlendMode blendMode = BlendMode.Normal;

        public static void InitTexture(int width, int height)
        {
            textureWidth = width;
            textureHeight = height;
            
            colorBuffer = new Color32[textureHeight * textureWidth];
            overdrawBuffer = new int[textureHeight * textureWidth];
            Clear();
        }

        public static void Clear()
        {
            new Span<Color32>(colorBuffer).Fill(clear);
            new Span<int>(overdrawBuffer).Fill(0);
        }

        public static void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, Color32 color)
        {
            x1 += offset.x;
            y1 += offset.y;
            x2 += offset.x;
            y2 += offset.y;
            x3 += offset.x;
            y3 += offset.y;
            Vector2[] points = new Vector2[]
            {
                new Vector2(x1, y1),
                new Vector2(x2, y2),
                new Vector2(x3, y3)
            };

            Array.Sort(
                points, 
                (a, b) =>
                {
                    return Math.Sign(a.Y - b.Y);
                }
            );

            int LerpPoints(int a, int b, int y)
            {
                float minX, maxX;
                bool flip = false;
                if (points[a].X < points[b].X)
                {
                    minX = points[a].X;
                    maxX = points[b].X;
                } else
                {
                    minX = points[b].X;
                    maxX = points[a].X;
                    flip = true;
                }

                float len = Math.Abs(points[a].Y - points[b].Y);
                float t = (y - Math.Min(points[a].Y, points[b].Y)) / len;
                if (flip) t = 1.0f - t;

                return Util.Lerp((int)minX, (int)maxX, t);
            }

            for (int j = (int)points[0].Y; j < points[2].Y; j++)
            {
                int minX = x1; 
                int maxX = x1;

                int b = LerpPoints(0, 2, j);
                if (j < points[1].Y)
                {
                    int a = LerpPoints(0, 1, j);
                    if (a < b)
                    {
                        minX = a;
                        maxX = b;
                    } else
                    {
                        minX = b;
                        maxX = a;
                    }
                } else
                {
                    int a = LerpPoints(1, 2, j);
                    if (a < b)
                    {
                        minX = a;
                        maxX = b;
                    }
                    else
                    {
                        minX = b;
                        maxX = a;
                    }
                }

                for (int i = minX; i <= maxX; i++)
                {
                    if (j >= 0 && i >= 0 && i < textureWidth && j < textureHeight)
                    {
                        int index = j * textureWidth + i;
                        if (index >= 0 && index < colorBuffer.Length)
                        {
                            colorBuffer[index] = Mix(colorBuffer[index], color, i, j);
                            overdrawBuffer[index] += 1;
                            maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                            SlowDraw();
                        }
                    }
                }
            }
        }

        public static void Pixel(int i, int j, Color32 color)
        {
            i += offset.x;
            j += offset.y;
            if (j >= 0 && i >= 0 && i < textureWidth && j < textureHeight)
            {
                int index = j * textureWidth + i;
                if (index >= 0 && index < colorBuffer.Length)
                {
                    colorBuffer[index] = Mix(colorBuffer[index], color, i, j);
                    overdrawBuffer[index] += 1;
                    maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                    SlowDraw();
                }
            }
        }

        public static void Line(Vector2 p1, Vector2 p2, Color32 color)
        {
            Line(new Vector2Int((int)p1.X, (int)p1.Y), new Vector2Int((int)p2.X, (int)p2.Y), color);
        }

        public static void Line(Vector2 p1, Vector2 p2, Color32 colorA, Color32 colorB)
        {
            Line((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, colorA, colorB);
        }

        public static void Line(Vector2Int p1, Vector2Int p2, Color32 color)
        {
            Line(p1.x, p1.y, p2.x, p2.y, color);
        }

        public static void Line(float x0, float y0, float x1, float y1, Color32 color)
        {
            Line((int)x0, (int)y0, (int)x1, (int)y1, color);
        }

        public static void Line(int x0, int y0, int x1, int y1, Color32 color)
        {
            x0 += offset.x;
            y0 += offset.y;
            x1 += offset.x;
            y1 += offset.y;

            int dx = (int)MathF.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = (int)MathF.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            //if (dx > 1000 || dy > 1000) return;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                if (y0 >= 0 && x0 >= 0 && x0 < textureWidth && y0 < textureHeight)
                {
                    int index = y0 * textureWidth + x0;
                    if (index >= 0 && index < colorBuffer.Length)
                    {
                        colorBuffer[index] = Mix(colorBuffer[index], color, x0, y0);
                        overdrawBuffer[index] += 1;
                        maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                        SlowDraw();
                    }
                }
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public static void Line(int x0, int y0, int x1, int y1, Color32 color0, Color32 color1)
        {
            x0 += offset.x;
            y0 += offset.y;
            x1 += offset.x;
            y1 += offset.y;

            float maxDist = Vector2.Distance(new Vector2(x0, y0), new Vector2(x1, y1));

            int dx = (int)MathF.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = (int)MathF.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            //if (dx > 1000 || dy > 1000) return;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                if (y0 >= 0 && x0 >= 0 && x0 < textureWidth && y0 < textureHeight)
                {
                    int index = y0 * textureWidth + x0;
                    float t = Vector2.Distance(new Vector2(x1, y1), new Vector2(x0, y0)) / maxDist;
                    if (index >= 0 && index < colorBuffer.Length)
                    {
                        var color = Color32.Lerp(color1, color0, t);
                        colorBuffer[index] = Mix(colorBuffer[index], color, x0, y0);
                        overdrawBuffer[index] += 1;
                        maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                        SlowDraw();
                    }
                }
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public static void Line(Vector3 start, Vector3 end, Color32 colorStart, Color32 colorEnd)
        {
            Vector3 cameraNormal = Vector3.Normalize(ScreenController.camera.target - ScreenController.camera.position);

            float dotStart = Vector3.Dot(
                Vector3.Normalize(start - ScreenController.camera.position), 
                cameraNormal
            );

            float dotEnd = Vector3.Dot(
                Vector3.Normalize(end - ScreenController.camera.position),
                cameraNormal
            );

            if (dotStart < 0f && dotEnd < 0f) return;

            //if (dotStart <0f ^ dotEnd <0f)
            //{
            //    colorStart = Colors.cloudblue;
            //    colorEnd = Colors.cloudblue;
            //}

            if (Util.IntersectSegmentPlane(
                start,
                end,
                Util.CreatePlaneFromPositionNormal(ScreenController.camera.position, cameraNormal),
                out float _,
                out Vector3 intersection))
            {
                if (dotStart < 0f)
                {
                    start = intersection + cameraNormal;
                }
                else
                {
                    end = intersection + cameraNormal; 
                }
                
                
            }

            Vector2 start2d = WorldToScreenPoint(start);
            Vector2 end2d = WorldToScreenPoint(end);

            if (start2d.LengthSquared() > 10000000) return;
            if (end2d.LengthSquared() > 10000000) return;

            Line(start2d, end2d, colorStart, colorEnd);
        }

        public static void FillRect(int x, int y, int width, int height, Color32 color)
        {
            if (x >= textureWidth) return;
            if (y >= textureHeight) return;

            x += offset.x;
            y += offset.y;

            int x2 = x + width;
            int y2 = y + height;

            if (x2 < 0) return;
            if (y2 < 0) return;

            if (x2 > textureWidth) x2 = textureWidth;
            if (y2 > textureHeight) y2 = textureHeight;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            for (int i = x; i < x2; i++)
            {
                for (int j = y; j < y2; j++)
                {
                    int yoff = j * textureWidth;
                    if (color.a != 255)
                    {
                        colorBuffer[i + yoff] = Mix(colorBuffer[i + yoff], color, i, j);
                        overdrawBuffer[i + yoff] += 1;
                        maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[i + yoff]);
                        SlowDraw();
                    }
                    else
                    {
                        colorBuffer[i + yoff] = color;
                        overdrawBuffer[i + yoff] += 1;
                        maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[i + yoff]);
                        SlowDraw();
                    }
                }
            }
        }

        static int[] bayer = new[] { 0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5 };
        static Random randomGenerator;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Color32 Mix(Color32 A, Color32 B, int x, int y)
        {
            
            byte r, g, b;
            float srcAlpha, destAlpha;
            switch(blendMode)
            {
                case BlendMode.Dither:
                    int index = x % 4 + (y % 4) * 4;
                    float alpha = MathF.Round(B.a / 16f);
                    if (alpha <= bayer[index])
                    {
                        return A;
                    } else
                    {
                        return new Color32(B.r, B.g, B.b, 255);
                    }
                  
                case BlendMode.Noise:
                    if (randomGenerator == null) randomGenerator = new Random();
                    if (randomGenerator.NextDouble() < B.a/256f)
                    {
                        return new Color32(B, 255);
                    } else
                    {
                        return new Color32(A, 255);
                    }
                case BlendMode.Add:
                    srcAlpha = B.a / 256f;
                    destAlpha = A.a / 256f;
                    r = (byte)MathF.Min(MathF.Floor((B.r * srcAlpha) + (A.r * destAlpha)), 255);
                    g = (byte)MathF.Min(MathF.Floor((B.g * srcAlpha) + (A.g * destAlpha)), 255);
                    b = (byte)MathF.Min(MathF.Floor((B.b * srcAlpha) + (A.b * destAlpha)), 255);
                    return new Color32(r, g, b, (byte)MathF.Min(MathF.Floor((srcAlpha + destAlpha) * 255f), 255));
                case BlendMode.Subtract:
                    srcAlpha = B.a / 256f;
                    destAlpha = A.a / 256f;
                    r = (byte)MathF.Min(MathF.Floor((B.r * srcAlpha) - (A.r * destAlpha)), 255);
                    g = (byte)MathF.Min(MathF.Floor((B.g * srcAlpha) - (A.g * destAlpha)), 255);
                    b = (byte)MathF.Min(MathF.Floor((B.b * srcAlpha) - (A.b * destAlpha)), 255);
                    return new Color32(r, g, b, (byte)MathF.Min(MathF.Floor((srcAlpha + destAlpha) * 255f), 255));
                default:
                case BlendMode.Normal:
                    if (B.a == 255) return B;
                    if (B.a == 0) return A;
                    srcAlpha = B.a / 256f;
                    destAlpha = A.a / 256f;
                    float oneMinusSrcAlpha = 1f - srcAlpha;
                    r = (byte)MathF.Min(MathF.Floor((B.r * srcAlpha) + (A.r * oneMinusSrcAlpha)), 255);
                    g = (byte)MathF.Min(MathF.Floor((B.g * srcAlpha) + (A.g * oneMinusSrcAlpha)), 255);
                    b = (byte)MathF.Min(MathF.Floor((B.b * srcAlpha) + (A.b * oneMinusSrcAlpha)), 255);
                    return new Color32(r, g, b, (byte)MathF.Min(MathF.Floor((srcAlpha + destAlpha * oneMinusSrcAlpha) * 255f), 255));

            }
        }

        public static unsafe void Wireframe(Mesh mesh, Matrix4x4 matrix, Color32 color, bool backfaceCulling, bool depth, bool filled)
        {
            var verts = (Vector3*)mesh.vertices;
            var tris = (short*)mesh.indices;

            //Matrix4x4.Invert(matrix, out matrix);
            for (int i = 0; i < mesh.triangleCount * 3; i += 3)
            {
                Vector3 v1, v2, v3;
                
                // check if mesh uses indices
                if (mesh.indices != IntPtr.Zero)
                {
                    v1 = Vector3.Transform(verts[tris[i + 0]], matrix);
                    v2 = Vector3.Transform(verts[tris[i + 1]], matrix);
                    v3 = Vector3.Transform(verts[tris[i + 2]], matrix);
                } else
                {
                    v1 = Vector3.Transform(verts[i + 0], matrix);
                    v2 = Vector3.Transform(verts[i + 1], matrix);
                    v3 = Vector3.Transform(verts[i + 2], matrix);
                }

                if (backfaceCulling)
                {
                    var midpoint = v1;
                    var dir = Vector3.Cross(v2 - v1, v3 - v1);
                    var norm = dir / dir.Length();
                    var camNorm = (midpoint - ScreenController.camera.position);// / (midpoint - ScreenController.camera.position).Length();
                    if (Vector3.Dot(camNorm, dir) > 0) continue;
                }

                var p1 = WorldToScreenPoint(v1);
                var p2 = WorldToScreenPoint(v2);
                var p3 = WorldToScreenPoint(v3);

                if (depth)
                {
                    var black = new Color32(0, 0, 0, 255);
                    var cpos = ScreenController.camera.position;
                    Color32 a = Color32.Lerp(black, color, MathF.Pow(1f + Vector3.Distance(cpos, v1) * 0.2f, -1.5f));
                    Color32 b = Color32.Lerp(black, color, MathF.Pow(1f + Vector3.Distance(cpos, v2) * 0.2f, -1.5f));
                    Color32 c = Color32.Lerp(black, color, MathF.Pow(1f + Vector3.Distance(cpos, v3) * 0.2f, -1.5f));
                    Line(p1, p2, a, b);
                    Line(p2, p3, b, c);
                    Line(p3, p1, c, a);
                } else
                {
                    if (filled)
                    {
                        Triangle((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, (int)p3.X, (int)p3.Y, color);
                    } else
                    {
                        Line(v1, v2, color, color);
                        Line(v2, v3, color, color);
                        Line(v3, v1, color, color);
                    }
                }

            }
        }

        static Vector2 WorldToScreenPoint(Vector3 worldPoint)
        {
            float ratioW = (float)ScreenController.screenWidth / (float)ScreenController.windowWidth;
            float ratioH = (float)ScreenController.screenHeight / (float)ScreenController.windowHeight;
            var p = Raylib.GetWorldToScreen(worldPoint, ScreenController.camera);
            p.X *= ratioW;
            p.Y *= ratioH;
            return p;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SlowDraw()
        {
            if (slowDraw)
            {
                slowDrawCount += 1;
                if (slowDrawCount >= slowDrawPixels)
                {
                    Program.screen.Update();
                    slowDrawCount = 0;
                }
            }
        }

        public static Color32[] GetOverdrawColorBuffer()
        {
            Color32[] output = new Color32[overdrawBuffer.Length];
            int mul = 255 / maxOverdraw;
            for (int i = 0; i < output.Length; i++)
            {
                byte val = (byte)(overdrawBuffer[i] * mul);
                output[i] = new Color32(val, val, val);
            }
            return output;
        }
    }
}