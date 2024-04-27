
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FortniteBurger.Classes
{
    internal class Settings
	{
        internal static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static string ProfilePath = LocalAppData + "/FortniteBurger/Configs/Profiles";
        private HttpClient WC = new HttpClient();

        internal Settings()
		{
            if (!Directory.Exists(LocalAppData + "/FortniteBurger")) Directory.CreateDirectory(LocalAppData + "/FortniteBurger");
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Settings")) Directory.CreateDirectory(LocalAppData + "/FortniteBurger/Settings");
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Configs")) Directory.CreateDirectory(LocalAppData + "/FortniteBurger/Configs");
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Configs/Profiles")) Directory.CreateDirectory(LocalAppData + "/FortniteBurger/Configs/Profiles");

            WC.Timeout = TimeSpan.FromMinutes(5);

            WC.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Chrome", "115.0.0.0"));
            WC.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Mozilla", "5.0"));
            WC.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppleWebKit", "537.36"));
            WC.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("Safari", "537.36"));

            try
            {
                DownloadSettings();
            }
            catch(Exception e)
            {
                MainWindow.ErrorLog.CreateLog("Failed to download Market Files");
                MainWindow.ErrorLog.CreateLog(e.Message);
            }
        }

        private static string BaseDir = "https://raw.githubusercontent.com/OssieFromDK/DeadByDaylight-Unlocker/main/MarketFiles/";

        internal async Task DownloadSettings()
        {
            await DownloadBytes(BaseDir + "GetAll.json", LocalAppData + "/FortniteBurger/Configs/Profiles/Profile.json");
            await DownloadBytes(BaseDir + "Bloodweb.json", LocalAppData + "/FortniteBurger/Configs/Profiles/Bloodweb.json");
            await DownloadBytes(BaseDir + "Market.json", LocalAppData + "/FortniteBurger/Configs/Profiles/SkinsWithItems.json");
            await DownloadBytes(BaseDir + "MarketDlcOnly.json", LocalAppData + "/FortniteBurger/Configs/Profiles/DlcOnly.json");
            await DownloadBytes(BaseDir + "MarketWithPerks.json", LocalAppData + "/FortniteBurger/Configs/Profiles/SkinsPerks.json");
            await DownloadBytes(BaseDir + "MarketNoSavefile.json", LocalAppData + "/FortniteBurger/Configs/Profiles/SkinsONLY.json");
            await DownloadBytes(BaseDir + "Currency.json", LocalAppData + "/FortniteBurger/Configs/Profiles/Currency.json");
            await DownloadBytes(BaseDir + "Level.json", LocalAppData + "/FortniteBurger/Configs/Profiles/Level.json");
        }

        internal async Task DownloadBytes(string uri, string output)
        {
            byte[] fileBytes = await WC.GetByteArrayAsync(uri);
            File.WriteAllBytes(output, fileBytes);
        }

        internal static void SaveBootTime(long bootTime)
        {
            Dictionary<string, object> SettingsObj = new Dictionary<string, object>()
            {
                ["lastBoot"] = bootTime
            };

            string JSON = JsonConvert.SerializeObject(SettingsObj);

            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Boot.json";
            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JSON);
                }
            }
        }

        internal static long GetLastBootTime()
        {
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Settings")) return 0;
            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Boot.json";
            if (!File.Exists(specificFolder)) return 0;
            string JSON = File.ReadAllText(specificFolder);

            if (string.IsNullOrEmpty(JSON)) return 0;
            Dictionary<string, object> SettingsObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(JSON);

            return (long)SettingsObj["lastBoot"];
        }

        internal static void SaveMods()
        {
            List<string> InstalledMods = new List<string>();

            foreach(KeyValuePair<string, dynamic> KVP in Mods.ModManager.Mods) {
                if((KVP.Value).IsInstalled)
                {
                    InstalledMods.Add(KVP.Key);
                }
            }

            string JSON = JsonConvert.SerializeObject(InstalledMods);

            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Mods.json";
            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JSON);
                }
            }
        }

        internal static void LoadMods()
        {
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Settings")) return;
            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Mods.json";
            if (!File.Exists(specificFolder)) return;
            string JSON = File.ReadAllText(specificFolder);

            if (string.IsNullOrEmpty(JSON)) return;
            List<string> InstalledMods = JsonConvert.DeserializeObject<List<string>>(JSON);

            foreach(string Mod in InstalledMods)
            {
                MainWindow.mods.SetIsInstalled(Mod);
                Mods.ModManager.Mods[Mod].IsInstalled = true;
            }

            Mods.ModManager.LoadModsConfig();
            Mods.ModManager.CheckInstalled();
        }

        internal static void SaveSettings()
        {
            Dictionary<string, object> SettingsObj = new Dictionary<string, object>()
            {
                ["OverlayEnabled"] = MainWindow.settingspage.OverlayEnabled,
                ["PakBypass"] = MainWindow.PakBypass.PakBypassedThisSession,
                ["HideOnLaunch"] = MainWindow.settingspage.HideToTray,
                ["Platform"] = MainWindow.CurrentType,
                ["RPC"] = MainWindow.settingspage.RPC,
            };

            MainWindow.main.Dispatcher.Invoke((Action)(() =>
            {
                SettingsObj["MainX"] = MainWindow.main.Left;
                SettingsObj["MainY"] = MainWindow.main.Top;
            }));

            string JSON = JsonConvert.SerializeObject(SettingsObj);

            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Settings.json";
            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JSON);
                }
            }
        }

        internal static void LoadSettings()
        {
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Settings")) return;
            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Settings.json";
            if (!File.Exists(specificFolder)) return;
            string JSON = File.ReadAllText(specificFolder);

            if (string.IsNullOrEmpty(JSON)) return;
            Dictionary<string, object> SettingsObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(JSON);

            if(SettingsObj.ContainsKey("OverlayEnabled"))
            {
                MainWindow.settingspage.OverlayEnabled = (bool)SettingsObj["OverlayEnabled"];
                MainWindow.settingspage.Overlay_Check.IsChecked = (bool)SettingsObj["OverlayEnabled"];
            }

            if (SettingsObj.ContainsKey("PakBypass"))
                MainWindow.PakBypass.PakBypassedThisSession = (bool)SettingsObj["PakBypass"];

            if (SettingsObj.ContainsKey("MainX"))
                MainWindow.main.Left = (double)SettingsObj["MainX"];

            if (SettingsObj.ContainsKey("MainY"))
                MainWindow.main.Top = (double)SettingsObj["MainY"];

            if (SettingsObj.ContainsKey("HideOnLaunch"))
            {
                MainWindow.settingspage.HideToTray = (bool)SettingsObj["HideOnLaunch"];
                MainWindow.settingspage.Sys_Check.IsChecked = (bool)SettingsObj["HideOnLaunch"];
            }

            if (SettingsObj.ContainsKey("Platform"))
            {
                MainWindow.CurrentType = (string)SettingsObj["Platform"];
                MainWindow.settingspage.UpdateTypeBox((string)SettingsObj["Platform"]);
            }

            if(SettingsObj.ContainsKey("RPC"))
            {
                MainWindow.settingspage.RPC = (bool)SettingsObj["RPC"];
                MainWindow.settingspage.RPC_Check.IsChecked = (bool)SettingsObj["RPC"];
            }

            Mods.ModManager.UpdateEngine();
            MainWindow.PakBypass.CheckForReboot();
        }

        internal static void SaveConfig()
        {
            Dictionary<string, object> SettingsObj = new Dictionary<string, object>();

            MainWindow.main.Dispatcher.Invoke((Action)(() =>
            {
                SettingsObj["PrestigeLevel"] = MainWindow.profile.PrestigeLevelBox.Text;
                SettingsObj["ItemAmount"] = MainWindow.profile.ItemAmountBox.Text;
                SettingsObj["Profile_Type"] = MainWindow.profile.GetProfileType();
                SettingsObj["Currency_Spoof"] = MainWindow.profile.Currency_Spoof;
                SettingsObj["Level_Spoof"] = MainWindow.profile.Level_Spoof;
            }));

            string JSON = JsonConvert.SerializeObject(SettingsObj);

            string specificFolder = LocalAppData + "/FortniteBurger/Configs/Profile.json";
            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JSON);
                }
            }
        }

        internal static void LoadConfig()
        {
            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Configs")) return;
            string specificFolder = LocalAppData + "/FortniteBurger/Configs/Profile.json";
            if (!File.Exists(specificFolder)) return;
            string JSON = File.ReadAllText(specificFolder);

            if (string.IsNullOrEmpty(JSON)) return;
            Dictionary<string, object> SettingsObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(JSON);

            if (SettingsObj.ContainsKey("Profile_Type"))
            {
                int ProfileType = (int)(long)SettingsObj["Profile_Type"];
                MainWindow.profile.SetProfileType(ProfileType);
            }

            if (SettingsObj.ContainsKey("PrestigeLevel"))
                MainWindow.profile.PrestigeLevelBox.Text = (string)SettingsObj["PrestigeLevel"];

            if (SettingsObj.ContainsKey("ItemAmount"))
                MainWindow.profile.ItemAmountBox.Text = (string)SettingsObj["ItemAmount"];

            if (SettingsObj.ContainsKey("Currency_Spoof"))
            {
                MainWindow.profile.Currency_Spoof = (bool)SettingsObj["Currency_Spoof"];
                MainWindow.profile.CurrencySpoof.IsChecked = (bool)SettingsObj["Currency_Spoof"];
            }

            if (SettingsObj.ContainsKey("Level_Spoof"))
            {
                MainWindow.profile.Level_Spoof = (bool)SettingsObj["Level_Spoof"];
                MainWindow.profile.LevelSpoof.IsChecked = (bool)SettingsObj["Level_Spoof"];
            }
        }
    }
}

