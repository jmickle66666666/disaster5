using Jurassic;
using Jurassic.Library;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace DisasterAPI
{
    [ClassDescription("All your favourite ways to draw stuff to the screen!")]
    public class Draw : ObjectInstance {

        public Draw(ScriptEngine engine) : base(engine) {
            PopulateFunctions();
        }

        [JSFunction(Name = "loadFont")]
        [FunctionDescription("Load a font for the software renderer. All future Draw.text calls will use the specified font.")]
        [ArgumentDescription("fontPath", "Path to the font texture. Fonts are 2-color images where pixels with a red value above zero are considered filled.")]
        public static void LoadFont(string fontPath) {
            if (!Disaster.Assets.LoadPath(fontPath, out string fontAssetPath))
            {
                return;
            }
            Disaster.TextController.LoadFont(fontAssetPath);
        }

        [JSFunction(Name = "loadFontTTF")]
        [FunctionDescription("Load a TTF font. All future Draw.textTTF calls will use the specified font.")]
        [ArgumentDescription("fontPath", "Path to the font file.")]
        public static void LoadFontTTF(string fontPath) {
            if (!Disaster.Assets.LoadPath(fontPath, out string fontAssetPath))
            {
                return;
            }
            Disaster.Assets.Font(fontPath); // Loads the font if it isn't already loaded
            Disaster.Assets.currentFont = fontPath;
        }

        [JSFunction(Name = "clear")]
        [FunctionDescription("Clear the 2D canvas.")]
        public static void Clear() {
            // TODO: This also clears the 3D canvas
            EnqueueRenderAction(() => { Raylib.ClearBackground(Raylib_cs.Color.BLACK); });
            if (!Disaster.BufferRenderer.inBuffer) 
                Disaster.NativeResRenderer.EnqueueRender(() => { Raylib.ClearBackground(new Disaster.Color32(0, 0, 0, 0)); });
        }

        [JSProperty(Name = "fontHeight")] 
        [PropertyDescription("Height, in pixels, of the currently loaded font.")]
        public static int fontHeight { get { return Disaster.TextController.fontHeight; } }
        [JSProperty(Name = "fontWidth")]
        [PropertyDescription("Width, in pixels, of the currently loaded font.")]
        public static int fontWidth { get { return Disaster.TextController.fontWidth; } }
        [JSProperty(Name = "screenWidth")]
        [PropertyDescription("Width, in pixels, of the screen resolution.")] 
        public static int screenWidth { get { return Disaster.ScreenController.screenWidth; } }
        [JSProperty(Name = "screenHeight")]
        [PropertyDescription("Height, in pixels, of the screen resolution.")]
        public static int screenHeight { get { return Disaster.ScreenController.screenHeight; } }

        [JSFunction(Name = "offset")]
        [FunctionDescription("Set a global offset for 2D rendering.")]
        [ArgumentDescription("x", "Pixels in the x axis to offset by")]
        [ArgumentDescription("y", "Pixels in the y axis to offset by")]
        public static void Offset(int x, int y)
        {
            Disaster.ScreenController.offset.x = x;
            Disaster.ScreenController.offset.y = y;
        }


        [JSFunction(Name = "rect")]
        [FunctionDescription("Draw a rectangle")]
        [ArgumentDescription("x", "x position of the rectangle")]
        [ArgumentDescription("y", "y position of the rectangle")]
        [ArgumentDescription("width", "width of the rectangle")]
        [ArgumentDescription("height", "height of the rectangle")]
        [ArgumentDescription("color", "Rectangle color", "{r, g, b, a}")]
        [ArgumentDescription("filled", "(optional) Draw a filled rect (true) or an outline (false, default)")]
        public static void Rect(int x, int y, int width, int height, ObjectInstance color, bool filled = false)
        {
            x += Disaster.ScreenController.offset.x;
            y += Disaster.ScreenController.offset.y;
            var col = Disaster.TypeInterface.Color32(color);

            if (filled)
                EnqueueRenderAction(() => { Raylib.DrawRectangle(x, y, width, height, col); });
            else
                EnqueueRenderAction(() => { Raylib.DrawRectangleLines(x, y, width, height, col); });
        }

        [JSFunction(Name = "triangle")]
        [FunctionDescription("Draw a triangle")]
        [ArgumentDescription("x1", "x position of the first point")]
        [ArgumentDescription("y1", "y position of the first point")]
        [ArgumentDescription("x2", "x position of the second point")]
        [ArgumentDescription("y2", "y position of the second point")]
        [ArgumentDescription("x3", "x position of the third point")]
        [ArgumentDescription("y3", "y position of the third point")]
        [ArgumentDescription("color", "color for the triangle", "{r, g, b, a}")]
        [ArgumentDescription("filled", "(optional) Draw a filled triangle (true) or an outline (false, default)")]
        public static void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, ObjectInstance color, bool filled = false)
        {
            var col = Disaster.TypeInterface.Color32(color);
            
            x1 += Disaster.ScreenController.offset.x;
            y1 += Disaster.ScreenController.offset.y;
            x2 += Disaster.ScreenController.offset.x;
            y2 += Disaster.ScreenController.offset.y;
            x3 += Disaster.ScreenController.offset.x;
            y3 += Disaster.ScreenController.offset.y;
            
            // Put these points in counter-clockwise order for raylib
            var a = new Vector2(x1, y1);
            var b = new Vector2(x2, y2);
            var c = new Vector2(x3, y3);
            var ab = b - a;
            var ac = c - a;
            var crossz = ab.X * ac.Y - ab.Y * ac.X;
            Vector2[] points = crossz < 0 ? new[]{a, b, c} : new[]{a, c, b};
            
            if (filled)
                EnqueueRenderAction(() => { Raylib.DrawTriangle(points[0], points[1], points[2], col); });
            else
                EnqueueRenderAction(() => { Raylib.DrawTriangleLines(points[0], points[1], points[2], col); });
        }

        [JSFunction(Name = "circle")]
        [FunctionDescription("Draw a circle")]
        [ArgumentDescription("x", "x position of the center")]
        [ArgumentDescription("y", "y position of the center")]
        [ArgumentDescription("radius", "radius of the circle (distance from center to edge)")]
        [ArgumentDescription("color", "color for the triangle", "{r, g, b, a}")]
        [ArgumentDescription("filled", "(optional) Draw a filled circle (true) or an outline (false, default)")]
        public static void Circle(int x, int y, double radius, ObjectInstance color, bool filled = false)
        {
            var col = Disaster.TypeInterface.Color32(color);
            var radiusF = (float)radius;
            x += Disaster.ScreenController.offset.x;
            y += Disaster.ScreenController.offset.y;
            
            if (filled)
                EnqueueRenderAction(() => { Raylib.DrawCircle(x, y, radiusF, col); });
            else
                EnqueueRenderAction(() => { Raylib.DrawCircleLines(x, y, radiusF, col); });
        }

        [JSFunction(Name = "line")]
        [FunctionDescription("Draw a 2d line, with an optional gradient.")]
        [ArgumentDescription("x1", "starting x position")]
        [ArgumentDescription("y1", "starting y position")]
        [ArgumentDescription("x2", "ending x position")]
        [ArgumentDescription("y2", "ending y position")]
        [ArgumentDescription("color", "line color", "{r, g, b, a}")]
        [ArgumentDescription("colorEnd", "(optional) line end color. if specified, will blend between the two colors along the line.", "{r, g, b, a}")]
        public static void Line(int x1, int y1, int x2, int y2, ObjectInstance color, ObjectInstance colorEnd = null)
        {
            var col = Disaster.TypeInterface.Color32(color);
            var col2 = colorEnd == null ? col : Disaster.TypeInterface.Color32(colorEnd);

            // Use Rlgl directly here so we can do gradient lines
            x1 += Disaster.ScreenController.offset.x;
            y1 += Disaster.ScreenController.offset.y;
            x2 += Disaster.ScreenController.offset.x;
            y2 += Disaster.ScreenController.offset.y;
            EnqueueRenderAction(() =>
            {
                Rlgl.rlBegin(Rlgl.RL_LINES);
                Rlgl.rlColor4ub(col.r, col.g, col.b, col.a);
                Rlgl.rlVertex2i(x1, y1);
                Rlgl.rlColor4ub(col2.r, col2.g, col2.b, col2.a);
                Rlgl.rlVertex2i(x2, y2);
                Rlgl.rlEnd();
            });
        }

        [JSFunction(Name = "line3d")]
        [FunctionDescription("Draw a 3d line!")]
        [ArgumentDescription("start", "start position", "{x, y, z}")]
        [ArgumentDescription("end", "end position", "{x, y, z}")]
        [ArgumentDescription("color", "line color", "{r, g, b, a}")]
        public static void Line3d(ObjectInstance start, ObjectInstance end, ObjectInstance color)
        {
            EnqueueRenderAction(() => {
                Raylib.BeginMode3D(Disaster.ScreenController.camera);
                Raylib.DrawLine3D(
                    Disaster.TypeInterface.Vector3(start),
                    Disaster.TypeInterface.Vector3(end),
                    Disaster.TypeInterface.Color32(color)
                );
                Raylib.EndMode3D();
            });
        }

        [JSFunction(Name = "worldToScreenPoint")]
        [FunctionDescription("Transform a point from world position to screen position.", "{x, y}")]
        [ArgumentDescription("position", "World position to transform", "{x, y, z}")]
        public static ObjectInstance WorldToScreenPoint(ObjectInstance position)
        {
            float ratioW = (float)Disaster.ScreenController.screenWidth / (float)Disaster.ScreenController.windowWidth;
            float ratioH = (float)Disaster.ScreenController.screenHeight / (float)Disaster.ScreenController.windowHeight;
            var p = Raylib.GetWorldToScreen(Disaster.TypeInterface.Vector3(position), Disaster.ScreenController.camera);
            p.X *= ratioW;
            p.Y *= ratioH;
            return Disaster.TypeInterface.Object(p);
        }

        [JSFunction(Name = "text")]
        [FunctionDescription("Draw a line of text.")]
        [ArgumentDescription("text", "the text content to draw")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        [ArgumentDescription("color", "text color", "{r, g, b, a}")]
        public static void Text(string text, int x, int y, ObjectInstance color)
        {
            Disaster.TextController.Text(x, y, Disaster.TypeInterface.Color32(color), text);
        }

        [JSFunction(Name = "textTTF")]
        [FunctionDescription("Draw a line of text using a TTF font. TTF text is drawn on top of all other draw elements.")]
        [ArgumentDescription("text", "the text content to draw")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        [ArgumentDescription("color", "text color", "{r, g, b, a}")]
        public static void TextTTF(string text, int x, int y, ObjectInstance color)
        {
            int scale = Disaster.ScreenController.windowHeight / Disaster.ScreenController.screenHeight;
            var font = Disaster.Assets.Font(Disaster.Assets.currentFont).font;
            var fontSize = Disaster.TextController.fontHeight * scale;
            var position = new Vector2(x + Disaster.ScreenController.offset.x, y + Disaster.ScreenController.offset.y);
            Disaster.NativeResRenderer.EnqueueRender(
                () => {
                    Raylib.DrawTextEx(font, text, position * scale, fontSize, 4, Disaster.TypeInterface.Color32(color));
                }
            );
        }

        [JSFunction(Name = "textStyled")]
        [FunctionDescription("Draw a line of text with styling options")]
        [ArgumentDescription("text", "the text to draw. $b for bold, $w for wavey, $s for drop shadow, $c for color, 0-F (e.g $c5hello $cAthere), $n to reset styling")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        public static void TextStyled(string text, int x, int y)
        {
            Disaster.TextController.TextStyled(x, y, text);
        }

        [JSFunction(Name = "wireframe")]
        [FunctionDescription("Draw a 3D wireframe.")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("color", "Color of the wireframe", "{r, g, b, a}")]
        [ArgumentDescription("backfaceCulling", "(optional) Whether to skip triangles that face away from the camera (default: false)")]
        [ArgumentDescription("drawDepth", "(optional) Whether to render depth on the lines (default: false)")]
        [ArgumentDescription("filled", "(optional) Whether to draw the triangles filled (default: false)")]
        public static void Wireframe(string modelPath, ObjectInstance position, ObjectInstance rotation, ObjectInstance color, bool backfaceCulling = false, bool drawDepth = false, bool filled = false)
        {
            // TODO: Support the optional arguments
            var model = Disaster.Assets.Model(modelPath);
            if (!model.succeeded) return;
            
            var col = Disaster.TypeInterface.Color32(color);
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);

            EnqueueRenderAction(() => { 
                Raylib.BeginMode3D(Disaster.ScreenController.camera);
                Raylib.DrawModelWiresEx(
                    model.model,
                    transform.position,
                    transform.rotationAxis,
                    transform.rotationAngle,
                    transform.scale,
                    col);
                Raylib.EndMode3D();
            });
        }

        [JSFunction(Name = "model")]
        [FunctionDescription("Draw a 3D model, optionally with a specified shader and parameters to use.")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("shaderPath", "(optional) Path of the shader to use (without extension)")]
        [ArgumentDescription("parameters", "(optional) Object of key/value pairs to send to the shader")]
        public static void Model(string modelPath, ObjectInstance position, ObjectInstance rotation, string shaderPath = "", ObjectInstance parameters = null)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);
            var model = Disaster.Assets.Model(modelPath);

            if (!model.succeeded)
            {
                return;
            }

            Shader shader;

            if (shaderPath == "")
            {
                shader = Disaster.Assets.defaultShader;
            } else
            {
                var loadedShader = Disaster.Assets.Shader(shaderPath);
                if (loadedShader.succeeded)
                {
                    shader = loadedShader.shader;
                } else
                {
                    shader = Disaster.Assets.defaultShader;
                }
            }

            if (parameters == null)
            {
                Disaster.ModelRenderer.EnqueueRender(model.model, shader, transform);
            } 
            else
            {
                var parms = Disaster.TypeInterface.ShaderParameters(parameters);
                Disaster.ModelRenderer.EnqueueRender(model.model, shader, transform, parms);
            }
        }

        [JSFunction(Name = "colorBuffer")]
        [FunctionDescription("Draw a color buffer.")]
        [ArgumentDescription("colors", "color array defining the image", "{r, g, b, a}[]")]
        [ArgumentDescription("x", "x position to draw at")]
        [ArgumentDescription("y", "y position to draw at")]
        [ArgumentDescription("width", "width of the image")]
        public static void ColorBuffer(ObjectInstance colors, int x, int y, int width)
        {
            var texture = new Disaster.PixelBuffer(Disaster.TypeInterface.Color32Array(colors), width).texture;
            EnqueueRenderAction(() => { Raylib.DrawTexture(texture, x, y, Raylib_cs.Color.WHITE); });
        }

        [JSFunction(Name = "startBuffer")]
        [FunctionDescription("Start drawing to a pixel buffer instead of the screen. Call with Draw.endBuffer();")]
        [ArgumentDescription("width", "Width of the new buffer to draw to")]
        [ArgumentDescription("height", "Height of the new buffer to draw to")]
        public static void StartBuffer(int width, int height)
        {
            if (!Disaster.BufferRenderer.StartBuffer(width, height))
                Console.WriteLine("In Buffer!!");
        }

        [JSFunction(Name = "endBuffer")]
        [FunctionDescription("Finish drawing to a pixel buffer and return a reference to the new texture.")]
        public static string EndBuffer()
        {
            return Disaster.BufferRenderer.EndBuffer();
        }

        [JSFunction(Name = "texture")]
        [FunctionDescription("Draw a part of an image to the software canvas, with transformations")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("rectangle", "(optional) rectangle defining the portion of the image to draw", "{ x, y, w, h }")]
        [ArgumentDescription("transformation", "(optional) scaling, rotation and origin properties", "{ originX, originY, rotation, scaleX, scaleY, alpha }")]
        public static void Texture(string texturePath, int x, int y, ObjectInstance rectangle = null, ObjectInstance transformation = null)
        {
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            if (pixelBuffer.succeeded)
            {
                x += Disaster.ScreenController.offset.x;
                y += Disaster.ScreenController.offset.y;
                var texture = pixelBuffer.pixelBuffer.texture;
                var rect = rectangle != null ? Disaster.TypeInterface.Rect(rectangle) : new Disaster.Rect(0, 0, texture.width, texture.height);
                var trans = transformation != null ? Disaster.TypeInterface.Transform2d(transformation) : new Disaster.Transform2D(new Vector2(0, 0), new Vector2(1, 1), 0f, 1f);

                if (trans.scale.X < 0) rect.width *= -1;
                if (trans.scale.Y < 0) rect.height *= -1;
                            
                var sourceRect = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                var destRect = new Rectangle(x, y, rect.width * trans.scale.X, rect.height * trans.scale.Y);

                trans.scale.X = Math.Abs(trans.scale.X);
                trans.scale.Y = Math.Abs(trans.scale.Y);

                EnqueueRenderAction(() =>
                {
                    Raylib.DrawTexturePro(texture, sourceRect, destRect, new Vector2(trans.origin.X * trans.scale.X, trans.origin.Y * trans.scale.Y), trans.rotation, Raylib_cs.Color.WHITE);
                });
            }
            else
            {
                Console.WriteLine($"Failed to draw texture: {texturePath}");
            }
        }

        [JSFunction(Name = "nineSlice")]
        [FunctionDescription("Draw a 9-sliceds sprite. Tiles the center and edges of a sprite over a given area. (look up 9-slice!)")]
        [ArgumentDescription("texturePath", "Texture to draw")]
        [ArgumentDescription("nineSliceArea", "A rectangle defining the center region of the 9-slide", "{x, y, w, h}")]
        [ArgumentDescription("x", "x position to draw to")]
        [ArgumentDescription("y", "y position to draw to")]
        [ArgumentDescription("width", "width of the area to draw to")]
        [ArgumentDescription("height", "height of the area to draw to")]
        public static void NineSlice(string texturePath, ObjectInstance nineSliceArea, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;
            
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            if (!pixelBuffer.succeeded)
            {
                Console.WriteLine($"Failed to draw texture: {texturePath}");
                return;
            }

            x += Disaster.ScreenController.offset.x;
            y += Disaster.ScreenController.offset.y;
            var texture = pixelBuffer.pixelBuffer.texture;
            var rect = Disaster.TypeInterface.Rect(nineSliceArea);

            // We don't use raylibs built in NPatch because it stretches rather than tiles
            var xs = new[] {0, (int) rect.x, (int) (rect.x + rect.width), texture.width};
            var ys = new[] {0, (int) rect.y, (int) (rect.y + rect.height), texture.height};

            var srcRects = new Rectangle[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    srcRects[i, j] = new Rectangle(xs[j], ys[i], xs[j + 1] - xs[j], ys[i + 1] - ys[i]);
                }
            }

            var w1 = Math.Min(xs[1] - xs[0], width);
            var w3 = Math.Min(xs[3] - xs[2], width - w1);
            var w2 = width - w1 - w3;
            var ws = new[] {w1, w2, w3};

            var h1 = Math.Min(ys[1] - ys[0], height);
            var h3 = Math.Min(ys[3] - ys[2], height - h1);
            var h2 = height - h1 - h3;
            var hs = new[] {h1, h2, h3};
            
            EnqueueRenderAction(() =>
            {
                int ry = y;
                for (int i = 0; i < 3; i++)
                {
                    if (hs[i] <= 0) break;

                    int rx = x;
                    for (int j = 0; j < 3; j++)
                    {
                        if (ws[j] <= 0) break;

                        var dstRect = new Rectangle(rx, ry, ws[j], hs[i]);
                        Raylib.DrawTextureTiled(texture, srcRects[i, j], dstRect, Vector2.Zero, 0, 1,
                            Raylib_cs.Color.WHITE);
                        rx += ws[j];
                    }

                    ry += hs[i];
                }
            });
        }

        [JSFunction(Name = "setCamera")]
        [FunctionDescription("Set the 3d camera position and rotation")]
        [ArgumentDescription("position", "position to set the camera to", "{x, y, z}")]
        [ArgumentDescription("rotation", "rotation to set the camera to, in euler angles", "{x, y, z}")]
        public static void SetCamera(ObjectInstance position, ObjectInstance rotation)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var forward = Disaster.Util.EulerToForward(rot);
            Disaster.ScreenController.camera.position = pos;
            Disaster.ScreenController.camera.target = pos + forward;
        }

        [JSFunction(Name = "setFOV")]
        [FunctionDescription("Set the field of view of the camera")]
        [ArgumentDescription("fov", "field of view")]
        public static void SetFOV(double fov)
        {
            Disaster.ScreenController.camera.fovy = (float)fov;
        }

        [JSFunction(Name = "getCameraTransform")]
        [FunctionDescription("Get the 3d camera transformation", "{forward, up, right, position, rotation}")]
        public static ObjectInstance GetCameraTransform()
        {
            var output = Disaster.JS.instance.engine.Object.Construct();
            output["forward"] = Disaster.TypeInterface.Object(Vector3.Normalize(Disaster.ScreenController.camera.target - Disaster.ScreenController.camera.position));
            output["up"] = Disaster.TypeInterface.Object(Disaster.ScreenController.camera.up);
            output["right"] = Disaster.TypeInterface.Object(Vector3.Cross(Disaster.ScreenController.camera.target - Disaster.ScreenController.camera.position, Disaster.ScreenController.camera.up));
            output["position"] = Disaster.TypeInterface.Object(Disaster.ScreenController.camera.position);
            output["rotation"] = Disaster.TypeInterface.Object(Disaster.Util.ForwardToEuler(Disaster.ScreenController.camera.target - Disaster.ScreenController.camera.position));
            return output;
        }

        [JSFunction(Name ="setBlendMode")]
        [FunctionDescription("Set the blending mode for future draw operations. normal, add, multiply")]
        public static void SetBlendMode(string blendMode)
        {
            BlendMode mode;
            switch (blendMode)
            {
                case "normal":
                    mode = BlendMode.BLEND_ALPHA;
                    break;
                case "add":
                    mode = BlendMode.BLEND_ADDITIVE;
                    break;
                case "multiply":
                    mode = BlendMode.BLEND_MULTIPLIED;
                    break;
                // Raylibs subtractive mode is bad
                // case "subtract":
                //     mode = BlendMode.BLEND_SUBTRACT_COLORS;
                //     break;
                default:
                    Console.WriteLine($"Unknown blendmode: {blendMode}");
                    return;
            }
            
            EnqueueRenderAction(() =>
            {
                Raylib.EndBlendMode();
                Disaster.BufferRenderer.blendMode = mode;
                Disaster.ShapeRenderer.blendMode = mode;
                Raylib.BeginBlendMode(mode);
            });
            
        }

        private static void EnqueueRenderAction(Action renderAction)
        {
            if (Disaster.BufferRenderer.inBuffer)
                Disaster.BufferRenderer.Enqueue(renderAction);
            else
                Disaster.ShapeRenderer.EnqueueRender(renderAction);
        }
    }
}