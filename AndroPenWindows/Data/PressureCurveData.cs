namespace AndroPen.Data;

public struct PressureCurveData
{
    public PointF Threshold { get; private set; }
    public void SetThreshold( PointF value ) => this.Threshold = value;

    public PointF Softness { get; private set; }
    public void SetSoftness( PointF value ) => this.Softness = value;

    public PointF Maximum { get; private set; }
    public void SetCap( PointF value ) => this.Maximum = value;
    
    /// <summary>
    /// Default constructor
    /// </summary>
    public PressureCurveData()
    {
        this.Threshold = default;
        this.Softness = new(0.5f, 0.5f);
        this.Maximum = new(1f, 1f);
    }

    /// <summary>
    /// Constructor that takes the base information required.
    /// </summary>
    /// <param name="threshold"><see cref="PointF"/> with X as the activation threshold
    /// and Y as the initial pressure value</param>
    /// <param name="softness"><see cref="PointF"/> coordinate used in bezier
    /// curve calculation</param>
    /// <param name="max"><see cref="PointF"/> with X as maximum input and Y as maximum output</param>
    public PressureCurveData( PointF threshold, PointF softness, PointF max )
    {
        this.Threshold = threshold;
        this.Softness = softness;
        this.Maximum = max;
    }

    /// <summary>
    /// Turns the <see cref="PressureCurveData"/> into a <see cref="string"/>.
    /// </summary>
    /// <returns><see cref="string"/> with serialized PressureCurveData.</returns>
    public readonly string SerializeToString() =>
        $"{this.Threshold.X},{this.Threshold.Y};{this.Softness.X},{this.Softness.Y};{this.Maximum.X},{this.Maximum.Y}";

    /// <summary>
    /// Instantiates a <see cref="PressureCurveData"/> with <see cref="string"/> data.
    /// </summary>
    /// <param name="data"><see cref="string"/> data to deserialize</param>
    /// <returns><see cref="PressureCurveData"/> populated with the data from input.</returns>
    public static PressureCurveData Deserialize( string data )
    {
        string[] points = data.Split( ';' );
        if ( points.Length != 3 )
            return new();

        static PointF ParsePoint( string pdata )
        {
            string[] coords = pdata.Split(',');

            return coords.Length == 2
                ? new PointF( float.Parse( coords[0] ), float.Parse( coords[1] ) )
                : default;
        }

        PressureCurveData rtn = new(
            ParsePoint( points[0] ),
            ParsePoint( points[1] ),
            ParsePoint( points[2] )
            );

        return rtn;
    }
}
