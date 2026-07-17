using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Per-axis easing descriptor (for vec2/vec4/color)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimEasePerAxis
{
    /// <summary>
    /// Easing for X component
    /// </summary>
    public ImAnimEaseDesc X;

    /// <summary>
    /// Easing for Y component
    /// </summary>
    public ImAnimEaseDesc Y;

    /// <summary>
    /// Easing for Z component (vec4/color only)
    /// </summary>
    public ImAnimEaseDesc Z;

    /// <summary>
    /// Easing for W/alpha component (vec4/color only)
    /// </summary>
    public ImAnimEaseDesc W;

    public static ImAnimEasePerAxis From(ImAnimEaseDesc all) => new()
    {
        X = all,
        Y = all,
        Z = all,
        W = all
    };

    public static ImAnimEasePerAxis From(ImAnimEaseDesc ex, ImAnimEaseDesc ey) => new()
    {
        X = ex,
        Y = ey,
        Z = new ImAnimEaseDesc { Type = ImAnimEaseType.Linear },
        W = new ImAnimEaseDesc { Type = ImAnimEaseType.Linear }
    };
}
