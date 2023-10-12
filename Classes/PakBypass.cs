using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FortniteBurger.Classes
{
    internal class PakBypass
    {
        internal bool PakBypassedThisSession = false;

        internal void CheckForReboot()
        {
            long savedLastBootTime = Settings.GetLastBootTime();
            long currentTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            long lastBootTime = currentTime - Environment.TickCount64;

            if (lastBootTime > savedLastBootTime)
            {
                PakBypassedThisSession = false;
            }

            Settings.SaveBootTime(lastBootTime);
        }

        private async Task CheckForSSL()
        {
            string AppDir = Environment.CurrentDirectory;
            string DBDPath = Path.Combine(AppDir, "dbdPath.txt");

            if(File.Exists(DBDPath))
            {
                string DBDInstallPath = File.ReadAllText(DBDPath);
                int Index = DBDInstallPath.IndexOf("\\Binaries");
                string DBDContentPath = DBDInstallPath.Substring(0, Index) + "\\Content\\Paks";
                string FinalPath = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.pak");

                if (File.Exists(FinalPath)) File.Delete(FinalPath);
            }
        }

        internal async Task LoadPakBypass() 
        {
            await CheckForSSL();

            string AppDir = Environment.CurrentDirectory;
            if (!Directory.Exists(AppDir + "/Paks")) Directory.CreateDirectory(AppDir + "/Paks");
            string Paksdir = Path.Combine(AppDir, "Paks/pakchunk4174-WindowsNoEditor.pak");
            if (!File.Exists(Paksdir))
            {
                File.WriteAllBytes(Paksdir, Properties.Resources.SSLBypass);
            }

            string path = Path.Combine(Path.GetTempPath(), "PakBypass.exe");
            File.WriteAllBytes(path, Properties.Resources.PakBypass);
            Process PakBypassProcess = Process.Start(path);

            while (!PakBypassProcess.HasExited)
            {
                await Task.Delay(1000);
            }

            File.Delete(path);
            File.Delete(Paksdir);
            Directory.Delete(AppDir + "/Paks");

            PakBypassedThisSession = true;
        }

        internal async Task LoadSSLBypass()
        {
            string AppDir = Environment.CurrentDirectory;
            string DBDPath = Path.Combine(AppDir, "dbdPath.txt");;
            string DBDInstallPath = File.ReadAllText(DBDPath);

            if (DBDInstallPath.Contains("Win64"))
            {
                int Index = DBDInstallPath.IndexOf("\\Binaries");
                string DBDContentPath = DBDInstallPath.Substring(0, Index) + "\\Content\\Paks";
                string FinalPath = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.pak");
                string ToDelete = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.bak");
                string ToDeleteSig = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.sig");
                string ToDeleteKek = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.kek");

                if (File.Exists(ToDelete)) 
                    File.Delete(ToDelete);

                if (File.Exists(ToDeleteSig))
                    File.Delete(ToDeleteSig);

                if (File.Exists(ToDeleteKek))
                    File.Delete(ToDeleteKek);

                File.WriteAllBytes(FinalPath, Properties.Resources.SSLBypass);
            }
        }
    }
}
