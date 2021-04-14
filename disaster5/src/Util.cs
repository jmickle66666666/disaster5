// extra stuff
using System.Numerics;

namespace Disaster
{
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

    public struct Transform2D
    {
        public Vector2 origin;
        public Vector2 scale;
        public float rotation;
        public Transform2D(Vector2 origin, Vector2 scale, float rotation)
        {
            this.origin = origin;
            this.scale = scale;
            this.rotation = rotation;
        }

        public Transform2D(float originX, float originY, float scaleX, float scaleY, float rotation)
        {
            this.origin = new Vector2(originX, originY);
            this.scale = new Vector2(scaleX, scaleY);
            this.rotation = rotation;
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
    }
}
