using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace FortniteBurger
{
    public partial class UpdateScreen : Page
    {
        public UpdateScreen()
        {
            InitializeComponent();
        }

        internal async void CheckForUpdate()
        {
            MainWindow.main.TotalFrame.Content = this;

            UpdateText.Text = "Checking for updates...";
            Spinner.Visibility = Visibility.Visible;
            await Task.Delay(1000);
            MainWindow.AutoUpdater.CheckForUpdates();
        }

        internal async void NoUpdate()
        {
            UpdateText.Text = "No updates found";
            Spinner.Visibility = Visibility.Hidden;
            Check.Visibility = Visibility.Visible;
            await Task.Delay(1000);
            this.Dispatcher.Invoke((Action)(() =>
            {
                MainWindow.main.TotalFrame.Content = null;
            }));
            MainWindow.main.UpdateCheckDone();
        }

        internal async void FailUpdate()
        {
            UpdateText.Text = "Could not check for updates";
            Spinner.Visibility = Visibility.Hidden;
            Error.Visibility = Visibility.Visible;
            await Task.Delay(1000);
            this.Dispatcher.Invoke((Action)(() =>
            {
                MainWindow.main.TotalFrame.Content = null;
            }));
            MainWindow.main.UpdateCheckDone();
        }

        internal void FoundUpdate(string Version)
        {
            UpdateText.Text = "Found Update: v" + Version;
        }
    }
}
