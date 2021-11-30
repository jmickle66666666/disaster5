using Jurassic;
using Jurassic.Library;
using Raylib_cs;
using System.Numerics;

namespace DisasterAPI
{
    [ClassDescription("Collision related functions")]
    public class Physics : ObjectInstance
    {
        public Physics(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "getCollisionRayGround")]
        [FunctionDescription("Cast a ray against the floor plane at the specified height", "{hit, distance, position, normal}")]
        [ArgumentDescription("ray", "Ray to cast", "{position, direction}")]
        [ArgumentDescription("height", "Height of the floor plane")]
        public static ObjectInstance GetCollisionRayGround(ObjectInstance ray, double height)
        {
            var rayo = Disaster.TypeInterface.Ray(ray);
            var output = Raylib.GetCollisionRayGround(rayo, (float)height);
            return Disaster.TypeInterface.Object(output);
        }

        [JSFunction(Name = "screenPointToRay")]
        [FunctionDescription("Create a ray from the screen position and return it", "{position, direction}")]
        [ArgumentDescription("x", "Screen X position to start the ray")]
        [ArgumentDescription("y", "Screen Y position to start the ray")]
        public static ObjectInstance ScreenPointToRay(int x, int y)
        {
            float ratioW = (float)Disaster.ScreenController.screenWidth / (float)Disaster.ScreenController.windowWidth;
            float ratioH = (float)Disaster.ScreenController.screenHeight / (float)Disaster.ScreenController.windowHeight;

            return Disaster.TypeInterface.Object(
                Raylib.GetMouseRay(
                    new Vector2(x / ratioW, y / ratioH),
                    Disaster.ScreenController.camera
                )
            );
        }

        [JSFunction(Name = "getCollisionRayModel")]
        [FunctionDescription("Cast a ray against a model at a specified position and rotation", "{hit, distance, position, normal}")]
        [ArgumentDescription("ray", "Ray to cast against the model", "{position, direction}")]
        [ArgumentDescription("position", "Position of the model", "{x, y, z}")]
        [ArgumentDescription("rotation", "Rotation of the model in euler angles", "{x, y, z}")]
        [ArgumentDescription("modelPath", "Path of the model to cast against")]
        public static ObjectInstance GetCollisionRayModel(ObjectInstance ray, ObjectInstance position, ObjectInstance rotation, string modelPath)
        {
            var rayo = Disaster.TypeInterface.Ray(ray);
            var model = Disaster.Assets.Model(modelPath);
            if (!model.succeeded)
            {
                System.Console.WriteLine($"Failed to load model: {modelPath}");
                return null;
            }
            var modelPosition = Disaster.TypeInterface.Vector3(position);
            var transform = new Disaster.Transformation(modelPosition, Disaster.TypeInterface.Vector3(rotation), Vector3.One).ToMatrix();
            Matrix4x4.Invert(transform, out Matrix4x4 inverse);
            rayo.position = Vector3.Transform(rayo.position, inverse);
            rayo.direction = Vector3.TransformNormal(rayo.direction, inverse);
            //model.transform = transform;


            
            RayHitInfo output = Raylib.GetCollisionRayModel(rayo, model.model);
            output.position = Vector3.Transform(output.position, transform);
            output.normal = Vector3.TransformNormal(output.normal, transform);
            
            //model.transform = Matrix4x4.Identity;
            return Disaster.TypeInterface.Object(output);
        }

        [JSFunction(Name = "getCollisionRayPlane")]
        [FunctionDescription("Cast a ray against a plane", "{hit, distance, position, normal}")]
        [ArgumentDescription("ray", "Ray to cast against the plane", "{position, direction}")]
        [ArgumentDescription("plane", "Position and normal of the plane", "{position: {x, y, z}, normal: {x, y, z}}")]
        public static ObjectInstance GetCollisionRayPlane(ObjectInstance ray, ObjectInstance plane)
        {
            return Disaster.TypeInterface.Object(
                Disaster.Util.GetCollisionRayPlane(
                    Disaster.TypeInterface.Ray(ray), 
                    Disaster.TypeInterface.Plane(plane)
                )
            );
        }
    }
}