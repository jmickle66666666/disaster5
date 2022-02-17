using Jurassic.Library;
using Jurassic;
using System.Numerics;

namespace DisasterAPI
{
    [ClassDescription("3D Math Functions.")]
    class VMath : ObjectInstance
    {
        public VMath(ScriptEngine engine) : base(engine)
        {
            this.PopulateFunctions();
        }

        [JSFunction(Name = "forwardToEuler")]
        public static ObjectInstance ForwardToEuler(ObjectInstance forward)
        {
            return Disaster.TypeInterface.Object(Disaster.Util.ForwardToEuler(Disaster.TypeInterface.Vector3(forward)));
        }

        [JSFunction(Name = "closestPointOnLine")]
        public static ObjectInstance ClosestPointOnLine(ObjectInstance position, ObjectInstance direction, ObjectInstance point)
        {
            var pos = Disaster.TypeInterface.Vector3(position);
            var dir = Disaster.TypeInterface.Vector3(direction);
            var pnt = Disaster.TypeInterface.Vector3(point);

            dir = Vector3.Normalize(dir);
            var pntToLine = pnt - pos;
            var dot = Vector3.Dot(pntToLine, dir);
            return Disaster.TypeInterface.Object(pos + dir * dot);
        }
    }
}
