// extra stuff
using System.Numerics;
using System;
using Raylib_cs;

namespace Disaster
{
    public static class Util
    {
        public static int Lerp(int a, int b, float t)
        {
            return (int) ((b * t) + (a * (1 - t)));
        }

        public static Vector3 EulerToForward(Vector3 eulers)
        {
            var pitch = eulers.X * MathF.PI / 180f;
            var yaw = eulers.Y * MathF.PI / 180f;
            var roll = eulers.Z * MathF.PI / 180f;

            var a = MathF.Sin(pitch);
            var b = MathF.Cos(yaw) * MathF.Cos(pitch);
            var c = (MathF.Sin(yaw) * MathF.Cos(pitch));

            return new Vector3(
                c, -a, b
            );
        }

        public static Vector3 ForwardToEuler(Vector3 forward)
        {
            forward = Vector3.Normalize(forward);
            return new Vector3(
                MathF.Asin(-forward.Y) * 180f / MathF.PI,
                MathF.Atan2(forward.X, forward.Z) * 180f / MathF.PI,
                0f
            );
        }

        public static (Vector3 axis, float rotation) EulerToAxisAngle(Vector3 eulers)
        {
            if (eulers.X >= 360f) eulers.X %= 360f;
            if (eulers.Y >= 360f) eulers.Y %= 360f;
            if (eulers.Z >= 360f) eulers.Z %= 360f;

            var heading = eulers.Y * Math.PI / 180;
            var altitude = eulers.X * Math.PI / 180;
            var bank = eulers.Z * Math.PI / 180;

            // Assuming the angles are in radians.
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(altitude / 2);
            double s2 = Math.Sin(altitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            double w = c1c2 * c3 - s1s2 * s3;
            double x = c1c2 * s3 + s1s2 * c3;
            double y = s1 * c2 * c3 + c1 * s2 * s3;
            double z = c1 * s2 * c3 - s1 * c2 * s3;
            double angle = 2 * Math.Acos(w);
            double norm = x * x + y * y + z * z;
            if (norm < 0.00001)
            { // when all euler angles are zero angle =0 so
              // we can set axis to anything to avoid divide by zero
                x = 1;
                y = z = 0;
            }
            else
            {
                norm = Math.Sqrt(norm);
                x /= norm;
                y /= norm;
                z /= norm;
            }

            return (
                new Vector3((float) x, (float)y, (float)z), (float)angle
            );
        }

        public static Raylib_cs.Mesh GetFirstMesh(this Raylib_cs.Model model)
        {
            unsafe
            {
                Raylib_cs.Mesh* meshes = (Raylib_cs.Mesh*)model.meshes;
                return meshes[0];
            }
        }

        public static Raylib_cs.RayHitInfo GetCollisionRayPlane(Raylib_cs.Ray ray, Plane plane)
        {
            float denom = Vector3.Dot(plane.Normal, ray.direction);
            if (denom > 0.00001f)
            {
                float t = (plane.D - Vector3.Dot(plane.Normal, ray.position)) / denom;
                Vector3 hitPoint = ray.position + ray.direction * t;
                return new Raylib_cs.RayHitInfo() { hit = 1, distance = t, normal = plane.Normal, position = hitPoint };
            //} else if (denom < 0.00001f)
            //{
            //    float t = (plane.D - Vector3.Dot(plane.Normal, ray.position)) / denom;
            //    Vector3 hitPoint = ray.position + ray.direction * t;
            //    return new Raylib_cs.RayHitInfo() { hit = 1, distance = t, normal = -plane.Normal, position = hitPoint };
            } else
            {
                return new Raylib_cs.RayHitInfo() { hit = 0 };
            }
        }

        public static Plane CreatePlaneFromPositionNormal(Vector3 position, Vector3 normal)
        {
            normal = Vector3.Normalize(normal);
            return new Plane(normal, Vector3.Dot(position, normal));
        }

        public static bool IntersectSegmentPlane(Vector3 a, Vector3 b, Plane p, out float t, out Vector3 intersection)
        {
            // Compute the t value for the directed line ab intersecting the plane
            Vector3 ab = b - a;
            t = (p.D - Vector3.Dot(p.Normal, a)) / Vector3.Dot(p.Normal, ab);
            // If t in [0..1] compute and return intersection point
            if (t >= 0.0f && t <= 1.0f)
            {
                intersection = a + t * ab;
                return true;
            }
            // Else no intersection
            intersection = Vector3.Zero;
            return false;

        }
    }

    public struct Color32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public Color32(byte r, byte g, byte b, byte a)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color32(byte r, byte g, byte b)
        {
            this.a = 255;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color32(Color32 color, byte alpha)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = alpha;
        }

        public static Color32 Lerp(Color32 a, Color32 b, float t)
        {
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;
            return new Color32(
                (byte) ((a.r * (1f - t)) + b.r * t),
                (byte) ((a.g * (1f - t)) + b.g * t),
                (byte) ((a.b * (1f - t)) + b.b * t),
                (byte) ((a.a * (1f - t)) + b.a * t)
            );
        }

        public static implicit operator Color(Color32 color)
        {
            return new Color(color.r, color.g, color.b, color.a);
        }
    }

    public struct Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float x2
        {
            get
            {
                return x + width;
            }
        }
        public float y2
        {
            get
            {
                return y + height;
            }
        }

        public Rect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        
        public Vector2 center
        {
            get
            {
                return new Vector2(x + width / 2, y + height / 2);
            }
        }
    }

    public struct Transformation
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 rotationAxis;
        public Vector3 eulers;
        public float rotationAngle;

        public Transformation(Vector3 position, Vector3 eulers, Vector3 scale)
        {
            this.position = position;
            this.scale = scale;
            var angleAxis = Util.EulerToAxisAngle(eulers);
            this.rotationAxis = angleAxis.axis;
            this.rotationAngle = angleAxis.rotation * (180f/MathF.PI);
            this.eulers = eulers;
        }

        public Matrix4x4 ToMatrix()
        {
            var rot = Matrix4x4.CreateFromYawPitchRoll(
                (float)(eulers.Y * Math.PI / 180F),
                (float)(eulers.X * Math.PI / 180F),
                (float)(eulers.Z * Math.PI / 180F)
            );
            //var rot = Matrix4x4.CreateFromAxisAngle(rotationAxis, rotationAngle);
            var pos = Matrix4x4.CreateTranslation(position);
            return rot * pos;
        }
    }

    public struct Transform2D
    {
        public Vector2 origin;
        public Vector2 scale;
        public float rotation;
        public float alpha;

        public Transform2D(Vector2 origin, Vector2 scale, float rotation, float alpha = 1.0f)
        {
            this.origin = origin;
            this.scale = scale;
            this.rotation = rotation;
            this.alpha = alpha;
        }

        public Transform2D(float originX, float originY, float scaleX, float scaleY, float rotation, float alpha = 1.0f)
        {
            this.origin = new Vector2(originX, originY);
            this.scale = new Vector2(scaleX, scaleY);
            this.rotation = rotation;
            this.alpha = alpha;
        }

        public static Transform2D identity
        {
            get
            {
                return new Transform2D(0, 0, 1, 1, 0);
            }
        }
    }

    public struct Vector2Int {
        public int x;
        public int y;
        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.x - b.x, a.y - b.y);
        public static Vector2Int operator /(Vector2Int a, int b) => new Vector2Int(a.x / b, a.y / b);
        public static explicit operator Vector2Int(Vector2 vector) => new Vector2Int((int)vector.X, (int)vector.Y);
    }

    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;
        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector3Int operator +(Vector3Int a, Vector3Int b) => new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3Int operator -(Vector3Int a, Vector3Int b) => new Vector3Int(a.x - b.x, a.y - b.y, a.z + b.z);
        public static explicit operator Vector3Int(Vector3 vector) => new Vector3Int((int) vector.X, (int) vector.Y, (int) vector.Z);
    }
}
