using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;

namespace FortniteBurger
{
    public partial class MainWindow : Window
    {
        internal static Classes.AutoUpdate AutoUpdater = new Classes.AutoUpdate();
        internal static UpdateScreen UpdateScreen = new UpdateScreen();
        internal static Cookie cookie = new Cookie();
        internal static Tome tome = new Tome();
        internal static BP bp = new BP();
        internal static BPTools BPTools = new BPTools();
        internal static Profile profile = new Profile();
        internal static MainWindow main;
        internal static Settings settingspage = new Settings();
        internal static Mods mods = new Mods();
        internal static Classes.ErrorLog ErrorLog = new Classes.ErrorLog();
        internal static Classes.Settings settings = new Classes.Settings();
        internal static Classes.PakBypass PakBypass = new Classes.PakBypass();
        //internal static Classes.LobbyInfo LobbyInfo = new Classes.LobbyInfo();
        internal static Classes.SysTray SysTray = new Classes.SysTray();
        internal static Overlay currentOverlay;
        internal static Classes.Mods.ModManager ModManager = new Classes.Mods.ModManager();

        internal static string DBDVersion = "8.0.1";
        internal static string CurrVersion = "3.7.4.2";
        internal static string CurrentType = "Steam";

        internal bool InQueue = false;
        internal int ETA = 0;
        internal string Pos = "0";


        public MainWindow()
        {
            InitializeComponent();
            main = this;

            try
            {
                Classes.Settings.LoadSettings();
            }
            catch (Exception ex)
            {
                ErrorLog.CreateLog("We were unable to load your settings");
                ErrorLog.CreateLog(ex.Message);
            }

            try
            {
                UpdateScreen.CheckForUpdate();
            } catch(Exception ex)
            {
                ErrorLog.CreateLog("We were unable to check for updates for the software");
                ErrorLog.CreateLog(ex.Message);
                UpdateCheckDone();
            }
        }

        internal void UpdateCheckDone()
        {
            this.VersionText.Text = "Burger: v" + CurrVersion;
            this.DbdVersionText.Text = "DBD: v" + DBDVersion;

            try {
                Classes.Worker.LoadWorker();
                Classes.Telemetry.Load();
                Classes.Telemetry.Add();
                Classes.Settings.LoadConfig();
                Classes.Settings.LoadMods();
            } catch(Exception ex) {
                ErrorLog.CreateLog(ex.Message);
            }

            if(Classes.RegCheck.FirstLaunch())
            {
                System.Windows.MessageBox.Show("Welcome to Fortnite Burger, this message will only show once. \n\nPlease make sure to check the settings page for any changes you may want to change. \n\n!!! ------------------------------------- !!!\n\nIf you have paid for this tool you have been scammed.\nFortnite Burger is completely free and is not paid in any ways. \n\nTake Care \n/Ossie");
            }
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Classes.CloseManager.Close();

            Environment.Exit(0);
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content != null)
            {
                MainFrame.Content = null;
            }
        }

        private void Cookie_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = cookie;
        }

        private void GoWeb(object sender, RoutedEventArgs e)
        {
            Classes.Launcher.LaunchWeb("https://fortniteburger.vip");
        }

        private void GoDiscord(object sender, RoutedEventArgs e)
        {
            Classes.Launcher.LaunchWeb("https://discord.gg/ySsrsYdGwx");
        }

        private void BP_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = BPTools;
        }

        private void Tome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = tome;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = profile;
        }

        private void Mods_Click(object sender, RoutedEventArgs e)
        {
            mods.CheckForMS();

            MainFrame.Content = mods;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = settingspage;
        }

        private async void Launch_Pressed(object sender, RoutedEventArgs e)
        {
            try
            {
                Classes.Utils.DoFileCheck();

                if (Classes.Utils.IsPakBypassRunning())
                {
                    UpdateText.Text = "Finish Pak Bypass before launching";
                    return;
                }

                if (profile.FullProfile)
                {
                    Classes.Utils.UpdateProfiles(int.Parse(profile.PrestigeLevelBox.Text), int.Parse(profile.ItemAmountBox.Text));
                }

                if (Classes.FiddlerCore.FiddlerIsRunning)
                {
                    Classes.FiddlerCore.StopFiddlerCore();
                }

                Classes.FiddlerCore.StartFiddlerCore();

                Launch.Visibility = Visibility.Hidden;
                Spinner.Visibility = Visibility.Visible;

                if (Classes.Mods.ModManager.HasInstalledNewMods) // Prevent Violation Error Crash
                {
                    UpdateText.Text = "Awaiting Pak Bypass...";
                    await PakBypass.LoadPakBypass();

                    if (CurrentType == "Steam")
                    {
                        UpdateText.Text = "Awaiting SSL Bypass...";
                        await PakBypass.LoadSSLBypass();
                    }
                }

                UpdateText.Text = "Awaiting Game Launch...";
                Classes.Launcher.LaunchDBD(CurrentType);
                Classes.Utils.CheckForGameRunning(CurrentType);
            } catch(Exception ex)
            {
                ErrorLog.CreateLog(ex.Message);
            }
        }

        internal Timer Timer = new Timer() { Interval = 250 };

        private void StartChecking()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (settingspage.HideToTray) SysTray.StartSysTray();
            }));
            HasStopped = false;
            Timer.Tick += CheckGameRunning;
            Timer.Start();
        }

        private bool HasStopped = false;
        private void CheckGameRunning(object sender, EventArgs e)
        {
            if (!Classes.Utils.IsGameCurrentlyRunning(CurrentType))
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Timer.Tick -= CheckGameRunning;
                    Timer.Stop();

                    if (settingspage.HideToTray) SysTray.StopSysTray();

                    this.UpdateText.Text = String.Empty;
                }));

                if (Classes.FiddlerCore.FiddlerIsRunning && !HasStopped)
                {
                    HasStopped = true;
                    Classes.FiddlerCore.StopFiddlerCore();
                }
            }
        }

        internal async void ReturnFromLaunch(string retval, bool completed, string type)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateText.Text = retval;
            }));

            if (completed)
            {
                Classes.FiddlerCore.StartWithoutShutdown();
                Classes.FiddlerCore.LaunchProfileEditor();

                if (currentOverlay != null) currentOverlay.Close();

                if (settingspage.OverlayEnabled)
                {
                    currentOverlay = new Overlay();
                    currentOverlay.IsHitTestVisible = false;
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Spinner.Visibility = Visibility.Hidden;
                    Check.Visibility = Visibility.Visible;
                }));

                await Task.Delay(2000);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Check.Visibility = Visibility.Hidden;
                    Launch.Visibility = Visibility.Visible;
                }));

                await Task.Delay(2000);

                StartChecking();
            }
            else
            {
                Classes.FiddlerCore.StopFiddlerCore();

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Spinner.Visibility = Visibility.Hidden;
                    Error.Visibility = Visibility.Visible;
                }));

                await Task.Delay(2000);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Error.Visibility = Visibility.Hidden;
                    Launch.Visibility = Visibility.Visible;
                }));
            }
        }
    }
}
