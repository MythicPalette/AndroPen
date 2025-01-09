using System.Net;
using System.Net.Sockets;

namespace AndroPen.Helpers;

internal class SocketManager
{
    public bool IsConnected => _listener != null;
    protected TcpListener? _listener;
    protected Thread? _listenThread;
    protected List<Client> _clients= [];

    protected bool _shutingDown = false;

    /// <summary>
    /// Start the listener and begin accepting clients.
    /// </summary>
    internal void Start()
    {
        // The listener has already be started.
        if( this._listener != null )
            return;

        this._shutingDown = false;
        this._listener = new( IPAddress.Any, Settings.Port );
        this._listenThread = new Thread( ListenWorker );
        this._listenThread.Start();
    }

    /// <summary>
    /// Restart the listener and begin accepting clients again.
    /// This is most important if the port setting is changed.
    /// </summary>
    internal void Restart()
    {
        Shutdown();
        Start();
    }

    /// <summary>
    /// Accepts client connections and creates client workers.
    /// </summary>
    protected void ListenWorker()
    {
        try
        {
            if( this._listener == null )
                return;

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
            if( !this._shutingDown )
            {
                Logging.Error( ex.ToString() );
                Shutdown();
            }
        }
    }

    /// <summary>
    /// Shutdown the socket manager and dispose of the listener.
    /// </summary>
    public void Shutdown()
    {
        this._shutingDown = true;
        this._listener?.Stop();
        this._listener?.Dispose();
        this._listener = null;
        this._listenThread = null;
    }
}
