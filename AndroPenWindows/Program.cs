using System.Runtime.InteropServices;
using AndroPen.Controls;
using AndroPen.Data;
using AndroPen.Helpers;

namespace AndroPen;

internal static class Program
{
    internal static InputHandler inputHandler = new InputHandler();
    internal static SocketManager socketManager = new();

    private static NotifyIcon? nIcon;

    internal static MainForm mainForm = new();

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        foreach (Screen s in Screen.AllScreens)
            Logging.Debug( $"Screen data dump: {s.DeviceName} :: {s.Primary} :: {s.Bounds}" );

        socketManager.Start();
        CreateIcon();
        Application.Run();
    }

    internal static void CreateIcon()
    {
        nIcon = new NotifyIcon()
        {
            Icon = new("./Resources/AndroTab.ico"),
            Text = "AndroPen",
            Visible = true
        };

        ContextMenuStrip cms = new();
        cms.Items.Add( "Show", null, ( s, e ) =>
        {
            mainForm.Show();
        } );
        cms.Items.Add( "Exit", null, ( s, e ) =>
        {
            nIcon.Visible = false;
            nIcon.Dispose();
            mainForm.Close();
            socketManager.Dispose();
            Application.Exit();
        } );

        nIcon.ContextMenuStrip = cms;
        nIcon.DoubleClick += ( s, e ) =>
        {
            mainForm.Show();
        };
    }

    internal static void SetCurve( PressureCurve pc, PressureCurveData data )
    {
        Settings.PressureCurve = data;
    }
}