using AndroPen.Helpers;

namespace AndroPen.Data;

public struct PressureCurveData
{
    // / <summary>
    // / Contains both the activation threshold and initial activation value.
    // / </summary>
    //public PointF Threshold { get; private set; }
    //public void SetThreshold( PointF value ) => this.Threshold = value;
    public float ActivationThreshold { get; private set; }
    public float InitialValue {  get; private set; }

    /// <summary>
    /// Defines the bend of the pressure curve.
    /// </summary>
    public PointF Softness { get; private set; }
    public void SetSoftness( PointF value ) => this.Softness = value;

    // / <summary>
    // / Contains both the maximum input requirements for maximum output and
    // / the total maximum output.
    // / </summary>
    //public PointF Maximum { get; private set; }
    //public void SetCap( PointF value ) => this.Maximum = value;
    public float MaxEffectiveInput { get; private set; }
    public float MaxOutput { get; private set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public PressureCurveData()
    {
        this.ActivationThreshold = Properties.Settings.Default.ActivationThreshold;
        this.InitialValue = Properties.Settings.Default.InitialValue;
        this.Softness = new(Properties.Settings.Default.SoftnessX, Properties.Settings.Default.SoftnessY);
        this.MaxEffectiveInput = Properties.Settings.Default.MaxEffectiveInput;
        this.MaxOutput = Properties.Settings.Default.MaxOutput;
    }

    /// <summary>
    /// Constructor that takes the base information required.
    /// </summary>
    /// <param name="threshold"><see cref="PointF"/> with X as the activation threshold
    /// and Y as the initial pressure value</param>
    /// <param name="softness"><see cref="PointF"/> coordinate used in bezier
    /// curve calculation</param>
    /// <param name="max"><see cref="PointF"/> with X as maximum input and Y as maximum output</param>
    public PressureCurveData( float threshold, float initial, PointF softness, float maxIn, float maxOut )
    {
        this.ActivationThreshold = threshold;
        this.InitialValue = initial;
        this.Softness = softness;
        this.MaxEffectiveInput = maxIn;
        this.MaxOutput = maxOut;
    }
}
