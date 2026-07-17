using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Descriptor for any easing (preset or parametric)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimEaseDesc
{
    public ImAnimEaseType Type;
    public float P0;
    public float P1;
    public float P2;
    public float P3;
}
