using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using AndroPen.Data;

namespace AndroPen.Helpers;

public class InputHandler
{
    // Constants for Touch Injection API
    private const uint MAX_TOUCH_COUNT = 10;


    [DllImport( "user32.dll", SetLastError = true )]
    private static extern bool InitializeTouchInjection( uint maxCount, PointerFeedbackMode mode );

    [DllImport( "user32.dll", SetLastError = true )]
    private static extern bool InjectTouchInput( int count, [In] PointerTouchInfo[] contacts );

    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern IntPtr CreateSyntheticPointerDevice( PointerInputType pointerType, uint maxCount, PointerFeedbackMode mode);

    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern bool InjectSyntheticPointerInput( IntPtr device, [In] PointerTypeInfoTouch[] pti, UInt32 count );

    [DllImport( "user32.dll", SetLastError = true )]
    internal static extern bool InjectSyntheticPointerInput( IntPtr device, [In] PointerTypeInfoPen[] pti, UInt32 count );

    internal delegate void PenInputEventHandler( Vector2 loc, float inPressure, float outPressure );
    internal event PenInputEventHandler? PenInput;

    protected IntPtr _pen;
    protected IntPtr _touch;

    // Constructor to initialize touch injection
    public InputHandler()
    {
        //if (!InitializeTouchInjection( MAX_TOUCH_COUNT, TOUCH_FEEDBACK_DEFAULT ))
        //{
        //    throw new InvalidOperationException( "Failed to initialize touch injection." );
        //}
        if ( !InitializeTouchInjection( MAX_TOUCH_COUNT, PointerFeedbackMode.VIS_ON ))
            Logging.Error( $"Failed to initialize touch injection." );

        this._pen = CreateSyntheticPointerDevice( PointerInputType.PT_PEN, 1, PointerFeedbackMode.VIS_ON );
        this._touch = CreateSyntheticPointerDevice( PointerInputType.PT_TOUCH, 10, PointerFeedbackMode.VIS_PRES );
    }

    // Simulate a single touch point
    public void SimulateTouch( RemotePointerInfo[] rpis ) //Vector2 point, bool isDown )
    {
        //List<PointerTypeInfoTouch> touches = new();
        PointerTypeInfoTouch[] outData = new PointerTypeInfoTouch[ rpis.Length ];
        for (int i = 0; i < rpis.Length; i++)// (RemotePointerInfo rp in rpis)
        {

            PointerFlags pointerFlags = PointerFlags.NONE;

            if (rpis[i].EvType == RemoteEventType.DOWN
                || rpis[i].EvType == RemoteEventType.POINTER_DOWN)
                pointerFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;

            else if (rpis[i].EvType == RemoteEventType.MOVE)
                pointerFlags = PointerFlags.UPDATE | PointerFlags.INRANGE | PointerFlags.INCONTACT;

            else
                pointerFlags = PointerFlags.UP;

            if (rpis[i].PointerId > 0)
                pointerFlags |= PointerFlags.NEW;

            Vector2 point = rpis[i].Translate(ScreenUtils.GetNamedBounds(Settings.ScreenDevice)); //new() { X = 1920, Y = 1080}, new() {X=1920, Y=0});

            outData[i] = new() {
                type = PointerInputType.PT_TOUCH,
                touchInfo = new PointerTouchInfo
                {
                    pointerInfo = new PointerInfo
                    {
                        pointerType = PointerInputType.PT_TOUCH,
                        pointerId = (uint)rpis[i].PointerId, // Set pointerId to 0 for the first touch, 1 for the second touch, etc.
                        pointerFlags = pointerFlags,
                        ptPixelLocation = point, // Example position (x: 100 and 200)
                        dwTime = 0, // Can be set to 0 or the actual time
                        historyCount = 0, // Optional
                        sourceDevice = IntPtr.Zero, // Optional
                        hwndTarget = IntPtr.Zero // Optional
                    },
                    touchFlags = TouchFlags.NONE,
                    touchMask = TouchMask.CONTACTAREA,
                    orientation = 0, // Example orientation
                    pressure = 32000, // Example pressure
                    rcContact = new Rect
                    {
                        Left = point.X - 2,
                        Top = point.Y - 2,
                        Right = point.X + 2,
                        Bottom = point.Y + 2
                    } // Contact area (width = 50, height = 50)
                }
            };
        }

        if (!InjectSyntheticPointerInput( this._touch, outData, (uint)outData.Length ))
            Logging.Error( "Failed to inject touch input: " + Marshal.GetLastWin32Error() );
    }

    // Simulate a pen input with pressure and tilt
    public void SimulatePen( RemotePointerInfo rpi )
    //Vector2 loc, uint pressure, bool isDown, uint orientation = 0, int tiltX = 0, int tiltY = 0 )
    {
        // Prepare the flags
        PointerFlags pFlags = PointerFlags.NONE;

        // If the pointer is down or moving, then it is in contact
        if (rpi.EvType == RemoteEventType.DOWN
            || rpi.EvType == RemoteEventType.MOVE)
            pFlags |= PointerFlags.DOWN | PointerFlags.INCONTACT | PointerFlags.INRANGE;

        // On Hover enter or move, it is in range but not down or in contact
        else if (rpi.EvType == RemoteEventType.HOVER_ENTER
            || rpi.EvType == RemoteEventType.HOVER_MOVE)
            pFlags |= PointerFlags.UP | PointerFlags.INRANGE;

        // This is when it leaves hover and/or stops touching.
        else pFlags = PointerFlags.UP;

        // Prepare the pen mask
        PenMask pMask = PenMask.TILT_X | PenMask.TILT_Y;
        if (rpi.Pressure > 0)
            pMask |= PenMask.PRESSURE;

        Vector2 point = rpi.Translate(ScreenUtils.GetNamedBounds(Settings.ScreenDevice));
        float outPressure = GetPressureValue( rpi.Pressure );

        // Craft the pointer type info
        var pti = new PointerTypeInfoPen
        {
            type = PointerInputType.PT_PEN, // Use PT_PEN for pen input
            penInfo = new PointerPenInfo
            {
                pointerInfo = new PointerInfo
                {
                    pointerType = PointerInputType.PT_PEN,
                    pointerId = (uint)rpi.PointerId,
                    pointerFlags = pFlags,
                    ptPixelLocation = point,
                },
                penFlags = PenFlags.NONE,                   // Optional flag for pen-related properties
                penMask = pMask ,                           // Indicate that the pressure information is included
                pressure = (uint)(1024f * outPressure),    //(uint)(rpi.Pressure * 1024),
                tiltX = (int)rpi.TiltX,                     // Optional, if neededS
                tiltY = (int)rpi.TiltY                      // Optional, if needed
            }
        };

        if (!InjectSyntheticPointerInput( this._pen, [pti], 1 ))
        {
            int errorCode = Marshal.GetLastWin32Error();
            Logging.Error( $"Pen input failed. Last error code: {errorCode}" );
            return;
        } else
        {
            this.PenInput?.Invoke( pti.penInfo.pointerInfo.ptPixelLocation, rpi.Pressure, outPressure );
        }
    }

    public static float GetPressureValue( float InputPressure )
    {
        PressureCurveData pcd = Settings.PressureCurve;

        // If input is below PressureStart.X, return 0
        if (InputPressure < pcd.Threshold.X)
            return 0;

        // If input is above PressureEnd.X, return PressureEnd.Y
        if (InputPressure > pcd.Cap.X)
            return pcd.Cap.Y;

        // Normalize the InputPressure to be between 0 and 1 based on the range [PressureStart.X, PressureEnd.X]
        float normalizedInput = (InputPressure - pcd.Threshold.X) / (pcd.Cap.X - pcd.Threshold.X);

        // Apply the softness curve (here we use a quadratic easing function, but you can experiment with others)
        float smoothValue = ApplySoftnessCurve(normalizedInput, pcd.Softness.X, pcd.Softness.Y);

        // Interpolate between PressureStart.Y and PressureEnd.Y based on the smoothed input
        return pcd.Threshold.Y + smoothValue * (pcd.Cap.Y - pcd.Threshold.Y);
    }

    // Function to apply softness curve, for example a quadratic easing function
    private static float ApplySoftnessCurve( float normalizedInput, float softnessX, float softnessY )
    {
        // Softness factor influences the curve. If softnessY > softnessX, the curve will be more pronounced.
        return (float)Math.Pow( normalizedInput, softnessY );
    }
}
