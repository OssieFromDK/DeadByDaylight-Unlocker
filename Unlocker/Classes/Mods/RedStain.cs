using FortniteBurger.Properties;
using System.Collections.Generic;

namespace FortniteBurger.Classes.Mods
{
    class RedStain
    {
        internal bool IsInstalled = false;

        internal byte[] PakResource = Resources.RedStain;

        internal string PakNameSteam = "pakchunk872-WindowsNoEditor";
        internal string PakNameEGS = "pakchunk872-EGS";

        internal bool RequireCore = true;

        internal bool HasINI = true;

        internal string INIHeader = "/Game/RedStain/RedStainSettings.RedStainSettings_C";

        internal Dictionary<string, string> INIValues = new Dictionary<string, string>()
        {
            ["bCastShadows"] = "True",
            ["AttenuationRadius"] = "450",
            ["Intensity"] = "25000",
            ["LightColor"] = "(R=0.75,G=0,B=0,A=1)",
            ["OuterConeAngle"] = "40",
            ["InnerConeAngle"] = "11"
        };
    }
}
