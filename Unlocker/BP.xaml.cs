using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FortniteBurger
{
    public partial class BP : Page
    {
        public BP()
        {
            InitializeComponent();
        }

        private void BP_AddBP(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Classes.CookieHandler.GetCookie()))
            {
                UpdateText.Text = "Trying to add Bloodpoints...";
                Button.Visibility = Visibility.Hidden;
                Spinner.Visibility = Visibility.Visible;
            }
            else
            {
                UpdateText.Text = "You need to fetch your cookie.";
            }
        }

        internal async void UpdateBP(string value, bool error)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateText.Text = value;

                Spinner.Visibility = Visibility.Hidden;

                if (error)
                {
                    Error.Visibility = Visibility.Visible;
                }
                else
                {
                    Check.Visibility = Visibility.Visible;
                }
            }));

            await Task.Delay(2500);

            this.Dispatcher.Invoke((Action)(() =>
            {
                UpdateText.Text = String.Empty;
                Check.Visibility = Visibility.Hidden;
                Error.Visibility = Visibility.Hidden;
                Button.Visibility = Visibility.Visible;

            }));
        }
    }
}
