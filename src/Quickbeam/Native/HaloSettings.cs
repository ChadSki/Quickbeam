using System;
using System.Collections.Generic;
using Microsoft.Win32;


namespace Quickbeam.Native
{
    public static class HaloSettings
    {
        public static string HaloExeDir
        {
            get
            {
                return (Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo",
                    "EXE Path", "") ?? ".").ToString();
            }
        }

        public static string HaloExePath
        {
            get { return HaloExeDir + @"\halo.exe"; }
        }

        public static string HaloVersion
        {
            get
            {
                return (Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo",
                    @"Version", @"") ?? "").ToString();
            }
        }
    }
}
