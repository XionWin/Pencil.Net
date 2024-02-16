namespace App.VG;

public class CompositeOperationState
{
	public BlendFactor SrcRGB { get; set; }
	public BlendFactor DstRGB { get; set; }
	public BlendFactor SrcAlpha { get; set; }
	public BlendFactor DstAlpha { get; set; }

	public CompositeOperationState(CompositeOperation co)
	{
		BlendFactor sFactor = BlendFactor.Zero, dFactor = BlendFactor.Zero;

		if (co == CompositeOperation.SourceOver)
		{
			sFactor = BlendFactor.One;
			dFactor = BlendFactor.OneMinusSrcAlpha;
		}
		else if (co == CompositeOperation.SourceIn)
		{
			sFactor = BlendFactor.DstAlpha;
			dFactor = BlendFactor.Zero;
		}
		else if (co == CompositeOperation.SourceOut)
		{
			sFactor = BlendFactor.OneMinusDstAlpha;
			dFactor = BlendFactor.Zero;
		}
		else if (co == CompositeOperation.ATop)
		{
			sFactor = BlendFactor.DstAlpha;
			dFactor = BlendFactor.OneMinusSrcAlpha;
		}
		else if (co == CompositeOperation.DestinationOver)
		{
			sFactor = BlendFactor.OneMinusDstAlpha;
			dFactor = BlendFactor.One;
		}
		else if (co == CompositeOperation.DestinationIn)
		{
			sFactor = BlendFactor.Zero;
			dFactor = BlendFactor.SrcAlpha;
		}
		else if (co == CompositeOperation.DestinationOut)
		{
			sFactor = BlendFactor.Zero;
			dFactor = BlendFactor.OneMinusSrcAlpha;
		}
		else if (co == CompositeOperation.DestinationATop)
		{
			sFactor = BlendFactor.OneMinusDstAlpha;
			dFactor = BlendFactor.SrcAlpha;
		}
		else if (co == CompositeOperation.Lighter)
		{
			sFactor = BlendFactor.One;
			dFactor = BlendFactor.One;
		}
		else if (co == CompositeOperation.Copy)
		{
			sFactor = BlendFactor.One;
			dFactor = BlendFactor.Zero;
		}
		else if (co == CompositeOperation.XOR)
		{
			sFactor = BlendFactor.OneMinusDstAlpha;
			dFactor = BlendFactor.OneMinusSrcAlpha;
		}

		this.SrcRGB = sFactor;
		this.DstRGB = dFactor;
		this.SrcAlpha = sFactor;
		this.DstAlpha = dFactor;
	}
}