using System.Diagnostics.PerformanceData;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using AndroPen.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace AndroPen.Helpers;

public static class Win32
{
    // Constants for Touch Injection API
    internal const uint MAX_TOUCH_COUNT = 10;

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-createsyntheticpointerdevice">
    /// Win32 API Function</see>
    /// Configures the pointer injection device for the calling application, and initializes the maximum number
    /// of simultaneous pointers that the app can inject.
    /// </summary>
    /// <param name="pointerType">A <see cref="PointerInputType"/> indicating what type of pointer is used.</param>
    /// <param name="maxCount">The maximum number of pointer devices active at any time.</param>
    /// <param name="mode">The <see cref="PointerFeedbackMode"/> to give the user.</param>
    /// <returns><see langword="true"/> if successful</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern nint CreateSyntheticPointerDevice( PointerInputType pointerType, uint maxCount, PointerFeedbackMode mode );

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-injectsyntheticpointerinput">
    /// Win32 API Function</see>
    /// Simulates touch input.
    /// </summary>
    /// <param name="device">The device that sends the touch.</param>
    /// <param name="pti">An array of <see cref="PointerTypeInfoTouch"/> representing injected pointers.</param>
    /// <param name="count">The number of contacts</param>
    /// <returns><see langword="true"/> if successful</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern bool InjectSyntheticPointerInput( nint device, [In] PointerTypeInfoTouch[] pti, uint count );

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-injectsyntheticpointerinput">
    /// Win32 API Function</see>
    /// Simulates pen input.
    /// </summary>
    /// <param name="device">The device that sends the touch.</param>
    /// <param name="pti">An array of <see cref="PointerTypeInfoPen"/> representing injected pointers.
    /// This should only ever contain one element.</param>
    /// <param name="count">The number of contacts. This should always be 1</param>
    /// <returns><see langword="true"/> if successful</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern bool InjectSyntheticPointerInput( nint device, [In] PointerTypeInfoPen[] pti, uint count );


    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics">
    /// Win32 API Function</see>
    /// Retrieves the specified system metric or system configuration setting.
    /// Note that all dimensions retrieved by GetSystemMetrics are in pixels.
    /// </summary>
    /// <param name="nIndex">The system metric or configuration setting to be retrieved.
    /// This parameter can be one of the following values.
    /// Note that all SM_CX* values are widths and all SM_CY* values are heights.
    /// Also note that all settings designed to return Boolean data represent TRUE as any nonzero value,
    /// and FALSE as a zero value.</param>
    /// <returns>An <see cref="int"/> value with the requested data.</returns>
    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern int GetSystemMetrics( [In] int nIndex );

    /*
     * The following two constants are for GetSystemMetrics
     */
    internal const int SM_XVIRTUALSCREEN = 76;
    internal const int SM_YVIRTUALSCREEN = 77;

    internal static string Stringify( this PointerTypeInfoTouch[] pti )
    {
        string result = "[";

        for( int i = 0; i < pti.Length; i++ )
        {
            result +=  $@"
{{
    ""type"": {pti[i].type},
    ""penInfo"": {{
        ""pointerInfo"": { pti[i].touchInfo.pointerInfo.Stringify()},
        ""touchFlags"": { pti[i].touchInfo.touchFlags},
        ""touchMask"": { pti[i].touchInfo.touchMask},
        ""rcContact"": { pti[i].touchInfo.rcContact },
        ""pressure"": { pti[i].touchInfo.pressure},
     }}
}}";
        }
        return result + "]";
    }

    internal static string Stringify( this PointerTypeInfoPen pti )
    {
        string result = $@"
{{
    ""type"": {pti.type},
    ""penInfo"": {{
        ""pointerInfo"": {pti.penInfo.pointerInfo.Stringify()},
        ""penFlags"": {pti.penInfo.penFlags},
        ""penMask"": {pti.penInfo.penMask},
        ""pressure"": { pti.penInfo.pressure},
        ""rotation"": {pti.penInfo.rotation},
        ""tilt"": {{ ""X"": {pti.penInfo.tiltX}, ""Y"": {pti.penInfo.tiltY} }},
    }}
}}
";
        return result;
    }

    internal static string Stringify( this PointerInfo pti )
    {
        return $@"
{{
    ""PointerType"": ""{pti.PointerType}"",
    ""pointerId"": {pti.pointerId},
    ""frameId"": {pti.frameId},
    ""pointerFlags"": ""{pti.pointerFlags}"",
    ""sourceDevice"": {pti.sourceDevice},
    ""hwndTarget"": {pti.hwndTarget},
    ""ptPixelLocation"": {{ ""X"": {pti.ptPixelLocation.X}, ""Y"": {pti.ptPixelLocation.Y} }},
    ""dwTime"": {pti.dwTime},
    ""historyCount"": {pti.historyCount},
    ""inputData"": {pti.inputData},
    ""dwKeyStates"": {pti.dwKeyStates},
    ""PerformanceCount"": {pti.PerformanceCount},
    ""ButtonChangeType"": ""{pti.ButtonChangeType}""
}}";
    }
}