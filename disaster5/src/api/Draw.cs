//using OpenGL;
using Jurassic;
using Jurassic.Library;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DisasterAPI
{
    [ClassDescription("All your favourite ways to draw stuff to the screen!")]
    public class Draw : ObjectInstance {

        public Draw(ScriptEngine engine) : base(engine) {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "loadFont")]
        [FunctionDescription("Load a font for the software renderer. All future Draw.text calls will use the specified font.")]
        [ArgumentDescription("fontPath", "Path to the font texture. Fonts are 2-color images where pixels with a red value above zero are considered filled.")]
        public static void LoadFont(string fontPath) {
            if (!Disaster.Assets.LoadPath(fontPath, out string fontAssetPath))
            {
                return;
            }
            Disaster.SoftwareCanvas.LoadFont(fontAssetPath);
        }

        [JSFunction(Name = "clear")]
        [FunctionDescription("Clear the 2D canvas.")]
        public static void Clear() {
            Disaster.SoftwareCanvas.Clear();
        }

        [JSProperty(Name = "fontHeight")] 
        [PropertyDescription("Height, in pixels, of the currently loaded font.")]
        public static int fontHeight { get { return Disaster.SoftwareCanvas.fontHeight; } }
        [JSProperty(Name = "fontWidth")]
        [PropertyDescription("Width, in pixels, of the currently loaded font.")]
        public static int fontWidth { get { return Disaster.SoftwareCanvas.fontWidth; } }
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
            Disaster.SoftwareCanvas.offset.x = x;
            Disaster.SoftwareCanvas.offset.y = y;
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
            if (filled)
            {
                Disaster.SoftwareCanvas.FillRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
            } else
            {
                Disaster.SoftwareCanvas.DrawRect(x, y, width, height, Disaster.TypeInterface.Color32(color));
            }
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
            if (filled)
            {
                Disaster.SoftwareCanvas.Triangle(x1, y1, x2, y2, x3, y3, col);
            } else
            {
                Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, col);
                Disaster.SoftwareCanvas.Line(x3, y3, x2, y2, col);
                Disaster.SoftwareCanvas.Line(x1, y1, x3, y3, col);
            }
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
            if (filled)
            {
                Disaster.SoftwareCanvas.CircleFilled(x, y, (float)radius, col);
            }
            else
            {
                Disaster.SoftwareCanvas.Circle(x, y, (float)radius, col);
            }
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
            if (colorEnd == null)
            {
                Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(color));
            } else
            {
                Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(color), Disaster.TypeInterface.Color32(colorEnd));
            }
        }

        [JSFunction(Name = "line3d")]
        [FunctionDescription("Draw a 3d line!")]
        [ArgumentDescription("start", "start position", "{x, y, z}")]
        [ArgumentDescription("end", "end position", "{x, y, z}")]
        [ArgumentDescription("color", "line color", "{r, g, b, a}")]
        [ArgumentDescription("colorEnd", "(optional) line end color. if specified, will blend between the two colors along the line.", "{r, g, b, a}")]
        public static void Line3d(ObjectInstance start, ObjectInstance end, ObjectInstance color, ObjectInstance colorEnd = null)
        {
            if (colorEnd == null) colorEnd = color;
            Disaster.SoftwareCanvas.Line(
                Disaster.TypeInterface.Vector3(start),
                Disaster.TypeInterface.Vector3(end),
                Disaster.TypeInterface.Color32(color),
                Disaster.TypeInterface.Color32(colorEnd)
            );
        }

        [JSFunction(Name = "worldToScreenPoint")]
        [FunctionDescription("Transform a point from world position to screen position.", "{x, y}")]
        [ArgumentDescription("position", "World position to transform", "{x, y, z}")]
        public static ObjectInstance WorldToScreenPoint(ObjectInstance position)
        {
            float ratioW = (float)Disaster.ScreenController.screenWidth / (float)Disaster.ScreenController.windowWidth;
            float ratioH = (float)Disaster.ScreenController.screenHeight / (float)Disaster.ScreenController.windowHeight;
            var p = Raylib_cs.Raylib.GetWorldToScreen(Disaster.TypeInterface.Vector3(position), Disaster.ScreenController.camera);
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
            Disaster.SoftwareCanvas.Text(x, y, Disaster.TypeInterface.Color32(color), text);
        }

        [JSFunction(Name = "textStyled")]
        [FunctionDescription("Draw a line of text with styling options")]
        [ArgumentDescription("text", "the text to draw. $b for bold, $w for wavey, $s for drop shadow, $c for color, 0-F (e.g $c5hello $cAthere), $n to reset styling")]
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        public static void TextStyled(string text, int x, int y)
        {
            Disaster.SoftwareCanvas.TextStyled(x, y, text);
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
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);
            var col = Disaster.TypeInterface.Color32(color);

            var model = Disaster.Assets.Model(modelPath);
            if (model.succeeded)
            {
                unsafe
                {
                    var mesh = ((Raylib_cs.Mesh*)model.model.meshes.ToPointer())[0];
                    Disaster.SoftwareCanvas.Wireframe(mesh, transform.ToMatrix(), col, backfaceCulling, drawDepth, filled);
                }
            }
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

            Raylib_cs.Shader shader;

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
            var pixelBuffer = new Disaster.PixelBuffer(Disaster.TypeInterface.Color32Array(colors), width);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, Disaster.Transform2D.identity);
        }

        [JSFunction(Name = "startBuffer")]
        [FunctionDescription("Start drawing to a pixel buffer instead of the screen. Call with Draw.endBuffer();")]
        [ArgumentDescription("width", "Width of the new buffer to draw to")]
        [ArgumentDescription("height", "Height of the new buffer to draw to")]
        public static void StartBuffer(int width, int height)
        {
            Disaster.SoftwareCanvas.StartBuffer(width, height);
        }

        [JSFunction(Name = "endBuffer")]
        [FunctionDescription("Finish drawing to a pixel buffer and return a reference to the new texture.")]
        public static string EndBuffer()
        {
            string output = Disaster.SoftwareCanvas.CreateAssetFromBuffer();
            Disaster.SoftwareCanvas.EndBuffer();
            return output;
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
                var rect = rectangle == null ? new Disaster.Rect(0, 0, pixelBuffer.pixelBuffer.width, pixelBuffer.pixelBuffer.height) : Disaster.TypeInterface.Rect(rectangle);
                var trans = transformation == null ? Disaster.Transform2D.identity : Disaster.TypeInterface.Transform2d(transformation);
                if (trans.rotation % 90 != 0)
                {
                    // AWFUL HACK BECAUSE ROTATION SUCKS RN
                    trans.rotation -= 2;
                    Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer.pixelBuffer, x, y, rect, trans);
                    trans.rotation += 7;
                    Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer.pixelBuffer, x, y, rect, trans);
                    trans.rotation -= 5;
                    Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer.pixelBuffer, x, y, rect, trans);
                } else
                {
                    Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer.pixelBuffer, x, y, rect, trans);
                }
            } else
            {
                System.Console.WriteLine($"Failed to draw texture: {texturePath}");
            }
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
        public static void SetFOV(double fov)
        {
            System.Console.WriteLine($"current fov: {Disaster.ScreenController.camera.fovy}");
            Disaster.ScreenController.camera.fovy = (float)fov;
        }

        [JSFunction(Name = "getCameraTransform")]
        [FunctionDescription("Get the 3d camera transformation", "{forward, up, right}")]
        public static ObjectInstance GetCameraTransform()
        {
            var output = Disaster.JS.instance.engine.Object.Construct();
            output["forward"] = Disaster.TypeInterface.Object(Vector3.Normalize(Disaster.ScreenController.camera.target - Disaster.ScreenController.camera.position));
            output["up"] = Disaster.TypeInterface.Object(Disaster.ScreenController.camera.up);
            output["right"] = Disaster.TypeInterface.Object(Vector3.Cross(Disaster.ScreenController.camera.target - Disaster.ScreenController.camera.position, Disaster.ScreenController.camera.up));
            return output;
        }

        [JSFunction(Name ="setBlendMode")]
        [FunctionDescription("Set the blending mode for future draw operations. normal, noise, add")]
        public static void SetBlendMode(string blendMode)
        {
            switch (blendMode)
            {
                case "normal":
                    Disaster.SoftwareCanvas.blendMode = Disaster.SoftwareCanvas.BlendMode.Normal;
                    break;
                case "noise":
                    Disaster.SoftwareCanvas.blendMode = Disaster.SoftwareCanvas.BlendMode.Noise;
                    break;
                case "add":
                    Disaster.SoftwareCanvas.blendMode = Disaster.SoftwareCanvas.BlendMode.Add;
                    break;
                case "dither":
                    Disaster.SoftwareCanvas.blendMode = Disaster.SoftwareCanvas.BlendMode.Dither;
                    break;
                default:
                    System.Console.WriteLine($"Unknown blendmode: {blendMode}");
                    break;
            }
        }

    }
}