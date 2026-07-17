using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

[StructLayout(LayoutKind.Sequential)]
public struct ImAnimDragFeedback
{
    /// <summary>
    /// Current animated position
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Offset from drag start
    /// </summary>
    public Vector2 Offset;

    /// <summary>
    /// Current velocity estimate
    /// </summary>
    public Vector2 Velocity;

    /// <summary>
    /// Currently being dragged
    /// </summary>
    public bool IsDragging;

    /// <summary>
    /// Currently snapping to target
    /// </summary>
    public bool IsSnapping;

    /// <summary>
    /// Snap animation progress (0-1)
    /// </summary>
    public float SnapProgress;
}
