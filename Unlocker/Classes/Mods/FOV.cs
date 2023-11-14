using FortniteBurger.Properties;
using System.Collections.Generic;
using System.Windows.Documents;

namespace FortniteBurger.Classes.Mods
{
    internal class FOV
    {
        internal bool IsInstalled = false;

        internal byte[] PakResource = Resources.FOV;

        internal string PakNameSteam = "pakchunk871-WindowsNoEditor";
        internal string PakNameEGS = "pakchunk871-EGS";

        internal bool RequireCore = true;

        internal bool HasINI = true;

        internal string INIHeader = "/Game/FovController/Settings.Settings_C";

        internal Dictionary<string, string> INIValues = new Dictionary<string, string>()
        {
            ["SurvivorFOV"] = "90",
            ["KillerFOV"] = "87"
        };
    }
}
