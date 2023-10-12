using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FortniteBurger
{
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        internal void StartLogin()
        {
            MainWindow.main.TotalFrame.Content = this;

            if(Classes.LicenseHandler.CheckForAppDataLicense())
            {
                LicenseBox.Text = Classes.LicenseHandler.GetAppDataLicense();
            }
        }

        private void Login_Check(object sender, RoutedEventArgs e)
        {
            string LicenseInput = LicenseBox.Text;

            if (string.IsNullOrEmpty(LicenseInput))
            {
                UpdateText.Text = "You need to input a license to continue";
                return;
            }

            UpdateText.Text = "Trying to login...";
            Button.Visibility = Visibility.Hidden;
            Spinner.Visibility = Visibility.Visible;

            Classes.LicenseHandler.CheckLicense(LicenseInput);
        }

        internal async void LoginReturn(string value, bool completed)
        {
            UpdateText.Text = value;

            if (completed)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Spinner.Visibility = Visibility.Hidden;
                    Check.Visibility = Visibility.Visible;
                }));

                await Task.Delay(2500);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.main.TotalFrame.Content = null;
                    MainWindow.main.LoginDone();
                }));
            } else
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Spinner.Visibility = Visibility.Hidden;
                    Error.Visibility = Visibility.Visible;
                }));

                await Task.Delay(2500);

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Error.Visibility = Visibility.Hidden;
                    Button.Visibility = Visibility.Visible;
                }));
            }
        }
    }
}
