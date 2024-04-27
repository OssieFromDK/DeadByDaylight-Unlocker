using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using AutoUpdaterDotNET;
using FortniteBurger.Properties;
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
            AutoUpdater.HttpUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
            AutoUpdater.RunUpdateAsAdmin = true;
            AutoUpdater.ParseUpdateInfoEvent += AutoUpdaterOnParseUpdateInfoEvent;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Icon = Resources.Icon;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                AutoUpdater.Start("https://api.github.com/repos/OssieFromDK/DeadByDaylight-Unlocker/releases/latest");
            }));
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    MainWindow.UpdateScreen.FoundUpdate(args.CurrentVersion);

                    try
                    {
                        if (AutoUpdater.DownloadUpdate(args))
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
                    catch (Exception ex)
                    {
                        MainWindow.ErrorLog.CreateLog(ex.Message);
                    }
                }
                else
                {
                    MainWindow.UpdateScreen.NoUpdate();
                }
            }
        }

        private void AutoUpdaterOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            dynamic json = JsonConvert.DeserializeObject(args.RemoteData);

            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = json.tag_name,
                DownloadURL = json.assets[0].browser_download_url,
            };
        }
    }
}