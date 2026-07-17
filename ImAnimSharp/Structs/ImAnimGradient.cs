using System;
using System.Numerics;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

/// <summary>
/// Color gradient with any number of stops (sorted by position)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimGradient() : IDisposable
{
    /// <summary>
    /// Positions along gradient [0,1], kept sorted
    /// </summary>
    public ImVector<float> Positions = new();

    /// <summary>
    /// Colors at each position (sRGB)
    /// </summary>
    public ImVector<Vector4> Colors = new();

    /// <summary>
    /// Get stop count
    /// </summary>
    public int StopCount => Positions.Size;

    public static ImAnimGradient Solid(Vector4 color)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, color);
        g.Add(1.0f, color);
        return g;
    }

    public static ImAnimGradient Solid(uint color)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, color);
        g.Add(1.0f, color);
        return g;
    }

    public static ImAnimGradient TwoColor(Vector4 start, Vector4 end)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, start);
        g.Add(1.0f, end);
        return g;
    }

    public static ImAnimGradient TwoColor(uint start, uint end)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, start);
        g.Add(1.0f, end);
        return g;
    }

    public static ImAnimGradient ThreeColor(Vector4 start, Vector4 mid, Vector4 end)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, start);
        g.Add(0.5f, mid);
        g.Add(1.0f, end);
        return g;
    }

    public static ImAnimGradient ThreeColor(uint start, uint mid, uint end)
    {
        var g = new ImAnimGradient();
        g.Add(0.0f, start);
        g.Add(0.5f, mid);
        g.Add(1.0f, end);
        return g;
    }

    public void Dispose()
    {
        Positions.Free();
        Colors.Free();
    }

    /// <summary>
    /// Add a stop to the gradient (automatically sorted by position)
    /// </summary>
    public ImAnimGradient Add(float position, Vector4 color)
    {
        fixed (ImAnimGradient* @this = &this)
            ImAnimNative.GradientAdd(@this, position, &color);
        return this;
    }

    /// <summary>
    /// Add a stop to the gradient (automatically sorted by position)
    /// </summary>
    public ImAnimGradient Add(float position, uint color)
    {
        var colorVec = ImGui.ColorConvertU32ToFloat4(color);
        fixed (ImAnimGradient* @this = &this)
            ImAnimNative.GradientAdd(@this, position, &colorVec);
        return this;
    }

    /// <summary>
    /// Sample the gradient at position t [0,1]
    /// </summary>
    public Vector4 Sample(float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        Vector4 ret = default;
        fixed (ImAnimGradient* @this = &this)
            ImAnimNative.GradientSample(&ret, @this, t, colorSpace);
        return ret;
    }
}
