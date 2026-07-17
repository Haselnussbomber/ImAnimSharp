using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimDragOpts
{
    /// <summary>
    /// Grid size for snapping (0,0 = no grid)
    /// </summary>
    public Vector2 SnapGrid;

    /// <summary>
    /// Array of custom snap points
    /// </summary>
    public Vector2* SnapPoints;

    /// <summary>
    /// Number of snap points
    /// </summary>
    public int SnapPointsCount;

    /// <summary>
    /// Duration of snap animation
    /// </summary>
    public float SnapDuration;

    /// <summary>
    /// Overshoot amount (0 = none, 1 = normal)
    /// </summary>
    public float Overshoot;

    /// <summary>
    /// Easing type for snap animation
    /// </summary>
    public ImAnimEaseType EaseType;

    public static ImAnimDragOpts Default() => new() { SnapDuration = 0.2f, EaseType = ImAnimEaseType.OutCubic };
}
