using Jurassic;
using Jurassic.Library;
using Raylib_cs;
using System.Numerics;

namespace DisasterAPI
{
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
    }
}