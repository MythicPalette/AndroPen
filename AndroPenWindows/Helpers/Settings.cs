using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroPen.Data;

namespace AndroPen.Helpers;

internal static class Settings
{
    internal static PressureCurveData PressureCurve
    {
        get => PressureCurveData.Deserialize( Properties.Settings.Default.PressureCurveValues );
        set
        {
            Properties.Settings.Default.PressureCurveValues = value.Serialize();
            Properties.Settings.Default.Save();
        }
    }

    internal static string ScreenDevice
    {
        get => Properties.Settings.Default.ScreenDevice;
        set
        {
            Properties.Settings.Default.ScreenDevice = value;
            Properties.Settings.Default.Save();
        }
    }
}