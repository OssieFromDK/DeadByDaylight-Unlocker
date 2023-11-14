using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FortniteBurger
{
    public partial class Cookie : Page
    {
        public Cookie()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:
                case Key.A:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        return;
                    break;

                case Key.Left:
                case Key.Up:
                case Key.V:
                case Key.Right:
                case Key.Down:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Home:
                case Key.End:
                    return;
            }
            e.Handled = true;
        }

        private void GrabCookie_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CookieBox.Text))
            {
                CookieBox.Text = string.Empty;
            }

            UpdateText.Text = "Grabbing Cookie...";
            Grab_Button.Visibility = Visibility.Hidden;
            Spinner.Visibility = Visibility.Visible;

            if (MainWindow.CurrentType == "Steam")
            {
                Classes.SteamWorks.GetBhvrSession();
            }
            else
            {
                if (!Classes.FiddlerCore.FiddlerIsRunning)
                {
                    Classes.FiddlerCore.StartFiddlerCore();
                }
                Classes.FiddlerCore.StartWithShutdown();
                Classes.Launcher.LaunchDBD(MainWindow.CurrentType);
            }
        }

        public async void ReturnFromCookie(string value, bool showcookie)
        {
            if(MainWindow.CurrentType != "Steam")
            {
                Classes.Launcher.KillDBD(MainWindow.CurrentType);
            }

            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateText.Text = value;

                if (showcookie)
                {
                    CookieBox.Text = Classes.CookieHandler.GetCookie();
                }

                Spinner.Visibility = Visibility.Hidden;
                Check.Visibility = Visibility.Visible;
            }));

            await Task.Delay(2500);

            this.Dispatcher.Invoke((Action)(() =>
            {
                Check.Visibility = Visibility.Hidden;
                Grab_Button.Visibility = Visibility.Visible;

                UpdateText.Text = "";
            }));
        }
    }
}
