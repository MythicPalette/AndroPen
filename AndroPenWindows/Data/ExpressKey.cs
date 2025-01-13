using AndroPen.Helpers;

namespace AndroPen.Data;
internal class ExpressKey
{
    internal int Id { get; set; } = 0;
    internal bool Control { get; set; } = false;
    internal bool Alt { get; set; } = false;
    internal bool Shift { get; set; } = false;
    internal ushort KeyCode { get; set; } = 0;

    public override string ToString() =>
        $"{this.Control},{this.Alt},{this.Shift},{this.KeyCode}";

    public static ExpressKey Parse(string str)
    {
        ExpressKey result = new();

        string[] lines = str.Split(',');
        if ( lines.Length != 4 ) return result;

        result.Control = bool.Parse(lines[0]);
        result.Alt = bool.Parse(lines[1]);
        result.Shift = bool.Parse(lines[2]);
        result.KeyCode = ushort.Parse(lines[3]);

        return result;
    }

    internal void Press()
    {
        Up();
        Down();
    }

    internal void Up()
    {
        // Press down any modifier keys that are specified.
        if( this.Control )
            InputHandler.SimulateKeyDown( (ushort)ModifierKey.Control );
        if( this.Alt )
            InputHandler.SimulateKeyDown( (ushort)ModifierKey.Alt );
        if( this.Shift )
            InputHandler.SimulateKeyDown( (ushort)ModifierKey.Shift );

        if ( this.KeyCode != 0 )
            InputHandler.SimulateKeyDown( this.KeyCode );
    }

    internal void Down()
    {
        if( this.KeyCode != 0 )
            InputHandler.SimulateKeyUp( this.KeyCode );

        // Release any modifier keys that are specified
        if( this.Shift )
            InputHandler.SimulateKeyUp( (ushort)ModifierKey.Shift );
        if( this.Alt )
            InputHandler.SimulateKeyUp( (ushort)ModifierKey.Alt );
        if( this.Control )
            InputHandler.SimulateKeyUp( (ushort)ModifierKey.Control );
    }
}
