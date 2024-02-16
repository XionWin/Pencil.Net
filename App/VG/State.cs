namespace App.VG;


public class State: ICloneable<State>
{
    public CompositeOperationState CompositeOperation { get; set; } = new CompositeOperationState(VG.CompositeOperation.SourceOver);
    public Paint FillPaint { get; private set; } = new Paint(new Color(255, 255, 255, 255));
    public Paint StrokePaint { get; private set; } = new Paint(new Color(0, 0, 0, 255));
    public float StrokeWidth { get; set; } = 1.0f;
    public float MiterLimit { get; set; } = 10.0f;
    public LineJoin LineJoin { get; set; } = LineJoin.Miter;
    public LineCap LineCap { get; set; } = LineCap.Butt;
    public float alpha { get; set; } = 1;
    public Matrix2D Transform { get; set; }
    public Scissor Scissor { get; private set; } = new Scissor();
    public float fontSize { get; set; }
    public float letterSpacing { get; set; }
    public float lineHeight { get; set; }
    public float fontBlur { get; set; }
    public int textAlign { get; set; }
    public int fontId { get; set; }

    public State Clone() => 
        new State()
        {
            CompositeOperation = this.CompositeOperation,
            FillPaint = this.FillPaint.Clone(),
            StrokePaint = this.StrokePaint.Clone(),
            StrokeWidth = this.StrokeWidth,
            MiterLimit = this.MiterLimit,
            LineJoin = this.LineJoin,
            LineCap = this.LineCap,
            alpha = this.alpha,
            Transform = this.Transform,
            Scissor = this.Scissor.Clone(),
            fontSize = this.fontSize,
            letterSpacing = this.letterSpacing,
            lineHeight = this.lineHeight,
            fontBlur = this.fontBlur,
            textAlign = this.textAlign,
            fontId = this.fontId,
        };
}


public static class StateExtension
{
    internal static float GetAverageScale(this State state)
    {
        float sx = (float)Math.Sqrt(state.Transform.M11 * state.Transform.M11 + state.Transform.M21 * state.Transform.M21);
        float sy = (float)Math.Sqrt(state.Transform.M12 * state.Transform.M12 + state.Transform.M22 * state.Transform.M22);
        return (sx + sy) * 0.5f;
    }
}