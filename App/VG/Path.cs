using System.Drawing;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;

namespace App.VG;

public enum PathType
{
    Stroke,
    Fill
}

public class Path
{
    private List<PathPoint> _points = new List<PathPoint>();

    private PathPoint[]? _pointCache = null;
    public PathPoint[] Points => _pointCache is null ? this._points.ToArray() : this._pointCache;
    public int Count => this._points.Count();
    public int First { get; set; }
    public bool IsClosed { get; set; }
    public int BevelCount { get; set; }
    public Winding Winding { get; set; }
    public bool IsConvex { get; set; }
    public Rect Bounds { get; private set; } = new Rect();
    public bool IsFinalized { get; internal set; }

    public void AddPoint(PathPoint point)
    {
        if(this.IsFinalized)
        {
            throw new Exception("Path is finalized");
        }
        this._points.Add(point);
        this.UpdateBounds(point);
        this._pointCache = null;
    }

    public PathPoint? GetFirstPoint() => this._points.FirstOrDefault() is PathPoint point ? point : null;
    public PathPoint? GetLastPoint() => this._points.LastOrDefault() is PathPoint point ? point : null;

    public void ReversePoints()
    {
        this._points.Reverse();
        this._pointCache = null;
    }

    private void UpdateBounds(PathPoint point)
    {
        var lastBounds = this.Bounds;
        this.Bounds = new Rect (
            Math.Min(lastBounds.Left, point.X),
            Math.Min(lastBounds.Top, point.Y),
            Math.Max(lastBounds.Right, point.X),
            Math.Max(lastBounds.Bottom, point.Y)
        );
    }

}

public static class PathExtension
{
    internal static void AddCommand(this Path path, Command command, float distTol)
    {
        switch (command.CommandType)
        {
            case CommandType.MoveTo:
            case CommandType.LineTo:
                path.AddCommandPoints(command.Points, PointFlags.Corner, distTol);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    internal static void AddCommandPoints(this Path path, CommandPoint[] points, PointFlags pointFlags, float distTol)
    {
        foreach (var point in points)
        {
            path.AddCommandPoint(point, pointFlags, distTol);
        }
    }


    internal static void AddCommandPoint(this Path path, CommandPoint point, PointFlags pointFlags, float distTol)
    {
        if(path.GetLastPoint() is PathPoint lastPoint)
        {
            if(lastPoint.Distance(point.X, point.Y) >= distTol)
            {
                var currentPoint = new PathPoint(point.X, point.Y, pointFlags);
                path.AddPoint(currentPoint);
            }
            else
            {
                lastPoint.Flags |= pointFlags;
            }
        }
        else
        {
            path.AddPoint(new PathPoint(point.X, point.Y, pointFlags));
        }
    }

    internal static void Expand(this Path path, Context context, PathType pathType)
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

        var nCap = context.CurveDivs();
        path.CalculateJoins(context);

    }

    private static void CalculateJoins(this Path path, Context context)
    {
        path.IsFinalized = true;
        var leftCount = 0;

        //Enforce winding
        path.EnforceWinding();


        var lastPoint = path.GetLastPoint() is PathPoint lastPointTemp ? lastPointTemp : throw new Exception("The last point is null");
        foreach (var point in path.Points)
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

            if (point.Flags.Contains(PointFlags.Bevel & PointFlags.InnerBevel))
                path.BevelCount++;

            lastPoint = point;
        }
        path.IsConvex = leftCount == path.Points.Count();

    }

    internal static void EnforceWinding(this Path path)
    {
        if(path.Area() is float area)
        {
            if((path.Winding is Winding.CCW && area < 0) || path.Winding is Winding.CW && area > 0)
            {
                path.ReversePoints();
            }
        }

        for (int i = 0; i < path.Points.Length - 1; i++)
        {
            var current = path.Points[i];
            var next = path.Points[i + 1];
            current.UpdateDelta(next.X - current.X, next.Y - current.Y);
        }

        var last = path.Points.Last();
        var first = path.Points.First();
        last.UpdateDelta(first.X - last.X, first.Y - last.Y);
    }

    internal static float? Area(this Path path)
    {
        var area = 0f;
        var points = path.Points;
        if(path.Count > 2)
        {
            for (int i = 2; i < points.Length; i++)
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



}