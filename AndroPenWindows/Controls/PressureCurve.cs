
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AndroPen.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AndroPen.Controls;

public class PressureCurve : Control
{
    public delegate void PressureCurveChangedEventHandler( PressureCurve sender, PressureCurveData e );
    public event PressureCurveChangedEventHandler? PressureCurveChanged;

    //private PointF point1 = new PointF(0.0f, 0.0f); // Activation threshold (bottom-left)
    //private PointF point2 = new PointF(0.5f, 0.5f); // Curve control (center)
    //private PointF point3 = new PointF(1.0f, 1.0f); // Max pressure (top-right)
    private PressureCurveData _data = new();

    private const int GridSize = 5;
    private int draggedPoint = -1; // -1: None, 1: Point1, 2: Point2, 3: Point3

    [Browsable(true)]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public string LabelY { get; set; }

    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public string LabelX { get; set; }

    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color NodeColor { get; set; }

    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color CurveColor { get; set; }

    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color GridColor { get; set; }

    public PressureCurve()
    {
        this.DoubleBuffered = true; // Prevent flickering
        this.SetStyle( ControlStyles.ResizeRedraw, true );
    }

    public void SetData(PressureCurveData e) => this._data = e;

    protected override void OnPaint( PaintEventArgs e )
    {
        base.OnPaint( e );
        var g = e.Graphics;
        g.Clear( this.BackColor );

        DrawGrid( g );
        DrawCurve( g );
        DrawPoints( g );
    }

    private void DrawGrid( Graphics g )
    {
        var cellWidth = (float)Width / GridSize;
        var cellHeight = (float)Height / GridSize;

        using var pen = new Pen(this.GridColor);
        for (int i = 1; i < GridSize; i++)
        {
            // Draw vertical lines
            g.DrawLine( pen, i * cellWidth, 0, i * cellWidth, Height );
            // Draw horizontal lines
            g.DrawLine( pen, 0, i * cellHeight, Width, i * cellHeight );
        }
    }

    private void DrawCurve( Graphics g )
    {
        var p1 = PointToScreenCoords(_data.Threshold); // Start point
        var p2 = PointToScreenCoords(_data.Softness); // Control point
        var p3 = PointToScreenCoords(_data.Cap); // End point

        using var pen = new Pen(this.CurveColor, 2);

        // Straight line if Point2 is at the exact center
        if (Math.Abs( _data.Softness.X - 0.5f ) < 0.001f && Math.Abs( _data.Softness.Y - 0.5f ) < 0.001f)
        {
            g.DrawLine( pen, p1, p3 );
        }
        else
        {
            // Approximate a quadratic Bézier curve manually
            const int segments = 100; // Number of segments for smoothness
            PointF[] curvePoints = new PointF[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                curvePoints[i] = QuadraticBezierInterpolation( p1, p2, p3, t );
            }

            g.DrawLines( pen, curvePoints );
        }
    }

    // Helper function for quadratic Bézier interpolation
    private PointF QuadraticBezierInterpolation( PointF p1, PointF p2, PointF p3, float t )
    {
        float x = (1 - t) * (1 - t) * p1.X + 2 * (1 - t) * t * p2.X + t * t * p3.X;
        float y = (1 - t) * (1 - t) * p1.Y + 2 * (1 - t) * t * p2.Y + t * t * p3.Y;
        return new PointF( x, y );
    }

    private PointF PointToScreenCoords( PointF point )
    {
        return new PointF( point.X * Width, (1 - point.Y) * Height ); // Flip Y-axis
    }

    private PointF ScreenCoordsToPoint( PointF screenPoint )
    {
        return new PointF( screenPoint.X / Width, 1 - (screenPoint.Y / Height) ); // Flip Y-axis
    }

    private void DrawPoints( Graphics g )
    {
        using var brush = new SolidBrush(this.NodeColor);
        DrawPoint( g, PointToScreenCoords( _data.Threshold ), brush );
        DrawPoint( g, PointToScreenCoords( _data.Softness ), brush );
        DrawPoint( g, PointToScreenCoords( _data.Cap ), brush );
    }

    private void DrawPoint( Graphics g, PointF p, Brush brush )
    {
        const int size = 8;
        g.FillEllipse( brush, p.X - size / 2, p.Y - size / 2, size, size );
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
        if ( e.Button != MouseButtons.Left ) return;

        var mousePoint = new PointF(e.X, e.Y);

        if (IsPointHit( PointToScreenCoords( _data.Threshold ), mousePoint ))
            draggedPoint = 1;
        else if (IsPointHit( PointToScreenCoords( _data.Softness ), mousePoint ))
            draggedPoint = 2;
        else if (IsPointHit( PointToScreenCoords( _data.Cap ), mousePoint ))
            draggedPoint = 3;
        else
            draggedPoint = -1;

        Invalidate(); // Redraw to show any changes
    }

    protected override void OnMouseDoubleClick( MouseEventArgs e )
    {
        base.OnMouseDoubleClick( e );
        var mousePoint = new PointF(e.X, e.Y);

        if (IsPointHit( PointToScreenCoords( _data.Threshold ), mousePoint ))
            _data.SetThreshold( new() { X = 0f, Y = 0f } );

        else if (IsPointHit( PointToScreenCoords( _data.Softness ), mousePoint ))
            _data.SetSoftness( new() { X = 0.5f, Y = 0.5f } );

        else if (IsPointHit( PointToScreenCoords( _data.Cap ), mousePoint ))
            _data.SetCap( new() { X=1f, Y = 1f } );
        Invalidate(); // Redraw to show the updated positions
    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
        if (draggedPoint == -1 || e.Button != MouseButtons.Left)
            return;

        var mousePoint = ScreenCoordsToPoint(new PointF(e.X, e.Y));
        mousePoint.X = Math.Clamp( mousePoint.X, 0f, 1f ); // Ensure within bounds
        mousePoint.Y = Math.Clamp( mousePoint.Y, 0f, 1f );

        switch (draggedPoint)
        {
            case 1:
                // Threshold can only ever move on the X Axis as it corresponds to
                // the 
                this._data.SetThreshold(mousePoint);
                break;
            case 2:
                this._data.SetSoftness( mousePoint );
                break;
            case 3:
                this._data.SetCap( mousePoint );
                break;
        }

        Invalidate(); // Redraw to show the updated positions
    }

    protected override void OnMouseUp( MouseEventArgs e )
    {
        draggedPoint = -1; // Stop dragging
        this.PressureCurveChanged?.Invoke( this, this._data );
    }

    private bool IsPointHit( PointF point, PointF mousePoint, float radius = 10f )
    {
        return Math.Pow( mousePoint.X - point.X, 2 ) + Math.Pow( mousePoint.Y - point.Y, 2 ) <= Math.Pow( radius, 2 );
    }
}
