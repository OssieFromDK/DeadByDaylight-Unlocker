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
                string ToFind = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.bak");
                string ToFindSig = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.sig");
                string ToFindKek = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.kek");

                if (File.Exists(ToFind)) File.Delete(ToFind);
                if (File.Exists(ToFindSig)) File.Delete(ToFindSig);
                if (File.Exists(ToFindKek)) File.Delete(ToFindKek);
            }
        }

        internal async Task LoadPakBypass() 
        {
            await CheckForSSL();

            string AppDir = Environment.CurrentDirectory;
            if (!Directory.Exists(AppDir + "/Mods")) Directory.CreateDirectory(AppDir + "/Mods");
            string Paksdir = Path.Combine(AppDir, "Mods/pakchunk4174-WindowsNoEditor.pak");
            if (!File.Exists(Paksdir))
            {
                File.WriteAllBytes(Paksdir, Properties.Resources.SSLBypass);
            }

            // Load Custom Mods
            if (Directory.Exists(AppDir + "/CustomMods"))
            {
                string[] Files = Directory.GetFiles(AppDir + "/CustomMods");
                foreach (string FileLocation in Files)
                {
                    string FileName = Path.GetFileName(FileLocation);
                    string NewFileName = MainWindow.CurrentType == "EGS" ? FileName.Replace("WindowsNoEditor", "EGS") : FileName.Replace("EGS", "WindowsNoEditor");
                    File.Copy(FileLocation, AppDir + "/Mods/" + NewFileName);
                }
            }

            string path = Path.Combine(Path.GetTempPath(), "PakBypass.exe");
            File.WriteAllBytes(path, Properties.Resources.PakBypass);
            Process PakBypassProcess = Process.Start(path);

            while (!PakBypassProcess.HasExited)
            {
                await Task.Delay(1000);
            }

            File.Delete(path);
            Directory.Delete(AppDir + "/Mods", true);
            PakBypassedThisSession = true;
        }

        internal async Task LoadSSLBypass()
        {
            string AppDir = Environment.CurrentDirectory;
            string DBDPath = Path.Combine(AppDir, "dbdPath.txt");;
            if (File.Exists(DBDPath))
            {
                string DBDInstallPath = File.ReadAllText(DBDPath);

                if (DBDInstallPath.Contains("Win64"))
                {
                    int Index = DBDInstallPath.IndexOf("\\Binaries");
                    string DBDContentPath = DBDInstallPath.Substring(0, Index) + "\\Content\\Paks";
                    string ToFind = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.bak");
                    string ToFindSig = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.sig");
                    string ToFindKek = Path.Combine(DBDContentPath, "pakchunk4174-WindowsNoEditor.kek");

                    if (!File.Exists(ToFind))
                        File.WriteAllBytes(ToFind, Properties.Resources.SSLBypass);

                    if (!File.Exists(ToFindSig))
                        File.WriteAllBytes(ToFindSig, Properties.Resources.Signature);

                    if (!File.Exists(ToFindKek))
                        File.WriteAllBytes(ToFindKek, Properties.Resources.Signature_KEK);
                }
            }
        }
    }
}
