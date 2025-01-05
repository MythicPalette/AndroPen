namespace AndroPen.Helpers;

internal static class Logging
{
    public delegate void LogOutputHandler( string line );
    public static event LogOutputHandler? LogOutput;

    private static readonly List<string> _collection = [];
    internal static int Count => _collection.Count;

    private static void Append( string value )
    {
        Thread t = new( () =>
        {
            string line = $"{DateTime.Now:HH:mm:ss} :: {value}";
            _collection.Add( line );
            LogOutput?.Invoke( line );
            Console.WriteLine( line );
            SaveLog();
        });
        t.Start();
    }

    internal static void Error( string value ) => Append( $"[ERR] {value}" );
    internal static void Debug( string value ) => Append( $"[DBG] {value}" );
    internal static void Log( string value ) => Append( $"[LOG] {value}" );
    internal static string Dump() => string.Join( "\r\n", _collection );

    private static void SaveLog()
    {
        string path = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
        string file = Path.Combine( path, "AndroPen.log" );
        try
        {
            File.WriteAllLines( file, _collection );
        }
        catch { }
    }
}
