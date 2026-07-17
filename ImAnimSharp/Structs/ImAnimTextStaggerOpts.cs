using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Text stagger options (<see href="https://github.com/soufianekhiat/ImAnim/blob/0e28f285/docs/text-stagger.md">Docs</see>)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimTextStaggerOpts()
{
    /// <summary>
    /// Base position for text
    /// </summary>
    public Vector2 Pos;

    /// <summary>
    /// ImAnimTextStaggerEffect value
    /// </summary>
    public ImAnimTextStaggerEffect Effect = ImAnimTextStaggerEffect.Fade;

    /// <summary>
    /// Delay between each character (seconds)
    /// </summary>
    public float CharDelay = 0.05f;

    /// <summary>
    /// Duration of each character's animation (seconds)
    /// </summary>
    public float CharDuration = 0.3f;

    /// <summary>
    /// Intensity of effect (pixels for slide, degrees for rotate, scale factor)
    /// </summary>
    public float EffectIntensity = 20.0f;

    /// <summary>
    /// Easing for character animation
    /// </summary>
    public ImAnimEaseDesc Ease = new() { Type = ImAnimEaseType.OutCubic };

    /// <summary>
    /// Text color
    /// </summary>
    public uint Color = 0xFFFFFFFF;

    /// <summary>
    /// Font to use (nullptr = current)
    /// </summary>
    public ImFontPtr Font;

    /// <summary>
    /// Font scale multiplier
    /// </summary>
    public float FontScale = 1.0f;

    /// <summary>
    /// Extra spacing between characters
    /// </summary>
    public float LetterSpacing;
}
