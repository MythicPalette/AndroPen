using System.ComponentModel;
using AndroPen.Data;

namespace AndroPen.Controls;

public class PressureCurve : Control
{
    public delegate void PressureCurveChangedEventHandler( PressureCurve sender, PressureCurveData e );
    public event PressureCurveChangedEventHandler? PressureCurveChanged;

    private PressureCurveData _data = new();

    private const int GRID_SIZE = 5;
    private int draggedPoint = -1; // -1: None, 1: Point1, 2: Point2, 3: Point3

    /// <summary>
    /// Defines the <see cref="Color"/> of the drag points on the graph.
    /// </summary>
    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color NodeColor { get; set; }

    /// <summary>
    /// Defines the <see cref="Color"/> of the curve line on the graph.
    /// </summary>
    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color CurveColor { get; set; }

    /// <summary>
    /// Defines the <see cref="Color"/> of the grid on the graph.
    /// </summary>
    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public Color GridColor { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public PressureCurve()
    {
        this.DoubleBuffered = true; // Prevent flickering
        SetStyle( ControlStyles.ResizeRedraw, true );
    }

    /// <summary>
    /// Sets the <see cref="PressureCurveData"/>. This is done through a setter
    /// to ensure that it is not accidentally tampered with.
    /// </summary>
    /// <param name="e"></param>
    public void SetData( PressureCurveData e ) => this._data = e;

    protected override void OnPaint( PaintEventArgs e )
    {
        base.OnPaint( e );
        Graphics g = e.Graphics;
        g.Clear( this.BackColor );

        DrawGrid( g );
        DrawCurve( g );
        DrawPoints( g );
    }

    private void DrawGrid( Graphics g )
    {
        float cellWidth = (float)this.Width / GRID_SIZE;
        float cellHeight = (float)this.Height / GRID_SIZE;

        using Pen pen = new( this.GridColor );
        for( int i = 1; i < GRID_SIZE; i++ )
        {
            // Draw vertical lines
            g.DrawLine( pen, i * cellWidth, 0, i * cellWidth, this.Height );
            // Draw horizontal lines
            g.DrawLine( pen, 0, i * cellHeight, this.Width, i * cellHeight );
        }
    }

    private void DrawCurve( Graphics g )
    {
        PointF p1 = PointToScreenCoords( this._data.Threshold ); // Start point
        PointF p2 = PointToScreenCoords( this._data.Softness ); // Control point
        PointF p3 = PointToScreenCoords( this._data.Maximum ); // End point

        using Pen pen = new( this.CurveColor, 2 );

        // Straight line if Point2 is at the exact center
        if( Math.Abs( this._data.Softness.X - 0.5f ) < 0.001f && Math.Abs( this._data.Softness.Y - 0.5f ) < 0.001f )
        {
            g.DrawLine( pen, p1, p3 );
        }
        else
        {
            // Approximate a quadratic Bézier curve manually
            const int segments = 100; // Number of segments for smoothness
            PointF[] curvePoints = new PointF[segments + 1];

            for( int i = 0; i <= segments; i++ )
            {
                float t = i / (float)segments;
                curvePoints[i] = QuadraticBezierInterpolation( p1, p2, p3, t );
            }

            g.DrawLines( pen, curvePoints );
        }
    }

    // Helper function for quadratic Bézier interpolation
    private static PointF QuadraticBezierInterpolation( PointF p1, PointF p2, PointF p3, float t )
    {
        float x = ( 1 - t ) * ( 1 - t ) * p1.X + 2 * ( 1 - t ) * t * p2.X + t * t * p3.X;
        float y = ( 1 - t ) * ( 1 - t ) * p1.Y + 2 * ( 1 - t ) * t * p2.Y + t * t * p3.Y;
        return new PointF( x, y );
    }

    private PointF PointToScreenCoords( PointF point ) =>
        new( point.X * this.Width, ( 1 - point.Y ) * this.Height ); // Flip Y-axis

    private PointF ScreenCoordsToPoint( PointF screenPoint ) =>
        new( screenPoint.X / this.Width, 1 - ( screenPoint.Y / this.Height ) ); // Flip Y-axis

    private void DrawPoints( Graphics g )
    {
        using SolidBrush brush = new( this.NodeColor );
        DrawPoint( g, PointToScreenCoords( this._data.Threshold ), brush );
        DrawPoint( g, PointToScreenCoords( this._data.Softness ), brush );
        DrawPoint( g, PointToScreenCoords( this._data.Maximum ), brush );
    }

    private static void DrawPoint( Graphics g, PointF p, Brush brush )
    {
        const int size = 8;
        g.FillEllipse( brush, p.X - size / 2, p.Y - size / 2, size, size );
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
        if( e.Button != MouseButtons.Left )
            return;

        PointF mousePoint = new(e.X, e.Y);

        if( IsPointHit( PointToScreenCoords( this._data.Threshold ), mousePoint ) )
            this.draggedPoint = 1;
        else if( IsPointHit( PointToScreenCoords( this._data.Softness ), mousePoint ) )
            this.draggedPoint = 2;
        else if( IsPointHit( PointToScreenCoords( this._data.Maximum ), mousePoint ) )
            this.draggedPoint = 3;
        else
            this.draggedPoint = -1;

        Invalidate(); // Redraw to show any changes
    }

    protected override void OnMouseDoubleClick( MouseEventArgs e )
    {
        base.OnMouseDoubleClick( e );
        PointF mousePoint = new(e.X, e.Y);

        if( IsPointHit( PointToScreenCoords( this._data.Threshold ), mousePoint ) )
            this._data.SetThreshold( new() { X = 0f, Y = 0f } );

        else if( IsPointHit( PointToScreenCoords( this._data.Softness ), mousePoint ) )
            this._data.SetSoftness( new() { X = 0.5f, Y = 0.5f } );

        else if( IsPointHit( PointToScreenCoords( this._data.Maximum ), mousePoint ) )
            this._data.SetCap( new() { X = 1f, Y = 1f } );
        Invalidate(); // Redraw to show the updated positions
    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
        if( this.draggedPoint == -1 || e.Button != MouseButtons.Left )
            return;

        PointF mousePoint = ScreenCoordsToPoint(new PointF(e.X, e.Y));
        mousePoint.X = Math.Clamp( mousePoint.X, 0f, 1f ); // Ensure within bounds
        mousePoint.Y = Math.Clamp( mousePoint.Y, 0f, 1f );

        switch( this.draggedPoint )
        {
            case 1:
                // Threshold can only ever move on the X Axis as it corresponds to
                // the 
                this._data.SetThreshold( mousePoint );
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
        this.draggedPoint = -1; // Stop dragging
        this.PressureCurveChanged?.Invoke( this, this._data );
    }

    private static bool IsPointHit( PointF point, PointF mousePoint, float radius = 10f ) =>
        Math.Pow( mousePoint.X - point.X, 2 ) + Math.Pow( mousePoint.Y - point.Y, 2 ) <= Math.Pow( radius, 2 );
}
