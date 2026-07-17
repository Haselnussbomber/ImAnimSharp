using System.Numerics;
using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Playback control for a clip
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimInstance
{
    public uint InstId;

    // Playback control

    public void Pause()
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstancePause(@this);
    }

    public void Resume()
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceResume(@this);
    }

    public void Stop()
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceStop(@this);
    }

    /// <summary>
    /// Remove instance from system (valid() will return false after this)
    /// </summary>
    public void Destroy()
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceDestroy(@this);
    }

    public void Seek(float time)
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceSeek(@this, time);
    }

    public void SetTimeScale(float scale)
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceSetTimeScale(@this, scale);
    }

    /// <summary>
    /// For layering/blending
    /// </summary>
    public void SetWeight(float weight)
    {
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceSetWeight(@this, weight);
    }

    // Animation chaining - play another clip when this one completes

    /// <summary>
    /// Chain another clip to play after this one.
    /// </summary>
    public ImAnimInstance Then(uint nextClipId)
    {
        ImAnimInstance ret = default;
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceThen(&ret, @this, nextClipId);
        return ret;
    }

    /// <summary>
    /// Chain with specific instance ID.
    /// </summary>
    public ImAnimInstance Then(uint nextClipId, uint nextInstanceId)
    {
        ImAnimInstance ret = default;
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceThenId(&ret, @this, nextClipId, nextInstanceId);
        return ret;
    }

    /// <summary>
    /// Set delay before chained clip starts.
    /// </summary>
    public ImAnimInstance ThenDelay(float delay)
    {
        ImAnimInstance ret = default;
        fixed (ImAnimInstance* @this = &this)
            ImAnimNative.InstanceThenDelay(&ret, @this, delay);
        return ret;
    }

    // Query state

    public float Time()
    {
        fixed (ImAnimInstance* @this = &this)
            return ImAnimNative.InstanceTime(@this);
    }

    public float Duration()
    {
        fixed (ImAnimInstance* @this = &this)
            return ImAnimNative.InstanceDuration(@this);
    }

    public bool IsPlaying()
    {
        fixed (ImAnimInstance* @this = &this)
            return ImAnimNative.InstanceIsPlaying(@this) == 1;
    }

    public bool IsPaused()
    {
        fixed (ImAnimInstance* @this = &this)
            return ImAnimNative.InstanceIsPaused(@this) == 1;
    }

    // Get animated values

    public bool GetFloat(uint channel, out float value)
    {
        fixed (ImAnimInstance* @this = &this)
        fixed (float* outVal = &value)
            return ImAnimNative.InstanceGetFloat(@this, channel, outVal) == 1;
    }

    public bool GetVec2(uint channel, out Vector2 value)
    {
        fixed (ImAnimInstance* @this = &this)
        fixed (Vector2* outVal = &value)
            return ImAnimNative.InstanceGetVec2(@this, channel, outVal) == 1;
    }

    public bool GetVec4(uint channel, out Vector4 value)
    {
        fixed (ImAnimInstance* @this = &this)
        fixed (Vector4* outVal = &value)
            return ImAnimNative.InstanceGetVec4(@this, channel, outVal) == 1;
    }

    public bool GetInt(uint channel, out int value)
    {
        fixed (ImAnimInstance* @this = &this)
        fixed (int* outVal = &value)
            return ImAnimNative.InstanceGetInt(@this, channel, outVal) == 1;
    }

    /// <summary>
    /// Color blended in specified color space.
    /// </summary>
    public bool GetColor(uint channel, out Vector4 value, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        fixed (ImAnimInstance* @this = &this)
        fixed (Vector4* outVal = &value)
            return ImAnimNative.InstanceGetColor(@this, channel, outVal, colorSpace) == 1;
    }

    // Check validity

    public bool Valid()
    {
        fixed (ImAnimInstance* @this = &this)
            return ImAnimNative.InstanceValid(@this) == 1;
    }
}
