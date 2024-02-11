

using OpenTK.Mathematics;

namespace Common
{
    public struct AttribLocation
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public AttribLocation(string name, int start, int len)
        {
            this.Name = name;
            this.Start = start;
            this.Length = len;
        }
    }


    public struct Vertex2
    {
        public readonly static IEnumerable<AttribLocation> AttribLocations = new[]
        {
            new AttribLocation("aPos", 0, 2),
            new AttribLocation("aTexCoord", 2, 2),
        };

        public Vector2 Position { get; init; }
        public Vector2 Coordinate { get; init; }

        private float[]? raw = null;
        public float[] Raw
        {
            get
            {
                if (raw is null)
                {
                    raw = this.GetRaw();
                }
                return raw;
            }
        }

        public Vertex2(Vector2 position, Vector2 coordinate)
        {
            this.Position = position;
            this.Coordinate = coordinate;
        }
    }

    public static class Vertex2Extension
    {
        public static float[] GetRaw(this Vertex2 vertex) => new[] { vertex.Position.X, vertex.Position.Y, vertex.Coordinate.X, vertex.Coordinate.Y };
        public static float[] GetRaw(this IEnumerable<Vertex2> vertices) => vertices.SelectMany(x => x.Raw).ToArray();
    }
}
