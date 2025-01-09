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
                //_socket.Receive( data );
                byte[] countBytes = Read( 4 );

                // Convert the data bytes to an int.
                int count = BitConverter.ToInt32( countBytes, 0 );

                // Count should never be greater than 10.
                if( count > 10 )
                {
                    // Something has gone wrong here. 
                    Logging.Error( $"Invalid Touch Count [{count}]" );
                    continue;
                }

                List<RemotePointerInfo> touches = [];
                RemotePointerInfo? pen = null;
                // Get the expected number of RemotePointerInfo
                for( int i = 0; i < count; i++ )
                {
                    // Read the full size of the packet.
                    byte[] data = Read( RemotePointerInfo.BYTE_LENGTH );
                    RemotePointerInfo rpi = RemotePointerInfo.DeserializePointerInfo( data );

                    // Only one pen should ever be down at a time.
                    if( rpi.PtrType == RemotePointerType.Pen )
                        pen = rpi;
                    // Up to 10 touches can exist at a time.
                    else
                        touches.Add( rpi );
                }

                // Simulate touches if necessary.
                if( touches.Count > 0 )
                    Program.inputHandler.SimulateTouch( [.. touches] );

                // Simulate pen if necessary.
                if( pen != null )
                    Program.inputHandler.SimulatePen( pen );
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
