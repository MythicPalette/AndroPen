using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace AndroPen.Helpers;

internal static class Logging
{
    private static readonly BlockingCollection<string> _queue = [];

    private static Thread _loggingThread = null!;

    /// <summary>
    /// Start the logging thread. Logging is done on its own thread to
    /// prevent the logging from slowing down the responsiveness.
    /// </summary>
    internal static void Init()
    {
        _loggingThread = new(() =>
        {
            string path = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
            string file = Path.Combine( path, "AndroPen.log" );

            foreach ( string log in _queue.GetConsumingEnumerable() )
            {
                Console.WriteLine( log );
                File.AppendAllLines( file, [log] );
            }
        });
        _loggingThread.Start();
    }

    /// <summary>
    /// Shutdown the logging thread and cleans up the process.
    /// </summary>
    internal static void Shutdown()
    {
        _queue.CompleteAdding();
        _loggingThread?.Join();
    }

    private static void Append( string value ) => _queue.Add( value );
    internal static void Error( string value ) => Append( $"{DateTime.Now:HH:mm:ss} [ERR] {value}" );
    internal static void Debug( string value ) => Append( $"{DateTime.Now:HH:mm:ss} [DBG] {value}" );
    internal static void Log( string value ) => Append( $"{DateTime.Now:HH:mm:ss} [LOG] {value}" );
}
