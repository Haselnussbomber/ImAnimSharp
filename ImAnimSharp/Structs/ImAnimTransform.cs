using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// 2D transform (position, rotation, scale) (<see href="https://github.com/soufianekhiat/ImAnim/blob/0e28f285/docs/transforms.md">Docs</see>)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimTransform
{
    /// <summary>
    /// Translation
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Scale (1,1 = identity)
    /// </summary>
    public Vector2 Scale;

    /// <summary>
    /// Rotation in radians
    /// </summary>
    public float Rotation;

    public static ImAnimTransform Identity() => new()
    {
        Position = Vector2.Zero,
        Scale = Vector2.One,
        Rotation = 0,
    };

    /// <summary>
    /// Apply transform to a point
    /// </summary>
    public Vector2 Apply(Vector2 point)
    {
        Vector2 ret = default;
        fixed (ImAnimTransform* pThis = &this)
            ImAnimNative.TransformApply(&ret, pThis, &point);
        return ret;
    }

    /// <summary>
    /// Get inverse transform
    /// </summary>
    public ImAnimTransform Inverse()
    {
        ImAnimTransform ret = default;
        fixed (ImAnimTransform* pThis = &this)
            ImAnimNative.TransformInverse(&ret, pThis);
        return ret;
    }

    /// <summary>
    /// Blend between two transforms with rotation interpolation
    /// </summary>
    public ImAnimTransform Lerp(ImAnimTransform b, float t, ImAnimRotationMode rotationMode = ImAnimRotationMode.Shortest)
    {
        return ImAnim.TransformLerp(this, b, t, rotationMode);
    }

    /// <summary>
    /// Decompose a 3x2 matrix into transform components
    /// </summary>
    public static ImAnimTransform FromMatrix(Matrix3x2 matrix)
    {
        return ImAnim.TransformFromMatrix(matrix);
    }

    /// <inheritdoc cref="FromMatrix(Matrix3x2)"/>
    public ImAnimTransform FromMatrix(float m00, float m01, float m10, float m11, float tx, float ty)
    {
        return ImAnim.TransformFromMatrix(m00, m01, m10, m11, tx, ty);
    }

    /// <summary>
    /// Convert transform to 3x2 matrix (row-major: [m00 m01 tx; m10 m11 ty])
    /// </summary>
    public Matrix3x2 ToMatrix()
    {
        return ImAnim.TransformToMatrix(this);
    }

    /// <summary>
    /// Combine transforms (this * other)
    /// </summary>
    public static ImAnimTransform operator *(ImAnimTransform left, ImAnimTransform right)
    {
        ImAnimTransform ret = default;
        ImAnimNative.TransformMultiply(&ret, &left, &right);
        return ret;
    }
}
