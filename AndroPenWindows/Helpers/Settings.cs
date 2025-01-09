namespace AndroPen.Helpers;

internal static class Settings
{
    internal static string ScreenDevice
    {
        get => Properties.Settings.Default.ScreenDevice;
        set
        {
            Properties.Settings.Default.ScreenDevice = value;
            Properties.Settings.Default.Save();
        }
    }

    internal static float ActivationThreshold
    {
        get => Properties.Settings.Default.ActivationThreshold;
        set
        {
            Properties.Settings.Default.ActivationThreshold = value;
            Properties.Settings.Default.Save();
        }
    }

    internal static float InitialValue
    {
        get => Properties.Settings.Default.InitialValue;
        set
        {
            Properties.Settings.Default.InitialValue = value;
            Properties.Settings.Default.Save();
        }
    }

    internal static PointF Softness
    {
        get => new() { 
            X = Properties.Settings.Default.SoftnessX,
            Y = Properties.Settings.Default.SoftnessY 
        };
        set
        {
            Properties.Settings.Default.SoftnessX = value.X;
            Properties.Settings.Default.SoftnessY = value.Y;
            Properties.Settings.Default.Save();
        }
    }

    internal static float MaxEffectiveInput
    {
        get => Properties.Settings.Default.MaxEffectiveInput;
        set
        {
            Properties.Settings.Default.MaxEffectiveInput = value;
            Properties.Settings.Default.Save();
        }
    }

    internal static float MaxOutput
    {
        get => Properties.Settings.Default.MaxOutput;
        set
        {
            Properties.Settings.Default.MaxOutput = value;
            Properties.Settings.Default.Save();
        }
    }

    internal static int Port
    {
        get => Properties.Settings.Default.Port;
        set
        {

        }
    }
}