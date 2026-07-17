using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Text stagger options
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimTextStaggerOpts
{
    /// <summary>
    /// Base position for text
    /// </summary>
    public Vector2 Pos;

    /// <summary>
    /// ImAnimTextStaggerEffect value
    /// </summary>
    public ImAnimTextStaggerEffect Effect;

    /// <summary>
    /// Delay between each character (seconds)
    /// </summary>
    public float CharDelay;

    /// <summary>
    /// Duration of each character's animation (seconds)
    /// </summary>
    public float CharDuration;

    /// <summary>
    /// Intensity of effect (pixels for slide, degrees for rotate, scale factor)
    /// </summary>
    public float EffectIntensity;

    /// <summary>
    /// Easing for character animation
    /// </summary>
    public ImAnimEaseDesc Ease;

    /// <summary>
    /// Text color
    /// </summary>
    public uint Color;

    /// <summary>
    /// Font to use (nullptr = current)
    /// </summary>
    public ImFontPtr Font;

    /// <summary>
    /// Font scale multiplier
    /// </summary>
    public float FontScale;

    /// <summary>
    /// Extra spacing between characters
    /// </summary>
    public float LetterSpacing;

    public static ImAnimTextStaggerOpts Default() => new()
    {
        Effect = ImAnimTextStaggerEffect.Fade,
        CharDelay = 0.05f,
        CharDuration = 0.3f,
        EffectIntensity = 20.0f,
        Ease = new ImAnimEaseDesc { Type = ImAnimEaseType.OutCubic },
        Color = 0xFFFFFFFF,
        FontScale = 1.0f
    };
}
