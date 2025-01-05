using System;
using System.Runtime.InteropServices;

namespace AndroPen.Data;

// Basic PointerInfo struct
[StructLayout( LayoutKind.Sequential )]
internal struct PointerInfo
{
    public PointerInputType pointerType;
    public uint pointerId;
    public uint frameId;
    public PointerFlags pointerFlags;
    public nint sourceDevice;
    public nint hwndTarget;
    public Vector2 ptPixelLocation;
    public Vector2 ptHimetricLocation;
    public Vector2 ptPixelLocationRaw;
    public Vector2 ptHimetricLocationRaw;
    public uint dwTime;
    public uint historyCount;
    public int inputData;
    public uint dwKeyStates;
    public ulong PerformanceCount;
    public PointerButtonChangeType ButtonChangeType;
}

// Pointer button info.
internal enum PointerButtonChangeType : uint
{
    NONE,
    FIRSTBUTTON_DOWN,
    FIRSTBUTTON_UP,
}

// This tells the computer what type of touch
// event is being done.
internal enum PointerInputType : uint
{
    PT_POINTER = 1,
    PT_TOUCH = 2,
    PT_PEN = 3,
    PT_MOUSE = 4,
    PT_TOUCHPAD = 5
}

// The basic flags for telling the system what information
// is valid in the struct. This is so it only uses
// available data and ignores everything else.
[Flags]
internal enum PointerFlags : uint
{
    NONE = 0x00000000,
    NEW = 0x00000001,
    INRANGE = 0x00000002,
    INCONTACT = 0x00000004,
    FIRSTBUTTON = 0x00000010,
    SECONDBUTTON = 0x00000020,
    THIRDBUTTON = 0x00000040,
    FOURTHBUTTON = 0x00000080,
    FIFTHBUTTON = 0x00000100,
    PRIMARY = 0x00002000,
    CONFIDENCE = 0x000004000,
    CANCELED = 0x000008000,
    DOWN = 0x00010000,
    UPDATE = 0x00020000,
    UP = 0x00040000,
    WHEEL = 0x00080000,
    HWHEEL = 0x00100000,
    CAPTURECHANGED = 0x00200000,
    HASTRANSFORMED = 0x00400000
}

// This should always be ON or PRES.
internal enum PointerFeedbackMode : uint
{
    VIS_OFF = 0x0000,
    VIS_ON = 0x0001,
    VIS_PRES = 0x0002
}

/*
 * The following four structs are used to send simulated
 * touch data to the system 
 */
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTypeInfoTouch
{
    public PointerInputType type;
    public PointerTouchInfo touchInfo;
}
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTouchInfo
{
    public PointerInfo pointerInfo;
    public TouchFlags touchFlags;
    public TouchMask touchMask;
    public Rect rcContact;
    public Rect rcContactRaw;
    public uint orientation;
    public uint pressure;
}
[Flags]
internal enum TouchFlags : uint
{
    NONE = 0x00000000
}

[Flags]
internal enum TouchMask : uint
{
    NONE = 0x00000000,
    CONTACTAREA = 0x00000001,
    ORIENTATION = 0x00000002,
    PRESSURE = 0x00000004,
}

/*
 * The following four structs are used to send simulated
 * pen data to the system 
 */
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTypeInfoPen
{
    public PointerInputType type;
    public PointerPenInfo penInfo;
}
[StructLayout( LayoutKind.Sequential )]
internal struct PointerPenInfo
{
    public PointerInfo pointerInfo;
    public PenFlags penFlags;
    public PenMask penMask;
    public uint pressure;
    public uint rotation;
    public int tiltX;
    public int tiltY;
}

[Flags]
public enum PenFlags : uint
{
    NONE = 0x0,
    BARREL = 0x1,
    INVERT = 0x2,
    ERASER = 0x4
}

[Flags]
public enum PenMask : uint
{
    NONE = 0x0,
    PRESSURE = 0x1,
    ROTATION = 0x2,
    TILT_X = 0x4,
    TILT_Y = 0x8,
}


// A simple 2 layer vector for passing
// two integers at a time.
[StructLayout( LayoutKind.Sequential )]
public struct Vector2
{
    public int X;
    public int Y;
}

// A basic Rect struct. This is used for rcContactArea in
// touch events.
[StructLayout( LayoutKind.Sequential )]
public struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}