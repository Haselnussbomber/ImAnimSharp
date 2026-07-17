using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Color gradient with any number of stops (sorted by position)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimGradient
{
    /// <summary>
    /// Positions along gradient [0,1], kept sorted
    /// </summary>
    public ImVector<Vector2> Positions;

    /// <summary>
    /// Colors at each position (sRGB)
    /// </summary>
    public ImVector<Vector2> Colors;
}
