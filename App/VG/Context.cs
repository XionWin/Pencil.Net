using System.Xml.Schema;

namespace App.VG;

public class Context
{
    // public NVGparams parameters;
    private Stack<State> _states = new Stack<State>();
    private Queue<Path> _paths = new Queue<Path>();

    public List<Path> Paths => this._paths.ToList();
    public Cache Cache { get; } = new Cache();
    public float TessTol { get; private set; }
    public float DistTol { get; private set; }
    public float FringeWidth { get; private set; }
    public float DevicePxRatio { get; private set; }
    // public FONScontext fs;
    //[NVG_MAX_FONTIMAGES];
    // public int[] fontImages;
    // public int fontImageIdx;
    public int drawCallCount;
    public int fillTriCount;
    public int strokeTriCount;
    public int textTriCount;

    public Context(float ratio = 1)
    {
        this.TessTol = 0.25f / ratio;
        this.DistTol = 0.01f / ratio;
        this.FringeWidth = 1.0f / ratio;
        this.DevicePxRatio = ratio;
    }

    public void SaveSate()
    {
        var currState = this._states.Peek();
        this._states.Push(currState.Clone());
    }

    public void RestoreSate()
    {
        if(this._states.Count > 1)
            this._states.Pop();
    }

    public State GetState()
    {
        if(this._states.Any() is false)
        {
            var state = new State();
            this._states.Push(state);
        }
        return this._states.Peek();
    }

    public void BeginPath()
    {
        this.Cache.Clear();
    }

    public void AddCommand(Command command)
    {
        if(command.CommandType == CommandType.MoveTo)
        {
            this._paths.Enqueue(new Path());
        }
        if(this._paths.Peek() is Path path)
        {
            path.AddCommand(command, this.DistTol);
        }
        else
        {
            throw new Exception("Can't find the last path");
        }
    }

    public void Stroke()
    {
        this.Expand();

    }
}

public static class ContextExtension
{
    internal static Command Transfrom(this Context context, Command command)
    {
        command.Transfrom(context.GetState().Transform);
        return command;
    }

    internal static void Expand(this Context context)
    {
        foreach (var path in context.Paths)
        {
            path.Expand(context, PathType.Stroke);
        }
    }

    internal static float GetedgeAntiAliasWidth(this Context context) => 
        (context.GetState().StrokeWidth + context.FringeWidth) / 2;

    internal static float CurveDivs(this Context context)
    {
        var aaWidth = context.GetedgeAntiAliasWidth();
        float da = (float)Math.Acos(aaWidth / (aaWidth + context.TessTol)) * 2.0f;
        return Math.Max(2, (int)Math.Ceiling(Math.PI / da));
    }


}