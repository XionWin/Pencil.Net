using System.Numerics;
using System.Runtime.InteropServices;

namespace App.VG;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{    public Vector2 Position { get; init; }
    public Vector2 Coordinate { get; init; }
}