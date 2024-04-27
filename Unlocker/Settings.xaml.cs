using System;
using System.Windows;
using System.Windows.Controls;

namespace FortniteBurger
{
    public partial class Settings : Page
    {
        internal bool OverlayEnabled = true;
        internal bool HideToTray = true;
        internal bool RPC = true;

        public Settings()
        {
            InitializeComponent();
        }

        private async void PakBypass_Start(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Outdated");
            //if (MainWindow.CurrentType == "MS") return;

            //await MainWindow.PakBypass.LoadPakBypass();

            //if (MainWindow.CurrentType == "Steam")
            //{
            //    await MainWindow.PakBypass.LoadSSLBypass();
            //}
        }

        private void Sys_Clicked(object sender, RoutedEventArgs e)
        {
            HideToTray = !HideToTray;
            Sys_Check.IsChecked = HideToTray;
        }

        private void RPC_Clicked(object sender, RoutedEventArgs e)
        {
            RPC = !RPC;
            RPC_Check.IsChecked = RPC;

            Classes.Settings.SaveSettings(); // Update to WorkerService to notice changes
        }

        private void Switch_Platform(object sender, RoutedEventArgs e)
        {
            Classes.CookieHandler.ResetCookie();
            MainWindow.cookie.CookieBox.Text = "";

            if (MainWindow.CurrentType == "Steam")
            {
                MainWindow.CurrentType = "EGS";
                TypeBox.Text = "Epic Games";

                Classes.Mods.ModManager.UpdateEngine();
            }
            else if(MainWindow.CurrentType == "EGS")
            {
                MainWindow.CurrentType = "MS";

                TypeBox.Text = "MS";
            }
            else if (MainWindow.CurrentType == "MS")
            {
                MainWindow.CurrentType = "Steam";
                TypeBox.Text = "Steam";

                Classes.Mods.ModManager.UpdateEngine();
            }
        }

        internal void UpdateTypeBox(string type)
        {
            switch (type)
            {
                case "Steam":
                    TypeBox.Text = "Steam";
                    break;
                case "EGS":
                    TypeBox.Text = "Epic Games";
                    break;
                case "MS":
                    TypeBox.Text = "MS";
                    break;
            }
        }

        private void Overlay_Clicked(object sender, RoutedEventArgs e)
        {
            OverlayEnabled = !OverlayEnabled; // Change State

            Overlay_Check.IsChecked = OverlayEnabled;

            if (!OverlayEnabled)
            {
                if (MainWindow.currentOverlay != null)
                {
                    Overlay.StopTimer();
                    MainWindow.currentOverlay.Close();
                }
            }
            else
            {
                if (Classes.Utils.IsGameCurrentlyRunning(MainWindow.CurrentType))
                {
                    MainWindow.currentOverlay = new Overlay();
                }
            }
        }
    }
}
