using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BurgerUninstall
{
    public partial class MainWindow : Window
    {
        private bool Settings = false;
        private bool Program = false;
        private bool Worker = false;
        private bool Mods = false;

        private string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        List<string> FileNames = new List<string>()
        {
            "FortniteBurger.exe", 
            "steam_api64.dll", 
            "steam_appid.txt",
            "dbdPath.txt",
        };

        List<string> FolderNames = new List<string>()
        {
            "CustomMods",
            "Mods",
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Topbar_Movedown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            this.Spinner.Visibility = Visibility.Visible;
            this.Uninstall.Content = String.Empty;

            // Start Uninstall //

            // Mods //
            if (Mods)
            {
                string AppDir = Environment.CurrentDirectory;
                string DBDPath = Path.Combine(AppDir, "dbdPath.txt");
                if (File.Exists(DBDPath))
                {
                    string DBDInstallPath = File.ReadAllText(DBDPath);
                    int Index = DBDInstallPath.IndexOf("\\Binaries");
                    string DBDContentPath = DBDInstallPath.Substring(0, Index) + "\\Content\\Paks";

                    string[] AllPaks = Directory.GetFiles(DBDContentPath);

                    foreach (string FilePath in AllPaks)
                    {
                        string FileName = Path.GetFileName(FilePath);
                        int PakNumber = int.Parse(Regex.Match(FileName, @"\d+").Value);

                        if (PakNumber > 348)
                        {
                            File.Delete(FilePath);
                        }
                    }
                }

                string specificFolder = LocalAppData + "/FortniteBurger/Settings/Mods.json";
                using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.SetLength(0);
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write("[]");
                    }
                }

                Mods = false;
            }

            // Program //
            if (Program)
            {
                Process[] BurgerProcesses = Process.GetProcessesByName("FortniteBurger");

                if(BurgerProcesses.Length > 0) 
                {
                    if (BurgerProcesses[0] != null)
                    {
                        BurgerProcesses[0].Kill();
                    }
                }

                string AppDir = Environment.CurrentDirectory;

                foreach (string FileName in FileNames)
                {
                    string FullFilePath = Path.Combine(AppDir, FileName);
                    if (File.Exists(FullFilePath))
                    {
                        File.Delete(FullFilePath);
                    }
                }

                foreach (string FolderName in FolderNames)
                {
                    string FullFolderPath = Path.Combine(AppDir, FolderName);
                    if (Directory.Exists(FullFolderPath))
                    {
                        Directory.Delete(FullFolderPath, true);
                    }
                }

                Program = false;
            }

            // Settings //
            if (Settings)
            {
                if (Directory.Exists(LocalAppData + "/FortniteBurger"))
                {
                    Directory.Delete(LocalAppData + "/FortniteBurger", true);
                }

                Settings = false;
            }

            // Worker Service //
            if (Worker)
            {
                Process[] WorkerProcesses = Process.GetProcessesByName("BurgerWorker");

                if(WorkerProcesses.Length > 0)
                {
                    if (WorkerProcesses[0] != null)
                    {
                        WorkerProcesses[0].Kill();
                    }
                }

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Microsoft/Windows/Start Menu/Programs/Startup/BurgerWorker.exe";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                Worker = false;
            }

            Slow();
        }

        async void Slow()
        {
            await Task.Delay(1500);

            this.Mods_Check.IsChecked = false;
            this.Worker_Check.IsChecked = false;
            this.Settings_Check.IsChecked = false;
            this.Program_Check.IsChecked = false;

            this.Spinner.Visibility = Visibility.Collapsed;
            this.Uninstall.Content = "Uninstall";
        }

        private void Program_Click(object sender, RoutedEventArgs e)
        {
            Program = !Program;
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings = !Settings;
        }

        private void Worker_Click(object sender, RoutedEventArgs e)
        {
            Worker = !Worker;
        }

        private void Mods_Click(object sender, RoutedEventArgs e)
        {
            Mods = !Mods;
        }
    }
}
