using System.Runtime.InteropServices;

namespace App.VG;

[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public Rect()
    {
        this.Left = float.MaxValue;
        this.Top = float.MaxValue;
        this.Right = float.MinValue;
        this.Bottom = float.MinValue;
    }

    public Rect(float left, float top, float right, float bottom)
    {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
    }
}