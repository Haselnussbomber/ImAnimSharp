using System.Runtime.InteropServices;

namespace ImAnimSharp;

/// <summary>
/// Spring parameters for physics-based animation
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ImAnimSpringParams
{
    public float Mass;
    public float Stiffness;
    public float Damping;
    public float InitialVelocity;
}
