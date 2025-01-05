using System.Runtime.InteropServices;

namespace AndroPen.Helpers;
internal static class ScreenUtils
{
    [DllImport( "user32.dll", SetLastError = true )]
    private static extern int GetSystemMetrics( [In] int nIndex );

    // Get the origin of the virtual screen.
    private const int SM_XVIRTUALSCREEN = 76;
    private const int SM_YVIRTUALSCREEN = 77;

    // Get the origin position as the virtual screen.
    internal static Point VirtualScreenOrigin => new()
    {
        X = GetSystemMetrics( SM_XVIRTUALSCREEN ),
        Y = GetSystemMetrics( SM_YVIRTUALSCREEN ),
    };

    // Extension method for the Screen class to translate
    // the coordinates to be virtual screen relative.
    internal static Rectangle Translate(this Rectangle rect )
    {
        return new()
        {
            X = rect.X - VirtualScreenOrigin.X,
            Y = rect.Y - VirtualScreenOrigin.Y,
            Width = rect.Width,
            Height = rect.Height
        };
    }

    internal static Screen GetNamedScreen( string screenName )
    {
        foreach (Screen screen in Screen.AllScreens)
        {
            if ( screen.DeviceName.Equals( screenName ) )
                return screen;
        }
        return Screen.PrimaryScreen!;
    }

    internal static Rectangle GetNamedBounds(string screenName) => GetNamedScreen( screenName ).Bounds.Translate();
}
