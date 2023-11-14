using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteBurger.Classes
{
    internal class Launcher
    {
        private static Dictionary<string, string> LaunchStrings = new Dictionary<string, string>()
        {
            ["Steam"] = "steam://run/381210",
            ["EGS"] = "com.epicgames.launcher://apps/611482b8586142cda48a0786eb8a127c%3A467a7bed47ec44d9b1c9da0c2dae58f7%3ABrill?action=launch&silent=true",
            ["MS"] = "shell:appsfolder\\BehaviourInteractive.DeadbyDaylightWindows_b1gz2xhdanwfm!AppDeadByDaylightShipping"
        };

        internal static void LaunchWeb(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        internal static void LaunchDBD(string type)
        {
            Process[] processesByName = Process.GetProcessesByName(Utils.ProccessNames[type]);
            if (processesByName.Length == 0)
            {
                LaunchWeb(LaunchStrings[type]);
            }
            else
            {
                foreach (Process process in processesByName)
                {
                    process.Kill();
                    LaunchWeb(LaunchStrings[type]);
                }
            }
        } 

        internal static void KillDBD(string type)
        {
            Process[] processesByName = Process.GetProcessesByName(Utils.ProccessNames[type]);
            if (processesByName.Length > 0)
            {
                foreach (Process process in processesByName)
                {
                    process.Kill();
                }
            }
        }
    }
}
