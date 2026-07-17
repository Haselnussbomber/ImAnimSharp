using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// 2D transform (position, rotation, scale) (<see href="https://github.com/soufianekhiat/ImAnim/blob/0e28f285/docs/transforms.md">Docs</see>)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimTransform
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

    // TODO: functions apply, inverse, lerp
    // TODO: * operator
}
