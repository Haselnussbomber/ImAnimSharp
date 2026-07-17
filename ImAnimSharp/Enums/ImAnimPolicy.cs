namespace ImAnimSharp;

/// <summary>
/// Control how animations behave when the target changes mid-animation.
/// </summary>
public enum ImAnimPolicy
{
    /// <summary>
    /// Smoothly blend to new target from current position
    /// </summary>
    Crossfade,

    /// <summary>
    /// Snap immediately to new target
    /// </summary>
    Cut,

    /// <summary>
    /// Queue new target, play after current animation completes
    /// </summary>
    Queue,
}
