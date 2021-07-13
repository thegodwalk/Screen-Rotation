Function Set-ScreenResolutionAndOrientation { 

    $pinvokeCode = @"
    using System; 
    using System.Runtime.InteropServices; 
    
    namespace Resolution 
    { 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct DISPLAY_DEVICE
            {
                
                public byte cb;
                public string DeviceName;
                public byte DeviceString;
                public uint StateFlags;
                public byte DeviceID;
                public byte DeviceKey;
                
            };
    
        [StructLayout(LayoutKind.Sequential)] 
        public struct DEVMODE 
        { 
           [MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)]
           public string dmDeviceName;
    
           public short  dmSpecVersion;
           public short  dmDriverVersion;
           public short  dmSize;
           public short  dmDriverExtra;
           public int    dmFields;
           public int    dmPositionX;
           public int    dmPositionY;
           public int    dmDisplayOrientation;
           public int    dmDisplayFixedOutput;
           public short  dmColor;
           public short  dmDuplex;
           public short  dmYResolution;
           public short  dmTTOption;
           public short  dmCollate;
    
           [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
           public string dmFormName;
    
           public short  dmLogPixels;
           public short  dmBitsPerPel;
           public int    dmPelsWidth;
           public int    dmPelsHeight;
           public int    dmDisplayFlags;
           public int    dmDisplayFrequency;
           public int    dmICMMethod;
           public int    dmICMIntent;
           public int    dmMediaType;
           public int    dmDitherType;
           public int    dmReserved1;
           public int    dmReserved2;
           public int    dmPanningWidth;
           public int    dmPanningHeight;
        }; 
    
        class NativeMethods 
        { 
            [DllImport("user32.dll")]
            public static extern int EnumDisplayDevices(string DeviceName,int DevNum,ref DISPLAY_DEVICE DispDev,uint dwFlags);
            [DllImport("user32.dll")] 
            public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode); 
            [DllImport("user32.dll")] 
            public static extern int ChangeDisplaySettingsEx(string Devicename, ref DEVMODE devMode, IntPtr hwnd, int flags, IntPtr Param); 
    
            public const int ENUM_CURRENT_SETTINGS = -1; 
            public const int CDS_UPDATEREGISTRY = 0x01; 
            public const int CDS_TEST = 0; 
            public const int DISP_CHANGE_SUCCESSFUL = 0; 
            public const int DISP_CHANGE_RESTART = 1; 
            public const int DISP_CHANGE_FAILED = -1;
            public const int DMDO_DEFAULT = 0;
            public const int DMDO_90 = 1;
            public const int DMDO_180 = 2;
            public const int DMDO_270 = 3;
        } 
        
    
           
    
    
    
        public class PrmaryScreenResolution 
        { 
            static public string ChangeResolution() 
            { 
    
                DEVMODE dm = GetDevMode(); 
                DISPLAY_DEVICE dd = GETDD();
                
                    
                    
            
    
    
                    NativeMethods.EnumDisplayDevices(null,0,ref dd,0);
                    NativeMethods.EnumDisplayDevices(dd.DeviceName,0,ref dd,0);
                
    
                if (0 != NativeMethods.EnumDisplaySettings(dd.DeviceName, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm)) 
                {
    
                    // swap width and height
                    int temp = dm.dmPelsHeight;
                    dm.dmPelsHeight = dm.dmPelsWidth;
                    dm.dmPelsWidth = temp;
    
                    // determine new orientation based on the current orientation
                    switch(dm.dmDisplayOrientation)
                    {
                        case NativeMethods.DMDO_DEFAULT:
                            dm.dmDisplayOrientation = NativeMethods.DMDO_90;
                            break;
                        case NativeMethods.DMDO_270:
                            dm.dmDisplayOrientation = NativeMethods.DMDO_DEFAULT;
                            break;
                        case NativeMethods.DMDO_180:
                            dm.dmDisplayOrientation = NativeMethods.DMDO_90;
                            break;
                        case NativeMethods.DMDO_90:
                            dm.dmDisplayOrientation = NativeMethods.DMDO_DEFAULT;
                            break;
                        default:
                            // unknown orientation value
                            // add exception handling here
                            break;
                    }
    
    
                    int iRet = NativeMethods.ChangeDisplaySettingsEx(dd.DeviceName, ref dm, IntPtr.Zero, NativeMethods.CDS_TEST, IntPtr.Zero); 
    
                    if (iRet == NativeMethods.DISP_CHANGE_FAILED) 
                    { 
                        return "Unable To Process Your Request. Sorry For This Inconvenience."; 
                    } 
                    else 
                    { 
                        iRet = NativeMethods.ChangeDisplaySettingsEx(dd.DeviceName, ref dm, IntPtr.Zero, NativeMethods.CDS_TEST, IntPtr.Zero); 
                        switch (iRet) 
                        { 
                            case NativeMethods.DISP_CHANGE_SUCCESSFUL: 
                                { 
                                    return dd.DeviceName; 
                                } 
                            case NativeMethods.DISP_CHANGE_RESTART: 
                                { 
                                    return "You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode."; 
                                } 
                            default: 
                                { 
                                    return "Failed To Change The Resolution"; 
                                } 
                        } 
    
                    } 
    
    
                } 
                else 
                { 
                    return "Failed To Change The Resolution."; 
                } 
            } 
    
            private static DEVMODE GetDevMode() 
            { 
                DEVMODE dm = new DEVMODE(); 
                dm.dmDeviceName = new String(new char[32]); 
                dm.dmFormName = new String(new char[32]); 
                dm.dmSize = (short)Marshal.SizeOf(dm); 
                return dm; 
            } 
            private static DISPLAY_DEVICE GETDD() 
                { 
                    DISPLAY_DEVICE dd = new DISPLAY_DEVICE(); 
                    dd.cb = (byte)Marshal.SizeOf(dd); 
                    return dd;
                }
            
        } 
    }   
"@
Add-Type $pinvokeCode -ErrorAction SilentlyContinue 
[Resolution.PrmaryScreenResolution]::ChangeResolution() 
}
Set-ScreenResolutionAndOrientation



