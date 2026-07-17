using System;
using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Color gradient with any number of stops (sorted by position)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimGradient() : IDisposable
{
    /// <summary>
    /// Positions along gradient [0,1], kept sorted
    /// </summary>
    public ImVector<Vector2> Positions = new();

    /// <summary>
    /// Colors at each position (sRGB)
    /// </summary>
    public ImVector<Vector2> Colors = new();

    public void Dispose()
    {
        Positions.Free();
        Colors.Free();
    }
}
