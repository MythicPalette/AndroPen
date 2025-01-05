using System.ComponentModel;
using AndroPen.Helpers;

namespace AndroPen;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
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
}
