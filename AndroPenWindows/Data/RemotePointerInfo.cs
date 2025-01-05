namespace AndroPen.Data;

/// <summary>
/// Enumerable for determining the type of event being sent from
/// the Android device.
/// </summary>
public enum RemoteEventType
{
    Down,
    PointerDown,
    Move,
    Up,
    PointerUp,
    HoverEnter,
    HoverMove,
    HoverExit
}

/// <summary>
/// Enumerable for determining the type of pointer that created
/// the event on the Android Device
/// </summary>
public enum RemotePointerType
{
    Pen,
    Touch,
    Finger
}

/// <summary>
/// Converts binary data from a remote event on the Android device
/// into named variables for easy usage.
/// </summary>
public class RemotePointerInfo
{
    /// <summary>
    /// The total number of bytes that a RemotePointInfo takes when
    /// serialized into a <see cref="byte"/> array.
    /// </summary>
    public const int BYTE_LENGTH = 56;

    /// <summary>
    /// The <see cref="int"/> id of the pointer. This is used for
    /// identifying the pointer to the system and is vital for updating
    /// the position of pointers when a finger or pen moves across the screen.
    /// </summary>
    public int PointerId { get; set; }

    /// <summary>
    /// The type of event that was triggered on the remote device. 
    /// </summary>
    public RemoteEventType EvType { get; set; }

    /// <summary>
    /// The x/y coordinates of where on the device the event occured.
    /// </summary>
    public PointF PixelPosition { get; set; }

    public float Pressure { get; set; }
    public PointF Tilt { get; set; }
    public long TimeStamp { get; set; }
    public RemotePointerType PtrType { get; set; }
    public PointF Velocity { get; set; }

    public Size Size {  get; set; }
    public static RemotePointerInfo DeserializePointerInfo( byte[] data )
    {
        RemotePointerInfo pi = new();

        /*
         * Create an offset tracker for reading the bytes. This
         * needs to be updated after every BitConverter operation
         * to ensure that we are working with the correct bytes.
         */
        int idx = 0;

        // Get the pointer ID.
        pi.PointerId = BitConverter.ToInt32( data, idx );
        idx += sizeof( int );

        // Get the event type and cast it to RemoteEventType
        pi.EvType = (RemoteEventType)BitConverter.ToInt32( data, idx );
        idx += sizeof( int );

        // Get the X and Y values for the PixelPosition
        float x = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        float y = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );
        
        // Assign the X and Y values to the position
        pi.PixelPosition = new( x, y );

        // Get the pen pressure as a float
        pi.Pressure = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        // Get the X and Y values for the pen tilt
        float tX = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        float tY = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        // Assign the X and Y values to the tilt
        pi.Tilt = new( tX, tY );

        // Get the event timestamp
        pi.TimeStamp = BitConverter.ToInt64( data, idx );
        idx += sizeof( long );

        // Get the type of pointer that triggered the event
        pi.PtrType = (RemotePointerType)BitConverter.ToInt32( data, idx );
        idx += sizeof( int );

        // Get the X and Y velocity values of the movement.
        float vX = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        float vY = BitConverter.ToSingle( data, idx );
        idx += sizeof( float );

        // Assign the velocity values.
        pi.Velocity = new( vX, vY );

        /*
         * Get the width and height of the remote View that triggered
         * the event. This is important as we use the size of the
         * remote view to get the scale multiplier for the coordinates.
         * Without this, it's impossible to translate coordinates from
         * the remote system to this one.
         */
        int width = BitConverter.ToInt32( data, idx );
        idx += sizeof( float );

        int height = BitConverter.ToInt32( data, idx );
        idx += sizeof( float );

        // Assign the width and height as a new parameter.
        pi.Size = new( width, height );

        return pi;
    }

    public override string ToString()
    {
        return $"RemotePointerInfo:\n" +
               $"PointerId: {this.PointerId}\n" +
               $"EvType: {this.EvType}\n" +
               $"PixelPosition: <{this.PixelPosition.X}, {this.PixelPosition.Y}>\n" +
               $"Pressure: {this.Pressure}\n" +
               $"Tilt: <{this.Tilt.X}, {this.Tilt.Y}>\n" +
               $"TimeStamp: {this.TimeStamp}\n" +
               $"PtrType: {this.PtrType}\n" +
               $"Velocity: <{this.Velocity.X}, {this.Velocity.Y}>\n" +
               $"Size: <{this.Size.Width}, {this.Size.Height}>";
    }

    /// <summary>
    /// Translates the PixelPosition of the <see cref="RemotePointerInfo"/>
    /// from the remote coordinate system to the local coordinate system.
    /// </summary>
    /// <param name="bounds">The <see cref="Rectangle"/> bounds of the local drawing area.</param>
    /// <returns>A <see cref="Vector2"/> with the PixelPosition in local coordinates.</returns>
    public Point Translate( Rectangle bounds )
    {
        float scaleX = (float)bounds.Width / this.Size.Width;
        float scaleY = (float)bounds.Height / this.Size.Height;
        Point p = new()
        {
            X = (int)(scaleX * this.PixelPosition.X),
            Y = (int)(scaleY * this.PixelPosition.Y)
        };

        p.X += bounds.X;
        p.Y += bounds.Y;
        return p;
    }
}
