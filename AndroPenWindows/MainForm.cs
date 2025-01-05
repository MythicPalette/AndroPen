using System.ComponentModel;
using AndroPen.Data;
using AndroPen.Helpers;

namespace AndroPen
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Program.inputHandler.PenInput += OnPenInput;
            this.pressureCuve1.PressureCurveChanged += ( s, e ) =>
            {
                Settings.PressureCurve = e;
            };

            foreach (Screen s in Screen.AllScreens)
                this.ScreenSelection.Items.Add( s.DeviceName );

            this.ScreenSelection.SelectedItem = Settings.ScreenDevice;
            //this.ScreenStats.Text = $"{screen?.Primary}\r\n{screen?.Bounds}\r\n{screen?.WorkingArea}";

            this.ScreenSelection.SelectedIndexChanged += ( s, e ) =>
            {
                Settings.ScreenDevice = this.ScreenSelection.Text;
                UpdateLabel();
            };
            UpdateLabel();
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            this.pressureCuve1.SetData( Settings.PressureCurve );
        }

        private void OnPenInput( Vector2 loc, float inPressure, float outPressure )
        {
            if (this.InvokeRequired)
                this.Invoke( new Action( () => OnPenInput( loc, inPressure, outPressure ) ) );
            else
            {
                this.barInputPressure.Value = Math.Clamp( 
                        (int)(barInputPressure.Maximum * inPressure),
                        0,
                        barInputPressure.Maximum 
                    );
                
                this.barOutputPressure.Value = Math.Clamp(
                        (int)(barOutputPressure.Maximum * outPressure),
                        0,
                        barOutputPressure.Maximum 
                    );
            }
        }

        private void UpdateLabel()
        {
            Screen screen = Screen.AllScreens.First(s => s.DeviceName == Settings.ScreenDevice);
            Rectangle bounds = screen.Bounds.Translate();
            this.labelPrimary.Text = screen.Primary.ToString();
            this.labelOrigin.Text = $"<{bounds.X},{bounds.Y}>";
            this.labelResolution.Text = $"{bounds.Width}x{bounds.Height}";
        }

        protected override void OnClosing( CancelEventArgs e )
        {
            //base.OnClosing( e );
            e.Cancel = true;
            this.Hide();
        }
    }
}
