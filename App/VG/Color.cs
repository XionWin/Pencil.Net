namespace App.VG;

public class Color: ICloneable<Color>
{
	public float R { get; set; }
	public float G { get; set; }
	public float B { get; set; }
	public float A { get; set; }

	public Color() {}

	public Color(float r, float g, float b, float a)
	{
		this.R = r;
		this.G = g;
		this.B = b;
		this.A = a;
	}

    public Color Clone() =>
		new Color()
		{
			R = this.R,
			G = this.G,
			B = this.B,
			A = this.A,
		};
}