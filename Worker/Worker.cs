using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using NetDiscordRpc;
using Newtonsoft.Json;
using NetDiscordRpc.RPC;

namespace BurgerWorker
{
    internal class Worker
    {
        private static DiscordRPC? RPCClient;
        private static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static HttpClient ApiClient = new()
        {
            BaseAddress = new Uri("https://burger.ossie.dk/api/")
        };
        private static bool UseRPC = true;

        static void Main(string[] args)
        {
            RPCClient = new DiscordRPC("1173063895582257162");
            ApiClient.DefaultRequestHeaders.UserAgent.ParseAdd("burger");
            bool RPCActive = false;
            bool HasSet = false;
            Process[] BurgerProcesses = Process.GetProcessesByName("FortniteBurger");
            Process? BurgerProcess = null;

            if (BurgerProcesses.Length > 0)
            {
                BurgerProcess = BurgerProcesses[0];
            }
            else
            {
                ShutDown(RPCClient);
                Environment.Exit(0);
            }

            while (true)
            {
                CheckForRPCSettings();

                if (!BurgerProcess.HasExited)
                {
                    // Remove Proxy Settings
                    if (!RPCActive && UseRPC)
                    {
                        if (!HasSet)
                        {
                            RPCClient.Initialize();
                            HasSet = true;
                        }

                        RPCClient.SetPresence(new RichPresence()
                        {
                            Details = "Playing Dead By Daylight",
                            State = "Unlocking items, skins, perks & characters",
                            Timestamps = new Timestamps(DateTime.UtcNow),
                            Assets = new Assets()
                            {
                                LargeImageKey = "noburger",
                                LargeImageText = "Fortnite Burger - Free Unlocker",
                            },
                            Buttons = new Button[]
                            {
                                new() { Label = "Discord", Url = "https://discord.gg/gDWBGtVkKJ" },
                                new() { Label = "Download", Url = "https://github.com/OssieFromDK/DeadByDaylight-Unlocker/releases/latest" }
                            }
                        });

                        RPCActive = true;
                    }
                    else
                    {
                        if(RPCActive && !UseRPC)
                        {
                            RPCClient.ClearPresence();
                            RPCActive = false;
                        }
                    }

                }
                else
                {
                    ShutDown(RPCClient);
                    break;
                }

                Thread.Sleep(1000);
            }

            Environment.Exit(0);
        }

        internal async static void RemoveUser(string GUID)
        {
            using HttpResponseMessage response = await ApiClient.PostAsync(
                "counter",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("type", "remove"),
                    new KeyValuePair<string, string>("guid", GUID),
                })
            );
        }

        internal static void CheckForRPCSettings()
        {

            if (!Directory.Exists(LocalAppData + "/FortniteBurger/Settings")) return;
            string specificFolder = LocalAppData + "/FortniteBurger/Settings/Settings.json";
            if (!File.Exists(specificFolder)) return;
            string JSON = File.ReadAllText(specificFolder);

            if (string.IsNullOrEmpty(JSON)) return;
            Dictionary<string, object> SettingsObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(JSON);

            if (SettingsObj.ContainsKey("RPC"))
            {
                UseRPC = (bool)SettingsObj["RPC"];
            }
        }

        internal static void ShutDown(DiscordRPC rpc)
        {
            try
            {
                RegistryKey RegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

                if (RegKey != null)
                {
                    object ProxyEnabled = RegKey?.GetValue("ProxyEnable");

                    if (ProxyEnabled?.ToString() == "1")
                    {
                        RegKey?.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex) { }


            // Remove User Count
            try
            {
                if (Directory.Exists(LocalAppData + "/FortniteBurger/Settings"))
                {
                    if (File.Exists(LocalAppData + "/FortniteBurger/Settings/UUID.txt"))
                    {
                        string GUID = File.ReadAllText(LocalAppData + "/FortniteBurger/Settings/UUID.txt");

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            RemoveUser(GUID);
                        }
                    }
                }
            }
            catch (Exception ex) { }


            // Shut down RPC
            try
            {
                rpc.Dispose();
            }
            catch (Exception ex) { }
        }
    }
}