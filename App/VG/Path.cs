using System.Drawing;
using System.Linq;

namespace App.VG;

public enum PathType
{
    Stroke,
    Fill
}

public class Path
{
    private List<Command> _commands = new List<Command>();
    public List<Command> Commands => this._commands;
    private PathPoint[]? _points = null;
    public PathPoint[] Points => this._points is PathPoint[] points ? points : throw new Exception("No point in this path");

    public int Count => this.Points?.Length ?? 0;
    public int BevelCount { get; set; }
    public int VertexCount { get; set; }
    public bool IsClosed { get; set; }
    public Winding Winding { get; set; }
    public bool IsConvex { get; set; }
    public Rect Bounds { get; private set; }
    public bool IsFinalized { get; set; }

    public void AddCommand(Command command)
    {
        if(this.IsFinalized)
        {
            throw new Exception("Can't add command into a finalized path");
        }
        this.Commands.Add(command);
    }

    public void Stroke(Context context, PathType pathType)
    {
        // Finalize path
        this.IsFinalized = true;
        this._points = this.FinalizePath(context.DistTol);
        this.UpdateBounds();

        this.Expand(context, pathType);

        // Calculate max vertex usage.
        var nCap = context.CurveDivs();
        var lineJoin = context.GetState().LineJoin;
        var lineCap = context.GetState().LineCap;
        var cverts = 0;
        if (lineJoin == LineJoin.Round)
            cverts += (this.Count + this.BevelCount * (nCap + 2) + 1) * 2; // plus one for loop
        else
            cverts += (this.Count + this.BevelCount * 5 + 1) * 2; // plus one for loop
        if (this.IsClosed is false)
        {
            // space for caps
            if (lineCap == LineCap.Round)
            {
                cverts += (nCap * 2 + 2) * 2;
            }
            else
            {
                cverts += (3 + 3) * 2;
            }
        }
        this.VertexCount = cverts;
    }

    // [TODO] Need update turly bounds when primitive generated
    private void UpdateBounds()
    {
        var minX = this.Points.Min(x => x.X);
        var maxX = this.Points.Max(x => x.X);
        var minY = this.Points.Min(x => x.Y);
        var maxY = this.Points.Max(x => x.Y);
        this.Bounds = new Rect (
            minX,
            minY,
            maxX,
            maxY
        );
    }
}

public static class FinalizedPathExtension
{
    public static PathPoint[] FinalizePath(this Path path, float distTol) =>
        path.Commands.SelectMany(x => x.ToPathPoints()).ToList()
        .Optimize(distTol)
        .EnforceWinding(path.Winding);

    private static IEnumerable<PathPoint> ToPathPoints(this Command command) =>
        command.CommandType switch
        {
            var type when type is CommandType.MoveTo || type is CommandType.LineTo => 
                command.Points.Select(x => new PathPoint(x.X, x.Y, PointFlags.Corner)),
            _ => throw new NotImplementedException()
        };

    internal static PathPoint[] EnforceWinding(this List<PathPoint> points, Winding winding)
    {

        if(points.Area() is float area)
        {
            if((winding is Winding.CCW && area < 0) || winding is Winding.CW && area > 0)
            {
                points.Reverse();
            }
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            var current = points[i];
            var next = points[i + 1];
            current.UpdateDelta(next.X - current.X, next.Y - current.Y);
        }

        var last = points.Last();
        var first = points.First();
        last.UpdateDelta(first.X - last.X, first.Y - last.Y);

        return points.ToArray();
    }
    
    public static List<PathPoint> Optimize(this List<PathPoint> points, float distTol)
    {
        for (int i = 1; i < points.Count; i++)
        {
            var current = points[i];
            var last = points[i - 1];
            if(last.Distance(current) < distTol)
            {
                last.Flags |= current.Flags;
                points.Remove(current);
            }
        }
        return points;
    }

    private static float? Area(this List<PathPoint> points)
    {
        var area = 0f;
        if(points.Count > 2)
        {
            for (int i = 2; i < points.Count; i++)
            {
                var a = points[0];
                var b = points[i - 1];
                var c = points[i];

                float abx = b.X - a.X;
                float aby = b.Y - a.Y;
                float acx = c.X - a.X;
                float acy = c.Y - a.Y;
                area += (acx * aby - abx * acy) / 2;
            }
            return area;
        }
        else
        {
            return null;
        }
    }

    public static void Expand(this Path path, Context context, PathType pathType)
    {
        var state = context.GetState();
        var scale = state.GetAverageScale();
        var strokeWidth = Math.Clamp(state.StrokeWidth * scale, 0.0f, 200.0f);

        var strokePaint = state.StrokePaint.Clone();

        if (strokeWidth < context.FringeWidth)
        {
            // If the stroke width is less than pixel size, use alpha to emulate coverage.
            // Since coverage is area, scale by alpha*alpha.
            float alpha = Math.Clamp(strokeWidth / context.FringeWidth, 0.0f, 1.0f);
            strokePaint.InnerColor.A *= alpha * alpha;
            strokePaint.OuterColor.A *= alpha * alpha;
            strokeWidth = context.FringeWidth;
        }

        // Apply global alpha
        strokePaint.InnerColor.A *= state.alpha;
        strokePaint.OuterColor.A *= state.alpha;

        path.CalculateJoins(context);

    }

    private static void CalculateJoins(this Path path, Context context)
    {
        var points = path.Points;

        var leftCount = 0;

        var lastPoint = points.LastOrDefault() is PathPoint lastPointTemp ? lastPointTemp : throw new Exception("The last point is null");
        foreach (var point in points)
        {
            var dmx = (lastPoint.Dy + point.Dy) / 2f;
            var dmy = (-lastPoint.Dx - point.Dx) / 2f;  
            var dmr2 = (float)Math.Pow(dmx, 2) + (float)Math.Pow(dmy, 2);
            if (dmr2 > 0.1e-6f)
            {
                float scale = 1.0f / dmr2;
                if (scale > 600.0f)
                {
                    scale = 600.0f;
                }
                point.Dmx = dmx * scale;
                point.Dmy = dmy * scale;
            }

            // Clear flags, but keep the corner.
            point.Flags = (point.Flags & PointFlags.Corner) != PointFlags.Null ? PointFlags.Corner : PointFlags.Null;

            // Keep track of left turns.
            var cross = point.Dx * lastPoint.Dy - lastPoint.Dx * point.Dy;
            if (cross > 0.0f)
            {
                leftCount++;
                point.Flags |= PointFlags.Left;
            }

            // Calculate if we should use bevel or miter for inner join.
            var strokeWidth = context.GetState().StrokeWidth;
            var miterLimit = context.GetState().MiterLimit;
            var lineJoin = context.GetState().LineJoin;
			var iw = 0.0f;
			if (strokeWidth > 0.0f)
				iw = 1.0f / strokeWidth;
            var limit = Math.Max(1.01f, Math.Min(lastPoint.Len, point.Len) * iw);
            if ((dmr2 * limit * limit) < 1.0f)
                point.Flags |= PointFlags.InnerBevel;

            // Check to see if the corner needs to be beveled.
            if (point.Flags.Contains(PointFlags.Corner))
            {
                if ((dmr2 * miterLimit * miterLimit) < 1.0f ||
                    lineJoin == LineJoin.Bevel ||
                    lineJoin == LineJoin.Round)
                {
                    point.Flags |= PointFlags.Bevel;
                }
            }

            if (point.Flags.Contains(PointFlags.Bevel | PointFlags.InnerBevel))
                path.BevelCount++;

            lastPoint = point;
        }
        path.IsConvex = leftCount == points.Length;

    }



}