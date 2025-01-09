using System.ComponentModel;
using AndroPen.Helpers;

namespace AndroPen;

public partial class MainForm : Form
{
    private Color _headColor = Color.FromArgb(24, 24, 24);
    protected int HeaderHeight { get; set; } = 32;
    protected Rectangle HeaderBox => new( 0, 0, this.Width, this.HeaderHeight );

    private Color _buttonHoverColor = Color.FromArgb(64, 64,64);
    public Color ButtonHoverColor { get; set; }
    protected Rectangle CloseBox => new( this.Width - 48, 0, 48, 32 );

    private Point? _mouseOffset;

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
    }

    private void OnPenInput( Point loc, float inPressure, float outPressure )
    {
        if( this.InvokeRequired )
        {
            Invoke( new Action( () => OnPenInput( loc, inPressure, outPressure ) ) );
            return;
        }

        this.barInputPressure.Value = Math.Clamp(
                (int)( this.barInputPressure.Maximum * inPressure ),
                0,
                this.barInputPressure.Maximum
            );

        this.barOutputPressure.Value = Math.Clamp(
                (int)( this.barOutputPressure.Maximum * outPressure ),
                0,
                this.barOutputPressure.Maximum
            );

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
        e.Graphics.FillRectangle( headBrush, this.HeaderBox );

        if ( this.Icon != null )
            e.Graphics.DrawIcon( this.Icon, new( 4, 4, this.HeaderHeight - 8, this.HeaderHeight - 8 ) );

        // Draw the title
        Font f = new(this.Font.FontFamily, 16f, FontStyle.Regular);
        e.Graphics.DrawString( "AndroPen", f, Brushes.White, new Point( 28, 0 ) );

        // Draw the connection dot
        Brush connectionBrush = new SolidBrush( Program.socketManager.IsConnected ? Color.Green : Color.Red );
        e.Graphics.FillEllipse( connectionBrush, new( 120, 4, 8, 8 ) );

        // Draw the close button
        if( this.CloseBox.Contains( this.PointToClient(Cursor.Position) ) )
        {
            Brush closeBrush = new SolidBrush(this._buttonHoverColor);
            e.Graphics.FillRectangle( closeBrush, this.CloseBox );
        }

        int padding = (this.CloseBox.Width - this.HeaderHeight) / 2 ;
        Point p1 = new(
            this.CloseBox.X + padding*2,
            this.CloseBox.Y + padding );
        Point p2 = new(
            this.CloseBox.X + this.HeaderHeight,
            this.CloseBox.Y + this.HeaderHeight - padding );
        e.Graphics.DrawLine( new( this.ForeColor, 2f ), p1, p2 );

        Point p3 = new(
            this.CloseBox.X + padding*2,
            this.CloseBox.Y + this.HeaderHeight - padding );
        Point p4 = new(
            this.CloseBox.X + this.HeaderHeight,
            this.CloseBox.Y + padding);
        e.Graphics.DrawLine( new( this.ForeColor, 2f ), p3, p4 );
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
        if( e.Button == MouseButtons.Left )
        {
            if ( this.HeaderBox.Contains(e.Location) )
            {
                this._mouseOffset = e.Location;
                return;
            }
        }
        base.OnMouseDown( e );

    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
        if( this.HeaderBox.Contains( e.Location ) )
            Invalidate();
        if ( this._mouseOffset is null )
        {
            if( this.CloseBox.Contains( e.Location ) )
            base.OnMouseMove( e );
            return;
        }

        // Dragging the form
        Point p = PointToScreen(e.Location);
        p.X -= this._mouseOffset?.X ?? 0;
        p.Y -= this._mouseOffset?.Y ?? 0;

        this.Location = p;
    }

    protected override void OnMouseUp( MouseEventArgs e )
    {
        this._mouseOffset = null;
        base.OnMouseUp( e );
    }

    protected override void OnMouseClick( MouseEventArgs e )
    {
        base.OnMouseClick( e );
        if( this.CloseBox.Contains( e.Location ) )
            Hide();
    }
}
