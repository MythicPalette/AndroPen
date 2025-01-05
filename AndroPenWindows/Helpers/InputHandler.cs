using System.Runtime.InteropServices;
using AndroPen.Data;

namespace AndroPen.Helpers;

public class InputHandler
{
    internal delegate void PenInputEventHandler( Point loc, float inPressure, float outPressure );
    internal event PenInputEventHandler? PenInput;

    protected IntPtr _pen;
    protected IntPtr _touch;

    // Constructor to initialize touch injection
    public InputHandler()
    {
        this._pen = Win32.CreateSyntheticPointerDevice( PointerInputType.Pen, 1, PointerFeedbackMode.On );
        this._touch = Win32.CreateSyntheticPointerDevice( PointerInputType.Touch, 10, PointerFeedbackMode.Pres );
    }

    // Simulate a single touch point
    public void SimulateTouch( RemotePointerInfo[] rpis ) //Vector2 point, bool isDown )
    {
        PointerTypeInfoTouch[] outData = new PointerTypeInfoTouch[ rpis.Length ];
        for( int i = 0; i < rpis.Length; i++ )// (RemotePointerInfo rp in rpis)
        {

            PointerFlags pointerFlags = PointerFlags.None;

            if( rpis[i].EvType is RemoteEventType.Down or RemoteEventType.PointerDown )
                pointerFlags = PointerFlags.Down | PointerFlags.InRange | PointerFlags.InContact;

            else if( rpis[i].EvType is RemoteEventType.Move )
                pointerFlags = PointerFlags.Update | PointerFlags.InRange | PointerFlags.InContact;

            else
                pointerFlags = PointerFlags.Up;

            if( rpis[i].PointerId > 0 )
                pointerFlags |= PointerFlags.New;

            Point point = rpis[i].Translate( ScreenUtils.GetNamedBounds( Settings.ScreenDevice ));

            outData[i] = new()
            {
                type = PointerInputType.Touch,
                touchInfo = new PointerTouchInfo
                {
                    pointerInfo = new PointerInfo
                    {
                        PointerType = PointerInputType.Touch,
                        pointerId = (uint)rpis[i].PointerId, // Set pointerId to 0 for the first touch, 1 for the second touch, etc.
                        pointerFlags = pointerFlags,
                        ptPixelLocation = point, // Example position (x: 100 and 200)
                        dwTime = 0, // Can be set to 0 or the actual time
                        historyCount = 0, // Optional
                        sourceDevice = IntPtr.Zero, // Optional
                        hwndTarget = IntPtr.Zero // Optional
                    },
                    touchFlags = TouchFlags.None,
                    touchMask = TouchMask.ContactArea,
                    orientation = 0, // Example orientation
                    pressure = 32000, // Example pressure
                    rcContact = new()
                    {
                        X = point.X - 2,
                        Y = point.Y - 2,
                        Width = point.X + 2,
                        Height = point.Y + 2
                    } // Contact area (width = 50, height = 50)
                }
            };
        }

        if( !Win32.InjectSyntheticPointerInput( this._touch, outData, (uint)outData.Length ) )
            Logging.Error( "Failed to inject touch input: " + Marshal.GetLastWin32Error() );
    }

    // Simulate a pen input with pressure and tilt
    public void SimulatePen( RemotePointerInfo rpi )
    //Vector2 loc, uint pressure, bool isDown, uint orientation = 0, int tiltX = 0, int tiltY = 0 )
    {
        // Prepare the flags
        PointerFlags pFlags = PointerFlags.None;

        // If the pointer is down or moving, then it is in contact
        if( rpi.EvType is RemoteEventType.Down or RemoteEventType.Move )
            pFlags |= PointerFlags.Down | PointerFlags.InContact | PointerFlags.InRange;

        // On Hover enter or move, it is in range but not down or in contact
        else if( rpi.EvType is RemoteEventType.HoverEnter or RemoteEventType.HoverMove )
            pFlags |= PointerFlags.Up | PointerFlags.InRange;

        // This is when it leaves hover and/or stops touching.
        else
            pFlags = PointerFlags.Up;

        // Prepare the pen mask
        PenMask pMask = PenMask.TiltX | PenMask.TiltY;
        if( rpi.Pressure > 0 )
            pMask |= PenMask.Pressure;

        Point point = rpi.Translate(ScreenUtils.GetNamedBounds(Settings.ScreenDevice));
        float outPressure = GetPressureValue( rpi.Pressure );

        // Craft the pointer type info
        PointerTypeInfoPen pti = new()
        {
            type = PointerInputType.Pen, // Use PT_PEN for pen input
            penInfo = new()
            {
                pointerInfo = new()
                {
                    PointerType = PointerInputType.Pen,
                    pointerId = (uint)rpi.PointerId,
                    pointerFlags = pFlags,
                    ptPixelLocation = point,
                },
                penFlags = PenFlags.None,                   // Optional flag for pen-related properties
                penMask = pMask ,                           // Indicate that the pressure information is included
                pressure = (uint)(1024f * outPressure),    //(uint)(rpi.Pressure * 1024),
                tiltX = (int)rpi.Tilt.X,                     // Optional, if neededS
                tiltY = (int)rpi.Tilt.Y                      // Optional, if needed
            }
        };

        if( !Win32.InjectSyntheticPointerInput( this._pen, [pti], 1 ) )
        {
            int errorCode = Marshal.GetLastWin32Error();
            Logging.Error( $"Pen input failed. Last error code: {errorCode}" );
            return;
        }
        else
        {
            this.PenInput?.Invoke( pti.penInfo.pointerInfo.ptPixelLocation, rpi.Pressure, outPressure );
        }
    }

    public static float GetPressureValue( float InputPressure )
    {
        // If input is below PressureStart.X, return 0
        if( InputPressure < Settings.ActivationThreshold )
            return 0;

        // If input is above PressureEnd.X, return PressureEnd.Y
        if( InputPressure > Settings.MaxEffectiveInput )
            return Settings.MaxOutput;

        // Normalize the InputPressure to be between 0 and 1 based on the range [PressureStart.X, PressureEnd.X]
        float normalizedInput = (InputPressure - Settings.ActivationThreshold)
            / (Settings.MaxEffectiveInput - Settings.ActivationThreshold);

        // Apply the softness curve (here we use a quadratic easing function, but you can experiment with others)
        float smoothValue = ApplySoftnessCurve(normalizedInput, Settings.Softness.X, Settings.Softness.Y);

        // Interpolate between PressureStart.Y and PressureEnd.Y based on the smoothed input
        return Settings.InitialValue + smoothValue * ( Settings.MaxOutput - Settings.InitialValue );
    }

    // Function to apply softness curve, for example a quadratic easing function
    // Softness factor influences the curve. If softnessY > softnessX, the curve will be more pronounced.
    private static float ApplySoftnessCurve( float normalizedInput, float softnessX, float softnessY ) =>
        (float)Math.Pow( normalizedInput, softnessY );
}
