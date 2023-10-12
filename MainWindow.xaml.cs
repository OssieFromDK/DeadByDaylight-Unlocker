using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace FortniteBurger
{
    public partial class MainWindow : Window
    {
        internal static Cookie cookie = new Cookie();
        internal static Tome tome = new Tome();
        internal static BP bp = new BP();
        internal static BPTools BPTools = new BPTools();
        internal static Login login = new Login();
        internal static Profile profile = new Profile();
        internal static MainWindow main;
        internal static Settings settingspage = new Settings();
        internal static Mods mods = new Mods();
        internal static Classes.Settings settings = new Classes.Settings();
        internal static Classes.PakBypass PakBypass = new Classes.PakBypass();
        //internal static Classes.LobbyInfo LobbyInfo = new Classes.LobbyInfo();
        internal static Classes.SysTray SysTray = new Classes.SysTray();
        internal static Overlay currentOverlay;

        internal static Classes.Mods.ModManager ModManager = new Classes.Mods.ModManager();

        internal static string CurrVersion = "3.6.0";

        internal static string CurrentType = "Steam";

        public MainWindow()
        {
            InitializeComponent();
            main = this;
            Classes.Settings.LoadSettings();
            this.VersionText.Text = "Burger: v" + CurrVersion;
            login.StartLogin();
        }

        internal void LoginDone()
        {
            Classes.Settings.LoadConfig();
            Classes.Settings.LoadMods();
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();

            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();
             
            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }

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
            if(profile.FullProfile)
            {
                Classes.Utils.UpdateProfiles(int.Parse(profile.PrestigeLevelBox.Text), int.Parse(profile.ItemAmountBox.Text));
            }

            if (!Classes.FiddlerCore.FiddlerIsRunning)
            {
                Classes.FiddlerCore.StartFiddlerCore();
            }

            Launch.Visibility = Visibility.Hidden;
            Spinner.Visibility = Visibility.Visible;


            if (CurrentType == "Steam")
            {
                if (!PakBypass.PakBypassedThisSession)
                {
                    UpdateText.Text = "Awaiting Pak Bypass...";
                    await PakBypass.LoadPakBypass();

                    UpdateText.Text = "Awaiting SSL Bypass...";
                    await PakBypass.LoadSSLBypass();
                }
            }

            UpdateText.Text = "Awaiting Game Launch...";
            Classes.Launcher.LaunchDBD(CurrentType);
            Classes.Utils.CheckForGameRunning(CurrentType);
        }

        private Timer Timer;

        private void StartChecking()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (settingspage.HideToTray) SysTray.StartSysTray();
            }));
            Timer = new Timer(250);
            Timer.AutoReset = true;
            Timer.Elapsed += new ElapsedEventHandler(CheckGameRunning);
            Timer.Enabled = true;
            Timer.Start();
        }

        private void CheckGameRunning(object sender, ElapsedEventArgs e)
        {
            if (!Classes.Utils.IsGameCurrentlyRunning(CurrentType))
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (settingspage.HideToTray) SysTray.StopSysTray();
                }));

                Classes.FiddlerCore.StopFiddlerCore();

                Timer.Stop();
                Timer.Dispose();
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
