namespace ImAnimSharp;

public enum ImAnimWaveType
{
    /// <summary>
    /// Smooth sine wave. Use case: Most natural motion.
    /// </summary>
    Sine,

    /// <summary>
    /// Triangle wave (linear up/down). Use case: Mechanical motion.
    /// </summary>
    Triangle,

    /// <summary>
    /// Sawtooth wave (linear up, instant reset). Use case: Sweeping effects.
    /// </summary>
    Sawtooth,

    /// <summary>
    /// Square wave (on/off pulse). Use case: Blinking, digital.
    /// </summary>
    Square,
}
