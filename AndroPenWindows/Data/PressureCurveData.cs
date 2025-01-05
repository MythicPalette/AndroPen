using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroPen.Data;

public struct PressureCurveData
{
    public PointF Threshold { get; private set; }
    public void SetThreshold( PointF value ) => Threshold = value;

    public PointF Softness { get; private set; }
    public void SetSoftness( PointF value ) => Softness = value;

    public PointF Cap { get; private set; }
    public void SetCap( PointF value ) => Cap = value;
    

    public PressureCurveData()
    {
        Threshold = default;
        Softness = new(0.5f, 0.5f);
        Cap = new(1f, 1f);
    }
    public PressureCurveData( PointF threshold, PointF softness, PointF cap )
    {
        Threshold = threshold;
        Softness = softness;
        Cap = cap;
    }

    public string Serialize() => $"{Threshold.X},{Threshold.Y};{Softness.X},{Softness.Y};{Cap.X},{Cap.Y}";
    public static PressureCurveData Deserialize( string data )
    {
        var points = data.Split(';');
        if (points.Length != 3)
            //throw new FormatException( "Invalid data format for points." );
            return new();

        PressureCurveData rtn = new(
            ParsePoint( points[0] ),
            ParsePoint( points[1] ),
            ParsePoint( points[2] )
            );

        return rtn;
    }
    private static PointF ParsePoint( string pointData )
    {
        var coords = pointData.Split(',');
        if (coords.Length != 2)
            throw new FormatException( "Invalid point format." );

        return new PointF( float.Parse( coords[0] ), float.Parse( coords[1] ) );
    }
}
