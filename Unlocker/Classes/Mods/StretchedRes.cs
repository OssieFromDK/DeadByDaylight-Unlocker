using FortniteBurger.Properties;
using System.Collections.Generic;

namespace FortniteBurger.Classes.Mods
{
    class StretchedRes
    {
        internal bool IsInstalled = false;

        internal byte[] PakResource = Resources.StretchedRes;

        internal string PakNameSteam = "pakchunk889-WindowsNoEditor";
        internal string PakNameEGS = "pakchunk889-EGS";

        internal bool RequireCore = true;

        internal bool HasINI = true;

        internal string INIHeader = "/Game/StretchedRes/Settings.Settings_C";

        internal Dictionary<string, string> INIValues = new Dictionary<string, string>()
        {
            ["bForceScreenResolution"] = "True",
            ["ScreenResolutionSettings"] = "1920x1080f"
        };
    }
}
