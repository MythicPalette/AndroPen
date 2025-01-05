using AndroPen.Data;

namespace AndroPen.Helpers;

internal static class Settings
{
    internal static PressureCurveData PressureCurve
    {
        get => PressureCurveData.Deserialize( Properties.Settings.Default.PressureCurveValues );
        set
        {
            Properties.Settings.Default.PressureCurveValues = value.SerializeToString();
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