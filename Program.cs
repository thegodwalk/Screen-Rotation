

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Resolution.DISPLAY_DEVICE f = Resolution.PrmaryScreenResolution.GETDD();
            Resolution.DEVMODE g = Resolution.PrmaryScreenResolution.GetDevMode();
            Resolution.NativeMethods.EnumDisplayDevices(null, 0, ref f, 0);
            Resolution.NativeMethods.EnumDisplaySettings(f.DeviceName, -1, ref g);
            if(g.dmDisplayOrientation == 1)
            {
            Resolution.PrmaryScreenResolution.SetPrimary(0);
            Resolution.PrmaryScreenResolution.ChangeResolution(0);
            }
            else
            {
                Resolution.PrmaryScreenResolution.SetPrimary(1);
            Resolution.PrmaryScreenResolution.ChangeResolution(0);
            }
            
            
            
            
        }
    }
}