using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Text path options (<see href="https://github.com/soufianekhiat/ImAnim/blob/0e28f285/docs/text-along-paths.md">Docs</see>)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImAnimTextPathOpts()
{
    /// <summary>
    /// Screen-space origin for rendering (path coords are offset by this)
    /// </summary>
    public Vector2 Origin;

    /// <summary>
    /// Starting offset along path (pixels)
    /// </summary>
    public float Offset;

    /// <summary>
    /// Extra spacing between characters (pixels)
    /// </summary>
    public float LetterSpacing;

    /// <summary>
    /// ImAnimTextPathAlign value
    /// </summary>
    public ImAnimTextPathAlign Align;

    /// <summary>
    /// Flip text vertically (for paths going right-to-left)
    /// </summary>
    public bool FlipY;

    /// <summary>
    /// Text color (default: white)
    /// </summary>
    public uint Color = 0xFFFFFFFF;

    /// <summary>
    /// Font to use (nullptr = current font)
    /// </summary>
    public ImFontPtr Font;

    /// <summary>
    /// Additional font scale (1.0 = normal)
    /// </summary>
    public float FontScale = 1.0f;
}
