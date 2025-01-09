using System.ComponentModel;
using AndroPen.Data;
using AndroPen.Helpers;

namespace AndroPen.Controls;

public class PressureCurve : Control
{
    public event EventHandler? PressureCurveChanged;

    private const int GRID_SIZE = 4;
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
    /// The distance in percentage from the center that the pin will snap to.
    /// </summary>
    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    public float GridSnapRange { get; set; } = 0.05f;


    /// <summary>
    /// Default constructor.
    /// </summary>
    public PressureCurve()
    {
        this.DoubleBuffered = true; // Prevent flickering
        SetStyle( ControlStyles.ResizeRedraw, true );
    }

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
        using Pen pen = new( this.CurveColor, 2 );

        PointF[] points = GetPoints();

        // Straight line if Point2 is at the exact center
        if( Math.Abs( points[1].X - 0.5f ) < 0.001f && Math.Abs( points[1].Y - 0.5f ) < 0.001f )
        {
            g.DrawLine( pen, points[0], points[2] );
        }
        else
        {
            // Approximate a quadratic Bézier curve manually
            const int segments = 100; // Number of segments for smoothness
            PointF[] curvePoints = new PointF[segments + 1];

            for( int i = 0; i <= segments; i++ )
            {
                float t = i / (float)segments;
                curvePoints[i] = QuadraticBezierInterpolation( points[0], points[1], points[2], t );
            }

            g.DrawLines( pen, curvePoints );
        }
    }

    private PointF[] GetPoints()
    {
        PointF p1 = PointToScreenCoords( new()
        {
            X = Settings.ActivationThreshold,
            Y = Settings.InitialValue
        }); // Start point

        PointF p2 = PointToScreenCoords( new()
        {
            X = Settings.Softness.X,
            Y = Settings.Softness.Y
        }); // Control point

        PointF p3 = PointToScreenCoords( new()
        {
            X = Settings.MaxEffectiveInput,
            Y = Settings.MaxOutput
        } ); // End point
        return [ p1, p2, p3 ];
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
        PointF[] points = GetPoints();
        using SolidBrush brush = new( this.NodeColor );
        DrawPoint( g, points[0], brush );
        DrawPoint( g, points[1], brush );
        DrawPoint( g, points[2], brush );
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

        PointF[] points = GetPoints();
        PointF mousePoint = new(e.X, e.Y);

        if( IsPointHit( points[0], mousePoint ) )
            this.draggedPoint = 1;
        else if( IsPointHit( points[1], mousePoint ) )
            this.draggedPoint = 2;
        else if( IsPointHit( points[2], mousePoint ) )
            this.draggedPoint = 3;
        else
            this.draggedPoint = -1;

        Invalidate(); // Redraw to show any changes
    }

    protected override void OnMouseDoubleClick( MouseEventArgs e )
    {
        base.OnMouseDoubleClick( e );

        PointF[] points = GetPoints();
        PointF mousePoint = new(e.X, e.Y);

        if( IsPointHit( points[0], mousePoint ) )
        {
            Settings.ActivationThreshold = 0f;
            Settings.InitialValue = 0f;
        }

        else if( IsPointHit( points[1], mousePoint ) )
        {
            Settings.Softness = new() { X = 0.5f, Y = 0.5f };
        }

        else if( IsPointHit( points[2], mousePoint ) )
        {
            Settings.MaxEffectiveInput = 1f;
            Settings.MaxOutput = 1f;
        }
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
                mousePoint = ValidateStartPoint( mousePoint );
                Settings.ActivationThreshold = mousePoint.X;
                Settings.InitialValue = mousePoint.Y;
                break;
            case 2:
                mousePoint = ValidateCenterPoint( mousePoint );
                Settings.Softness = mousePoint;
                break;
            case 3:
                mousePoint = ValidateEndPoint( mousePoint );
                Settings.MaxEffectiveInput = mousePoint.X;
                Settings.MaxOutput = mousePoint.Y;
                break;
        }

        Invalidate(); // Redraw to show the updated positions
    }

    protected virtual PointF DoSnap( PointF point, PointF snap )
    {
        if(
            point.X < snap.X + this.GridSnapRange
            && point.X > snap.X - this.GridSnapRange
            && point.Y < snap.Y + this.GridSnapRange
            && point.Y > snap.Y - this.GridSnapRange )
            point = snap;
        return point;
    }

    protected virtual PointF ValidateStartPoint( PointF point )
    {
        // Never allow the curve to get backwards.
        if( point.X >= Settings.MaxEffectiveInput - 0.1f )
            point.X = Settings.MaxEffectiveInput - 0.1f;
        if( point.Y >= Settings.MaxOutput - 0.1f )
            point.Y = Settings.MaxOutput - 0.1f;

        // Run snapping and return the value.
        return DoSnap( point, new( 0, 0 ) );
    }

    protected virtual PointF ValidateCenterPoint( PointF point ) =>
        // Run snapping and return the value.
        DoSnap( point, new( 0.5f, 0.5f ) );

    protected virtual PointF ValidateEndPoint( PointF point )
    {
        // Never allow the curve to get backwards.
        if( point.X <= Settings.ActivationThreshold + 0.1f )
            point.X = Settings.ActivationThreshold + 0.1f;
        if( point.Y <= Settings.InitialValue + 0.1f )
            point.Y = Settings.InitialValue + 0.1f;

        return DoSnap( point, new(1f,1f));
    }

    protected override void OnMouseUp( MouseEventArgs e )
    {
        this.draggedPoint = -1; // Stop dragging
        this.PressureCurveChanged?.Invoke(this, EventArgs.Empty);
    }

    private static bool IsPointHit( PointF point, PointF mousePoint, float radius = 10f ) =>
        Math.Pow( mousePoint.X - point.X, 2 ) + Math.Pow( mousePoint.Y - point.Y, 2 ) <= Math.Pow( radius, 2 );
}
