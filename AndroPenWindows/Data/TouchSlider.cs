using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroPen.Helpers;

namespace AndroPen.Data;
internal class TouchSlider
{
    internal int Id { get; set; } = 0;
    internal ExpressKey Up { get; set; } = new();
    internal ExpressKey Down { get; set; } = new();

    public override string ToString() =>
        $"{this.Up};{this.Down}";

    public static TouchSlider Parse( string str )
    {
        TouchSlider result = new();
        string[] split = str.Split(';');

        if( split.Length != 2 )
            return result;

        result.Up = ExpressKey.Parse( split[0] );
        result.Down = ExpressKey.Parse( split[1] );

        return result;
    }

    public void Slide(bool up)
    {
        if( up )
            this.Up.Press();
        else
            this.Down.Press();
    }
}
