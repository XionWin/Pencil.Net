namespace App.VG;

public class Scissor: ICloneable<Scissor>
{
    public float[] Xform { get; private set; } = new float[6];
    public float[] Extent { get; private init; } = new float[2];

    public Scissor Clone() => 
        new Scissor()
        {
            Xform = this.Xform,
            Extent = this.Extent,
        };
}