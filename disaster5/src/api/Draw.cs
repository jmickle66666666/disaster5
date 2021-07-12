//using OpenGL;
using Jurassic;
using Jurassic.Library;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DisasterAPI
{

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

        [JSProperty(Name = "fontHeight")] public static int fontHeight { get { return Disaster.SoftwareCanvas.fontHeight; } }
        [JSProperty(Name = "fontWidth")] public static int fontWidth { get { return Disaster.SoftwareCanvas.fontWidth; } }
        [JSProperty(Name = "screenWidth")] public static int screenWidth { get { return Disaster.ScreenController.screenWidth; } }
        [JSProperty(Name = "screenHeight")] public static int screenHeight { get { return Disaster.ScreenController.screenHeight; } }

        [JSFunction(Name = "offset")]
        [FunctionDescription("Set a global offset for 2D rendering.")]
        [ArgumentDescription("x", "Pixels in the x axis to offset by")]
        [ArgumentDescription("y", "Pixels in the y axis to offset by")]
        public static void Offset(int x, int y)
        {
            Disaster.SoftwareCanvas.offsetX = x;
            Disaster.SoftwareCanvas.offsetY = y;
        }


        [JSFunction(Name = "rect")]
        [FunctionDescription("Draw a rectangle")]
        [ArgumentDescription("x", "x position of the rectangle")]
        [ArgumentDescription("y", "y position of the rectangle")]
        [ArgumentDescription("width", "width of the rectangle")]
        [ArgumentDescription("height", "height of the rectangle")]
        [ArgumentDescription("color", "Rectangle color", "{r, g, b, a}")]
        [ArgumentDescription("filled", "Draw a filled rect (true) or an outline (false)")]
        public static void Rect(int x, int y, int width, int height, ObjectInstance color, bool filled)
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
        [ArgumentDescription("filled", "Draw a filled triangle (true) or an outline (false)")]
        public static void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, ObjectInstance color, bool filled)
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
        [ArgumentDescription("filled", "Draw a filled circle (true) or an outline (false)")]
        public static void Circle(int x, int y, double radius, ObjectInstance color, bool filled)
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
        [FunctionDescription("Draw a 2d line.")]
        [ArgumentDescription("x1", "starting x position")]
        [ArgumentDescription("y1", "starting y position")]
        [ArgumentDescription("x2", "ending x position")]
        [ArgumentDescription("y2", "ending y position")]
        [ArgumentDescription("color", "line color", "{r, g, b, a}")]
        public static void Line(int x1, int y1, int x2, int y2, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(color));
        }

        [JSFunction(Name = "lineGradient")]
        [FunctionDescription("Draw a 2d line with a gradient.")]
        [ArgumentDescription("x1", "starting x position")]
        [ArgumentDescription("y1", "starting y position")]
        [ArgumentDescription("x2", "ending x position")]
        [ArgumentDescription("y2", "ending y position")]
        [ArgumentDescription("colorStart", "color at the start of the line", "{r, g, b, a}")]
        [ArgumentDescription("colorEnd", "color at the end of the line", "{r, g, b, a}")]
        public static void Line(int x1, int y1, int x2, int y2, ObjectInstance colorStart, ObjectInstance colorEnd)
        {
            Disaster.SoftwareCanvas.Line(x1, y1, x2, y2, Disaster.TypeInterface.Color32(colorStart), Disaster.TypeInterface.Color32(colorEnd));
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
        [ArgumentDescription("x", "x position of the text")]
        [ArgumentDescription("y", "x position of the text")]
        [ArgumentDescription("text", "the text content to draw")]
        [ArgumentDescription("color", "text color", "{r, g, b, a}")]
        public static void Text(int x, int y, string text, ObjectInstance color)
        {
            Disaster.SoftwareCanvas.Text(x, y, Disaster.TypeInterface.Color32(color), text);
        }

        [JSFunction(Name = "model")]
        [FunctionDescription("Draw a 3D model.")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        public static void Model(ObjectInstance position, ObjectInstance rotation, string modelPath)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);

            var model = Disaster.Assets.Model(modelPath);
            Disaster.ModelRenderer.EnqueueRender(model, Disaster.Assets.defaultShader, transform);
        }

        [JSFunction(Name = "wireframe")]
        [FunctionDescription("Draw a 3D wireframe.")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("color", "Color of the wireframe", "{r, g, b, a}")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        [ArgumentDescription("backfaceCulling", "Whether to skip triangles that face away from the camera")]
        [ArgumentDescription("drawDepth", "Whether to render depth on the lines")]
        public static void Wireframe(ObjectInstance position, ObjectInstance rotation, ObjectInstance color, string modelPath, bool backfaceCulling, bool drawDepth, bool filled)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);
            var col = Disaster.TypeInterface.Color32(color);

            var model = Disaster.Assets.Model(modelPath);
            
            unsafe
            {
                var mesh = ((Raylib_cs.Mesh*)model.meshes.ToPointer())[0];
                Disaster.SoftwareCanvas.Wireframe(mesh, transform.ToMatrix(), col, backfaceCulling, drawDepth, filled);
            }
        }

        [JSFunction(Name = "modelShader")]
        [FunctionDescription("Draw a 3D model with a specified shader.")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("shaderPath", "Path of the shader to use (without extension)")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        public static void Model(ObjectInstance position, ObjectInstance rotation, string shaderPath, string modelPath)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);
            var model = Disaster.Assets.Model(modelPath);
            var shader = Disaster.Assets.Shader(shaderPath);
            Disaster.ModelRenderer.EnqueueRender(model, shader, transform);
        }

        [JSFunction(Name = "modelShaderParams")]
        [FunctionDescription("Draw a 3D model with a specified shader and parameters to use.")]
        [ArgumentDescription("position", "Position to draw at", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation in euler angles", "{x, y, z}")]
        [ArgumentDescription("shaderPath", "Path of the shader to use (without extension)")]
        [ArgumentDescription("parameters", "Object of key/value pairs to send to the shader")]
        [ArgumentDescription("modelPath", "Path of the model to draw")]
        public static void Model(ObjectInstance position, ObjectInstance rotation, string shaderPath, ObjectInstance parameters, string modelPath)
        {
            var rot = Disaster.TypeInterface.Vector3(rotation);
            var pos = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(pos, rot, Vector3.One);
            var model = Disaster.Assets.Model(modelPath);
            var shader = Disaster.Assets.Shader(shaderPath);
            var parms = Disaster.TypeInterface.ShaderParameters(parameters);
            Disaster.ModelRenderer.EnqueueRender(model, shader, transform, parms);
        }

        [JSFunction(Name = "colorBuffer")]
        [FunctionDescription("Draw a color buffer.")]
        [ArgumentDescription("x", "x position to draw at")]
        [ArgumentDescription("y", "y position to draw at")]
        [ArgumentDescription("colors", "color array defining the image", "{r, g, b, a}[]")]
        [ArgumentDescription("width", "width of the image")]
        public static void ColorBuffer(int x, int y, ObjectInstance colors, int width)
        {
            int len = (int)colors.GetPropertyValue("length");
            Disaster.Color32[] color32s = new Disaster.Color32[len];
            for (int i = 0; i < len; i++)
            {
                color32s[i] = Disaster.TypeInterface.Color32((ObjectInstance)colors.GetPropertyValue(i));
            }
            var pixelBuffer = new Disaster.PixelBuffer(color32s, width);
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
            return Disaster.SoftwareCanvas.EndBuffer();
        }

        [JSFunction(Name = "texture")]
        [FunctionDescription("Draw an image to the software canvas.")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void Texture(int x, int y, string texturePath)
        {
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, Disaster.Transform2D.identity);
        }

        [JSFunction(Name = "textureTransformed")]
        [FunctionDescription("Draw an image to the software canvas, with scaling, rotation and origin offset.")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("transformation", "scaling, rotation and origin properties", "{ originX, originY, rotation, scaleX, scaleY }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TextureTransformed(int x, int y, ObjectInstance transformation, string texturePath)
        {
            var trans = Disaster.TypeInterface.Transform2d(transformation);
            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, trans);
        }

        [JSFunction(Name = "texturePart")]
        [FunctionDescription("Draw a part of an image to the software canvas")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("rectangle", "rectangle defining the portion of the image to draw", "{ x, y, w, h }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TexturePart(int x, int y, ObjectInstance rectangle, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, rect);
        }

        [JSFunction(Name = "texturePartTransformed")]
        [FunctionDescription("Draw a part of an image to the software canvas, with transformations")]
        [ArgumentDescription("x", "x position of the image")]
        [ArgumentDescription("y", "x position of the image")]
        [ArgumentDescription("rectangle", "rectangle defining the portion of the image to draw", "{ x, y, w, h }")]
        [ArgumentDescription("transformation", "scaling, rotation and origin properties", "{ originX, originY, rotation, scaleX, scaleY }")]
        [ArgumentDescription("texturePath", "path to the image asset")]
        public static void TexturePartTransformed(int x, int y, ObjectInstance rectangle, ObjectInstance transformation, string texturePath)
        {
            var rect = Disaster.TypeInterface.Rect(rectangle);
            var trans = Disaster.TypeInterface.Transform2d(transformation);

            var pixelBuffer = Disaster.Assets.PixelBuffer(texturePath);
            Disaster.SoftwareCanvas.PixelBuffer(pixelBuffer, x, y, rect, trans);
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

    }
}