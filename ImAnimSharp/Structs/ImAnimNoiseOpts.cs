using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Noise options
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimNoiseOpts()
{
    /// <summary>
    /// Noise algorithm.
    /// </summary>
    public ImAnimNoiseType Type = ImAnimNoiseType.Perlin;

    /// <summary>
    /// Number of octaves for fractal noise (1-8)
    /// </summary>
    public int Octaves = 1;

    /// <summary>
    /// Amplitude multiplier per octave (0.0-1.0)
    /// </summary>
    public float Persistence = 0.5f;

    /// <summary>
    /// Frequency multiplier per octave (typically 2.0)
    /// </summary>
    public float Lacunarity = 2.0f;

    /// <summary>
    /// Random seed for noise generation
    /// </summary>
    public int Seed = 0;
}
