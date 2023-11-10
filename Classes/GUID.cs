using System;
using System.IO;

namespace FortniteBurger.Classes
{
    internal class GUID
    {
        private static string CurrGUID = null;

        static GUID()
        {
            if (!Directory.Exists(Settings.LocalAppData + "/FortniteBurger/Settings"))
                Directory.CreateDirectory(Settings.LocalAppData + "/FortniteBurger/Settings");

            if (!File.Exists(Settings.LocalAppData + "/FortniteBurger/Settings/UUID.txt"))
                File.Create(Settings.LocalAppData + "/FortniteBurger/Settings/UUID.txt").Close();

            string Text = File.ReadAllText(Settings.LocalAppData + "/FortniteBurger/Settings/UUID.txt");

            if (!string.IsNullOrEmpty(Text))
            {
                CurrGUID = Text;
            }
            else
            {
                string NewGUID = GenerateGUID();

                using (var fs = File.Open(Settings.LocalAppData + "/FortniteBurger/Settings/UUID.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.SetLength(0);
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(NewGUID);
                    }
                }

                CurrGUID = NewGUID;
            }
        }

        private static string GenerateGUID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        internal static string GetGUID()
        {
            return CurrGUID;
        }
    }
}
