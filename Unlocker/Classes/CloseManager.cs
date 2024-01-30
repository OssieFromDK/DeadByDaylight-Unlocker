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
            Telemetry.Remove();

            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }
    }
}
