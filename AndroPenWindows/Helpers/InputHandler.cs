using System.Runtime.InteropServices;
using AndroPen.Data;

namespace AndroPen.Helpers;

public class InputHandler
{
    internal delegate void PenInputEventHandler( Point loc, float inPressure, float outPressure );
    internal event PenInputEventHandler? PenInput;

    protected IntPtr _pen;
    protected IntPtr _touch;

    private long _lastStamp = 0;

    // Constructor to initialize touch injection
    public InputHandler()
    {
        this._pen = Win32.CreateSyntheticPointerDevice( PointerInputType.Pen, 1, PointerFeedbackMode.On );
        this._touch = Win32.CreateSyntheticPointerDevice( PointerInputType.Touch, 10, PointerFeedbackMode.Pres );
    }

    // Simulate a single touch point
    public void SimulateTouch( RemotePointerInfo[] rpis ) //Vector2 point, bool isDown )
    {
        PointerTypeInfo[] outData = new PointerTypeInfo[ rpis.Length ];
        for( int i = 0; i < rpis.Length; i++ )// (RemotePointerInfo rp in rpis)
        {
            PointerFlags pointerFlags = AssembleFlags(rpis[i]);

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
                    }
                }
            };
        }

        SafelyInjectSyntheticPointerInput( this._touch, outData );
    }

    // Simulate a pen input with pressure and tilt
    public void SimulatePen( RemotePointerInfo rpi )
    {
        // Prepare the flags
        PointerFlags pFlags = AssembleFlags(rpi);

        // Prepare the pen mask
        PenMask pMask = PenMask.TiltX | PenMask.TiltY;
        if( rpi.Pressure > 0 )
            pMask |= PenMask.Pressure;

        Point point = rpi.Translate(ScreenUtils.GetNamedBounds(Settings.ScreenDevice));
        float outPressure = GetPressureValue( rpi.Pressure );

        // Craft the pointer type info
        PointerTypeInfo pti = new()
        {
            type = PointerInputType.Pen, // Use PT_PEN for pen input
            penInfo = new()
            {
                pointerInfo = new()
                {
                    PointerType = PointerInputType.Pen,
                    pointerId = (uint)rpi.PointerId,    // Unique identifier for the pointer.
                    pointerFlags = pFlags,              // These flags describe the event.
                    ptPixelLocation = point,            // The coordinates of the event
                },
                penFlags = TouchFlags.None,               // Optional flag for pen-related properties
                penMask = pMask ,                       // Indicate that the pressure and tilt information is included
                pressure = (uint)(1024f * outPressure), //(uint)(rpi.Pressure * 1024),
                tiltX = (int)rpi.Tilt.X,                // Optional, if neededS
                tiltY = (int)rpi.Tilt.Y                 // Optional, if needed
            }
        };

        /*
         * It appears that it's possible to break InjectSyntheticPointerInput if you send events
         * with a delta time of 0 so we need to track the delta time and, if the current delta is
         * 0, sleep for a single millisecond.
         */
        SafelyInjectSyntheticPointerInput( this._pen, [pti] );
        this.PenInput?.Invoke( new((int)rpi.PixelPosition.X, (int)rpi.PixelPosition.Y), rpi.Pressure, outPressure );
    }

    private PointerFlags AssembleFlags( RemotePointerInfo rpi )
    {
        PointerFlags pointerFlags = PointerFlags.None;
        if( rpi.EvType is AndroidEventType.Down or AndroidEventType.PointerDown )
        {
            pointerFlags = PointerFlags.Down | PointerFlags.InRange | PointerFlags.InContact;

            // If the pointer is down and not the original, mark it as new.
            if( rpi.PointerId > 0 )
                pointerFlags |= PointerFlags.New;
        }
        else if( rpi.EvType is AndroidEventType.Move )
            pointerFlags = PointerFlags.Update | PointerFlags.InRange | PointerFlags.InContact;

        // On Hover enter it is in range but not down or in contact
        else if( rpi.EvType is AndroidEventType.HoverEnter )
        {
            pointerFlags = PointerFlags.InRange;
        }

        // Update the position of the cursor 
        else if( rpi.EvType is AndroidEventType.HoverMove )
        {
            pointerFlags |= PointerFlags.Update | PointerFlags.InRange;
        }
        else
        {
            pointerFlags = PointerFlags.Up;
        }

        return pointerFlags;
    }

    private void SafelyInjectSyntheticPointerInput( nint device, PointerTypeInfo[] outData )
    {
        /*
         * It appears that it's possible to break InjectSyntheticPointerInput if you send events
         * with a delta time of 0 so we need to track the delta time and, if the current delta is
         * 0, sleep for a single millisecond.
         */
        long stamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();    // Get the current stamp
        long delta = stamp - this._lastStamp;                           // Get the delta
        this._lastStamp = stamp;                                        // Save the current time stamp
        if( delta == 0 )
            Thread.Sleep( 1 );

        if( !Win32.InjectSyntheticPointerInput( device, outData, (uint)outData.Length ) )
            Logging.Error( "Failed to inject touch input: " + Marshal.GetLastWin32Error() );
    }

    private static float GetPressureValue( float InputPressure )
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

    /// <summary>
    /// Assembles the <see cref="INPUT"/> as a key down event and sends it to the Windows API.
    /// </summary>
    /// <param name="virtualKeyCode"></param>
    public static void SimulateKeyDown( ushort virtualKeyCode )
    {
        INPUT down = new()
        {
            Type = Win32.INPUT_KEYBOARD,
            Data = new()
            {
                Keyboard = new()
                {
                    KeyCode = virtualKeyCode,
                    Scan = 0,
                    Flags = 0,
                    ExtraInfo = IntPtr.Zero
                }
            }
        };

        INPUT[] inputs = { down };
        if( Win32.SendInput( (uint)inputs.Length, inputs, Marshal.SizeOf( typeof( INPUT ) ) ) == 0 )
        {
            //throw new Exception( "Failed to send input." );
            Logging.Error( $"Failed to send input. Err: ${Marshal.GetHRForLastWin32Error()}" );
        }
    }

    /// <summary>
    /// Assembles the <see cref="INPUT"/> as a key up event and sends it to the Windows API.
    /// </summary>
    /// <param name="virtualKeyCode"></param>
    public static void SimulateKeyUp( ushort virtualKeyCode )
    {
        INPUT up = new()
        {
            Type = Win32.INPUT_KEYBOARD,
            Data = new()
            {
                Keyboard = new()
                {
                    KeyCode = virtualKeyCode,
                    Scan = 0,
                    Flags = Win32.KEYEVENTF_KEYUP,
                    ExtraInfo = IntPtr.Zero
                }
            }
        };

        INPUT[] inputs = { up };
        if( Win32.SendInput( (uint)inputs.Length, inputs, Marshal.SizeOf( typeof( INPUT ) ) ) == 0 )
        {
            //throw new Exception( "Failed to send input." );
            Logging.Error( $"Failed to send input. Err: ${Marshal.GetHRForLastWin32Error()}" );
        }
    }
}
