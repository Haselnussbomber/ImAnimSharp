namespace ImAnimSharp;

public enum ImAnimNoiseType
{
    /// <summary>
    /// Classic Perlin noise, smooth gradients
    /// </summary>
    Perlin,

    /// <summary>
    /// Faster, fewer artifacts than Perlin
    /// </summary>
    Simplex,

    /// <summary>
    /// Simple value noise, blocky
    /// </summary>
    Value,

    /// <summary>
    /// Worley/cellular noise
    /// </summary>
    Worley,
}
