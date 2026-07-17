using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Morph options for path interpolation
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimMorphOpts()
{
    /// <summary>
    /// Number of sample points for resampling (default: 64)
    /// </summary>
    public int Samples = 64;

    /// <summary>
    /// Force endpoints to match exactly (default: true)
    /// </summary>
    public bool MatchEndpoints = true;

    /// <summary>
    /// Use arc-length parameterization for smoother morphing (default: true)
    /// </summary>
    public bool UseArcLength = true;
}
