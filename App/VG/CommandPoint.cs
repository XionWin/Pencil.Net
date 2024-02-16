using System.Runtime.InteropServices;

namespace App.VG;

[StructLayout(LayoutKind.Sequential)]
public struct CommandPoint
{
    public float X { get; set; }
    public float Y { get; set; }
}