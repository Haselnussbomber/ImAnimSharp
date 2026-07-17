namespace ImAnimSharp;

/// <summary>
/// Color Space
/// </summary>
public enum ImAnimColorSpace
{
    /// <summary>
    /// Direct sRGB blend (not physically linear). Best for simple blends.
    /// </summary>
    Srgb,

    /// <summary>
    /// Linear RGB blend. Best for physical accuracy.
    /// </summary>
    SrgbLinear,

    /// <summary>
    /// HSV blend (shortest hue arc). Best for rainbow effects.
    /// </summary>
    Hsv,

    /// <summary>
    /// OKLAB perceptual blend. Best for UI transitions.
    /// </summary>
    Oklab,

    /// <summary>
    /// OKLCH cylindrical blend. Best for vibrant gradients.
    /// </summary>
    Oklch,
}
