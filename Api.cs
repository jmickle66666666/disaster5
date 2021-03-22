// extra stuff

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

    public struct Color33
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
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
