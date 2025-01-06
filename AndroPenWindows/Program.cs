using System.Runtime.InteropServices;
using AndroPen.Controls;
using AndroPen.Data;
using AndroPen.Helpers;

namespace AndroPen;

internal static class Program
{
    private static NotifyIcon? nIcon;

    internal static InputHandler inputHandler = new InputHandler();
    internal static SocketManager socketManager = new();
    internal static MainForm mainForm = null!;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Logging.Init();
        socketManager.Start();
        CreateIcon();
        mainForm = new();

        Application.Run();

        Logging.Shutdown();
    }

    private static void CreateIcon()
    {
        nIcon = new NotifyIcon()
        {
            Icon = new("./Resources/AndroTab.ico"),
            Text = "AndroPen",
            Visible = true
        };

        ContextMenuStrip cms = new();
        _ = cms.Items.Add( "Show", null, ( s, e ) => mainForm.Show() );
        _ = cms.Items.Add( "Exit", null, ( s, e ) =>
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
}