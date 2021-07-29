// software renderer. commented out blocks are to be implemented

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Numerics;
using System.IO;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Disaster
{
    public class SoftwareCanvas
    {
        //public static IntPtr drawTexture;
        public static int textureWidth;
        public static int textureHeight;
        public static Color32[] colorBuffer;
        
        public static Color32[] tempBuffer;
        public static int tempWidth;
        public static int tempHeight;

        public static int fontWidth;
        public static int fontHeight;
        //public static IntPtr pixels;

        public static int offsetX;
        public static int offsetY;

        public static Color32 clear = new Color32() { r = 0, g = 0, b = 0, a = 0 };

        public static bool slowDraw = false;
        public static int slowDrawPixels = 1;
        static int slowDrawCount = 0;

        public static bool overdraw = false;
        static int[] overdrawBuffer;
        static int maxOverdraw = 0;

        public static int MaxTextLength()
        {
            return (textureWidth / fontWidth);
        }

        public static string[] SplitLineToFitScreen(string message)
        {
            int necessaryLines = 1 + (message.Length / MaxTextLength());
            string[] output = new string[necessaryLines];
            for (int i = 0; i < necessaryLines; i++)
            {
                int end = (int)MathF.Min(message.Length - (i * MaxTextLength()), MaxTextLength());
                output[i] = message.Substring(i * MaxTextLength(), end);
            }
            return output;
        }

        public static void InitTexture(int width, int height)
        {
            fontCache = new Dictionary<string, (int width, int height, bool[,] data)>();

            textureWidth = width;
            textureHeight = height;
            
            //pixels = Marshal.AllocHGlobal(textureWidth * textureHeight * 4);

            colorBuffer = new Color32[textureHeight * textureWidth];
            overdrawBuffer = new int[textureHeight * textureWidth];
            Clear();
        }

        static Dictionary<string, (int width, int height, bool[,] data)> fontCache;
        static bool[,] fontBuffer;
        public static void LoadFont(string fontPath)
        {
            if (fontCache.ContainsKey(fontPath))
            {
                var cachedFont = fontCache[fontPath];
                fontWidth = cachedFont.width;
                fontHeight = cachedFont.height;
                fontBuffer = cachedFont.data;
                return;
            }

            var image = Raylib.LoadImage(fontPath);

            fontWidth = image.width;
            fontHeight = image.height;

            fontBuffer = new bool[fontWidth, fontHeight];
            unsafe
            {
                Color32* colors = (Color32*)image.data;

                for (int i = 0; i < fontWidth; i++)
                {
                    for (int j = 0; j < fontHeight; j++)
                    {
                        int fontColorBufferIndex = (j * fontWidth) + i;
                        fontBuffer[i, fontHeight - j - 1] = colors[fontColorBufferIndex].r > 0;

                    }
                }
            }

            fontWidth = image.width/ 16;
            fontHeight = image.height / 8;

            fontCache.Add(fontPath, (fontWidth, fontHeight, fontBuffer));
        }

        public static void Clear()
        {
            new Span<Color32>(colorBuffer).Fill(clear);
            new Span<int>(overdrawBuffer).Fill(0);
        }

        public static void Clear(Color32 clearColor)
        {
            new Span<Color32>(colorBuffer).Fill(clearColor);
            new Span<int>(overdrawBuffer).Fill(0);
        }

        public static void PixelBuffer(PixelBuffer texture, int x, int y, Rect rect)
        {
            int twidth = texture.width;

            rect.x = Math.Clamp(rect.x, 0, texture.width);
            rect.y = Math.Clamp(rect.y, 0, texture.height);
            rect.width = Math.Clamp(rect.width, 0, texture.width - rect.x);
            rect.height = Math.Clamp(rect.height, 0, texture.height - rect.y);

            x += offsetX;
            y += offsetY;

            int sx = (int)rect.x;
            int sy = (int)rect.y;
            int sw = (int)rect.width;
            int sh = (int)rect.height;

            x -= sx;
            y -= sy;

            for (int i = sx; i < sx + sw; i++)
            {
                for (int j = sy; j < sy + sh; j++)
                {
                    if (i + x < 0 || i + x >= textureWidth) continue;
                    if (j + y < 0 || j + y >= textureHeight) continue;
                    if (i < 0 || j < 0 || i >= texture.width || j >= texture.height) continue;

                    Color32 tcol = texture.pixels[(j * twidth) + i];
                    if (tcol.a == 0) continue;

                    // Draw to screen
                    int index = ((j + y) * textureWidth) + (i + x);
                    colorBuffer[index] = tcol;
                    overdrawBuffer[index] += 1;
                    maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                    SlowDraw();
                }
            }
        }

        public static void PixelBuffer(PixelBuffer texture, int x, int y, Transform2D transform)
        {
            PixelBuffer(texture, x, y, new Rect(0, 0, texture.width, texture.height), transform);
        }

        public static void PixelBuffer(PixelBuffer texture, int x, int y, Rect rect, Transform2D transform)
        {
            int twidth = texture.width;
            double radians = transform.rotation * 0.0174532925199;

            rect.x = Math.Clamp(rect.x, 0, texture.width);
            rect.y = Math.Clamp(rect.y, 0, texture.height);
            rect.width = Math.Clamp(rect.width, 0, texture.width - rect.x);
            rect.height = Math.Clamp(rect.height, 0, texture.height - rect.y);

            x += offsetX;
            y += offsetY;

            int sx = (int)rect.x;
            int sy = (int)rect.y;
            int sw = (int)rect.width;
            int sh = (int)rect.height;

            x -= sx;
            y -= sy;


            double c = Math.Cos(radians);
            double s = Math.Sin(radians);

            for (int i = sx; i < sx + sw * transform.scale.X; i++)
            {
                for (int j = sy; j < sy + sh * transform.scale.Y; j++)
                {
                    // Figure out the target x and y position
                    int targetX = i - (int)(transform.origin.X * transform.scale.X);
                    int targetY = j - (int)(transform.origin.Y * transform.scale.Y);

                    // Don't bother with the math if we aren't rotating
                    if (transform.rotation != 0)
                    {
                        // Determine offset
                        int ii = i - (int)(transform.origin.X * transform.scale.X) - sx;
                        int jj = j - (int)(transform.origin.Y * transform.scale.Y) - sy;

                        targetX = (int)(ii * c - jj * s) + sx;
                        targetY = (int)(jj * c + ii * s) + sy;
                    }

                    if (targetX + x < 0 || targetX + x >= textureWidth) continue;
                    if (targetY + y < 0 || targetY + y >= textureHeight) continue;

                    // Determine the pixel from the source texture to be used
                    int sourceX = ((int)((i - sx) / transform.scale.X)) + sx;
                    int sourceY = ((int)((j - sy) / transform.scale.Y)) + sy;

                    Color32 tcol = texture.pixels[(sourceY * twidth) + sourceX];
                    if (tcol.a == 0) continue;

                    // Draw to screen
                    int index = ((targetY + y) * textureWidth) + (targetX + x);
                    colorBuffer[index] = tcol;
                    overdrawBuffer[index] += 1;
                    maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                    SlowDraw();
                }
            }
        }

        // public static void DrawTextureTiled(Texture2D texture, int x, int y, int width, int height)
        // {
        //     Color32[] texColors = texture.GetPixels32();
        //     int twidth = texture.width;
        //     int theight = texture.height;


        //     for (int i = x; i < x + width; i++) {
        //         for (int j = y; j < y + height; j++) {

        //             if (i < 0) continue;
        //             if (j < 0) continue;
        //             if (i >= textureWidth) continue;
        //             if (j >= textureHeight) continue;

        //             int tx = (i - x) % twidth;
        //             int ty = (j - y) % theight;
        //             Color32 tcol = texColors[(ty * twidth) + tx];
        //             if (tcol.a == 0) continue;

        //             colorBuffer[(j * textureWidth) + i] = tcol;
        //         }
        //     }
        // }
        
        public static void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, Color32 color)
        {
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
                            colorBuffer[index] = color;
                            overdrawBuffer[index] += 1;
                            maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                            SlowDraw();
                        }
                    }
                }
            }
        }

        static float root2 = 1.4142135623730951f;

        public static void Circle(int x, int y, float radius, Color32 color)
        {
            int length = (int)(root2 * radius);
            float r2 = radius * radius;
            for (int i = -length/2; i <= length/2; i++)
            {
                int j = (int) MathF.Sqrt(MathF.Abs((i * i) - r2));

                PutPixel(x + i, y + j, color);
                PutPixel(x + j, y + i, color);
                PutPixel(x - i, y - j, color);
                PutPixel(x - j, y - i, color);
            }
        }

        static void PutPixel(int i, int j, Color32 color)
        {
            if (j >= 0 && i >= 0 && i < textureWidth && j < textureHeight)
            {
                int index = j * textureWidth + i;
                if (index >= 0 && index < colorBuffer.Length)
                {
                    colorBuffer[index] = color;
                    overdrawBuffer[index] += 1;
                    maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                    SlowDraw();
                }
            }
        }

        public static void CircleFilled(int x, int y, float radius, Color32 color)
        {
            int r = (int)radius + 1;
            int minx = x - r;
            int maxx = x + r;
            int miny = y - r;
            int maxy = y + r;
            float sqrd = radius * radius;
            for (int i = minx; i < maxx; i++)
            {
                for (int j = miny; j < maxy; j++)
                {
                    float a = i - x;
                    float b = j - y;
                    float sd = (a * a) + (b * b);
                    if (sd < sqrd)
                    {
                        if (j >= 0 && i >= 0 && i < textureWidth && j < textureHeight)
                        {
                            int index = j * textureWidth + i;
                            if (index >= 0 && index < colorBuffer.Length)
                            {
                                colorBuffer[index] = color;
                                overdrawBuffer[index] += 1;
                                maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                                SlowDraw();
                            }
                        }
                    }
                }
            }
        }

        public static void DrawRect(int x1, int y1, int width, int height, Color32 color)
        {
            x1 += offsetX;
            y1 += offsetY;

            Line(x1, y1, x1 + width - 1, y1, color);
            Line(x1 + width - 1, y1, x1 + width - 1, y1 + height - 1, color);
            Line(x1 + width - 1, y1 + height - 1, x1, y1 + height - 1, color);
            Line(x1, y1 + height - 1, x1, y1, color);
        }

        // public static void DrawLine(Vector3 p1, Vector3 p2, Color32 color, int steps = 2)
        // {
        //     var a = WorldToScreenPoint2(p1);
        //     var b = WorldToScreenPoint2(p2);
        //     if (a.broke && b.broke) return;
        //     if (steps <= 0) return;
        //     // if (Vector3.Distance(p1, p2) < .001f) return;

        //     if (a.broke) {
        //         var diff = p2 - p1;
        //         DrawLine(p1, p1 + (diff/2), color, steps - 1);
        //         return;
        //     }

        //     if (b.broke) {
        //         var diff = p1 - p2;
        //         DrawLine(p2, p2 + (diff/2), color, steps - 1);
        //         return;
        //     }

        //     DrawLine(a.point, b.point, color);
        // }

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
            x0 += offsetX;
            y0 += offsetY;
            x1 += offsetX;
            y1 += offsetY;

            int dx = (int)MathF.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = (int)MathF.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            if (dx > 1000 || dy > 1000) return;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                if (y0 >= 0 && x0 >= 0 && x0 < textureWidth && y0 < textureHeight)
                {
                    int index = y0 * textureWidth + x0;
                    if (index >= 0 && index < colorBuffer.Length)
                    {
                        colorBuffer[index] = color;
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
            x0 += offsetX;
            y0 += offsetY;
            x1 += offsetX;
            y1 += offsetY;

            float maxDist = Vector2.Distance(new Vector2(x0, y0), new Vector2(x1, y1));

            int dx = (int)MathF.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = (int)MathF.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            if (dx > 1000 || dy > 1000) return;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                if (y0 >= 0 && x0 >= 0 && x0 < textureWidth && y0 < textureHeight)
                {
                    int index = y0 * textureWidth + x0;
                    float t = Vector2.Distance(new Vector2(x1, y1), new Vector2(x0, y0)) / maxDist;
                    if (index >= 0 && index < colorBuffer.Length)
                    {
                        colorBuffer[index] = Color32.Lerp(color1, color0, t);
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

            if (Util.IntersectSegmentPlane(
                start, 
                end, 
                Util.CreatePlaneFromPositionNormal(ScreenController.camera.position, cameraNormal),
                out float _,
                out Vector3 intersection))
            {
                Console.WriteLine("gotit");
                if (dotStart < 0f)
                {
                    end = intersection;
                } else
                {
                    start = intersection;
                }
            }

            Vector2 start2d = WorldToScreenPoint(start);
            Vector2 end2d = WorldToScreenPoint(end);

            Line(start2d, end2d, dotStart > 0 ? Colors.red : Colors.slimegreen, dotEnd > 0 ? Colors.red : Colors.slimegreen);
        }

        public static void FillRect(int x, int y, int width, int height, Color32 color)
        {
            if (x >= textureWidth) return;
            if (y >= textureHeight) return;

            x += offsetX;
            y += offsetY;

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
                        colorBuffer[i + yoff] = Mix(colorBuffer[i + yoff], color);
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

        static Color32 Mix(Color32 A, Color32 B)
        {
            byte r, g, b;
            float a = B.a / 256f;
            float ba = A.a / 256f;
            float ia = 1f - a;
            r = (byte)MathF.Floor((A.r * ia) + (B.r * a));
            g = (byte)MathF.Floor((A.g * ia) + (B.g * a));
            b = (byte)MathF.Floor((A.b * ia) + (B.b * a));
            return new Color32(r, g, b, (byte)MathF.Floor(a * ba * 255f));
        }

        public static void Path(Vector2[] path, Color32 color, bool closed = true)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Line(
                    path[i],
                    path[i + 1],
                    color
                );
            }

            if (closed)
            {
                Line(
                    path[path.Length - 1],
                    path[0],
                    color
                );
            }
        }

        public static void DrawPath(Vector2Int[] path, Color32 color, bool closed = true)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Line(
                    path[i],
                    path[i + 1],
                    color
                );
            }

            if (closed)
            {
                Line(
                    path[path.Length - 1],
                    path[0],
                    color
                );
            }
        }

        // public static void DrawPath(Vector3[] path, Color32 color, bool closed = true)
        // {
        //     for (int i = 0; i < path.Length-1; i++) {
        //         DrawLine(
        //             WorldToScreenPoint(path[i]),
        //             WorldToScreenPoint(path[i+1]),
        //             color
        //         );
        //     }

        //     if (closed) {
        //         DrawLine(
        //             WorldToScreenPoint(path[path.Length-1]),
        //             WorldToScreenPoint(path[0]),
        //             color
        //         );
        //     }
        // }

        // public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Color32 color) {
        //     DrawMesh(mesh, matrix, Vector3.zero, color);
        // }

        public static unsafe void Wireframe(Mesh mesh, Matrix4x4 matrix, Color32 color, bool backfaceCulling, bool depth, bool filled)
        {
            var verts = (Vector3*)mesh.vertices;
            var tris = (short*)mesh.indices;
            //Matrix4x4.Invert(matrix, out matrix);
            for (int i = 0; i < mesh.triangleCount * 3; i += 3)
            {
                var v1 = Vector3.Transform(verts[tris[i + 0]], matrix);
                var v2 = Vector3.Transform(verts[tris[i + 1]], matrix);
                var v3 = Vector3.Transform(verts[tris[i + 2]], matrix);

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
                        Line(p1, p2, color);
                        Line(p2, p3, color);
                        Line(p3, p1, color);
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

        public static void Paragraph(int x, int y, Color32 color, string text, int maxWidth = -1)
        {
            string[] lines = text.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            int line = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (maxWidth != -1)
                {
                    for (int j = 0; j < lines[i].Length; j+= maxWidth)
                    {
                        Text(x, y + (line * fontHeight), color, lines[i].Substring(j, Math.Min(lines[i].Length-j, maxWidth)));
                        line += 1;
                    }
                } else
                {
                    Text(x, y + (i * fontHeight), color, lines[i]);
                }
                line += 1;
            }
        }

        public static void Text(int x, int y, Color32 color, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Character(x + (i * fontWidth), y, text[i], color);
            }
        }

        static void Character(int x, int y, int character, Color32 color)
        {
            x += offsetX;
            y += offsetY;

            int charX = (character % 16) * fontWidth;
            int charY = (int)(MathF.Floor(character / 16));
            charY = 8 - charY - 1;
            charY *= fontHeight;
            for (int i = 0; i < fontWidth; i++)
            {
                for (int j = 0; j < fontHeight; j++)
                {
                    if (fontBuffer[charX + i, (charY + fontHeight) - j - 1])
                    {
                        if (x + i < 0) continue;
                        if (x + i >= textureWidth) continue;
                        if (y + j < 0) continue;
                        if (y + j >= textureHeight) continue;
                        int index = PointToBufferIndex(x + i, y + j);
                        if (index >= 0 && index < colorBuffer.Length)
                        {
                            colorBuffer[index] = color;
                            overdrawBuffer[index] += 1;
                            maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[index]);
                            SlowDraw();
                        }
                    }
                }
            }
        }

        public static void Pixel(int x, int y, Color32 color)
        {
            x += offsetX;
            y += offsetY;

            colorBuffer[PointToBufferIndex(x, y)] = color;
            overdrawBuffer[PointToBufferIndex(x, y)] += 1;
            maxOverdraw = Math.Max(maxOverdraw, overdrawBuffer[PointToBufferIndex(x, y)]);
            SlowDraw();
        }

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

        // public static (bool broke, Vector2Int point) WorldToScreenPoint2(Vector3 position)
        // {
        //     var point = Camera.main.WorldToScreenPoint(position);

        //     return (point.z<0, new Vector2Int(
        //         Mathf.FloorToInt(textureWidth * point.x / Camera.main.pixelWidth),
        //         Mathf.FloorToInt(textureHeight * point.y / Camera.main.pixelHeight)
        //     ));
        // }

        // public static Vector2Int WorldToScreenPoint(Vector3 position)
        // {
        //     var point = Camera.main.WorldToScreenPoint(position);

        //     // This is still super broke
        //     if (point.z < 0) {
        //         point.x *= point.z;
        //         point.y *= point.z;
        //     }

        //     return new Vector2Int(
        //         Mathf.FloorToInt(textureWidth * point.x / Camera.main.pixelWidth),
        //         Mathf.FloorToInt(textureHeight * point.y / Camera.main.pixelHeight)
        //     );
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int PointToBufferIndex(int x, int y)
        {
            // if (x < 0 || x >= textureWidth || y < 0 || y >= textureHeight) return -1;
            return (y * textureWidth) + x;
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

        public static void StartBuffer(int width, int height)
        {
            tempBuffer = colorBuffer;
            tempWidth = textureWidth;
            tempHeight = textureHeight;

            textureWidth = width;
            textureHeight = height;

            colorBuffer = new Color32[width * height];
            overdrawBuffer = new int[width * height];
        }

        public static string EndBuffer()
        {
            var output = new PixelBuffer(colorBuffer, textureWidth);
            var hashnum = output.GetHashCode();
            while (Assets.pixelBuffers.ContainsKey(hashnum.ToString()))
            {
                hashnum += 1;
            }
            var hash = hashnum.ToString();

            colorBuffer = tempBuffer;
            textureWidth = tempWidth;
            textureHeight = tempHeight;
            overdrawBuffer = new int[textureWidth * textureHeight];

            Assets.pixelBuffers.Add(hash, output);

            return hash;
        }


    }
}