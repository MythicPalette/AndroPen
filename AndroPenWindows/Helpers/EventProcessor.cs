using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroPen.Data;

namespace AndroPen.Helpers;
internal class EventProcessor
{
    internal const int DRAW_AREA_ID = 0;
    internal static void ProcessEvent(RemoteEvent re)
    {
        if( re.Sender == DRAW_AREA_ID )
            ProcessDrawArea( re );
        else
            ProcessSlider1( re );
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
        if( re.Touches[0].Velocity.Y == 0 )
            return;
        else if( re.Touches[0].Velocity.Y > 0 )
            InputHandler.SimulateKeyPress( ']'.GetVirtualKeyCode() );
        else
            InputHandler.SimulateKeyPress( '['.GetVirtualKeyCode() );
    }

    protected static void ProcessSlider2( RemoteEvent re )
    {

    }
}
