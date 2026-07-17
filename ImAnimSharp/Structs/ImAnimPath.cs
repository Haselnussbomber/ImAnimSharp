using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimPath(uint pathId)
{
    public uint PathId = pathId;

    /// <summary>
    /// Get approximate path length.
    /// </summary>
    public readonly float Length => ImAnimNative.PathLength(PathId);

    /// <summary>
    /// Start building a path at position.
    /// </summary>
    public static ImAnimPath Begin(uint pathId, Vector2 start)
    {
        return ImAnim.PathBegin(pathId, start);
    }

    /// <summary>
    /// Finalize and register path.
    /// </summary>
    public void End()
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathEnd(@this);
    }

    /// <summary>
    /// Add linear segment to endpoint.
    /// </summary>
    public ImAnimPath LineTo(Vector2 end)
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathLineTo(@this, &end);
        return this;
    }

    /// <summary>
    /// Add quadratic bezier segment.
    /// </summary>
    public ImAnimPath QuadraticTo(Vector2 ctrl, Vector2 end)
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathQuadraticTo(@this, &ctrl, &end);
        return this;
    }

    /// <summary>
    /// Add cubic bezier segment.
    /// </summary>
    public ImAnimPath CubicTo(Vector2 ctrl1, Vector2 ctrl2, Vector2 end)
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathCubicTo(@this, &ctrl1, &ctrl2, &end);
        return this;
    }

    /// <summary>
    /// Add Catmull-Rom segment to endpoint.
    /// </summary>
    public ImAnimPath CatmullTo(Vector2 end, float tension = 0.5f)
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathCubicTo(@this, &end, tension);
        return this;
    }

    /// <summary>
    /// Close path back to start point.
    /// </summary>
    public ImAnimPath Close()
    {
        fixed (ImAnimPath* @this = &this)
            ImAnimNative.PathClose(@this);
        return this;
    }

    /// <summary>
    /// Sample path at t [0,1].
    /// </summary>
    public Vector2 Evaluate(float t)
    {
        return ImAnim.PathEvaluate(PathId, t);
    }

    /// <summary>
    /// Get tangent (normalized direction) at t.
    /// </summary>
    public Vector2 Tangent(float t)
    {
        return ImAnim.PathTangent(PathId, t);
    }

    /// <summary>
    /// Get rotation angle (radians) at t.
    /// </summary>
    public float Angle(float t)
    {
        return ImAnimNative.PathAngle(PathId, t);
    }
}
