﻿using Jurassic;
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
        public static ObjectInstance GetCollisionRayGround(ObjectInstance ray, double height)
        {
            var rayo = Disaster.TypeInterface.Ray(ray);
            var output = Raylib.GetCollisionRayGround(rayo, (float)height);
            return Disaster.TypeInterface.Object(output);
        }

        [JSFunction(Name = "screenPointToRay")]
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