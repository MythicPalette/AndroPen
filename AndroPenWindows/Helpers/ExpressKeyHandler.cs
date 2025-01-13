using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroPen.Data;

namespace AndroPen.Helpers;
internal static class ExpressKeyHandler
{
    internal const int SLIDER_1_ID = 1;
    internal const int SLIDER_2_ID = 2;
    internal const int EKEY_1_ID = 110;

    private static readonly List<ExpressKey> _keys = [];
    private static readonly List<TouchSlider> _sliders = [];

    internal static void Init()
    {
        /*
         * The following two sliders are the default.
         * TODO: These should be set via saved setting and stored there when changed.
         * For now, this works.
         */
        _sliders.Add(
            new TouchSlider()
            {
                Id = SLIDER_1_ID,
                Up = new()
                {
                    KeyCode = ']'.GetVirtualKeyCode()
                },
                Down = new()
                {
                    KeyCode = '['.GetVirtualKeyCode(),
                }
            } );

        _sliders.Add(
            new TouchSlider()
            {
                Id = SLIDER_2_ID,
                Up = new()
                {
                    KeyCode = ']'.GetVirtualKeyCode(),
                    Control = true
                },
                Down = new()
                {
                    KeyCode = '['.GetVirtualKeyCode(),
                    Control = true
                }
            } );

        _keys.Add(
            new()
            {
                Id = EKEY_1_ID,
                Shift = true
            } );
    }

    internal static void Slide(int id, bool up) =>
        _sliders.FirstOrDefault( s => s.Id == id )?.Slide( up );

    internal static void ProcessEKey( int id, AndroidEventType aet )
    {
        ExpressKey? key = _keys.FirstOrDefault( k => k.Id == id );
        if( key is null )
            return;

        if( aet == AndroidEventType.Down
            || aet == AndroidEventType.PointerDown )
        {
            key.Down();
        }
        else if( aet == AndroidEventType.Up
            || aet == AndroidEventType.PointerUp )
        {
            key.Up();
        }
    }
}
