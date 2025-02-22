using System;
using System.IO;

namespace FortniteBurger.Classes
{
    internal class CloseManager
    {
        internal static void Close(bool error = false, string errormsg = "")
        {
            if (error)
            {
                MainWindow.ErrorLog.CreateLog(errormsg);
            }


            FiddlerCore.StopFiddlerCore();
            Settings.SaveConfig();
            Settings.SaveSettings();
            Settings.SaveMods();

            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }

            string flagDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/FortniteBurger/Flags";
            string renameFlag = Path.Combine(flagDir, "renamed.flag");

            if (File.Exists(renameFlag))
                File.Delete(renameFlag);
        }
    }
}
