using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Float variation
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ImAnimVariationFloat
{
    public ImAnimVariationMode Mode;
    public float Amount;
    public float MinClamp;
    public float MaxClamp;
    public uint Seed;
    public delegate* unmanaged[Cdecl]<int, void*, float> Callback;
    public void* User;

    /// <summary>
    /// No variation
    /// </summary>
    public static ImAnimVariationFloat None => new()
    {
        Mode = ImAnimVariationMode.None,
        Amount = 0,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Increment by amt
    /// </summary>
    public static ImAnimVariationFloat Inc(float amt) => new()
    {
        Mode = ImAnimVariationMode.Increment,
        Amount = amt,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Decrement by amt
    /// </summary>
    public static ImAnimVariationFloat Dec(float amt) => new()
    {
        Mode = ImAnimVariationMode.Decrement,
        Amount = amt,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Multiply by f
    /// </summary>
    public static ImAnimVariationFloat Mul(float f) => new()
    {
        Mode = ImAnimVariationMode.Multiply,
        Amount = f,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Random [-r, +r]
    /// </summary>
    public static ImAnimVariationFloat Rand(float r) => new()
    {
        Mode = ImAnimVariationMode.Random,
        Amount = r,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Random [0, r]
    /// </summary>
    public static ImAnimVariationFloat RandAbs(float r) => new()
    {
        Mode = ImAnimVariationMode.RandomAbs,
        Amount = r,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Alternate +/-
    /// </summary>
    public static ImAnimVariationFloat PingPong(float amt) => new()
    {
        Mode = ImAnimVariationMode.PingPong,
        Amount = amt,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = null,
        User = null
    };

    /// <summary>
    /// Custom callback
    /// </summary>
    public static ImAnimVariationFloat Fn(ImAnim.VariationFloatFn fn) => new()
    {
        Mode = ImAnimVariationMode.Callback,
        Amount = 0,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = (delegate* unmanaged[Cdecl]<int, void*, float>)Marshal.GetFunctionPointerForDelegate(fn),
        User = null
    };

    /// <summary>
    /// Custom callback
    /// </summary>
    public static ImAnimVariationFloat Fn(ImAnim.VariationFloatFn fn, void* userData = null) => new()
    {
        Mode = ImAnimVariationMode.Callback,
        Amount = 0,
        MinClamp = float.MinValue,
        MaxClamp = float.MaxValue,
        Seed = 0,
        Callback = (delegate* unmanaged[Cdecl]<int, void*, float>)Marshal.GetFunctionPointerForDelegate(fn),
        User = userData
    };

    /// <summary>
    /// Add clamp range
    /// </summary>
    public ImAnimVariationFloat Clamp(float min, float max)
    {
        MinClamp = min;
        MaxClamp = max;
        return this;
    }
}
