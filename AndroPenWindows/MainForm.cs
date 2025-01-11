using System.ComponentModel;
using AndroPen.Helpers;

namespace AndroPen;

public partial class MainForm : Form
{
    /// <summary>
    /// This is the size of the X on the close button
    /// </summary>
    private const int CROSS_SIZE = 16;

    /// <summary>
    /// This is the height of the form header.
    /// </summary>
    private const int HEADER_HEIGHT = 32;

    /// <summary>
    /// This is the background color of the header section
    /// </summary>
    private Color _headColor = Color.FromArgb(16, 16, 16);

    /// <summary>
    /// This is the bounding box for the header
    /// </summary>
    protected Rectangle GetHeaderBox() => new( 0, 0, this.Width, HEADER_HEIGHT );

    /// <summary>
    /// This is the color of the close button when the mouse is hovering. We only need
    /// the hover color because the button has no color unless moused over.
    /// </summary>
    private Color _buttonHoverColor = Color.FromArgb(64, 64,64);

    /// <summary>
    /// This is the bounding box for close button.
    /// </summary>
    protected Rectangle GetCloseBox() => new( this.Width - 48, 0, 48, 32 );

    /// <summary>
    /// When not dragging, this will be null, serving the dual purpose of tracking the
    /// drag offset of the mouse and marking the drag state of the form.
    /// </summary>
    private Point? _mouseOffset;

    /// <summary>
    /// This just wraps a condition that is used several times while checking the drag state.
    /// </summary>
    private bool IsDragging() => this._mouseOffset is not null;

    public MainForm()
    {
        InitializeComponent();
        this.DoubleBuffered = true;
        Program.inputHandler.PenInput += OnPenInput;

        foreach( Screen s in Screen.AllScreens )
            _ = this.ScreenSelection.Items.Add( s.DeviceName );

        this.ScreenSelection.SelectedItem = Settings.ScreenDevice != string.Empty
            ? Settings.ScreenDevice
            : Screen.PrimaryScreen?.DeviceName;

        this.ScreenSelection.SelectedIndexChanged += ( s, e ) =>
        {
            Settings.ScreenDevice = this.ScreenSelection.Text;
            UpdateLabel();
        };
        UpdateLabel();

        this.PortInput.Value = Settings.Port;
        this.PortInput.ValueChanged += ( s, e ) => Settings.Port = (int)this.PortInput.Value;
        this.PortInput.KeyDown += ( s, e ) =>
        {
            if( e.KeyCode == Keys.Enter )
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        };
    }

    private void OnPenInput( Point loc, float inPressure, float outPressure )
    {
        if( this.InvokeRequired )
        {
            Invoke( new Action( () => OnPenInput( loc, inPressure, outPressure ) ) );
            return;
        }

        this.barInputPressure.Progress = inPressure;
        this.barOutputPressure.Progress = outPressure;

    }

    private void UpdateLabel()
    {
        Screen? screen = Settings.ScreenDevice.Length > 0
            ?  Screen.AllScreens.First(s => s.DeviceName == Settings.ScreenDevice)
            : Screen.PrimaryScreen;

        if( screen == null )
            return;

        Rectangle bounds = screen.Bounds.Translate();
        this.labelPrimary.Text = screen.Primary.ToString();
        this.labelOrigin.Text = $"<{bounds.X},{bounds.Y}>";
        this.labelResolution.Text = $"{bounds.Width}x{bounds.Height}";
    }

    protected override void OnClosing( CancelEventArgs e )
    {
        e.Cancel = true;
        Hide();
    }

    protected override void OnPaint( PaintEventArgs e )
    {
        // Paint the form
        e.Graphics.Clear( this.BackColor );

        // Draw the header bar
        Brush headBrush = new SolidBrush(_headColor);
        e.Graphics.FillRectangle( headBrush, this.GetHeaderBox() );
        e.Graphics.DrawLine( new( Color.Plum, 2 ), new( 0, HEADER_HEIGHT + 1 ), new( this.Width, HEADER_HEIGHT + 1 ) );

        if( this.Icon != null )
            e.Graphics.DrawIcon( this.Icon, new( 4, 4, HEADER_HEIGHT - 8, HEADER_HEIGHT - 8 ) );

        // Draw the title
        Font f = new(this.Font.FontFamily, 16f, FontStyle.Regular);
        e.Graphics.DrawString( "AndroPen", f, Brushes.White, new Point( 28, 0 ) );

        // Draw the connection dot
        Brush connectionBrush = new SolidBrush( Program.socketManager.IsConnected ? Color.Green : Color.Red );
        e.Graphics.FillEllipse( connectionBrush, new( 124, 4, 8, 8 ) );

        DrawCloseButton( e.Graphics );
    }

    protected void DrawCloseButton( Graphics g )
    {
        Rectangle closeBox = GetCloseBox();

        /*
         * The close button has a transparent background so only draw the rectangle for the close button
         * if the mouse is currently over it. This will give the highlighted effect.
         */
        if( closeBox.Contains( PointToClient( Cursor.Position ) ) )
        {
            Brush closeBrush = new SolidBrush(this._buttonHoverColor);
            g.FillRectangle( closeBrush, closeBox );
        }

        /*
         * To draw the X on the button accurately we need
         * the center of the button first.
         */
        int centerX = closeBox.Width / 2 + closeBox.X;
        int centerY = closeBox.Height / 2 + closeBox.Y;

        // halfSize is half the width of the X, required for calculating corners
        int halfSize = CROSS_SIZE / 2;

        // Calculate the four corners of the X
        int xStart = centerX - halfSize;
        int yStart = centerY - halfSize;
        int xEnd = centerX + halfSize;
        int yEnd = centerY + halfSize;

        // Using the four points we calculated, draw the lines
        Pen linePen = new( this.ForeColor, 2f );
        g.DrawLine( linePen, xStart, yStart, xEnd, yEnd );
        g.DrawLine( linePen, xStart, yEnd, xEnd, yStart );
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
        if( e.Button == MouseButtons.Left )
        {
            // With a left click, check if the mouse is inside the header
            if( this.GetHeaderBox().Contains( e.Location ) )
            {
                // With the mouse in the header, capture the offset from the form for dragging
                this._mouseOffset = e.Location;
                return;
            }
        }
        base.OnMouseDown( e );

    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
        /*
         * If the mouse is moving around inside the header and we're not currently dragging the form
         * redraw the form for button highlighting.
         */
        if( this.GetHeaderBox().Contains( e.Location ) && this.IsDragging() )
            Invalidate();

        // If not dragging then send the base event and exit.
        if( !this.IsDragging() )
        {
            base.OnMouseMove( e );
            return;
        }

        // Dragging the form so get the screen location of the mouse
        Point p = PointToScreen(e.Location);

        // Offset the location by the mouse location
        p.X -= this._mouseOffset?.X ?? 0;
        p.Y -= this._mouseOffset?.Y ?? 0;

        // Move the form to the offset location
        this.Location = p;
    }

    protected override void OnMouseUp( MouseEventArgs e )
    {
        // On mouse up, make _mouseOffset null to stop dragging events.
        this._mouseOffset = null;
        base.OnMouseUp( e );
    }

    protected override void OnMouseClick( MouseEventArgs e )
    {
        // Check if the close button is clicked
        if( GetCloseBox().Contains( e.Location ) )
        {
            Hide();
            return;
        }

        // If not clicking the close button then fire the base event.
        base.OnMouseClick( e );
    }

    protected override void OnMouseLeave( EventArgs e )
    {
        /*
         * Run the base event and invalidate the form to redraw. This is important
         * for when the mouse leaves the form while over the close button. Without
         * this, the close button will stay highlighted.
         */
        base.OnMouseLeave( e );
        Invalidate();
    }

    private void GroupboxPaint( object sender, PaintEventArgs e )
    {
        if ( sender is GroupBox gb )
        {
            using( Brush brush = new SolidBrush( this.ForeColor ) )
                e.Graphics.DrawString( gb.Text, gb.Font, brush, 8, 0 );
        }
    }
}
