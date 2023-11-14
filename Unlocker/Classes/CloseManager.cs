namespace FortniteBurger.Classes
{
    internal class CloseManager
    {
        internal static void Close()
        {
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
