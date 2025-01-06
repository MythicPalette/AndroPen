using System.Net;
using System.Net.Sockets;

namespace AndroPen.Helpers;

internal class SocketManager
{
    protected int _port = 18998;
    protected TcpListener _listener;

    protected Thread _listenThread;

    protected List<Client> _clients;

    protected bool _shutingDown = false;

    /// <summary>
    /// Default constructor.
    /// </summary>
    internal SocketManager()
    {
        this._clients = [];
        this._listener = new( IPAddress.Any, this._port );
        this._listenThread = new Thread( ListenWorker );
    }

    /// <summary>
    /// Start the listener and begin accepting clients.
    /// </summary>
    internal void Start() => this._listenThread.Start();

    /// <summary>
    /// Accepts client connections and creates client workers.
    /// </summary>
    protected void ListenWorker()
    {
        try
        {
            this._listener.Start();
            Logging.Log( "Waiting for connection." );
            while( !this._shutingDown )
            {
                Socket sock = this._listener.AcceptSocket();
                Client client = new(sock);
                client.Disposed += ( s, e ) => this._clients.Remove( client );
                this._clients.Add( client );
            }
        }
        catch( Exception ex )
        {
            Logging.Error( ex.ToString() );
        }
    }

    /// <summary>
    /// Shutdown the socket manager and dispose of the listener.
    /// </summary>
    public void Dispose()
    {
        this._listener.Stop();
        this._listener.Dispose();
        this._shutingDown = true;
        this._listenThread.Join();
    }
}
