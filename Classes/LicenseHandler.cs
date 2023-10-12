using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace FortniteBurger.Classes
{
    internal class LicenseHandler
    {
        internal async static void CheckLicense(string license)
        {
            MainWindow.login.LoginReturn("Successfully Validated License", true);
        }
        private static void SaveLicense(string license)
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(LocalAppData + "/FortniteBurger")) Directory.CreateDirectory(LocalAppData + "/FortniteBurger");
            string specificFolder = LocalAppData + "/FortniteBurger/license.txt";
            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0); // Empty File
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(license); // Add License
                }
            }
        }

        internal static bool CheckForAppDataLicense()
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(LocalAppData + "/FortniteBurger")) return false;
            string specificFolder = LocalAppData + "/FortniteBurger/license.txt";
            if(!File.Exists(specificFolder)) return false;

            return true;
        }

        internal static string GetAppDataLicense()
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string specificFolder = LocalAppData + "/FortniteBurger/license.txt";
            return File.ReadAllText(specificFolder);
        }
    }
}
