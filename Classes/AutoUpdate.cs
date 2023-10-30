using System;
using System.Windows;
using AutoUpdaterDotNET;
using Newtonsoft.Json;

namespace FortniteBurger.Classes
{
    internal class AutoUpdate
    {
        internal void CheckForUpdates()
        {
            AutoUpdater.InstalledVersion = new Version(MainWindow.CurrVersion);
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.HttpUserAgent = "AutoUpdater";
            AutoUpdater.RunUpdateAsAdmin = true;
            AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                AutoUpdater.Start("https://api.github.com/repos/OssieFromDK/DeadByDaylight-Unlocker/releases/latest");
            }));
        }

        private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            dynamic json = JsonConvert.DeserializeObject(args.RemoteData);
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = json.tag_name,
                DownloadURL = json.assets.browser_download_url,
            };
        }

        private void AutoUpdater_ApplicationExitEvent()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.main.Close();
            }));

            FiddlerCore.StopFiddlerCore();
            Settings.SaveConfig();
            Settings.SaveSettings();

            Environment.Exit(0);
        }
    }
}