using System;
using System.Net;
using System.Net.Sockets;

namespace AndroPen.Helpers;

internal class SocketManager
{
    protected byte[] PACKET_SIGNATURE = [0x0D, 0x64, 0x81, 0x40, 0xA5, 0x34, 0x76, 0x34];
    protected int _port = 18998;
    protected TcpListener _listener;

    protected Thread _listenThread;

    protected List<Client> _clients;

    // initialize
    internal SocketManager()
    {
        _clients = new();
        _listener = new( IPAddress.Any , this._port );
        _listenThread = new Thread( () => ListenWorker() );
    }

    // Accept Connection
    internal void Start()
    {
        _listenThread.Start();
    }

    /// <summary>
    /// Accepts client connections and creates client workers.
    /// </summary>
    protected void ListenWorker() {
        try
        {
            _listener.Start();
            Logging.Log( "Waiting for connection." );
            while (true)
            {
                Socket sock = this._listener.AcceptSocket();
                Client client = new(sock);
                client.Disposed += ( s, e ) => this._clients.Remove( client );
                //Logging.Log( "Connection established" );
                //var childThread = new Thread(() => ClientWorker(sock));
                //childThread.Start();
                this._clients.Add( client );
            }
        } catch (Exception ex) 
        {
            Logging.Error( ex.ToString());
        }
    }

    protected bool IsSignature( byte[] test)
    {
        // Length must match.
        if (test.Length != PACKET_SIGNATURE.Length)
            return false;

        // Match every byte.
        for ( int i = 0; i < PACKET_SIGNATURE.Length; i++ )
            if (test[i] != PACKET_SIGNATURE[i] ) return false;

        return true;
    }

    public void Dispose()
    {
        this._listener.Stop();
        this._listener.Dispose();
        this._listenThread.Join();
    }
}
