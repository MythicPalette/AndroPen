using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroPen.Data;

public enum RemoteEventType
{
    DOWN,
    POINTER_DOWN,
    MOVE,
    UP,
    POINTER_UP,
    HOVER_ENTER,
    HOVER_MOVE,
    HOVER_EXIT
}

public enum RemotePointerType
{
    PEN,
    TOUCH,
    FINGER
}

public class RemotePointerInfo
{
    public const int BYTE_LENGTH = 56;
    public int PointerId { get; set; }
    public RemoteEventType EvType { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Pressure { get; set; }
    public float TiltX { get; set; }
    public float TiltY { get; set; }
    public long TimeStamp { get; set; }
    public RemotePointerType PtrType { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public int ViewWidth { get; set; }
    public int ViewHeight { get; set; }
    public static RemotePointerInfo DeserializePointerInfo( byte[] data )
    {
        RemotePointerInfo pi = new();
        int idx = 0;
        pi.PointerId = BitConverter.ToInt32( data, idx );
        idx += sizeof( Int32 );

        pi.EvType = (RemoteEventType)BitConverter.ToInt32( data, idx );
        idx += sizeof( Int32 );

        pi.X = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.Y = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.Pressure = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.TiltX = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.TiltY = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.TimeStamp = BitConverter.ToInt64( data, idx );
        idx += sizeof( Int64 );

        pi.PtrType = (RemotePointerType)BitConverter.ToInt32( data, idx );
        idx += sizeof( Int32 );

        pi.VelocityX = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.VelocityY = BitConverter.ToSingle( data, idx );
        idx += sizeof( Single );

        pi.ViewWidth = BitConverter.ToInt32( data, idx );
        idx += sizeof( Single );

        pi.ViewHeight = BitConverter.ToInt32( data, idx );
        idx += sizeof( Single );
        return pi;
    }
    public override string ToString()
    {
        return $"RemotePointerInfo:\n" +
               $"PointerId: {PointerId}\n" +
               $"EvType: {EvType}\n" +
               $"X: {X}\n" +
               $"Y: {Y}\n" +
               $"Pressure: {Pressure}\n" +
               $"TiltX: {TiltX}\n" +
               $"TiltY: {TiltY}\n" +
               $"TimeStamp: {TimeStamp}\n" +
               $"PtrType: {PtrType}\n" +
               $"VelocityX: {VelocityX}\n" +
               $"VelocityY: {VelocityY}\n" +
               $"ViewWidth: {ViewWidth}\n" +
               $"ViewHeight: {ViewHeight}";
    }
    public byte[] Serialize()
    {
        List<byte> buffer = new List<byte>();

        // Serialize each field in the correct order
        buffer.AddRange( BitConverter.GetBytes( PointerId ) );
        buffer.AddRange( BitConverter.GetBytes( (int)EvType ) );
        buffer.AddRange( BitConverter.GetBytes( X ) );
        buffer.AddRange( BitConverter.GetBytes( Y ) );
        buffer.AddRange( BitConverter.GetBytes( Pressure ) );
        buffer.AddRange( BitConverter.GetBytes( TiltX ) );
        buffer.AddRange( BitConverter.GetBytes( TiltY ) );
        buffer.AddRange( BitConverter.GetBytes( TimeStamp ) );
        buffer.AddRange( BitConverter.GetBytes( (int)PtrType ) );
        buffer.AddRange( BitConverter.GetBytes( VelocityX ) );
        buffer.AddRange( BitConverter.GetBytes( VelocityY ) );
        buffer.AddRange( BitConverter.GetBytes( ViewWidth ) );
        buffer.AddRange( BitConverter.GetBytes( ViewHeight ) );

        return buffer.ToArray();
    }

    public Vector2 Translate( Rectangle bounds )
    {
        float scale = (float)bounds.Width / this.ViewWidth;
        Vector2 p = new()
        {
            X = (int)(scale * this.X),
            Y = (int)(scale * this.Y)
        };

        p.X += bounds.X;
        p.Y += bounds.Y;
        return p;
    }
}
