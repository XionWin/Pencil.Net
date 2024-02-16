namespace App.VG;

public class PathPoint
{
    public float X { get; set; }
    public float Y { get; set; }

    public float Dx { get; private set; }
    public float Dy { get; private set; }

    public float Len { get; private set; }
    public float Dmx { get; set; }
    public float Dmy { get; set; }
    public PointFlags Flags { get; set; }

    public PathPoint(float x, float y, PointFlags pointFlags)
    {
        this.X = x;
        this.Y = y;
        this.Flags = pointFlags;
    }

    public void UpdateDelta(float dx, float dy)
    {
        var len = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        if(len > 0)
        {
            var id = 1.0f / len;
            this.Dx = dx * id is var dxx && dxx == 0 ? 0 : dxx;
            this.Dy = dy * id is var dyy && dyy == 0 ? 0 : dyy;
        }
        this.Len = len;
    }
}

public static class PointExtension
{
    internal static float Distance(this PathPoint point, PathPoint other) => (float)Math.Sqrt(Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2));

    internal static float Distance(this PathPoint point, float x, float y) => (float)Math.Sqrt(Math.Pow(point.X - x, 2) + Math.Pow(point.Y - y, 2));

}