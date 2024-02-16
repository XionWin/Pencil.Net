namespace App.VG;

public class Primitive
{
    // public int First { get; set; }
    // public int Count { get; set; }
    // public bool IsClosed { get; set; }
    // public int BevelCount { get; set; }
    public List<Vertex> Fill { get; private set; } = new List<Vertex>();
    public List<Vertex> Stroke { get; private set; } = new List<Vertex>();
    // public Winding Winding { get; set; }
    // public bool IsConvex { get; set; }

}