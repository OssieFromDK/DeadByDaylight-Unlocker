using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace FortniteBurger.Classes
{
    internal class Utils
    {
        internal static Dictionary<string, string> ProccessNames = new Dictionary<string, string>()
        {
            ["Steam"] = "DeadByDaylight-Win64-Shipping",
            ["EGS"] = "DeadByDaylight-EGS-Shipping",
            ["MS"] = "DeadByDaylight-WinGDK-Shipping"
        };

        internal static int CalculateMMR(string input)
        {
            return Convert.ToInt32(double.Parse(input.Split(',')[0], CultureInfo.InvariantCulture));
        }

        internal static int CalculateETA(string input)
        {
            double num = Math.Abs(Math.Round(double.Parse(input.Split(',')[0], CultureInfo.InvariantCulture) / 1000.0));
            return Convert.ToInt32(num);
        }

        private static string FullProfilePath = Settings.ProfilePath + "/Profile.json";
        private static string BloodWebPath = Settings.ProfilePath + "/Bloodweb.json";
        private static string SkinsItemsPath = Settings.ProfilePath + "/SkinsWithItems.json";

        internal static void DoFileCheck()
        {
            if(!File.Exists(FullProfilePath))
                RewriteMarketFiles();

            if (!File.Exists(BloodWebPath))
                RewriteMarketFiles();

            if (!File.Exists(SkinsItemsPath))
                RewriteMarketFiles();
        }

        private static void RewriteMarketFiles()
        {
            File.WriteAllBytes(Settings.ProfilePath + "/Profile.json", Properties.Resources.Profile);
            File.WriteAllBytes(Settings.ProfilePath + "/Bloodweb.json", Properties.Resources.Bloodweb);
            File.WriteAllBytes(Settings.ProfilePath + "/SkinsWithItems.json", Properties.Resources.SkinsWithItems);
            File.WriteAllBytes(Settings.ProfilePath + "/DlcOnly.json", Properties.Resources.DlcOnly);
            File.WriteAllBytes(Settings.ProfilePath + "/SkinsPerks.json", Properties.Resources.SkinsPerks);
            File.WriteAllBytes(Settings.ProfilePath + "/SkinsONLY.json", Properties.Resources.SkinsONLY);
            File.WriteAllBytes(Settings.ProfilePath + "/Currency.json", Properties.Resources.Currency);
            File.WriteAllBytes(Settings.ProfilePath + "/Level.json", Properties.Resources.Level);

            MainWindow.ErrorLog.CreateLog("We were unable to fetch the latest market files, so the embedded ones were used instead.");
        }

        internal static void UpdateProfiles(int Prestige, int itemAmount)
        {
            int realItemAmount = itemAmount / 2;

            string Profile_Text = File.ReadAllText(FullProfilePath);
            JObject Profile_JSON = JsonConvert.DeserializeObject<JObject>(Profile_Text);
            string Bloodweb_Text = File.ReadAllText(BloodWebPath);
            JObject Bloodweb_JSON = JsonConvert.DeserializeObject<JObject>(Bloodweb_Text);
            string SkinsItems_Text = File.ReadAllText(SkinsItemsPath);
            JObject SkinsItems_JSON = JsonConvert.DeserializeObject<JObject>(SkinsItems_Text);

            for (int i = 0; i < Profile_JSON["List"].Count(); i++)
            {
                Profile_JSON["List"][i]["PrestigeLevel"] = Prestige;

                for (int q = 0; q < Profile_JSON["List"][i]["CharacterItems"].Count(); q++)
                {
                    if ((int)Profile_JSON["List"][i]["CharacterItems"][q]["Quantity"] > 3)
                    {
                        Profile_JSON["List"][i]["CharacterItems"][q]["Quantity"] = realItemAmount;
                    }
                }
            }

            Bloodweb_JSON["PrestigeLevel"] = Prestige;
            for (int i = 0; i < Bloodweb_JSON["CharacterItems"].Count(); i++)
            {
                if ((int)Bloodweb_JSON["CharacterItems"][i]["Quantity"] > 3)
                {
                    Bloodweb_JSON["CharacterItems"][i]["Quantity"] = realItemAmount;
                }
            }

            for (int i = 0; i < SkinsItems_JSON["Data"]["Inventory"].Count(); i++)
            {
                if ((int)SkinsItems_JSON["Data"]["Inventory"][i]["Quantity"] > 3)
                {
                    SkinsItems_JSON["Data"]["Inventory"][i]["Quantity"] = realItemAmount;
                }
            }

            string Profile_Text_After = JsonConvert.SerializeObject(Profile_JSON);
            string Bloodweb_Text_After = JsonConvert.SerializeObject(Bloodweb_JSON);
            string SkinsItems_Text_After = JsonConvert.SerializeObject(SkinsItems_JSON);

            WriteText(FullProfilePath, Profile_Text_After);
            WriteText(BloodWebPath, Bloodweb_Text_After);
            WriteText(SkinsItemsPath, SkinsItems_Text_After);
        }

        private static void WriteText(string path, string text)
        {
            using (var fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(text);
                }
            }
        }

        internal static string GetGamePakDir()
        {
            string AppDir = Environment.CurrentDirectory;
            string DBDPath = Path.Combine(AppDir, "dbdPath.txt"); ;

            if (!File.Exists(DBDPath)) return null;

            string DBDInstallPath = File.ReadAllText(DBDPath);

            int Index = DBDInstallPath.IndexOf("\\Binaries");
            string DBDContentPath = DBDInstallPath.Substring(0, Index) + "\\Content\\Paks";

            return DBDContentPath;
        }

        internal static string GetGameINIDir()
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string ConfigPath = LocalAppData + "\\DeadByDaylight\\Saved\\Config";

            bool EGS = MainWindow.CurrentType == "EGS";

            string FinalPath = null;

            if (EGS) FinalPath = ConfigPath + "\\EGS";

            if (!EGS)
            {
                if(Directory.Exists(ConfigPath + "\\WindowsNoEditor"))
                {
                    FinalPath = ConfigPath + "\\WindowsNoEditor";
                }
                else
                {
                    FinalPath = ConfigPath + "\\WindowsClient";
                }
            }

            return FinalPath;
        }

        internal static bool IsGameCurrentlyRunning(string TYPE)
        {
            Process[] processesByName = Process.GetProcessesByName(ProccessNames[TYPE]);
            
            if (processesByName.Length > 0) return true;

            return false;
        }

        internal static async Task CheckForGameRunning(string TYPE)
        {
            int I = 0;
            bool Found = false;
            Process[] processesByName;

            while (I <= 120)
            {
                processesByName = Process.GetProcessesByName(ProccessNames[TYPE]);

                await Task.Delay(1000);
                I++;
                if (processesByName.Length > 0)
                {
                    MainWindow.main.ReturnFromLaunch("Successfully Started", true, TYPE);
                    Found = true;
                    break;
                }
            }

            if (!Found)
            {
                MainWindow.main.ReturnFromLaunch("Couldn't start... Try Again", false, TYPE);
            }
        }

        internal static bool IsPakBypassRunning()
        {
            Process[] processesByName = Process.GetProcessesByName("PakBypass");

            if (processesByName.Length > 0) return true;

            return false;
        }

        internal static void UpdatedBloodweb(string returnVal)
        {
            string path = Settings.ProfilePath + "/Bloodweb.json";
            int num = 0;
            string BloodWebCharacterName = (string)JObject.Parse(returnVal)["characterName"];
            string BloodWebJson = System.IO.File.ReadAllText(Settings.ProfilePath + "/Bloodweb.json");
            string ProfileJson = System.IO.File.ReadAllText(Settings.ProfilePath + "/Profile.json");
            JObject BloodWebObject = JObject.Parse(BloodWebJson);
            foreach (JToken ProfileToken in JObject.Parse(ProfileJson)["list"])
            {
                if ((string)ProfileToken["characterName"] == BloodWebCharacterName)
                {
                    BloodWebObject["prestigeLevel"] = (JToken)(int)ProfileToken["prestigeLevel"];
                    foreach (JToken ProfileTokenItems in ProfileToken["characterItems"])
                    {
                        if ((string)ProfileTokenItems["itemId"] == "AzarovKey")
                            num = (int)ProfileTokenItems["quantity"];
                    }
                }
            }

            bool flag = false;
            foreach (JToken BloodWebToken in BloodWebObject["characterItems"])
            {
                string ItemId = (string)BloodWebToken["itemId"];
                if (ItemId == "Item_Camper_AlexsToolbox")
                    flag = true;
                if (flag)
                    BloodWebToken["quantity"] = (JToken)num;
                if (ItemId == "Anniversary2023Offering")
                    break;
            }


            File.WriteAllText(path, BloodWebObject.ToString());
        }

        private class SteamRootobject
        {
            internal string providerId { get; set; }

            internal string provider { get; set; }
        }

        internal static string TranslateCharacter(string name)
        {
            string str;
            return !new Dictionary<string, string>()
            {
                {
                    "Nightmare",
                    "Freddy"
                },
                {
                    "Spirit",
                    "Spirit"
                },
                {
                    "K20",
                    "Pyramid Head"
                },
                {
                    "K21",
                    "Blight"
                },
                {
                    "K22",
                    "Twins"
                },
                {
                    "K23",
                    "Trickster"
                },
                {
                    "K24",
                    "Nemesis"
                },
                {
                    "K25",
                    "Pinhead"
                },
                {
                    "K26",
                    "Artist"
                },
                {
                    "K27",
                    "Onryo"
                },
                {
                    "K28",
                    "Dredge"
                },
                {
                    "K29",
                    "Albert Wesker"
                },
                {
                    "K30",
                    "Knight"
                },
                {
                    "K31",
                    "Skull Merchant"
                },
                {
                    "K32",
                    "Singularity"
                },
                {
                    "K33",
                    "Xenomorph"
                },
                {
                    "Chuckles",
                    "Trapper"
                },
                {
                    "Ghostface",
                    "Ghostface"
                },
                {
                    "Plague",
                    "Plague"
                },
                {
                    "Bob",
                    "Wraith"
                },
                {
                    "Smoke",
                    "David"
                },
                {
                    "Eric",
                    "Tapp"
                },
                {
                    "Bear",
                    "Huntress"
                },
                {
                    "Cannibal",
                    "Bubba"
                },
                {
                    "Shape",
                    "Myers"
                },
                {
                    "Killer07",
                    "Doctor"
                },
                {
                    "Harpie",
                    "Hag"
                },
                {
                    "Pig",
                    "Pig"
                },
                {
                    "Clown",
                    "Clown"
                },
                {
                    "Legion",
                    "Legion"
                },
                {
                    "K17",
                    "Demogorgon"
                },
                {
                    "Oni",
                    "Oni"
                },
                {
                    "GunSlinger",
                    "Deahtslinger"
                },
                {
                    "HillBilly",
                    "Hillbilly"
                },
                {
                    "Nurse",
                    "Nurse"
                },
                {
                    "S22",
                    "Cheryl"
                },
                {
                    "S23",
                    "Felix"
                },
                {
                    "S24",
                    "Élodie"
                },
                {
                    "S25",
                    "Yun-Jin"
                },
                {
                    "S26",
                    "Jill"
                },
                {
                    "S27",
                    "Leon"
                },
                {
                    "S28",
                    "Mikaela"
                },
                {
                    "S29",
                    "Jonah"
                },
                {
                    "S30",
                    "Yoichi"
                },
                {
                    "S31",
                    "Haddie"
                },
                {
                    "S32",
                    "Ada"
                },
                {
                    "S33",
                    "Rebecca"
                },
                {
                    "S34",
                    "Vittorio"
                },
                {
                    "S35",
                    "Thalita"
                },
                {
                    "S36",
                    "Renato"
                },
                {
                    "S37",
                    "Nicolas Cage"
                },
                {
                    "S38",
                    "New Survivor"
                }
            }.TryGetValue(name, out str) ? name : str;
        }

        internal static string TranslateCountry(string country)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
                {
                  "CZ",
                  "Czech Republic"
                },
                {
                  "SK",
                  "Slovakia"
                },
                {
                  "FR",
                  "France"
                },
                {
                  "DE",
                  "Germany"
                },
                {
                  "FI",
                  "Finland"
                },
                {
                  "RU",
                  "Russia"
                },
                {
                  "CH",
                  "Switzerland"
                },
                {
                  "US",
                  "USA"
                },
                {
                  "PL",
                  "Poland"
                },
                {
                  "UA",
                  "Ukraine"
                },
                {
                  "GB",
                  "Great Britain"
                },
                {
                  "AT",
                  "Austria"
                },
                {
                  "HU",
                  "Hungary"
                },
                {
                  "DK",
                  "Denmark"
                },
                {
                  "SI",
                  "Slovenia"
                },
                {
                  "NO",
                  "Norway"
                },
                {
                  "SE",
                  "Sweden"
                },
                {
                  "BG",
                  "Belgium"
                },
                {
                  "NL",
                  "Netherlands"
                },
                {
                  "ES",
                  "Spain"
                },
                {
                  "PT",
                  "Portugal"
                },
                {
                  "IT",
                  "Italia"
                },
                {
                  "GR",
                  "Greece"
                },
                {
                  "IS",
                  "Iceland"
                },
                {
                  "HR",
                  "Croatia"
                },
                {
                  "RS",
                  "Serbia"
                },
                {
                  "RO",
                  "Romania"
                },
                {
                  "CA",
                  "Canada"
                },
                {
                  "LU",
                  "Luxembourg"
                },
                {
                  "CN",
                  "China"
                },
                {
                  "JN",
                  "Japan"
                },
                {
                  "BR",
                  "Brazil"
                },
                {
                  "MX",
                  "Mexico"
                },
                {
                  "SA",
                  "Saudi Arabia"
                }
            };
            return dictionary.ContainsKey(country) ? dictionary[country] : country;
        }
    }
}

