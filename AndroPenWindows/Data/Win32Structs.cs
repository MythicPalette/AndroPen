using System.Runtime.InteropServices;

namespace AndroPen.Data;


/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_info">
/// POINTER_INFO</see>
/// struct from the official Win32 API
/// </summary>
[StructLayout( LayoutKind.Sequential )]
internal struct PointerInfo
{
    public PointerInputType PointerType;
    public uint pointerId;
    public uint frameId;
    public PointerFlags pointerFlags;
    public nint sourceDevice;
    public nint hwndTarget;
    public Point ptPixelLocation;
    public Point ptHimetricLocation;
    public Point ptPixelLocationRaw;
    public Point ptHimetricLocationRaw;
    public uint dwTime;
    public uint historyCount;
    public int inputData;
    public uint dwKeyStates;
    public ulong PerformanceCount;
    public PointerButtonChangeType ButtonChangeType;
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ne-winuser-pointer_button_change_type">
/// POINTER_BUTTON_CHANGE_TYPE</see>
/// enumerable from the official Win32 API
/// </summary>
internal enum PointerButtonChangeType : uint
{
    None,
    FirstButtonDown,
    FirstButtonUp,
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ne-winuser-tagpointer_input_type">
/// POINTER_INPUT_TYPE</see>
///  enumerable from the official Win32 API
/// </summary>
internal enum PointerInputType : uint
{
    Pointer = 1,
    Touch = 2,
    Pen = 3,
    Mouse = 4,
    Touchpad = 5
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputmsg/pointer-flags-contants">Pointer Flags</see>
/// constants as enumberables from the official Win32 API
/// </summary>
internal enum PointerFlags : uint
{
    None = 0x00000000,
    New = 0x00000001,
    InRange = 0x00000002,
    InContact = 0x00000004,
    FirstButton = 0x00000010,
    SecondButton = 0x00000020,
    ThirdButton = 0x00000040,
    FourthButton = 0x00000080,
    FifthButton = 0x00000100,
    Primary = 0x00002000,
    Confidence = 0x000004000,
    Canceled = 0x000008000,
    Down = 0x00010000,
    Update = 0x00020000,
    Up = 0x00040000,
    Wheel = 0x00080000,
    HWheel = 0x00100000,
    CaptureChanged = 0x00200000,
    HasTransformed = 0x00400000
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ne-winuser-pointer_feedback_mode">
/// POINTER_FEEDBACK_MODE</see> enumerable from the official Win32 API
/// </summary>
internal enum PointerFeedbackMode : uint
{
    Off = 0x0000,
    On = 0x0001,
    Pres = 0x0002
}

#region POINTER_TYPE_INFO
/*
 * Because C# does not have the UNION function for structs like C does,
 * the two variations of POINTER_TYPE_INFO are instead declared as
 * two different structs.
 */
/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_type_info">POINTER_TYPE_INFO</see>
/// struct from the official Win32 API. This is the touch variation
/// </summary>
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTypeInfoTouch
{
    public PointerInputType type;
    public PointerTouchInfo touchInfo;
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_touch_info">POINTER_TOUCH_INFO</see>
/// struct from the official Win32 API.
/// </summary>
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTouchInfo
{
    public PointerInfo pointerInfo;
    public TouchFlags touchFlags;
    public TouchMask touchMask;
    public Rectangle rcContact;
    public Rectangle rcContactRaw;
    public uint orientation;
    public uint pressure;
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputmsg/touch-flags-constants">Touch Flags</see>
/// flags from the official Win32 API. There is only the one value.
/// </summary>
[Flags]
internal enum TouchFlags : uint
{
    None = 0x00000000
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputmsg/touch-mask-constants">Touch Flags</see>
/// flag mask from the official Win32 API. Used to indicate which properties of the <see cref="PointerTouchInfo"/>
/// are set and valid.
/// </summary>
[Flags]
internal enum TouchMask : uint
{
    None = 0x00000000,
    ContactArea = 0x00000001,
    Orientation = 0x00000002,
    Pressure = 0x00000004,
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_type_info">POINTER_TYPE_INFO</see>
/// struct from the official Win32 API. This is the pen variation
/// </summary>
[StructLayout( LayoutKind.Sequential )]
internal struct PointerTypeInfoPen
{
    public PointerInputType type;
    public PointerPenInfo penInfo;
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-pointer_pen_info">POINTER_PEN_INFO</see>
/// struct from the official Win32 API.
/// </summary>
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

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputmsg/touch-flags-constants">Pen Flags</see>
/// flags from the official Win32 API. Used to indicate pen states.
/// </summary>
[Flags]
public enum PenFlags : uint
{
    None = 0x0,     // No flags
    Barrel = 0x1,   // Barrel button is pressed
    Invert = 0x2,   // The pen is inverted
    Eraser = 0x4    // The eraser button is pressed
}

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/inputmsg/pen-mask-constants">Pen Mask</see>
/// flag mask from the official Win32 API. Used to indicate which properties of the <see cref="PointerPenInfo"/>
/// are set and valid.
/// </summary>
[Flags]
public enum PenMask : uint
{
    None = 0x0,
    Pressure = 0x1,
    Rotation = 0x2,
    TiltX = 0x4,
    TiltY = 0x8,
}
#endregion