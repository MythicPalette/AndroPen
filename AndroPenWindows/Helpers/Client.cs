using System.Net.Sockets;
using AndroPen.Data;

namespace AndroPen.Helpers;
internal class Client
{
    internal event EventHandler<EventArgs>? Disposed;

    protected Socket _socket;
    protected Thread _listenThread;
    internal Client( Socket sock )
    {
        this._socket = sock;
        this._socket.ReceiveTimeout = 1000;
        this._listenThread = new Thread( () => Worker( sock ) );
        this._listenThread.Start();
    }

    protected void Worker( Socket client )
    {
        while( client.Connected )
        {
            try
            {
                // Prepare a remote event receiver.
                RemoteEvent remoteEvent = new();

                /*
                 * Get the ID of the sender. This is important for tracking which view 
                 * sent the event as different views have different uses.
                 */
                byte[] senderBytes = Read(4);
                remoteEvent.Sender = BitConverter.ToInt32( senderBytes, 0 );

                // The second set of bytes contains the number of touch events to read
                byte[] countBytes = Read( 4 );
                int count = BitConverter.ToInt32( countBytes, 0 );

                // Get the expected number of RemotePointerInfo
                for( int i = 0; i < count; i++ )
                {
                    // Read the full size of the packet.
                    byte[] data = Read( RemotePointerInfo.BYTE_LENGTH );
                    RemotePointerInfo rpi = RemotePointerInfo.DeserializePointerInfo( data );

                    if ( rpi.PtrType == RemotePointerType.Pen ) remoteEvent.Pen = rpi;
                    else remoteEvent.Touches.Add( rpi );
                }
                EventProcessor.ProcessEvent( remoteEvent );
            }
            catch( SocketException se )
            {
                // A timeout is normal every second.
                if( se.SocketErrorCode != SocketError.TimedOut )
                {
                    Logging.Error( se.ToString() );
                    return;
                }
            }
            catch( Exception ex )
            {
                Logging.Error( ex.ToString() );
                continue;
            }
        }
        Logging.Log( "Socket closed" );
    }

    protected byte[] Read( int count )
    {
        // Prepare the return value
        byte[] rtn = new byte[count];

        // Collect all expected bytes
        int received = 0;
        do
            received += this._socket.Receive( rtn, 0, count - received, SocketFlags.None );
        while( received < count ); // Verify we got all expected bytes.

        return rtn;
    }

    internal void Dispose()
    {
        this._socket.Close();
        this._socket.Dispose();
        this._listenThread.Join();
        this.Disposed?.Invoke( this, EventArgs.Empty );
    }
}
