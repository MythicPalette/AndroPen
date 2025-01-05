using System.Runtime.InteropServices;

namespace AndroPen.Helpers;
internal static class ScreenUtils
{
    /// <summary>
    /// Get the origin position as the virtual screen.
    /// </summary>
    internal static Point VirtualScreenOrigin => new()
    {
        X = Win32.GetSystemMetrics( Win32.SM_XVIRTUALSCREEN ),
        Y = Win32.GetSystemMetrics( Win32.SM_YVIRTUALSCREEN ),
    };

    /// <summary>
    /// Extension method for the Rectangle struct to translate it from
    /// default to a point in the virtual screen.
    /// </summary>
    /// <param name="rect">The monitor bounds <see cref="Rectangle"/> as defined by Windows.</param>
    /// <returns>A new <see cref="Rectangle"/> with accurate X/Y coordinates for
    /// the virtual screen.</returns>
    internal static Rectangle Translate( this Rectangle rect )
    {
        return new()
        {
            X = rect.X - VirtualScreenOrigin.X,
            Y = rect.Y - VirtualScreenOrigin.Y,
            Width = rect.Width,
            Height = rect.Height
        };
    }

    /// <summary>
    /// Returns a Screen object by name.
    /// </summary>
    /// <param name="screenName">The <see cref="string"/> name of the display to get</param>
    /// <returns><see cref="Screen"/> with the matching name or the primary display if
    /// unable to locate the named screen.</returns>
    internal static Screen GetNamedScreen( string screenName )
    {
        foreach( Screen screen in Screen.AllScreens )
        {
            if( screen.DeviceName.Equals( screenName ) )
                return screen;
        }
        return Screen.PrimaryScreen!;
    }

    /// <summary>
    /// Returns the translated bounds of a screen given its name.
    /// </summary>
    /// <param name="screenName">The <see cref="string"/> name of the display to get the bounds of.</param>
    /// <returns></returns>
    internal static Rectangle GetNamedBounds( string screenName ) => GetNamedScreen( screenName ).Bounds.Translate();
}
