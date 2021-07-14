    using System; 
    using System.Runtime.InteropServices; 
    
    namespace Resolution 
    { 
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct DISPLAY_DEVICE
            {
                 [MarshalAs(UnmanagedType.U4)]
                public int cb;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public string DeviceName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceString;
                [MarshalAs(UnmanagedType.U4)]
                public DisplayDeviceStateFlags StateFlags;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceID;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string DeviceKey;
                
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
            
            
            
            [Flags] public enum ChangeDisplaySettingsFlags : uint
            {
                CDS_NONE = 0,
                CDS_UPDATEREGISTRY = 0x00000001,
                CDS_TEST = 0x00000002,
                CDS_FULLSCREEN = 0x00000004,
                CDS_GLOBAL = 0x00000008,
                CDS_SET_PRIMARY = 0x00000010,
                CDS_VIDEOPARAMETERS = 0x00000020,
                CDS_ENABLE_UNSAFE_MODES = 0x00000100,
                CDS_DISABLE_UNSAFE_MODES = 0x00000200,
                CDS_RESET = 0x40000000,
                CDS_RESET_EX = 0x20000000,
                CDS_NORESET = 0x10000000
            };
            
            
            [Flags] public enum DisplayDeviceStateFlags : uint
            {
                /// <summary>The device is part of the desktop.</summary>
                AttachedToDesktop = 0x1,
                MultiDriver = 0x2,
                /// <summary>The device is part of the desktop.</summary>
                PrimaryDevice = 0x4,
                /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
                MirroringDriver = 0x8,
                /// <summary>The device is VGA compatible.</summary>
                VGACompatible = 0x10,
                /// <summary>The device is removable; it cannot be the primary display.</summary>
                Removable = 0x20,
                /// <summary>The device has more display modes than its output devices support.</summary>
                ModesPruned = 0x8000000,
                Remote = 0x4000000,
                Disconnect = 0x2000000,
            };
    
        class NativeMethods 
        { 
            [DllImport("user32.dll")]
            public static extern bool EnumDisplayDevices(string DeviceName,uint DevNum,ref DISPLAY_DEVICE DispDev,uint dwFlags);
            [DllImport("user32.dll")] 
            public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode); 
            [DllImport("user32.dll")] 
            public static extern int ChangeDisplaySettingsEx(string Devicename, IntPtr lpDEVMODE, IntPtr hwnd, ChangeDisplaySettingsFlags flags, IntPtr Param); 
            [DllImport("user32.dll")] 
            public static extern int ChangeDisplaySettingsEx(string Devicename, ref DEVMODE devmode, IntPtr hwnd, ChangeDisplaySettingsFlags flags, IntPtr Param);
            
    
            public const int ENUM_CURRENT_SETTINGS = -1; 
            public const int CDS_UPDATEREGISTRY = 0x01; 
            public const int CDS_TEST = 0x02; 
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
            
            public static void SetPrimary(uint id)
            {
                DEVMODE dm = GetDevMode(); 
                DISPLAY_DEVICE dd = GETDD();
                
                   NativeMethods.EnumDisplayDevices(null,id,ref dd, 0);
                    
                    NativeMethods.EnumDisplaySettings(dd.DeviceName, -1, ref dm);
                    var offsetx = dm.dmPositionX;
                    var offsety = dm.dmPositionY;
                    dm.dmPositionX = 0;
                    dm.dmPositionY = 0;
                   
                        NativeMethods.ChangeDisplaySettingsEx(
                        dd.DeviceName,
                        ref dm,
                        (IntPtr)null,
                        (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET),
                        IntPtr.Zero);
                        dd = GETDD();
                        


                     for (uint otherid = 0; NativeMethods.EnumDisplayDevices(null, otherid, ref dd, 0); otherid++)
                        {
                            dd = GETDD();
                            NativeMethods.EnumDisplayDevices(null,otherid,ref dd, 0);
                            if (dd.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop) && otherid != id)
                            {
                                dd.cb = (byte)Marshal.SizeOf(dd);
                                var otherDeviceMode = new DEVMODE();

                                NativeMethods.EnumDisplaySettings(dd.DeviceName, -1, ref otherDeviceMode);

                                otherDeviceMode.dmPositionX -= offsetx;
                                otherDeviceMode.dmPositionY -= offsety;

                                NativeMethods.ChangeDisplaySettingsEx(
                                    dd.DeviceName,
                                    ref otherDeviceMode,
                                    (IntPtr)null,
                                    (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET),
                                    IntPtr.Zero);

                            }

                            dd.cb = (byte)Marshal.SizeOf(dd);
                        }

                        // Apply settings
                        NativeMethods.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
            }
            static public string ChangeResolution(uint id) 
            { 
    
                DEVMODE dm = GetDevMode(); 
                DISPLAY_DEVICE dd = GETDD();
                
                    
                    
            
                    
    
                    NativeMethods.EnumDisplayDevices(null,id,ref dd,0);
                    
                   
                    
                
    
                if (NativeMethods.EnumDisplaySettings(dd.DeviceName, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm)) 
                {
    
                   
                    int temp = dm.dmPelsHeight;
                    dm.dmPelsHeight = dm.dmPelsWidth;
                    dm.dmPelsWidth = temp;
    
                    
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
                            
                            break;
                    }
    
    
                    int iRet = NativeMethods.ChangeDisplaySettingsEx(dd.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero); 
    
                    if (iRet == NativeMethods.DISP_CHANGE_FAILED) 
                    { 
                        return "Unable To Process Your Request. Sorry For This Inconvenience."; 
                    } 
                    else 
                    { 
                        iRet = NativeMethods.ChangeDisplaySettingsEx(dd.DeviceName, ref dm, IntPtr.Zero, ChangeDisplaySettingsFlags.CDS_NONE, IntPtr.Zero); 
                        switch (iRet) 
                        { 
                            case NativeMethods.DISP_CHANGE_SUCCESSFUL: 
                                { 
                                    return "Orientation Changed Successfullyt"; 
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
    
             public static DEVMODE GetDevMode() 
            { 
                DEVMODE dm = new DEVMODE(); 
                dm.dmDeviceName = new String(new char[32]); 
                dm.dmFormName = new String(new char[32]); 
                dm.dmSize = (short)Marshal.SizeOf(dm); 
                return dm; 
            } 
            public static DISPLAY_DEVICE GETDD() 
                { 
                    DISPLAY_DEVICE dd = new DISPLAY_DEVICE(); 
                    dd.cb = (byte)Marshal.SizeOf(dd); 
                    return dd;
                }
            
        }
    }
    