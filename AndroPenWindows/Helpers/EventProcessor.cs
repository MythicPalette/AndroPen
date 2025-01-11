using AndroPen.Data;

namespace AndroPen.Helpers;
internal class EventProcessor
{
    internal const int DRAW_AREA_ID = 0;
    internal const int SLIDER_1_ID = 1;
    internal const int SLIDER_2_ID = 2;
    internal static void ProcessEvent(RemoteEvent re)
    {
        switch( re.Sender )
        {
            case DRAW_AREA_ID:
                ProcessDrawArea( re ); break;
            case SLIDER_1_ID:
                ProcessSlider1( re ); break;
            case SLIDER_2_ID:
                ProcessSlider2( re ); break;
        }
    }

    protected static void ProcessDrawArea( RemoteEvent re )
    {
        // Simulate touches if necessary.
        if( re.Touches.Count > 0 )
            Program.inputHandler.SimulateTouch( [.. re.Touches] );

        // Simulate pen if necessary.
        if( re.Pen != null )
            Program.inputHandler.SimulatePen( re.Pen );
    }

    protected static void ProcessSlider1( RemoteEvent re )
    {
        RemotePointerInfo? ev = re.Touches.Count > 0 ? re.Touches[0] : re.Pen;
        if( ev is null )
            return;

        if( ev.Velocity.Y == 0 )
            return;
        else if( ev?.Velocity.Y > 0 ) // A positive velocity means going down.
            InputHandler.SimulateKeyPress( '['.GetVirtualKeyCode() );
        else
            InputHandler.SimulateKeyPress( ']'.GetVirtualKeyCode() );
    }

    protected static void ProcessSlider2( RemoteEvent re )
    {
        RemotePointerInfo? ev = re.Touches.Count > 0 ? re.Touches[0] : re.Pen;
        if( ev is null )
            return;


        if( ev.Velocity.Y == 0 )
            return;

        InputHandler.SimulateKeyDown( 0x11 );
        if( ev?.Velocity.Y > 0 ) // A positive velocity means going down.
            InputHandler.SimulateKeyPress( '['.GetVirtualKeyCode() );
        else
            InputHandler.SimulateKeyPress( ']'.GetVirtualKeyCode() );

        InputHandler.SimulateKeyUp( 0x11 );
    }
}
