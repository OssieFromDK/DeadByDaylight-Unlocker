using FortniteBurger.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FortniteBurger.Classes.Mods
{
    internal class ModManager
    {
        static IniFile EngineIni;
        internal static bool HasInstalledNewMods = false;

        internal ModManager()
        {
            EngineIni = new IniFile(Utils.GetGameINIDir() + "\\Engine.ini");
        }

        internal static void UpdateEngine()
        {
            EngineIni = new IniFile(Utils.GetGameINIDir() + "\\Engine.ini");
        }

        internal static Dictionary<string, dynamic> Mods = new Dictionary<string, dynamic>()
        {
            ["Core"] = new Core(),
            ["Corn"] = new CornRemover(),
            ["FOV"] = new FOV(),
            ["KR"] = new KillerDetector(),
            ["RS"] = new RedStain(),
            ["RES"] = new StretchedRes(),
            ["OA"] = new OldAnim(),
        };

        internal static void LoadModsConfig()
        {
            foreach (KeyValuePair<string, dynamic> kvp in Mods)
            {
                if (!kvp.Value.IsInstalled) continue;

                Dictionary<string, string> ModConfig = new Dictionary<string, string>();

                if (!kvp.Value.HasINI) continue;

                foreach (KeyValuePair<string, string> KVP in kvp.Value.INIValues)
                {
                    if (EngineIni.KeyExists(KVP.Key, kvp.Value.INIHeader))
                    {
                        ModConfig[KVP.Key] = EngineIni.Read(KVP.Key, kvp.Value.INIHeader);
                    }
                }

                if (ModConfig.Count > 0)
                {
                    MainWindow.mods.SetIsInstalled(kvp.Key, ModConfig);
                }
            }
        }

        internal static void CheckInstalled()
        {
            foreach (KeyValuePair<string, dynamic> KVP in Mods)
            {
                if(KVP.Value.IsInstalled)
                {
                    string PakDir = Utils.GetGamePakDir();

                    if (string.IsNullOrEmpty(PakDir)) return;

                    bool EGS = MainWindow.CurrentType == "EGS";

                    string PakInstall = null;
                    string SigInstall = null;
                    string KekInstall = null;

                    if (EGS)
                    {
                        PakInstall = Path.Combine(PakDir, KVP.Value.PakNameEGS + ".bak");
                        SigInstall = Path.Combine(PakDir, KVP.Value.PakNameEGS + ".sig");
                        KekInstall = Path.Combine(PakDir, KVP.Value.PakNameEGS + ".kek");
                    }
                    else
                    {
                        PakInstall = Path.Combine(PakDir, KVP.Value.PakNameSteam + ".bak");
                        SigInstall = Path.Combine(PakDir, KVP.Value.PakNameSteam + ".sig");
                        KekInstall = Path.Combine(PakDir, KVP.Value.PakNameSteam + ".kek");
                    }

                    if (!File.Exists(PakInstall))
                        File.WriteAllBytes(PakInstall, KVP.Value.PakResource);

                    if (!File.Exists(SigInstall))
                        File.WriteAllBytes(SigInstall, Resources.Signature);

                    if (!File.Exists(KekInstall))
                        File.WriteAllBytes(KekInstall, Resources.Signature_KEK);
                }
            }
        }

        internal static void InstallINI(dynamic SelectedMod)
        {
            foreach (KeyValuePair<string, string> KVP in SelectedMod.INIValues) 
            {
                EngineIni.Write(KVP.Key, KVP.Value, SelectedMod.INIHeader);
            }
        }

        internal static void EditINI(string ModName, dynamic Values)
        {
            if (!Mods.ContainsKey(ModName)) return;

            dynamic FoundMod = Mods[ModName];

            foreach (KeyValuePair<string, string> KVP in Values)
            {
                EngineIni.Write(KVP.Key, KVP.Value, FoundMod.INIHeader);
            }
        }

        internal static void DeleteINI(string ModName)
        {
            if (!Mods.ContainsKey(ModName)) return;

            dynamic FoundMod = Mods[ModName];

            EngineIni.DeleteSection(FoundMod.INIHeader);
        }

        internal static void InstallMod(string ModName)
        {
            if (!Mods.ContainsKey(ModName)) return;

            string PakDir = Utils.GetGamePakDir();

            if (string.IsNullOrEmpty(PakDir)) return;

            dynamic FoundMod = Mods[ModName];

            if(FoundMod.HasINI)
            {
                InstallINI(FoundMod);
            }

            bool EGS = MainWindow.CurrentType == "EGS";

            string PakInstall = null;
            string SigInstall = null;
            string KekInstall = null;

            if (EGS)
            {
                PakInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".bak");
                SigInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".sig");
                KekInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".kek");
            }
            else
            {
                PakInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".bak");
                SigInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".sig");
                KekInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".kek");
            }

            File.WriteAllBytes(PakInstall, FoundMod.PakResource);
            File.WriteAllBytes(SigInstall, Resources.Signature);
            File.WriteAllBytes(KekInstall, Resources.Signature_KEK);


            FoundMod.IsInstalled = true;

            if (FoundMod.RequireCore && !Mods["Core"].IsInstalled)
            {

                dynamic CoreMod = Mods["Core"];

                string CorePakInstall = null;
                string CoreSigInstall = null;
                string CoreKekInstall = null;

                if (EGS)
                {
                    CorePakInstall = Path.Combine(PakDir, CoreMod.PakNameEGS + ".bak");
                    CoreSigInstall = Path.Combine(PakDir, CoreMod.PakNameEGS + ".sig");
                    CoreKekInstall = Path.Combine(PakDir, CoreMod.PakNameEGS + ".kek");
                }
                else
                {
                    CorePakInstall = Path.Combine(PakDir, CoreMod.PakNameSteam + ".bak");
                    CoreSigInstall = Path.Combine(PakDir, CoreMod.PakNameSteam + ".sig");
                    CoreKekInstall = Path.Combine(PakDir, CoreMod.PakNameSteam + ".kek");
                }
                if(!File.Exists(CorePakInstall))
                    File.WriteAllBytes(CorePakInstall, CoreMod.PakResource);

                if(!File.Exists(CoreSigInstall))
                    File.WriteAllBytes(CoreSigInstall, Resources.Signature);

                if (!File.Exists(CoreKekInstall))
                    File.WriteAllBytes(CoreKekInstall, Resources.Signature_KEK);

                CoreMod.IsInstalled = true;
            }

            HasInstalledNewMods = true;
        }

        internal static void DeleteMod(string ModName)
        {

            if (!Mods.ContainsKey(ModName)) return;

            string PakDir = Utils.GetGamePakDir();

            if (string.IsNullOrEmpty(PakDir)) return;

            dynamic FoundMod = Mods[ModName];

            if (FoundMod.HasINI)
            {
                DeleteINI(ModName);
            }

            bool EGS = MainWindow.CurrentType == "EGS";

            string PakInstall = null;
            string SigInstall = null;
            string KekInstall = null;

            if (EGS)
            {
                PakInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".bak");
                SigInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".sig");
                KekInstall = Path.Combine(PakDir, FoundMod.PakNameEGS + ".kek");
            }
            else
            {
                PakInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".bak");
                SigInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".sig");
                KekInstall = Path.Combine(PakDir, FoundMod.PakNameSteam + ".kek");
            }

            if (File.Exists(PakInstall))
                File.Delete(PakInstall);

            if (File.Exists(SigInstall))
                File.Delete(SigInstall);

            if (File.Exists(KekInstall))
                File.Delete(KekInstall);

            FoundMod.IsInstalled = false;

            bool FoundOtherMods = false;

            foreach (KeyValuePair<string, dynamic> KVP in Mods)
            {
                if ((KVP.Value).IsInstalled && KVP.Key != "Core")
                {
                    FoundOtherMods = true;
                }
            }

            if (!FoundOtherMods && ModName != "Core")
            {
                DeleteMod("Core");
            }
        }

        internal static void AddCustomMod(string FileLocation)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "/CustomMods"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "/CustomMods");

            string FileName = Path.GetFileName(FileLocation);

            if (File.Exists(Environment.CurrentDirectory + "/CustomMods/" + FileName))
                return;

            File.Copy(FileLocation, Environment.CurrentDirectory + "/CustomMods/" + FileName);

            HasInstalledNewMods = true;
        }
    }
}
