using AndroPen.Data;

namespace AndroPen.Helpers;
internal class EventProcessor
{
    internal static void ProcessEvent(RemoteEvent re)
    {
        RemotePointerInfo? ev = re.Touches.Count > 0 ? re.Touches[0] : re.Pen;

        switch( re.Sender )
        {
            case 0: // The main drawing surface is zero.
                ProcessDrawArea( re ); break;

            case ExpressKeyHandler.SLIDER_1_ID:
            case ExpressKeyHandler.SLIDER_2_ID: // 
                ExpressKeyHandler.Slide( re.Sender, ev?.Velocity.Y < 0 );
                break;

            default:
                if( ev is null )
                    return;
                ExpressKeyHandler.ProcessEKey( re.Sender, ev.EvType ); break;
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
}
