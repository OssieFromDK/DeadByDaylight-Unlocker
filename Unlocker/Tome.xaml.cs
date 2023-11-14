using System;
using System.Windows;
using System.Windows.Controls;
namespace FortniteBurger
{
    public partial class Tome : Page
    {
        public Tome()
        {
            InitializeComponent();
        }

        private void Tome_Start(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Classes.CookieHandler.GetCookie()))
            {
                TomeBox.Text = "Error: You need to fetch your cookie.";
                return;
            }

            TomeBox.Text = string.Empty;
            Spinner.Visibility = Visibility.Visible;
            Start.Visibility = Visibility.Hidden;
            Stop.Visibility = Visibility.Visible;
        }

        private void Tome_Stop(object sender, RoutedEventArgs e)
        {
            Spinner.Visibility = Visibility.Hidden;
            Stop.Visibility = Visibility.Hidden;
            Start.Visibility = Visibility.Visible;
        }


        internal void UpdateTome(string value, bool error)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (!error)
                {
                    TomeBox.Text += value + Environment.NewLine;
                }
                else
                {
                    TomeBox.Text += "Error: " + value + Environment.NewLine;
                }
            }));
        }
    }
}
