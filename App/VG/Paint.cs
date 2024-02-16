namespace App.VG;

public class Paint: ICloneable<Paint>
{
    public Matrix2D Xform { get; private set; }
    public float[] Extent { get; private set; } = new float[2];
    public float Radius { get; set; } = 0;
    public float Feather { get; set; } = 1;
    public Color InnerColor { get; set; } = new Color();
    public Color OuterColor { get; set; } = new Color();
    public int Texture { get; set; }

    public Paint()
    {
    }

    public Paint(Color color)
    {
        this.InnerColor = this.OuterColor = color;
    }

    public Paint Clone() => 
        new Paint()
        {
            Xform = this.Xform,
            Extent = this.Extent,
            Radius = this.Radius,
            Feather = this.Feather,
            InnerColor = this.InnerColor.Clone(),
            OuterColor = this.OuterColor.Clone(),
            Texture = this.Texture,
        };
}