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
        if ( re.Sender == DRAW_AREA_ID )
            ProcessDrawArea( re );
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

    }

    protected static void ProcessSlider2( RemoteEvent re )
    {

    }
}
