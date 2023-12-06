using System;
using System.Diagnostics;
using System.IO;

namespace FortniteBurger.Classes
{
    internal class Worker
    {
        internal static void LoadWorker()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "/BurgerWorker.exe";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllBytes(path, Properties.Resources.BurgerWorker);

            Process.Start(new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true
            });
        }
    } 
}
