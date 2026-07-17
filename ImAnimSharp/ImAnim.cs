using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Dalamud.Bindings.ImGui;

namespace ImAnimSharp;

// TODO: Helper functions for variations https://github.com/soufianekhiat/ImAnim/blob/0e28f285/docs/variations.md

public static unsafe class ImAnim
{
    public delegate void ClipCallback(uint instId, void* userData); // iam_clip_callback
    public delegate void MarkerCallback(uint instId, uint markerId, float markerTime, void* userData); // iam_marker_callback
    public delegate float EaseFn(float t); // iam_ease_fn

    public delegate float FloatResolver(void* userData); // iam_float_resolver
    public delegate Vector2 Vec2Resolver(void* userData); // iam_vec2_resolver
    public delegate Vector4 Vec4Resolver(void* userData); // iam_vec4_resolver
    public delegate int IntResolver(void* userData); // iam_int_resolver

    public delegate float VariationFloatFn(int index, void* userData); // iam_variation_float_fn
    public delegate int VariationIntFn(int index, void* userData); // iam_variation_int_fn
    public delegate Vector2 VariationVec2Fn(int index, void* userData); // iam_variation_vec2_fn
    public delegate Vector4 VariationVec4Fn(int index, void* userData); // iam_variation_vec4_fn

    // ----------------------------------------------------
    // Public API declarations
    // ----------------------------------------------------

    public static void SetImGuiContext(ImGuiContext* context)
    {
        ImAnimNative.SetImGuiContext(context);
    }

    public static void SetImGuiAllocatorFunctions(ImGuiMemAllocFunc alloc_func, ImGuiMemFreeFunc free_func, void* user_data = null)
    {
        ImAnimNative.SetImGuiAllocatorFunctions(alloc_func, free_func, user_data);
    }

    [OverloadResolutionPriority(1)]
    public static void SetImGuiAllocatorFunctions(delegate*<nuint, void*, void*> alloc_func, delegate*<void*, void*, void> free_func, void* user_data = null)
    {
        ImAnimNative.SetImGuiAllocatorFunctions(alloc_func, free_func, user_data);
    }

    public static void DemoWindow()
    {
        ImAnimNative.DemoWindow();
    }

    // Frame management

    /// <summary>
    /// Call once per frame before any tweens.
    /// </summary>
    public static void UpdateBeginFrame()
    {
        ImAnimNative.UpdateBeginFrame();
    }

    /// <summary>
    /// Remove stale tween entries older than <c>maxAgeFrames</c>.
    /// </summary>
    public static void Gc(uint maxAgeFrames = 600)
    {
        ImAnimNative.Gc(maxAgeFrames);
    }

    /// <summary>
    /// Clear all pools immediately (useful for scene transitions, level resets).
    /// </summary>
    public static void PoolClear()
    {
        ImAnimNative.PoolClear();
    }

    /// <summary>
    /// Pre-allocate pool capacity.
    /// </summary>
    public static void Reserve(int capFloat, int capVec2, int capVec4, int capInt, int capColor)
    {
        ImAnimNative.Reserve(capFloat, capVec2, capVec4, capInt, capColor);
    }

    /// <summary>
    /// Set LUT resolution for parametric easings (default: 256).
    /// </summary>
    public static void SetEaseLutSamples(int count)
    {
        ImAnimNative.SetEaseLutSamples(count);
    }

    // Global time scale (for slow-motion / fast-forward debugging)

    /// <summary>
    /// Set global time multiplier (1.0 = normal, 0.5 = half speed, 2.0 = double).
    /// </summary>
    public static void SetGlobalTimeScale(float scale)
    {
        ImAnimNative.SetGlobalTimeScale(scale);
    }

    /// <summary>
    /// Get current global time scale.
    /// </summary>
    public static float GetGlobalTimeScale()
    {
        return ImAnimNative.GetGlobalTimeScale();
    }

    // Lazy Initialization - defer channel creation until animation is needed

    /// <summary>
    /// Enable/disable lazy initialization (default: <see langword="true"/>).
    /// </summary>
    public static void SetLazyInit(bool enable)
    {
        ImAnimNative.SetLazyInit((byte)(enable ? 1 : 0));
    }

    /// <summary>
    /// Check if lazy init is enabled.
    /// </summary>
    public static byte IsLazyInitEnabled()
    {
        return ImAnimNative.IsLazyInitEnabled();
    }

    /// <summary>
    /// Register custom easing in slot 0-15. Use with <see cref="EaseCustomFn(int)">.
    /// </summary>
    public static void RegisterCustomEase(int slot, EaseFn fn)
    {
        ImAnimNative.RegisterCustomEase(slot, (delegate* unmanaged[Cdecl]<float, float>)Marshal.GetFunctionPointerForDelegate(fn));
    }

    /// <summary>
    /// Register custom easing in slot 0-15. Use with <see cref="EaseCustomFn(int)">.
    /// </summary>
    public static void RegisterCustomEaseUnmanaged(int slot, delegate* unmanaged[Cdecl]<float, float> fn)
    {
        ImAnimNative.RegisterCustomEase(slot, fn);
    }

    /// <summary>
    /// Get registered custom easing function.
    /// </summary>
    public static EaseFn? GetCustomEase(int slot)
    {
        delegate* unmanaged[Cdecl]<float, float> retFn = null;
        ImAnimNative.GetCustomEase(&retFn, slot);
        return retFn == null ? null : Marshal.GetDelegateForFunctionPointer<EaseFn>((nint)retFn);
    }

    /// <summary>
    /// Get registered custom easing function.
    /// </summary>
    public static delegate* unmanaged[Cdecl]<float, float> GetCustomEaseUnmanaged(int slot)
    {
        delegate* unmanaged[Cdecl]<float, float> retFn = null;
        ImAnimNative.GetCustomEase(&retFn, slot);
        return retFn;
    }

    // Debug UI

    /// <summary>
    /// Show unified inspector (merges debug window + animation inspector).
    /// </summary>
    public static void ShowUnifiedInspector()
    {
        ImAnimNative.ShowUnifiedInspector();
    }

    /// <inheritdoc cref="ShowUnifiedInspector()"/>
    public static void ShowUnifiedInspector(ref bool pOpen)
    {
        var open = (byte)(pOpen ? 1 : 0);
        ImAnimNative.ShowUnifiedInspector(&open);
        pOpen = open == 1;
    }

    /// <summary>
    /// Show debug timeline for a clip instance.
    /// </summary>
    public static void ShowDebugTimeline(uint instanceId)
    {
        ImAnimNative.ShowDebugTimeline(instanceId);
    }

    /// <inheritdoc cref="ShowDebugTimeline(uint)"/>
    public static void ShowDebugTimeline(ImU8String instanceId)
    {
        ShowDebugTimeline(instanceId.GetId());
    }

    // Performance Profiler

    /// <summary>
    /// Enable/disable the performance profiler.
    /// </summary>
    public static void ProfilerEnable(bool enable)
    {
        ImAnimNative.ProfilerEnable((byte)(enable ? 1 : 0));
    }

    /// <summary>
    /// Check if profiler is enabled.
    /// </summary>
    public static byte ProfilerIsEnabled()
    {
        return ImAnimNative.ProfilerIsEnabled();
    }

    /// <summary>
    /// Call at frame start when profiler is enabled.
    /// </summary>
    public static void ProfilerBeginFrame()
    {
        ImAnimNative.ProfilerBeginFrame();
    }

    /// <summary>
    /// Call at frame end when profiler is enabled.
    /// </summary>
    public static void ProfilerEndFrame()
    {
        ImAnimNative.ProfilerEndFrame();
    }

    /// <summary>
    /// Begin a named profiler section.
    /// </summary>
    public static void ProfilerBegin(ImU8String name)
    {
        fixed (byte* namePtr = &name.GetPinnableNullTerminatedReference())
            ImAnimNative.ProfilerBegin(namePtr);
        name.Recycle();
    }

    /// <summary>
    /// End the current profiler section.
    /// </summary>
    public static void ProfilerEnd()
    {
        ImAnimNative.ProfilerEnd();
    }

    // Drag Feedback - animated feedback for drag operations

    /// <summary>
    /// Start tracking drag at position.
    /// </summary>
    public static ImAnimDragFeedback DragBegin(uint id, Vector2 pos)
    {
        ImAnimDragFeedback ret = default;
        ImAnimNative.DragBegin(&ret, id, &pos);
        return ret;
    }

    /// <inheritdoc cref="DragBegin(uint, Vector2)"/>
    public static ImAnimDragFeedback DragBegin(ImU8String id, Vector2 pos)
    {
        return DragBegin(id.GetId(), pos);
    }

    /// <summary>
    /// Update drag position during drag.
    /// </summary>
    public static ImAnimDragFeedback DragUpdate(uint id, Vector2 pos, float dt)
    {
        ImAnimDragFeedback ret = default;
        ImAnimNative.DragUpdate(&ret, id, &pos, dt);
        return ret;
    }

    /// <inheritdoc cref="DragUpdate(uint, Vector2, float)"/>
    public static ImAnimDragFeedback DragUpdate(ImU8String id, Vector2 pos, float dt)
    {
        return DragUpdate(id.GetId(), pos, dt);
    }

    /// <summary>
    /// Release drag with animated feedback.
    /// </summary>
    public static ImAnimDragFeedback DragRelease(uint id, Vector2 pos, ImAnimDragOpts opts, float dt)
    {
        ImAnimDragFeedback ret = default;
        ImAnimNative.DragRelease(&ret, id, &pos, &opts, dt);
        return ret;
    }

    /// <inheritdoc cref="DragRelease(uint, Vector2, ImAnimDragOpts, float)"/>
    public static ImAnimDragFeedback DragRelease(ImU8String id, Vector2 pos, ImAnimDragOpts opts, float dt)
    {
        return DragRelease(id.GetId(), pos, opts, dt);
    }

    /// <summary>
    /// Cancel drag tracking.
    /// </summary>
    public static void DragCancel(uint id)
    {
        ImAnimNative.DragCancel(id);
    }

    /// <inheritdoc cref="DragCancel(uint)"/>
    public static void DragCancel(ImU8String id)
    {
        DragCancel(id.GetId());
    }

    // Oscillators - continuous periodic animations

    /// <summary>
    /// Returns oscillating value [-amplitude, +amplitude].
    /// </summary>
    public static float Oscillate(uint id, float amplitude, float frequency, ImAnimWaveType waveType, float phase, float dt)
    {
        return ImAnimNative.Oscillate(id, amplitude, frequency, waveType, phase, dt);
    }

    /// <inheritdoc cref="Oscillate(uint, float, float, ImAnimWaveType, float, float)"/>
    public static float Oscillate(ImU8String id, float amplitude, float frequency, ImAnimWaveType waveType, float phase, float dt)
    {
        return Oscillate(id.GetId(), amplitude, frequency, waveType, phase, dt);
    }

    /// <summary>
    /// Returns oscillating integer value [-amplitude, +amplitude].
    /// </summary>
    public static int OscillateInt(uint id, int amplitude, float frequency, ImAnimWaveType waveType, float phase, float dt)
    {
        return ImAnimNative.OscillateInt(id, amplitude, frequency, waveType, phase, dt);
    }

    /// <inheritdoc cref="OscillateInt(uint, int, float, ImAnimWaveType, float, float)"/>
    public static int OscillateInt(ImU8String id, int amplitude, float frequency, ImAnimWaveType waveType, float phase, float dt)
    {
        return OscillateInt(id.GetId(), amplitude, frequency, waveType, phase, dt);
    }

    /// <summary>
    /// 2D oscillation.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitude per axis</param>
    /// <param name="frequency">Frequency per axis</param>
    /// <param name="waveType"></param>
    /// <param name="phase">Phase per axis</param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector2 OscillateVec2(uint id, Vector2 amplitude, Vector2 frequency, ImAnimWaveType waveType, Vector2 phase, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.OscillateVec2(&ret, id, &amplitude, &frequency, waveType, &phase, dt);
        return ret;
    }

    /// <inheritdoc cref="OscillateVec2(uint, Vector2, Vector2, ImAnimWaveType, Vector2, float)"/>
    public static Vector2 OscillateVec2(ImU8String id, Vector2 amplitude, Vector2 frequency, ImAnimWaveType waveType, Vector2 phase, float dt)
    {
        return OscillateVec2(id.GetId(), amplitude, frequency, waveType, phase, dt);
    }

    /// <summary>
    /// 4D oscillation.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitudes</param>
    /// <param name="frequency">Frequencies</param>
    /// <param name="waveType"></param>
    /// <param name="phase">Phases</param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector4 OscillateVec4(uint id, Vector4 amplitude, Vector4 frequency, ImAnimWaveType waveType, Vector4 phase, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.OscillateVec4(&ret, id, &amplitude, &frequency, waveType, &phase, dt);
        return ret;
    }

    /// <inheritdoc cref="OscillateVec4(uint, Vector4, Vector4, ImAnimWaveType, Vector4, float)"/>
    public static Vector4 OscillateVec4(ImU8String id, Vector4 amplitude, Vector4 frequency, ImAnimWaveType waveType, Vector4 phase, float dt)
    {
        return OscillateVec4(id.GetId(), amplitude, frequency, waveType, phase, dt);
    }

    /// <summary>
    /// Color oscillation in specified color space.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="baseColor">Base color (sRGB)</param>
    /// <param name="amplitude">Amplitude (affects V in HSV)</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="waveType"></param>
    /// <param name="phase">Phase</param>
    /// <param name="colorSpace">Color space</param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static Vector4 OscillateColor(uint id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimWaveType waveType, float phase, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.OscillateColor(&ret, id, &baseColor, &amplitude, frequency, waveType, phase, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="OscillateColor(uint, Vector4, Vector4, float, ImAnimWaveType, float, ImAnimColorSpace, float)"/>
    public static Vector4 OscillateColor(ImU8String id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimWaveType waveType, float phase, ImAnimColorSpace colorSpace, float dt)
    {
        return OscillateColor(id.GetId(), baseColor, amplitude, frequency, waveType, phase, colorSpace, dt);
    }

    // Shake/Wiggle - procedural noise animations

    /// <summary>
    /// Decaying random shake.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="intensity">Intensity (max displacement)</param>
    /// <param name="frequency">Frequency (oscillations per second)</param>
    /// <param name="decayTime">Decay time (seconds)</param>
    /// <param name="dt"></param>
    /// <returns>Offset that decays to 0.</returns>
    public static float Shake(uint id, float intensity, float frequency, float decayTime, float dt)
    {
        return ImAnimNative.Shake(id, intensity, frequency, decayTime, dt);
    }

    /// <inheritdoc cref="Shake(uint, float, float, float, float)"/>
    public static float Shake(ImU8String id, float intensity, float frequency, float decayTime, float dt)
    {
        return Shake(id.GetId(), intensity, frequency, decayTime, dt);
    }

    /// <summary>
    /// Decaying random shake for integers.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="intensity">Intensity (max displacement)</param>
    /// <param name="frequency">Frequency (oscillations per second)</param>
    /// <param name="decayTime">Decay time (seconds)</param>
    /// <param name="dt"></param>
    /// <returns>Offset that decays to 0.</returns>
    public static int ShakeInt(uint id, int intensity, float frequency, float decayTime, float dt)
    {
        return ImAnimNative.ShakeInt(id, intensity, frequency, decayTime, dt);
    }

    /// <inheritdoc cref="ShakeInt(uint, int, float, float, float)"/>
    public static int ShakeInt(ImU8String id, int intensity, float frequency, float decayTime, float dt)
    {
        return ShakeInt(id.GetId(), intensity, frequency, decayTime, dt);
    }

    /// <summary>
    /// 2D decaying shake.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="intensity">Different intensity per axis</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="decayTime">Decay time</param>
    /// <param name="dt"></param>
    /// <returns>Offset that decays to 0.</returns>
    public static Vector2 ShakeVec2(uint id, Vector2 intensity, float frequency, float decayTime, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.ShakeVec2(&ret, id, &intensity, frequency, decayTime, dt);
        return ret;
    }

    /// <inheritdoc cref="ShakeVec2(uint, Vector2, float, float, float)"/>
    public static Vector2 ShakeVec2(ImU8String id, Vector2 intensity, float frequency, float decayTime, float dt)
    {
        return ShakeVec2(id.GetId(), intensity, frequency, decayTime, dt);
    }

    /// <summary>
    /// 4D decaying shake.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="intensity">Intensity per component</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="decayTime">Decay time</param>
    /// <param name="dt"></param>
    /// <returns>Offset that decays to 0.</returns>
    public static Vector4 ShakeVec4(uint id, Vector4 intensity, float frequency, float decayTime, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.ShakeVec4(&ret, id, &intensity, frequency, decayTime, dt);
        return ret;
    }

    /// <inheritdoc cref="ShakeVec4(uint, Vector4, float, float, float)"/>
    public static Vector4 ShakeVec4(ImU8String id, Vector4 intensity, float frequency, float decayTime, float dt)
    {
        return ShakeVec4(id.GetId(), intensity, frequency, decayTime, dt);
    }

    /// <summary>
    /// Color shake in specified color space.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="baseColor">Base color</param>
    /// <param name="intensity">Intensity</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="decayTime">Decay time</param>
    /// <param name="colorSpace">Color space</param>
    /// <param name="dt"></param>
    /// <returns>Offset that decays to 0.</returns>
    public static Vector4 ShakeColor(uint id, Vector4 baseColor, Vector4 intensity, float frequency, float decayTime, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.ShakeColor(&ret, id, &baseColor, &intensity, frequency, decayTime, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="ShakeColor(uint, Vector4, Vector4, float, float, ImAnimColorSpace, float)"/>
    public static Vector4 ShakeColor(ImU8String id, Vector4 baseColor, Vector4 intensity, float frequency, float decayTime, ImAnimColorSpace colorSpace, float dt)
    {
        return ShakeColor(id.GetId(), baseColor, intensity, frequency, decayTime, colorSpace, dt);
    }

    /// <summary>
    /// Continuous smooth random movement.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitude</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="dt"></param>
    public static float Wiggle(uint id, float amplitude, float frequency, float dt)
    {
        return ImAnimNative.Wiggle(id, amplitude, frequency, dt);
    }

    /// <inheritdoc cref="Wiggle(uint, float, float, float)"/>
    public static float Wiggle(ImU8String id, float amplitude, float frequency, float dt)
    {
        return Wiggle(id.GetId(), amplitude, frequency, dt);
    }

    /// <summary>
    /// Continuous smooth random movement for integers.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitude</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="dt"></param>
    public static int WiggleInt(uint id, int amplitude, float frequency, float dt)
    {
        return ImAnimNative.WiggleInt(id, amplitude, frequency, dt);
    }

    /// <inheritdoc cref="WiggleInt(uint, int, float, float)"/>
    public static int WiggleInt(ImU8String id, int amplitude, float frequency, float dt)
    {
        return WiggleInt(id.GetId(), amplitude, frequency, dt);
    }

    /// <summary>
    /// 2D continuous wiggle.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitude</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="dt"></param>
    public static Vector2 WiggleVec2(uint id, Vector2 amplitude, float frequency, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.WiggleVec2(&ret, id, &amplitude, frequency, dt);
        return ret;
    }

    /// <inheritdoc cref="WiggleVec2(uint, Vector2, float, float)"/>
    public static Vector2 WiggleVec2(ImU8String id, Vector2 amplitude, float frequency, float dt)
    {
        return WiggleVec2(id.GetId(), amplitude, frequency, dt);
    }

    /// <summary>
    /// 4D continuous wiggle.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amplitude">Amplitude</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="dt"></param>
    public static Vector4 WiggleVec4(uint id, Vector4 amplitude, float frequency, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.WiggleVec4(&ret, id, &amplitude, frequency, dt);
        return ret;
    }

    /// <inheritdoc cref="WiggleVec4(uint, Vector4, float, float)"/>
    public static Vector4 WiggleVec4(ImU8String id, Vector4 amplitude, float frequency, float dt)
    {
        return WiggleVec4(id.GetId(), amplitude, frequency, dt);
    }

    /// <summary>
    /// Color wiggle in specified color space.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="baseColor">Base color</param>
    /// <param name="amplitude">Amplitude</param>
    /// <param name="frequency">Frequency</param>
    /// <param name="colorSpace">Color space</param>
    /// <param name="dt"></param>
    public static Vector4 WiggleColor(uint id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.WiggleColor(&ret, id, &baseColor, &amplitude, frequency, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="WiggleColor(uint, Vector4, Vector4, float, ImAnimColorSpace, float)"/>
    public static Vector4 WiggleColor(ImU8String id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimColorSpace colorSpace, float dt)
    {
        return WiggleColor(id.GetId(), baseColor, amplitude, frequency, colorSpace, dt);
    }

    /// <summary>
    /// Trigger/restart a shake animation.
    /// </summary>
    public static void TriggerShake(uint id)
    {
        ImAnimNative.TriggerShake(id);
    }

    /// <inheritdoc cref="TriggerShake(uint)"/>
    public static void TriggerShake(ImU8String id)
    {
        TriggerShake(id.GetId());
    }

    // Easing evaluation

    /// <summary>
    /// Evaluate a preset easing function at time t (0-1).
    /// </summary>
    public static float EvalPreset(ImAnimEaseType type, float t)
    {
        return ImAnimNative.EvalPreset(type, t);
    }

    // Tween API - smoothly interpolate values over time

    /// <summary>
    /// Animate a float value.
    /// </summary>
    /// <param name="id">Owner ID - typically <c>ImGui::GetID("widget")</c></param>
    /// <param name="channelId">Property ID - e.g., <c>ImHashStr("alpha")</c></param>
    /// <param name="target">Target value to animate towards</param>
    /// <param name="dur">Animation duration in seconds</param>
    /// <param name="ez">Easing function</param>
    /// <param name="policy">How to handle target changes mid-animation</param>
    /// <param name="dt">Delta time, usually <c>ImGui::GetIO().DeltaTime</c></param>
    /// <param name="initValue">Initial value when channel is first created (optional)</param>
    public static float TweenFloat(uint id, uint channelId, float target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, float initValue = 0f)
    {
        return ImAnimNative.TweenFloat(id, channelId, target, dur, &ez, policy, dt, initValue);
    }

    /// <inheritdoc cref="TweenFloat(uint, uint, float, float, ImAnimEaseDesc, ImAnimPolicy, float, float)"/>
    public static float TweenFloat(ImU8String id, ImU8String channelId, float target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, float initValue = 0f)
    {
        return TweenFloat(id.GetId(), channelId.GetId(), target, dur, ez, policy, dt, initValue);
    }

    /// <summary>
    /// Animate a 2D vector.
    /// </summary>
    /// <param name="id">Owner ID - typically <c>ImGui::GetID("widget")</c></param>
    /// <param name="channelId">Property ID - e.g., <c>ImHashStr("alpha")</c></param>
    /// <param name="target">Target value to animate towards</param>
    /// <param name="dur">Animation duration in seconds</param>
    /// <param name="ez">Easing function</param>
    /// <param name="policy">How to handle target changes mid-animation</param>
    /// <param name="dt">Delta time, usually <c>ImGui::GetIO().DeltaTime</c></param>
    /// <param name="initValue">Initial value when channel is first created (optional)</param>
    public static Vector2 TweenVec2(uint id, uint channelId, Vector2 target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, Vector2? initValue = null)
    {
        Vector2 ret = default;
        Vector2 init_value = initValue ?? Vector2.Zero;
        ImAnimNative.TweenVec2(&ret, id, channelId, &target, dur, &ez, policy, dt, &init_value);
        return ret;
    }

    /// <inheritdoc cref="TweenVec2(uint, uint, Vector2, float, ImAnimEaseDesc, ImAnimPolicy, float, Vector2?)"/>
    public static Vector2 TweenVec2(ImU8String id, ImU8String channelId, Vector2 target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, Vector2? initValue = null)
    {
        return TweenVec2(id.GetId(), channelId.GetId(), target, dur, ez, policy, dt, initValue);
    }

    /// <summary>
    /// Animate a 4D vector.
    /// </summary>
    /// <param name="id">Owner ID - typically <c>ImGui::GetID("widget")</c></param>
    /// <param name="channelId">Property ID - e.g., <c>ImHashStr("alpha")</c></param>
    /// <param name="target">Target value to animate towards</param>
    /// <param name="dur">Animation duration in seconds</param>
    /// <param name="ez">Easing function</param>
    /// <param name="policy">How to handle target changes mid-animation</param>
    /// <param name="dt">Delta time, usually <c>ImGui::GetIO().DeltaTime</c></param>
    /// <param name="initValue">Initial value when channel is first created (optional)</param>
    public static Vector4 TweenVec4(uint id, uint channelId, Vector4 target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, Vector4? initValue = null)
    {
        Vector4 ret = default;
        Vector4 init_value = initValue ?? Vector4.Zero;
        ImAnimNative.TweenVec4(&ret, id, channelId, &target, dur, &ez, policy, dt, &init_value);
        return ret;
    }

    /// <inheritdoc cref="TweenVec4(uint, uint, Vector4, float, ImAnimEaseDesc, ImAnimPolicy, float, Vector4?)"/>
    public static Vector4 TweenVec4(ImU8String id, ImU8String channelId, Vector4 target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, Vector4? initValue = null)
    {
        return TweenVec4(id.GetId(), channelId.GetId(), target, dur, ez, policy, dt, initValue);
    }

    /// <summary>
    /// Animate an integer value.
    /// </summary>
    /// <param name="id">Owner ID - typically <c>ImGui::GetID("widget")</c></param>
    /// <param name="channelId">Property ID - e.g., <c>ImHashStr("alpha")</c></param>
    /// <param name="target">Target value to animate towards</param>
    /// <param name="dur">Animation duration in seconds</param>
    /// <param name="ez">Easing function</param>
    /// <param name="policy">How to handle target changes mid-animation</param>
    /// <param name="dt">Delta time, usually <c>ImGui::GetIO().DeltaTime</c></param>
    /// <param name="initValue">Initial value when channel is first created (optional)</param>
    public static int TweenInt(uint id, uint channelId, int target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, int initValue = 0)
    {
        return ImAnimNative.TweenInt(id, channelId, target, dur, &ez, policy, dt, initValue);
    }

    /// <inheritdoc cref="TweenInt(uint, uint, int, float, ImAnimEaseDesc, ImAnimPolicy, float, int)"/>
    public static int TweenInt(ImU8String id, ImU8String channelId, int target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt, int initValue = 0)
    {
        return TweenInt(id.GetId(), channelId.GetId(), target, dur, ez, policy, dt, initValue);
    }

    /// <summary>
    /// Animate a color in specified color space.
    /// </summary>
    /// <param name="id">Owner ID - typically <c>ImGui::GetID("widget")</c></param>
    /// <param name="channelId">Property ID - e.g., <c>ImHashStr("alpha")</c></param>
    /// <param name="target">Target value to animate towards</param>
    /// <param name="dur">Animation duration in seconds</param>
    /// <param name="ez">Easing function</param>
    /// <param name="policy">How to handle target changes mid-animation</param>
    /// <param name="dt">Delta time, usually <c>ImGui::GetIO().DeltaTime</c></param>
    /// <param name="initValue">Initial value when channel is first created (optional)</param>
    public static Vector4 TweenColor(uint id, uint channelId, Vector4 targetSrgb, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt, Vector4? initValue = null)
    {
        Vector4 ret = default;
        Vector4 init_value = initValue ?? Vector4.Zero;
        ImAnimNative.TweenColor(&ret, id, channelId, &targetSrgb, dur, &ez, policy, colorSpace, dt, &init_value);
        return ret;
    }

    /// <inheritdoc cref="TweenColor(uint, uint, Vector4, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimColorSpace, float, Vector4?)"/>
    public static Vector4 TweenColor(ImU8String id, ImU8String channelId, Vector4 targetSrgb, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt, Vector4? initValue = null)
    {
        return TweenColor(id.GetId(), channelId.GetId(), targetSrgb, dur, ez, policy, colorSpace, dt, initValue);
    }

    // Resize-friendly helpers

    /// <summary>
    /// Get dimensions of anchor space (window, viewport, etc.).
    /// </summary>
    public static Vector2 GetAnchorSize(ImAnimAnchorSpace space)
    {
        Vector2 ret = default;
        ImAnimNative.GetAnchorSize(&ret, space);
        return ret;
    }

    // Relative target tweens (percent of anchor + pixel offset) - survive window resizes

    /// <summary>
    /// Float relative to anchor (axis: 0=x, 1=y).
    /// </summary>
    public static float TweenFloatRel(uint id, uint channelId, float percent, float pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, int axis, float dt)
    {
        return ImAnimNative.TweenFloatRel(id, channelId, percent, pxBias, dur, &ez, policy, anchorSpace, axis, dt);
    }

    /// <inheritdoc cref="TweenFloatRel(uint, uint, float, float, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimAnchorSpace, int, float)"/>
    public static float TweenFloatRel(ImU8String id, ImU8String channelId, float percent, float pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, int axis, float dt)
    {
        return TweenFloatRel(id.GetId(), channelId.GetId(), percent, pxBias, dur, ez, policy, anchorSpace, axis, dt);
    }

    /// <summary>
    /// Vec2 relative to anchor.
    /// </summary>
    public static Vector2 TweenVec2Rel(uint id, uint channelId, Vector2 percent, Vector2 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.TweenVec2Rel(&ret, id, channelId, &percent, &pxBias, dur, &ez, policy, anchorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec2Rel(uint, uint, Vector2, Vector2, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimAnchorSpace, float)"/>
    public static Vector2 TweenVec2Rel(ImU8String id, ImU8String channelId, Vector2 percent, Vector2 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, float dt)
    {
        return TweenVec2Rel(id.GetId(), channelId.GetId(), percent, pxBias, dur, ez, policy, anchorSpace, dt);
    }

    /// <summary>
    /// Vec4 with x,y relative to anchor.
    /// </summary>
    public static Vector4 TweenVec4Rel(uint id, uint channelId, Vector4 percent, Vector4 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenVec4Rel(&ret, id, channelId, &percent, &pxBias, dur, &ez, policy, anchorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec4Rel(uint, uint, Vector4, Vector4, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimAnchorSpace, float)"/>
    public static Vector4 TweenVec4Rel(ImU8String id, ImU8String channelId, Vector4 percent, Vector4 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimAnchorSpace anchorSpace, float dt)
    {
        return TweenVec4Rel(id.GetId(), channelId.GetId(), percent, pxBias, dur, ez, policy, anchorSpace, dt);
    }

    /// <summary>
    /// Color with component offsets.
    /// </summary>
    public static Vector4 TweenColorRel(uint id, uint channelId, Vector4 percent, Vector4 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, ImAnimAnchorSpace anchorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenColorRel(&ret, id, channelId, &percent, &pxBias, dur, &ez, policy, colorSpace, anchorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenColorRel(uint, uint, Vector4, Vector4, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimColorSpace, ImAnimAnchorSpace, float)"/>
    public static Vector4 TweenColorRel(ImU8String id, ImU8String channelId, Vector4 percent, Vector4 pxBias, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, ImAnimAnchorSpace anchorSpace, float dt)
    {
        return TweenColorRel(id.GetId(), channelId.GetId(), percent, pxBias, dur, ez, policy, colorSpace, anchorSpace, dt);
    }

    // Rebase functions - change target of in-progress animation without restarting

    /// <summary>
    /// Smoothly redirect float animation to new target.
    /// </summary>
    public static void RebaseFloat(uint id, uint channelId, float newTarget, float dt)
    {
        ImAnimNative.RebaseFloat(id, channelId, newTarget, dt);
    }

    /// <inheritdoc cref="RebaseFloat(uint, uint, float, float)"/>
    public static void RebaseFloat(ImU8String id, ImU8String channelId, float newTarget, float dt)
    {
        RebaseFloat(id.GetId(), channelId.GetId(), newTarget, dt);
    }

    /// <summary>
    /// Smoothly redirect vec2 animation to new target.
    /// </summary>
    public static void RebaseVec2(uint id, uint channelId, Vector2 newTarget, float dt)
    {
        ImAnimNative.RebaseVec2(id, channelId, &newTarget, dt);
    }

    /// <inheritdoc cref="RebaseVec2(uint, uint, Vector2, float)"/>
    public static void RebaseVec2(ImU8String id, ImU8String channelId, Vector2 newTarget, float dt)
    {
        RebaseVec2(id.GetId(), channelId.GetId(), newTarget, dt);
    }

    /// <summary>
    /// Smoothly redirect vec4 animation to new target.
    /// </summary>
    public static void RebaseVec4(uint id, uint channelId, Vector4 newTarget, float dt)
    {
        ImAnimNative.RebaseVec4(id, channelId, &newTarget, dt);
    }

    /// <inheritdoc cref="RebaseVec4(uint, uint, Vector4, float)"/>
    public static void RebaseVec4(ImU8String id, ImU8String channelId, Vector4 newTarget, float dt)
    {
        RebaseVec4(id.GetId(), channelId.GetId(), newTarget, dt);
    }

    /// <summary>
    /// Smoothly redirect color animation to new target.
    /// </summary>
    public static void RebaseInt(uint id, uint channelId, int newTarget, float dt)
    {
        ImAnimNative.RebaseInt(id, channelId, newTarget, dt);
    }

    /// <inheritdoc cref="RebaseInt(uint, uint, int, float)"/>
    public static void RebaseInt(ImU8String id, ImU8String channelId, int newTarget, float dt)
    {
        RebaseInt(id.GetId(), channelId.GetId(), newTarget, dt);
    }

    /// <summary>
    /// Smoothly redirect int animation to new target.
    /// </summary>
    public static void RebaseColor(uint id, uint channelId, Vector4 newTarget, float dt)
    {
        ImAnimNative.RebaseColor(id, channelId, &newTarget, dt);
    }

    /// <inheritdoc cref="RebaseColor(uint, uint, Vector4, float)"/>
    public static void RebaseColor(ImU8String id, ImU8String channelId, Vector4 newTarget, float dt)
    {
        RebaseColor(id.GetId(), channelId.GetId(), newTarget, dt);
    }

    // Resolved tweens - target computed dynamically by callback each frame

    /// <summary>
    /// Float with dynamic target.
    /// </summary>
    public static float TweenFloatResolved(uint id, uint channelId, Func<float> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        float callback(void* userData) => fn();
        return ImAnimNative.TweenFloatResolved(id, channelId, (delegate* unmanaged[Cdecl]<void*, float>)Marshal.GetFunctionPointerForDelegate(callback), null, dur, &ez, policy, dt);
    }

    /// <inheritdoc cref="TweenFloatResolved(uint, uint, Func{float}, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static float TweenFloatResolved(ImU8String id, ImU8String channelId, Func<float> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenFloatResolved(id.GetId(), channelId.GetId(), fn, dur, ez, policy, dt);
    }

    /// <summary>
    /// Float with dynamic target.
    /// </summary>
    public static float TweenFloatResolved(uint id, uint channelId, FloatResolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return ImAnimNative.TweenFloatResolved(id, channelId, (delegate* unmanaged[Cdecl]<void*, float>)Marshal.GetFunctionPointerForDelegate(fn), userData, dur, &ez, policy, dt);
    }

    /// <inheritdoc cref="TweenFloatResolved(uint, uint, FloatResolver, void*, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static float TweenFloatResolved(ImU8String id, ImU8String channelId, FloatResolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenFloatResolved(id.GetId(), channelId.GetId(), fn, userData, dur, ez, policy, dt);
    }

    /// <summary>
    /// Vec2 with dynamic target.
    /// </summary>
    public static Vector2 TweenVec2Resolved(uint id, uint channelId, Func<Vector2> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        Vector2 callback(void* userData) => fn();
        Vector2 ret = default;
        ImAnimNative.TweenVec2Resolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector2>)Marshal.GetFunctionPointerForDelegate(callback), null, dur, &ez, policy, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec2Resolved(uint, uint, Func{Vector2}, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector2 TweenVec2Resolved(ImU8String id, ImU8String channelId, Func<Vector2> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenVec2Resolved(id.GetId(), channelId.GetId(), fn, dur, ez, policy, dt);
    }

    /// <summary>
    /// Vec2 with dynamic target.
    /// </summary>
    public static Vector2 TweenVec2Resolved(uint id, uint channelId, Vec2Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.TweenVec2Resolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector2>)Marshal.GetFunctionPointerForDelegate(fn), userData, dur, &ez, policy, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec2Resolved(uint, uint, Vec2Resolver, void*, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector2 TweenVec2Resolved(ImU8String id, ImU8String channelId, Vec2Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenVec2Resolved(id.GetId(), channelId.GetId(), fn, userData, dur, ez, policy, dt);
    }

    /// <summary>
    /// Vec4 with dynamic target.
    /// </summary>
    public static Vector4 TweenVec4Resolved(uint id, uint channelId, Func<Vector4> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        Vector4 callback(void* userData) => fn();
        Vector4 ret = default;
        ImAnimNative.TweenVec4Resolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector4>)Marshal.GetFunctionPointerForDelegate(callback), null, dur, &ez, policy, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec4Resolved(uint, uint, Func{Vector4}, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector4 TweenVec4Resolved(ImU8String id, ImU8String channelId, Func<Vector4> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenVec4Resolved(id.GetId(), channelId.GetId(), fn, dur, ez, policy, dt);
    }

    /// <summary>
    /// Vec4 with dynamic target.
    /// </summary>
    public static Vector4 TweenVec4Resolved(uint id, uint channelId, Vec4Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenVec4Resolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector4>)Marshal.GetFunctionPointerForDelegate(fn), userData, dur, &ez, policy, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenVec4Resolved(uint, uint, Vec4Resolver, void*, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector4 TweenVec4Resolved(ImU8String id, ImU8String channelId, Vec4Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenVec4Resolved(id.GetId(), channelId.GetId(), fn, userData, dur, ez, policy, dt);
    }

    /// <summary>
    /// Color with dynamic target.
    /// </summary>
    public static Vector4 TweenColorResolved(uint id, uint channelId, Func<Vector4> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 callback(void* userData) => fn();
        Vector4 ret = default;
        ImAnimNative.TweenColorResolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector4>)Marshal.GetFunctionPointerForDelegate(callback), null, dur, &ez, policy, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenColorResolved(uint, uint, Func{Vector4}, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimColorSpace, float)"/>
    public static Vector4 TweenColorResolved(ImU8String id, ImU8String channelId, Func<Vector4> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        return TweenColorResolved(id.GetId(), channelId.GetId(), fn, dur, ez, policy, colorSpace, dt);
    }

    /// <summary>
    /// Color with dynamic target.
    /// </summary>
    public static Vector4 TweenColorResolved(uint id, uint channelId, Vec4Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenColorResolved(&ret, id, channelId, (delegate* unmanaged[Cdecl]<void*, Vector4>)Marshal.GetFunctionPointerForDelegate(fn), userData, dur, &ez, policy, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenColorResolved(uint, uint, Vec4Resolver, void*, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimColorSpace, float)"/>
    public static Vector4 TweenColorResolved(ImU8String id, ImU8String channelId, Vec4Resolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        return TweenColorResolved(id.GetId(), channelId.GetId(), fn, userData, dur, ez, policy, colorSpace, dt);
    }

    /// <summary>
    /// Int with dynamic target.
    /// </summary>
    public static int TweenIntResolved(uint id, uint channelId, Func<int> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        int callback(void* userData) => fn();
        return ImAnimNative.TweenIntResolved(id, channelId, (delegate* unmanaged[Cdecl]<void*, int>)Marshal.GetFunctionPointerForDelegate(callback), null, dur, &ez, policy, dt);
    }

    /// <inheritdoc cref="TweenIntResolved(uint, uint, Func{int}, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static int TweenIntResolved(ImU8String id, ImU8String channelId, Func<int> fn, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenIntResolved(id.GetId(), channelId.GetId(), fn, dur, ez, policy, dt);
    }

    /// <summary>
    /// Int with dynamic target.
    /// </summary>
    public static int TweenIntResolved(uint id, uint channelId, IntResolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return ImAnimNative.TweenIntResolved(id, channelId, (delegate* unmanaged[Cdecl]<void*, int>)Marshal.GetFunctionPointerForDelegate(fn), userData, dur, &ez, policy, dt);
    }

    /// <inheritdoc cref="TweenIntResolved(uint, uint, IntResolver, void*, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static int TweenIntResolved(ImU8String id, ImU8String channelId, IntResolver fn, void* userData, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt)
    {
        return TweenIntResolved(id.GetId(), channelId.GetId(), fn, userData, dur, ez, policy, dt);
    }

    // Color blending utility

    /// <summary>
    /// Blend two sRGB colors in specified color space.
    /// </summary>
    public static Vector4 GetBlendedColor(Vector4 a, Vector4 b, float t, ImAnimColorSpace colorSpace)
    {
        Vector4 ret = default;
        ImAnimNative.GetBlendedColor(&ret, &a, &b, t, colorSpace);
        return ret;
    }


    // ----------------------------------------------------
    // Convenience shorthands for common easings
    // ----------------------------------------------------

    /// <summary>
    /// Create descriptor from preset enum.
    /// </summary>
    public static ImAnimEaseDesc EasePreset(ImAnimEaseType type)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EasePreset(&ret, type);
        return ret;
    }

    /// <summary>
    /// Create cubic bezier easing.
    /// </summary>
    public static ImAnimEaseDesc EaseBezier(float x1, float y1, float x2, float y2)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseBezier(&ret, x1, y1, x2, y2);
        return ret;
    }

    /// <summary>
    /// Create step function easing.
    /// </summary>
    public static ImAnimEaseDesc EaseStepsDesc(int steps, ImAnimEaseStepsMode mode)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseStepsDesc(&ret, steps, mode);
        return ret;
    }

    /// <summary>
    /// Create back easing with overshoot.
    /// </summary>
    public static ImAnimEaseDesc EaseBack(float overshoot)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseBack(&ret, overshoot);
        return ret;
    }

    /// <summary>
    /// Create elastic easing.
    /// </summary>
    public static ImAnimEaseDesc EaseElastic(float amplitude, float period)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseElastic(&ret, amplitude, period);
        return ret;
    }

    /// <summary>
    /// Create physics spring.
    /// </summary>
    public static ImAnimEaseDesc EaseSpring(float mass, float stiffness, float damping, float v0)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseSpring(&ret, mass, stiffness, damping, v0);
        return ret;
    }

    /// <summary>
    /// Use registered custom easing (slot 0-15).
    /// </summary>
    public static ImAnimEaseDesc EaseCustomFn(int slot)
    {
        ImAnimEaseDesc ret = default;
        ImAnimNative.EaseCustomFn(&ret, slot);
        return ret;
    }

    // Scroll animation - smooth scrolling for ImGui windows

    /// <summary>
    /// Scroll current window to Y position.
    /// </summary>
    public static void ScrollToY(float targetY, float duration)
    {
        ImAnimNative.ScrollToY(targetY, duration, null);
    }

    /// <summary>
    /// Scroll current window to Y position.
    /// </summary>
    public static void ScrollToY(float targetY, float duration, ImAnimEaseDesc ez)
    {
        ImAnimNative.ScrollToY(targetY, duration, &ez);
    }

    /// <summary>
    /// Scroll current window to X position.
    /// </summary>
    public static void ScrollToX(float targetX, float duration)
    {
        ImAnimNative.ScrollToX(targetX, duration, null);
    }

    /// <summary>
    /// Scroll current window to X position.
    /// </summary>
    public static void ScrollToX(float targetX, float duration, ImAnimEaseDesc ez)
    {
        ImAnimNative.ScrollToX(targetX, duration, &ez);
    }

    /// <summary>
    /// Scroll to top of window.
    /// </summary>
    public static void ScrollToTop(float duration = 0.3f)
    {
        ImAnimNative.ScrollToTop(duration, null);
    }

    /// <summary>
    /// Scroll to top of window.
    /// </summary>
    public static void ScrollToTop(float duration, ImAnimEaseDesc ez)
    {
        ImAnimNative.ScrollToTop(duration, &ez);
    }

    /// <summary>
    /// Scroll to bottom of window.
    /// </summary>
    public static void ScrollToBottom(float duration = 0.3f)
    {
        ImAnimNative.ScrollToBottom(duration, null);
    }

    /// <summary>
    /// Scroll to bottom of window.
    /// </summary>
    public static void ScrollToBottom(float duration, ImAnimEaseDesc ez)
    {
        ImAnimNative.ScrollToBottom(duration, &ez);
    }


    // ----------------------------------------------------
    // Per-axis easing - different easing per component
    // ----------------------------------------------------

    // Tween with per-axis easing - each component uses its own easing curve

    public static Vector2 TweenVec2PerAxis(ImU8String id, ImU8String channelId, Vector2 target, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.TweenVec2PerAxis(&ret, id.GetId(), channelId.GetId(), &target, dur, &ez, policy, dt);
        return ret;
    }

    public static Vector2 TweenVec2PerAxis(uint id, uint channelId, Vector2 target, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.TweenVec2PerAxis(&ret, id, channelId, &target, dur, &ez, policy, dt);
        return ret;
    }

    public static Vector4 TweenVec4PerAxis(ImU8String id, ImU8String channelId, Vector4 target, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenVec4PerAxis(&ret, id.GetId(), channelId.GetId(), &target, dur, &ez, policy, dt);
        return ret;
    }

    public static Vector4 TweenVec4PerAxis(uint id, uint channelId, Vector4 target, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenVec4PerAxis(&ret, id, channelId, &target, dur, &ez, policy, dt);
        return ret;
    }

    public static Vector4 TweenColorPerAxis(ImU8String id, ImU8String channelId, Vector4 targetSrgb, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenColorPerAxis(&ret, id.GetId(), channelId.GetId(), &targetSrgb, dur, &ez, policy, colorSpace, dt);
        return ret;
    }

    public static Vector4 TweenColorPerAxis(uint id, uint channelId, Vector4 targetSrgb, float dur, ImAnimEasePerAxis ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.TweenColorPerAxis(&ret, id, channelId, &targetSrgb, dur, &ez, policy, colorSpace, dt);
        return ret;
    }


    // ----------------------------------------------------
    // Motion Paths - animate along curves and splines
    // ----------------------------------------------------

    // Single-curve evaluation functions (stateless, for direct use)

    /// <summary>
    /// Evaluate quadratic bezier at t [0,1].
    /// </summary>
    public static Vector2 BezierQuadratic(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 ret = default;
        ImAnimNative.BezierQuadratic(&ret, &p0, &p1, &p2, t);
        return ret;
    }

    /// <summary>
    /// Evaluate cubic bezier at t [0,1].
    /// </summary>
    public static Vector2 BezierCubic(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector2 ret = default;
        ImAnimNative.BezierCubic(&ret, &p0, &p1, &p2, &p3, t);
        return ret;
    }

    /// <summary>
    /// Evaluate Catmull-Rom spline at t [0,1]. Points go through p1 and p2.
    /// </summary>
    public static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, float tension)
    {
        Vector2 ret = default;
        ImAnimNative.CatmullRom(&ret, &p0, &p1, &p2, &p3, t, tension);
        return ret;
    }

    // Derivatives (for tangent/velocity)

    /// <summary>
    /// Derivative of quadratic bezier.
    /// </summary>
    public static Vector2 BezierQuadraticDeriv(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector2 ret = default;
        ImAnimNative.BezierQuadraticDeriv(&ret, &p0, &p1, &p2, t);
        return ret;
    }

    /// <summary>
    /// Derivative of cubic bezier.
    /// </summary>
    public static Vector2 BezierCubicDeriv(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector2 ret = default;
        ImAnimNative.BezierCubicDeriv(&ret, &p0, &p1, &p2, &p3, t);
        return ret;
    }

    /// <summary>
    /// Derivative of Catmull-Rom.
    /// </summary>
    public static Vector2 CatmullRomDeriv(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t, float tension)
    {
        Vector2 ret = default;
        ImAnimNative.CatmullRomDeriv(&ret, &p0, &p1, &p2, &p3, t, tension);
        return ret;
    }

    // Query path info

    /// <summary>
    /// Check if path exists.
    /// </summary>
    public static bool PathExists(uint pathId)
    {
        return ImAnimNative.PathExists(pathId) == 1;
    }

    /// <inheritdoc cref="PathExists(uint)"/>
    public static bool PathExists(ImU8String pathId)
    {
        return PathExists(pathId.GetId());
    }

    /// <summary>
    /// Get approximate path length.
    /// </summary>
    public static float PathLength(uint pathId)
    {
        return ImAnimNative.PathLength(pathId);
    }

    /// <inheritdoc cref="PathLength(uint)"/>
    public static float PathLength(ImU8String pathId)
    {
        return PathLength(pathId.GetId());
    }

    /// <summary>
    /// Sample path at t [0,1].
    /// </summary>
    public static Vector2 PathEvaluate(uint pathId, float t)
    {
        Vector2 ret = default;
        ImAnimNative.PathEvaluate(&ret, pathId, t);
        return ret;
    }

    /// <inheritdoc cref="PathEvaluate(uint, float)"/>
    public static Vector2 PathEvaluate(ImU8String pathId, float t)
    {
        return PathEvaluate(pathId.GetId(), t);
    }

    /// <summary>
    /// Get tangent (normalized direction) at t.
    /// </summary>
    public static Vector2 PathTangent(uint pathId, float t)
    {
        Vector2 ret = default;
        ImAnimNative.PathTangent(&ret, pathId, t);
        return ret;
    }

    /// <inheritdoc cref="PathTangent(uint, float)"/>
    public static Vector2 PathTangent(ImU8String pathId, float t)
    {
        return PathTangent(pathId.GetId(), t);
    }

    /// <summary>
    /// Get rotation angle (radians) at t.
    /// </summary>
    public static float PathAngle(uint pathId, float t)
    {
        return ImAnimNative.PathAngle(pathId, t);
    }

    /// <inheritdoc cref="PathAngle(uint, float)"/>
    public static float PathAngle(ImU8String pathId, float t)
    {
        return PathAngle(pathId.GetId(), t);
    }

    // Tween along a path

    /// <summary>
    /// Animate position along path.
    /// </summary>
    public static Vector2 TweenPath(uint id, uint channelId, uint pathId, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt = -1f)
    {
        Vector2 ret = default;
        ImAnimNative.TweenPath(&ret, id, channelId, pathId, dur, &ez, policy, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenPath(uint, uint, uint, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector2 TweenPath(ImU8String id, ImU8String channelId, ImU8String pathId, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt = -1f)
    {
        return TweenPath(id.GetId(), channelId.GetId(), pathId.GetId(), dur, ez, policy, dt);
    }

    /// <summary>
    /// Animate rotation angle along path.
    /// </summary>
    public static float TweenPathAngle(uint id, uint channelId, uint pathId, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt = -1f)
    {
        return ImAnimNative.TweenPathAngle(id, channelId, pathId, dur, &ez, policy, dt);
    }

    /// <inheritdoc cref="TweenPathAngle(uint, uint, uint, float, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static float TweenPathAngle(ImU8String id, ImU8String channelId, ImU8String pathId, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, float dt = -1f)
    {
        return TweenPathAngle(id.GetId(), channelId.GetId(), pathId.GetId(), dur, ez, policy, dt);
    }


    // ----------------------------------------------------
    // Arc-length parameterization (for constant-speed animation)
    // ----------------------------------------------------

    // Build arc-length lookup table for a path (call once per path, improves accuracy)

    /// <summary>
    /// Build LUT with specified resolution.
    /// </summary>
    public static void PathBuildArcLut(uint pathId, int subdivisions)
    {
        ImAnimNative.PathBuildArcLut(pathId, subdivisions);
    }

    /// <inheritdoc cref="PathBuildArcLut(uint, int)"/>
    public static void PathBuildArcLut(ImU8String pathId, int subdivisions)
    {
        PathBuildArcLut(pathId.GetId(), subdivisions);
    }

    /// <summary>
    /// Check if path has precomputed LUT.
    /// </summary>
    public static bool PathHasArcLut(uint pathId)
    {
        return ImAnimNative.PathHasArcLut(pathId) == 1;
    }

    /// <inheritdoc cref="PathHasArcLut(uint)"/>
    public static bool PathHasArcLut(ImU8String pathId)
    {
        return PathHasArcLut(pathId.GetId());
    }

    // Distance-based path evaluation (uses arc-length LUT for constant speed)

    /// <summary>
    /// Convert arc-length distance to t parameter.
    /// </summary>
    public static float PathDistanceToT(uint pathId, float distance)
    {
        return ImAnimNative.PathDistanceToT(pathId, distance);
    }

    /// <inheritdoc cref="PathDistanceToT(uint, float)"/>
    public static float PathDistanceToT(ImU8String pathId, float distance)
    {
        return PathDistanceToT(pathId.GetId(), distance);
    }

    /// <summary>
    /// Get position at arc-length distance.
    /// </summary>
    public static Vector2 PathEvaluateAtDistance(uint pathId, float distance)
    {
        Vector2 ret = default;
        ImAnimNative.PathEvaluateAtDistance(&ret, pathId, distance);
        return ret;
    }

    /// <inheritdoc cref="PathEvaluateAtDistance(uint, float)"/>
    public static Vector2 PathEvaluateAtDistance(ImU8String pathId, float distance)
    {
        return PathEvaluateAtDistance(pathId.GetId(), distance);
    }

    /// <summary>
    /// Get rotation angle at arc-length distance.
    /// </summary>
    public static float PathAngleAtDistance(uint pathId, float distance)
    {
        return ImAnimNative.PathAngleAtDistance(pathId, distance);
    }

    /// <inheritdoc cref="PathAngleAtDistance(uint, float)"/>
    public static float PathAngleAtDistance(ImU8String pathId, float distance)
    {
        return PathAngleAtDistance(pathId.GetId(), distance);
    }

    /// <summary>
    /// Get tangent at arc-length distance.
    /// </summary>
    public static Vector2 PathTangentAtDistance(uint pathId, float distance)
    {
        Vector2 ret = default;
        ImAnimNative.PathTangentAtDistance(&ret, pathId, distance);
        return ret;
    }

    /// <inheritdoc cref="PathTangentAtDistance(uint, float)"/>
    public static Vector2 PathTangentAtDistance(ImU8String pathId, float distance)
    {
        return PathTangentAtDistance(pathId.GetId(), distance);
    }


    // ----------------------------------------------------
    // Path Morphing - interpolate between two paths
    // ----------------------------------------------------

    /// <summary>
    /// Evaluate morphed path at parameter t [0,1] with blend factor [0,1]<br/>
    /// path_a at blend=0, path_b at blend=1<br/>
    /// Paths can have different numbers of segments - they are resampled to match
    /// </summary>
    public static Vector2 PathMorph(uint pathA, uint pathB, float t, float blend)
    {
        Vector2 ret = default;
        ImAnimNative.PathMorph(&ret, pathA, pathB, t, blend, null);
        return ret;
    }

    /// <inheritdoc cref="PathMorph(uint, uint, float, float)"/>
    public static Vector2 PathMorph(ImU8String pathA, ImU8String pathB, float t, float blend)
    {
        return PathMorph(pathA.GetId(), pathB.GetId(), t, blend);
    }

    /// <inheritdoc cref="PathMorph(uint, uint, float, float)"/>
    public static Vector2 PathMorph(uint pathA, uint pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        Vector2 ret = default;
        ImAnimNative.PathMorph(&ret, pathA, pathB, t, blend, &opts);
        return ret;
    }

    /// <inheritdoc cref="PathMorph(uint, uint, float, float)"/>
    public static Vector2 PathMorph(ImU8String pathA, ImU8String pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        return PathMorph(pathA.GetId(), pathB.GetId(), t, blend, opts);
    }

    /// <summary>
    /// Get tangent of morphed path
    /// </summary>
    public static Vector2 PathMorphTangent(uint pathA, uint pathB, float t, float blend)
    {
        Vector2 ret = default;
        ImAnimNative.PathMorphTangent(&ret, pathA, pathB, t, blend, null);
        return ret;
    }

    /// <inheritdoc cref="PathMorphTangent(uint, uint, float, float)"/>
    public static Vector2 PathMorphTangent(ImU8String pathA, ImU8String pathB, float t, float blend)
    {
        return PathMorphTangent(pathA.GetId(), pathB.GetId(), t, blend);
    }

    /// <inheritdoc cref="PathMorphTangent(uint, uint, float, float)"/>
    public static Vector2 PathMorphTangent(uint pathA, uint pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        Vector2 ret = default;
        ImAnimNative.PathMorphTangent(&ret, pathA, pathB, t, blend, &opts);
        return ret;
    }

    /// <inheritdoc cref="PathMorphTangent(uint, uint, float, float, ImAnimMorphOpts)"/>
    public static Vector2 PathMorphTangent(ImU8String pathA, ImU8String pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        return PathMorphTangent(pathA.GetId(), pathB.GetId(), t, blend, opts);
    }

    /// <summary>
    /// Get angle (radians) of morphed path
    /// </summary>
    public static float PathMorphAngle(uint pathA, uint pathB, float t, float blend)
    {
        return ImAnimNative.PathMorphAngle(pathA, pathB, t, blend, null);
    }

    /// <inheritdoc cref="PathMorphAngle(uint, uint, float, float)"/>
    public static float PathMorphAngle(ImU8String pathA, ImU8String pathB, float t, float blend)
    {
        return PathMorphAngle(pathA.GetId(), pathB.GetId(), t, blend);
    }

    /// <inheritdoc cref="PathMorphAngle(uint, uint, float, float)"/>
    public static float PathMorphAngle(uint pathA, uint pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        return ImAnimNative.PathMorphAngle(pathA, pathB, t, blend, &opts);
    }

    /// <inheritdoc cref="PathMorphAngle(uint, uint, float, float)"/>
    public static float PathMorphAngle(ImU8String pathA, ImU8String pathB, float t, float blend, ImAnimMorphOpts opts)
    {
        return PathMorphAngle(pathA.GetId(), pathB.GetId(), t, blend, opts);
    }

    /// <summary>
    /// Tween along a morphing path - animates both position along path AND the morph blend
    /// </summary>
    public static Vector2 TweenPathMorph(uint id, uint channelId, uint pathA, uint pathB, float targetBlend, float dur, ImAnimEaseDesc pathEase, ImAnimEaseDesc morphEase, ImAnimPolicy policy, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.TweenPathMorph(&ret, id, channelId, pathA, pathB, targetBlend, dur, &pathEase, &morphEase, policy, dt, null);
        return ret;
    }

    /// <inheritdoc cref="TweenPathMorph(uint, uint, uint, uint, float, float, ImAnimEaseDesc, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector2 TweenPathMorph(ImU8String id, ImU8String channelId, ImU8String pathA, ImU8String pathB, float targetBlend, float dur, ImAnimEaseDesc pathEase, ImAnimEaseDesc morphEase, ImAnimPolicy policy, float dt)
    {
        return TweenPathMorph(id.GetId(), channelId.GetId(), pathA.GetId(), pathB.GetId(), targetBlend, dur, pathEase, morphEase, policy, dt);
    }

    /// <inheritdoc cref="TweenPathMorph(uint, uint, uint, uint, float, float, ImAnimEaseDesc, ImAnimEaseDesc, ImAnimPolicy, float)"/>
    public static Vector2 TweenPathMorph(uint id, uint channelId, uint pathA, uint pathB, float targetBlend, float dur, ImAnimEaseDesc pathEase, ImAnimEaseDesc morphEase, ImAnimPolicy policy, float dt, ImAnimMorphOpts opts)
    {
        Vector2 ret = default;
        ImAnimNative.TweenPathMorph(&ret, id, channelId, pathA, pathB, targetBlend, dur, &pathEase, &morphEase, policy, dt, &opts);
        return ret;
    }

    /// <inheritdoc cref="TweenPathMorph(uint, uint, uint, uint, float, float, ImAnimEaseDesc, ImAnimEaseDesc, ImAnimPolicy, float, ImAnimMorphOpts)"/>
    public static Vector2 TweenPathMorph(ImU8String id, ImU8String channelId, ImU8String pathA, ImU8String pathB, float targetBlend, float dur, ImAnimEaseDesc pathEase, ImAnimEaseDesc morphEase, ImAnimPolicy policy, float dt, ImAnimMorphOpts opts)
    {
        return TweenPathMorph(id.GetId(), channelId.GetId(), pathA.GetId(), pathB.GetId(), targetBlend, dur, pathEase, morphEase, policy, dt, opts);
    }

    /// <summary>
    /// Get current morph blend value from a tween (for querying state)
    /// </summary>
    public static float GetMorphBlend(uint id, uint channelId)
    {
        return ImAnimNative.GetMorphBlend(id, channelId);
    }

    /// <inheritdoc cref="GetMorphBlend(uint, uint)"/>
    public static float GetMorphBlend(ImU8String id, ImU8String channelId)
    {
        return GetMorphBlend(id.GetId(), channelId.GetId());
    }


    // ----------------------------------------------------
    // Text along motion paths
    // ----------------------------------------------------

    /// <summary>
    /// Render text along a path (static - no animation)
    /// </summary>
    public static void TextPath(uint pathId, ImU8String text)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextPath(pathId, textPtr, null);
        text.Recycle();
    }

    /// <inheritdoc cref="TextPath(uint, ImU8String)"/>
    public static void TextPath(ImU8String pathId, ImU8String text)
    {
        TextPath(pathId.GetId(), text);
    }

    /// <inheritdoc cref="TextPath(uint, ImU8String)"/>
    public static void TextPath(uint pathId, ImU8String text, ImAnimTextPathOpts opts)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextPath(pathId, textPtr, &opts);
        text.Recycle();
    }

    /// <inheritdoc cref="TextPath(uint, ImU8String, ImAnimTextPathOpts)"/>
    public static void TextPath(ImU8String pathId, ImU8String text, ImAnimTextPathOpts opts)
    {
        TextPath(pathId.GetId(), text, opts);
    }

    /// <summary>
    /// Animated text along path (characters appear progressively)
    /// </summary>
    public static void TextPathAnimated(uint pathId, ImU8String text, float progress)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextPathAnimated(pathId, textPtr, progress, null);
        text.Recycle();
    }

    /// <inheritdoc cref="TextPathAnimated(uint, ImU8String, float)"/>
    public static void TextPathAnimated(ImU8String pathId, ImU8String text, float progress)
    {
        TextPathAnimated(pathId.GetId(), text, progress);
    }

    /// <inheritdoc cref="TextPathAnimated(uint, ImU8String, float)"/>
    public static void TextPathAnimated(uint pathId, ImU8String text, float progress, ImAnimTextPathOpts opts)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextPathAnimated(pathId, textPtr, progress, &opts);
        text.Recycle();
    }

    /// <inheritdoc cref="TextPathAnimated(uint, ImU8String, float, ImAnimTextPathOpts)"/>
    public static void TextPathAnimated(ImU8String pathId, ImU8String text, float progress, ImAnimTextPathOpts opts)
    {
        TextPathAnimated(pathId.GetId(), text, progress, opts);
    }

    /// <summary>
    /// Helper: Get text width for path layout calculations
    /// </summary>
    public static float TextPathWidth(ImU8String text)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextPathWidth(textPtr, null);
        text.Recycle();
        return ret;
    }

    /// <inheritdoc cref="TextPathWidth(ImU8String)"/>
    public static float TextPathWidth(ImU8String text, ImAnimTextPathOpts opts)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextPathWidth(textPtr, &opts);
        text.Recycle();
        return ret;
    }


    // ----------------------------------------------------
    // Quad transform helpers (for advanced custom rendering)
    // ----------------------------------------------------

    /// <summary>
    /// Transform a quad (4 vertices) by rotation and translation
    /// </summary>
    public static void TransformQuad(ref Quaternion quad, Vector2 center, float angleRad, Vector2 translation)
    {
        ImAnimNative.TransformQuad((Quaternion*)Unsafe.AsPointer(ref quad), &center, angleRad, &translation);
    }

    /// <summary>
    /// Create a rotated quad for a glyph at a position on the path
    /// </summary>
    public static void MakeGlyphQuad(ref Quaternion quad, Vector2 pos, float angleRad, float glyphWidth, float glyphHeight, float baselineOffset)
    {
        ImAnimNative.MakeGlyphQuad((Quaternion*)Unsafe.AsPointer(ref quad), &pos, angleRad, glyphWidth, glyphHeight, baselineOffset);
    }


    // ----------------------------------------------------
    // Text Stagger - per-character animation effects
    // ----------------------------------------------------

    /// <summary>
    /// Render text with per-character stagger animation
    /// </summary>
    public static void TextStagger(uint id, ImU8String text, float progress)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextStagger(id, textPtr, progress, null);
        text.Recycle();
    }

    /// <inheritdoc cref="TextStagger(uint, ImU8String, float)"/>
    public static void TextStagger(ImU8String id, ImU8String text, float progress)
    {
        TextStagger(id.GetId(), text, progress);
    }

    /// <inheritdoc cref="TextStagger(uint, ImU8String, float)"/>
    public static void TextStagger(uint id, ImU8String text, float progress, ImAnimTextStaggerOpts opts)
    {
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ImAnimNative.TextStagger(id, textPtr, progress, &opts);
        text.Recycle();
    }

    /// <inheritdoc cref="TextStagger(uint, ImU8String, float, ImAnimTextStaggerOpts)"/>
    public static void TextStagger(ImU8String id, ImU8String text, float progress, ImAnimTextStaggerOpts opts)
    {
        TextStagger(id.GetId(), text, progress, opts);
    }

    /// <summary>
    /// Get text width for layout calculations
    /// </summary>
    public static float TextStaggerWidth(ImU8String text)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextStaggerWidth(textPtr, null);
        text.Recycle();
        return ret;
    }

    /// <inheritdoc cref="TextStaggerWidth(ImU8String)"/>
    public static float TextStaggerWidth(ImU8String text, ImAnimTextStaggerOpts opts)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextStaggerWidth(textPtr, &opts);
        text.Recycle();
        return ret;
    }

    /// <summary>
    /// Get total animation duration for text (accounts for stagger delays)
    /// </summary>
    public static float TextStaggerDuration(ImU8String text)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextStaggerDuration(textPtr, null);
        text.Recycle();
        return ret;
    }

    /// <inheritdoc cref="TextStaggerDuration(ImU8String)"/>
    public static float TextStaggerDuration(ImU8String text, ImAnimTextStaggerOpts opts)
    {
        float ret = default;
        fixed (byte* textPtr = &text.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.TextStaggerDuration(textPtr, &opts);
        text.Recycle();
        return ret;
    }


    // ----------------------------------------------------
    // Noise Channels - Perlin/Simplex noise for organic movement
    // ----------------------------------------------------

    // Sample noise at a point (returns value in [-1, 1])

    /// <summary>
    /// 2D noise
    /// </summary>
    public static float Noise2D(float x, float y)
    {
        return ImAnimNative.Noise2D(x, y, null);
    }

    /// <inheritdoc cref="Noise2D(float, float)"/>
    public static float Noise2D(float x, float y, ImAnimNoiseOpts opts)
    {
        return ImAnimNative.Noise2D(x, y, &opts);
    }

    /// <summary>
    /// 3D noise
    /// </summary>
    public static float Noise3D(float x, float y, float z)
    {
        return ImAnimNative.Noise3D(x, y, z, null);
    }

    /// <inheritdoc cref="Noise3D(float, float)"/>
    public static float Noise3D(float x, float y, float z, ImAnimNoiseOpts opts)
    {
        return ImAnimNative.Noise3D(x, y, z, &opts);
    }

    // Animated noise channels - continuous noise that evolves over time

    /// <summary>
    /// 1D animated noise
    /// </summary>
    public static float NoiseChannelFloat(uint id, float frequency, float amplitude, ImAnimNoiseOpts opts, float dt)
    {
        return ImAnimNative.NoiseChannelFloat(id, frequency, amplitude, &opts, dt);
    }

    /// <inheritdoc cref="NoiseChannelFloat(uint, float, float, ImAnimNoiseOpts, float)"/>
    public static float NoiseChannelFloat(ImU8String id, float frequency, float amplitude, ImAnimNoiseOpts opts, float dt)
    {
        return NoiseChannelFloat(id.GetId(), frequency, amplitude, opts, dt);
    }

    /// <summary>
    /// 2D animated noise
    /// </summary>
    public static Vector2 NoiseChannelVec2(uint id, Vector2 frequency, Vector2 amplitude, ImAnimNoiseOpts opts, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.NoiseChannelVec2(&ret, id, &frequency, &amplitude, &opts, dt);
        return ret;
    }

    /// <inheritdoc cref="NoiseChannelVec2(uint, Vector2, Vector2, ImAnimNoiseOpts, float)"/>
    public static Vector2 NoiseChannelVec2(ImU8String id, Vector2 frequency, Vector2 amplitude, ImAnimNoiseOpts opts, float dt)
    {
        return NoiseChannelVec2(id.GetId(), frequency, amplitude, opts, dt);
    }

    /// <summary>
    /// 4D animated noise
    /// </summary>
    public static Vector4 NoiseChannelVec4(uint id, Vector4 frequency, Vector4 amplitude, ImAnimNoiseOpts opts, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.NoiseChannelVec4(&ret, id, &frequency, &amplitude, &opts, dt);
        return ret;
    }

    /// <inheritdoc cref="NoiseChannelVec4(uint, Vector4, Vector4, ImAnimNoiseOpts, float)"/>
    public static Vector4 NoiseChannelVec4(ImU8String id, Vector4 frequency, Vector4 amplitude, ImAnimNoiseOpts opts, float dt)
    {
        return NoiseChannelVec4(id.GetId(), frequency, amplitude, opts, dt);
    }

    /// <summary>
    /// Animated color noise in specified color space
    /// </summary>
    public static Vector4 NoiseChannelColor(uint id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimNoiseOpts opts, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.NoiseChannelColor(&ret, id, &baseColor, &amplitude, frequency, &opts, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="NoiseChannelColor(uint, Vector4, Vector4, float, ImAnimNoiseOpts, ImAnimColorSpace, float)"/>
    public static Vector4 NoiseChannelColor(ImU8String id, Vector4 baseColor, Vector4 amplitude, float frequency, ImAnimNoiseOpts opts, ImAnimColorSpace colorSpace, float dt)
    {
        return NoiseChannelColor(id.GetId(), baseColor, amplitude, frequency, opts, colorSpace, dt);
    }

    // Convenience: smooth random movement (like wiggle but using noise)

    /// <summary>
    /// Simple 1D smooth noise
    /// </summary>
    public static float SmoothNoiseFloat(uint id, float amplitude, float speed, float dt)
    {
        return ImAnimNative.SmoothNoiseFloat(id, amplitude, speed, dt);
    }

    /// <inheritdoc cref="SmoothNoiseFloat(uint, float, float, float)"/>
    public static float SmoothNoiseFloat(ImU8String id, float amplitude, float speed, float dt)
    {
        return SmoothNoiseFloat(id.GetId(), amplitude, speed, dt);
    }

    /// <summary>
    /// Simple 2D smooth noise
    /// </summary>
    public static Vector2 SmoothNoiseVec2(uint id, Vector2 amplitude, float speed, float dt)
    {
        Vector2 ret = default;
        ImAnimNative.SmoothNoiseVec2(&ret, id, &amplitude, speed, dt);
        return ret;
    }

    /// <inheritdoc cref="SmoothNoiseFloat(uint, float, float, float)"/>
    public static Vector2 SmoothNoiseVec2(ImU8String id, Vector2 amplitude, float speed, float dt)
    {
        return SmoothNoiseVec2(id.GetId(), amplitude, speed, dt);
    }

    /// <summary>
    /// Simple 4D smooth noise
    /// </summary>
    public static Vector4 SmoothNoiseVec4(uint id, Vector4 amplitude, float speed, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.SmoothNoiseVec4(&ret, id, &amplitude, speed, dt);
        return ret;
    }

    /// <inheritdoc cref="SmoothNoiseVec4(uint, Vector4, float, float)"/>
    public static Vector4 SmoothNoiseVec4(ImU8String id, Vector4 amplitude, float speed, float dt)
    {
        return SmoothNoiseVec4(id.GetId(), amplitude, speed, dt);
    }

    /// <summary>
    /// Smooth noise for colors in specified color space
    /// </summary>
    public static Vector4 SmoothNoiseColor(uint id, Vector4 baseColor, Vector4 amplitude, float speed, ImAnimColorSpace colorSpace, float dt)
    {
        Vector4 ret = default;
        ImAnimNative.SmoothNoiseColor(&ret, id, &baseColor, &amplitude, speed, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="SmoothNoiseColor(uint, Vector4, Vector4, float, ImAnimColorSpace, float)"/>
    public static Vector4 SmoothNoiseColor(ImU8String id, Vector4 baseColor, Vector4 amplitude, float speed, ImAnimColorSpace colorSpace, float dt)
    {
        return SmoothNoiseColor(id.GetId(), baseColor, amplitude, speed, colorSpace, dt);
    }


    // ----------------------------------------------------
    // Style Interpolation - animate between ImGuiStyle themes
    // ----------------------------------------------------

    // Register a named style for interpolation

    /// <summary>
    /// Register a style snapshot
    /// </summary>
    public static void StyleRegister(uint styleId, ImGuiStylePtr style)
    {
        ImAnimNative.StyleRegister(styleId, style);
    }

    /// <inheritdoc cref="StyleRegister(uint, ImGuiStylePtr)"/>
    public static void StyleRegister(ImU8String styleId, ImGuiStylePtr style)
    {
        StyleRegister(styleId.GetId(), style);
    }

    /// <summary>
    /// Register current ImGui style
    /// </summary>
    public static void StyleRegisterCurrent(uint styleId)
    {
        ImAnimNative.StyleRegisterCurrent(styleId);
    }

    /// <inheritdoc cref="StyleRegisterCurrent(uint)"/>
    public static void StyleRegisterCurrent(ImU8String styleId)
    {
        StyleRegisterCurrent(styleId.GetId());
    }

    /// <summary>
    /// Blend between two registered styles (result applied to ImGui::GetStyle())
    /// Uses iam_color_space for color blending mode (iam_col_oklab recommended)
    /// </summary>
    public static void StyleBlend(uint styleA, uint styleB, float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        ImAnimNative.StyleBlend(styleA, styleB, t, colorSpace);
    }

    /// <inheritdoc cref="StyleBlend(uint, uint, float, ImAnimColorSpace)"/>
    public static void StyleBlend(ImU8String styleA, ImU8String styleB, float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        StyleBlend(styleA.GetId(), styleB.GetId(), t, colorSpace);
    }

    /// <summary>
    /// Tween between styles over time
    /// </summary>
    public static void StyleTween(uint id, uint targetStyle, float duration, ImAnimEaseDesc ease, ImAnimColorSpace colorSpace, float dt)
    {
        ImAnimNative.StyleTween(id, targetStyle, duration, &ease, colorSpace, dt);
    }

    /// <inheritdoc cref="StyleTween(uint, uint, float, ImAnimEaseDesc, ImAnimColorSpace, float)"/>
    public static void StyleTween(ImU8String id, uint targetStyle, float duration, ImAnimEaseDesc ease, ImAnimColorSpace colorSpace, float dt)
    {
        StyleTween(id.GetId(), targetStyle, duration, ease, colorSpace, dt);
    }

    /// <summary>
    /// Get interpolated style without applying
    /// </summary>
    public static ImGuiStyle StyleBlendTo(uint styleA, uint styleB, float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        ImGuiStyle ret = default;
        ImAnimNative.StyleBlendTo(styleA, styleB, t, &ret, colorSpace);
        return ret;
    }

    /// <inheritdoc cref="StyleBlendTo(uint, uint, float, ImAnimColorSpace)"/>
    public static ImGuiStyle StyleBlendTo(ImU8String styleA, ImU8String styleB, float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        return StyleBlendTo(styleA.GetId(), styleB.GetId(), t, colorSpace);
    }

    /// <summary>
    /// Check if a style is registered
    /// </summary>
    public static bool StyleExists(uint styleId)
    {
        return ImAnimNative.StyleExists(styleId) == 1;
    }

    /// <inheritdoc cref="StyleExists(uint)"/>
    public static bool StyleExists(ImU8String styleId)
    {
        return StyleExists(styleId.GetId());
    }

    /// <summary>
    /// Remove a registered style
    /// </summary>
    public static void StyleUnregister(uint styleId)
    {
        ImAnimNative.StyleUnregister(styleId);
    }

    /// <inheritdoc cref="StyleUnregister(uint)"/>
    public static void StyleUnregister(ImU8String styleId)
    {
        StyleUnregister(styleId.GetId());
    }


    // ----------------------------------------------------
    // Gradient Interpolation - animate between color gradients
    // ----------------------------------------------------

    /// <summary>
    /// Blend between two gradients
    /// </summary>
    public static ImAnimGradient GradientLerp(ImAnimGradient a, ImAnimGradient b, float t, ImAnimColorSpace colorSpace = ImAnimColorSpace.Oklab)
    {
        ImAnimGradient ret = default;
        ImAnimNative.GradientLerp(&ret, &a, &b, t, colorSpace);
        return ret;
    }

    /// <summary>
    /// Tween between gradients over time
    /// </summary>
    public static ImAnimGradient TweenGradient(uint id, uint channelId, ImAnimGradient target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        ImAnimGradient ret = default;
        ImAnimNative.TweenGradient(&ret, id, channelId, &target, dur, &ez, policy, colorSpace, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenGradient(uint, uint, ImAnimGradient, float, ImAnimEaseDesc, ImAnimPolicy, ImAnimColorSpace, float)"/>
    public static ImAnimGradient TweenGradient(ImU8String id, ImU8String channelId, ImAnimGradient target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, ImAnimColorSpace colorSpace, float dt)
    {
        return TweenGradient(id.GetId(), channelId.GetId(), target, dur, ez, policy, colorSpace, dt);
    }


    // ----------------------------------------------------
    // Transform Interpolation - animate 2D transforms
    // ----------------------------------------------------

    /// <summary>
    /// Blend between two transforms with rotation interpolation
    /// </summary>
    public static ImAnimTransform TransformLerp(ImAnimTransform a, ImAnimTransform b, float t, ImAnimRotationMode rotationMode = ImAnimRotationMode.Shortest)
    {
        ImAnimTransform ret = default;
        ImAnimNative.TransformLerp(&ret, &a, &b, t, rotationMode);
        return ret;
    }

    /// <summary>
    /// Tween between transforms over time
    /// </summary>
    public static ImAnimTransform TweenTransform(uint id, uint channelId, ImAnimTransform target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, int rotationMode, float dt)
    {
        ImAnimTransform ret = default;
        ImAnimNative.TweenTransform(&ret, id, channelId, &target, dur, &ez, policy, rotationMode, dt);
        return ret;
    }

    /// <inheritdoc cref="TweenTransform(uint, uint, ImAnimTransform, float, ImAnimEaseDesc, ImAnimPolicy, int, float)"/>
    public static ImAnimTransform TweenTransform(ImU8String id, ImU8String channelId, ImAnimTransform target, float dur, ImAnimEaseDesc ez, ImAnimPolicy policy, int rotationMode, float dt)
    {
        return TweenTransform(id.GetId(), channelId.GetId(), target, dur, ez, policy, rotationMode, dt);
    }

    /// <summary>
    /// Decompose a 3x2 matrix into transform components
    /// </summary>
    public static ImAnimTransform TransformFromMatrix(Matrix3x2 matrix)
    {
        ImAnimTransform ret = default;
        ImAnimNative.TransformFromMatrix(&ret, matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
        return ret;
    }

    /// <inheritdoc cref="TransformFromMatrix(Matrix3x2)"/>
    public static ImAnimTransform TransformFromMatrix(float m00, float m01, float m10, float m11, float tx, float ty)
    {
        ImAnimTransform ret = default;
        ImAnimNative.TransformFromMatrix(&ret, m00, m01, m10, m11, tx, ty);
        return ret;
    }

    /// <summary>
    /// Convert transform to 3x2 matrix (row-major: [m00 m01 tx; m10 m11 ty])
    /// </summary>
    public static Matrix3x2 TransformToMatrix(ImAnimTransform t)
    {
        Matrix3x2 ret = default;
        ImAnimNative.TransformToMatrix(&t, &ret);
        return ret;
    }


    // ----------------------------------------------------
    // iam_clip - fluent API for authoring animations
    // ----------------------------------------------------

    /// <summary>
    /// Start building a new clip with the given ID
    /// </summary>
    public static ImAnimClip ClipBegin(uint clipId)
    {
        ImAnimClip ret = default;
        ImAnimNative.ClipBegin(&ret, clipId);
        return ret;
    }

    /// <inheritdoc cref="ClipBegin(uint)"/>
    public static ImAnimClip ClipBegin(ImU8String clipId)
    {
        return ClipBegin(clipId.GetId());
    }


    // ----------------------------------------------------
    // Clip System API
    // ----------------------------------------------------

    /// <summary>
    /// Initialize/shutdown (optional - auto-init on first use)
    /// </summary>
    public static void ClipInit(int initialClipCap, int initialInstCap)
    {
        ImAnimNative.ClipInit(initialClipCap, initialInstCap);
    }

    public static void ClipShutdown()
    {
        ImAnimNative.ClipShutdown();
    }

    /// <summary>
    /// Per-frame update (call after <see cref="UpdateBeginFrame"/>)
    /// </summary>
    public static void ClipUpdate(float dt)
    {
        ImAnimNative.ClipUpdate(dt);
    }

    /// <summary>
    /// Garbage collection for instances
    /// </summary>
    public static void ClipGc(uint maxAgeFrames)
    {
        ImAnimNative.ClipGc(maxAgeFrames);
    }

    /// <summary>
    /// Play a clip on an instance (creates or reuses instance)
    /// </summary>
    public static ImAnimInstance Play(uint clipId, uint instanceId)
    {
        ImAnimInstance ret = default;
        ImAnimNative.Play(&ret, clipId, instanceId);
        return ret;
    }

    /// <inheritdoc cref="Play(uint, uint)"/>
    public static ImAnimInstance Play(ImU8String clipId, uint instanceId)
    {
        return Play(clipId.GetId(), instanceId);
    }

    /// <summary>
    /// Get an existing instance (returns invalid iam_instance if not found)
    /// </summary>
    public static ImAnimInstance GetInstance(uint instanceId)
    {
        ImAnimInstance ret = default;
        ImAnimNative.GetInstance(&ret, instanceId);
        return ret;
    }

    /// <inheritdoc cref="GetInstance(uint)"/>
    public static ImAnimInstance GetInstance(ImU8String instanceId)
    {
        return GetInstance(instanceId.GetId());
    }

    // Query clip info

    /// <summary>
    /// Get clip duration in seconds.
    /// </summary>
    public static float ClipDuration(uint clipId)
    {
        return ImAnimNative.ClipDuration(clipId);
    }

    /// <inheritdoc cref="ClipDuration(uint)"/>
    public static float ClipDuration(ImU8String clipId)
    {
        return ClipDuration(clipId.GetId());
    }

    /// <summary>
    /// Check if clip exists.
    /// </summary>
    public static bool ClipExists(uint clipId)
    {
        return ImAnimNative.ClipExists(clipId) == 1;
    }

    /// <inheritdoc cref="ClipExists(uint)"/>
    public static bool ClipExists(ImU8String clipId)
    {
        return ClipExists(clipId.GetId());
    }

    // Stagger helpers - compute delay for indexed instances

    /// <summary>
    /// Get stagger delay for element at index.
    /// </summary>
    public static float StaggerDelay(uint clipId, int index)
    {
        return ImAnimNative.StaggerDelay(clipId, index);
    }

    /// <inheritdoc cref="StaggerDelay(uint, int)"/>
    public static float StaggerDelay(ImU8String clipId, int index)
    {
        return StaggerDelay(clipId.GetId(), index);
    }

    /// <summary>
    /// Play with stagger delay applied.
    /// </summary>
    public static ImAnimInstance PlayStagger(uint clipId, uint instanceId, int index)
    {
        ImAnimInstance ret = default;
        ImAnimNative.PlayStagger(&ret, clipId, instanceId, index);
        return ret;
    }

    /// <inheritdoc cref="PlayStagger(uint, uint, int)"/>
    public static ImAnimInstance PlayStagger(ImU8String clipId, ImU8String instanceId, int index)
    {
        return PlayStagger(clipId.GetId(), instanceId.GetId(), index);
    }

    // Layering support - blend multiple animation instances

    /// <summary>
    /// Start blending into target instance.
    /// </summary>
    public static void LayerBegin(uint instanceId)
    {
        ImAnimNative.LayerBegin(instanceId);
    }

    /// <inheritdoc cref="LayerBegin(uint)"/>
    public static void LayerBegin(ImU8String instanceId)
    {
        LayerBegin(instanceId.GetId());
    }

    /// <summary>
    /// Add source instance with weight.
    /// </summary>
    public static void LayerAdd(ImAnimInstance inst, float weight)
    {
        ImAnimNative.LayerAdd(&inst, weight);
    }

    /// <summary>
    /// Finalize blending and normalize weights.
    /// </summary>
    public static void LayerEnd(uint instanceId)
    {
        ImAnimNative.LayerEnd(instanceId);
    }

    /// <inheritdoc cref="LayerEnd(uint)"/>
    public static void LayerEnd(ImU8String instanceId)
    {
        LayerEnd(instanceId.GetId());
    }

    /// <summary>
    /// Get blended float value.
    /// </summary>
    public static bool GetBlendedFloat(uint instanceId, uint channel, out float value)
    {
        fixed (float* outVal = &value)
            return ImAnimNative.GetBlendedFloat(instanceId, channel, outVal) == 1;
    }

    /// <inheritdoc cref="GetBlendedFloat(uint, uint, out float)"/>
    public static bool GetBlendedFloat(ImU8String instanceId, ImU8String channel, out float value)
    {
        return GetBlendedFloat(instanceId.GetId(), channel.GetId(), out value);
    }

    /// <summary>
    /// Get blended vec2 value.
    /// </summary>
    public static bool GetBlendedVec2(uint instanceId, uint channel, out Vector2 value)
    {
        fixed (Vector2* outVal = &value)
            return ImAnimNative.GetBlendedVec2(instanceId, channel, outVal) == 1;
    }

    /// <inheritdoc cref="GetBlendedVec2(uint, uint, out Vector2)"/>
    public static bool GetBlendedVec2(ImU8String instanceId, ImU8String channel, out Vector2 value)
    {
        return GetBlendedVec2(instanceId.GetId(), channel.GetId(), out value);
    }

    /// <summary>
    /// Get blended vec4 value.
    /// </summary>
    public static bool GetBlendedVec4(uint instanceId, uint channel, out Vector4 value)
    {
        fixed (Vector4* outVal = &value)
            return ImAnimNative.GetBlendedVec4(instanceId, channel, outVal) == 1;
    }

    /// <inheritdoc cref="GetBlendedVec4(uint, uint, out Vector4)"/>
    public static bool GetBlendedVec4(ImU8String instanceId, ImU8String channel, out Vector4 value)
    {
        return GetBlendedVec4(instanceId.GetId(), channel.GetId(), out value);
    }

    /// <summary>
    /// Get blended int value.
    /// </summary>
    public static bool GetBlendedInt(uint instanceId, uint channel, out int value)
    {
        fixed (int* outVal = &value)
            return ImAnimNative.GetBlendedInt(instanceId, channel, outVal) == 1;
    }

    /// <inheritdoc cref="GetBlendedInt(uint, uint, out int)"/>
    public static bool GetBlendedInt(ImU8String instanceId, ImU8String channel, out int value)
    {
        return GetBlendedInt(instanceId.GetId(), channel.GetId(), out value);
    }

    // Persistence (optional)

    public static void SetConfigDirectory(ImU8String path)
    {
        fixed (byte* pPath = &path.GetPinnableNullTerminatedReference())
            ImAnimNative.SetConfigDirectory(pPath);
        path.Recycle();
    }

    public static ImAnimResult ClipSave(uint clipId, ImU8String path)
    {
        ImAnimResult ret = default;
        fixed (byte* pathPtr = &path.GetPinnableNullTerminatedReference())
            ret = ImAnimNative.ClipSave(clipId, pathPtr);
        path.Recycle();
        return ret;
    }

    public static ImAnimResult ClipSave(ImU8String clipId, ImU8String path)
    {
        return ClipSave(clipId.GetId(), path);
    }

    public static ImAnimResult ClipLoad(ImU8String path, out uint clipId)
    {
        ImAnimResult ret = default;
        fixed (byte* pathPtr = &path.GetPinnableNullTerminatedReference())
        fixed (uint* outClipId = &clipId)
            ret = ImAnimNative.ClipLoad(pathPtr, outClipId);
        path.Recycle();
        return ret;
    }
}
