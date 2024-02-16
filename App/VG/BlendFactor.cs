namespace App.VG;

[Flags]
public enum BlendFactor
{
    Zero = 1 << 0,
    One = 1 << 1,
    SrcColor = 1 << 2,
    OneMinusSrcColor = 1 << 3,
    DstColor = 1 << 4,
    OneMinusDstColor = 1 << 5,
    SrcAlpha = 1 << 6,
    OneMinusSrcAlpha = 1 << 7,
    DstAlpha = 1 << 8,
    OneMinusDstAlpha = 1 << 9,
    SrcAlphaSaturate = 1 << 10,
}