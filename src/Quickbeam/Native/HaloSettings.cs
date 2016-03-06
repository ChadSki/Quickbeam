using Microsoft.Win32;

namespace Quickbeam.Native
{
    public static class HaloSettings
    {
        public const string HaloRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft Games\Halo";

        public static string HaloExeDir
        {
            get
            {
                return (Registry.GetValue(HaloRegistryKey, "EXE Path", ".") ?? ".").ToString();
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
                return (Registry.GetValue(HaloRegistryKey, "Version", "0") ?? "0").ToString();
            }
        }
    }
}
