using Hardcodet.Wpf.TaskbarNotification;
using System.Reflection;
using System.Windows;
using System.Drawing;
using System;

namespace FortniteBurger.Classes
{
    internal class SysTray
    {
        TaskbarIcon MainSysIcon;

        internal SysTray()
        {
            MainSysIcon = new TaskbarIcon();
            MainSysIcon.Visibility = Visibility.Collapsed;
            MainSysIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            MainSysIcon.ToolTip = "Fortnite Burger";

            MainSysIcon.TrayMouseDoubleClick += MainSysIcon_TrayMouseDoubleClick;
        }

        internal void StartSysTray()
        {

            if (MainWindow.main.IsVisible)
            {
                MainWindow.main.Hide();
            }

            MainSysIcon.Visibility = Visibility.Visible;
        }

        internal void StopSysTray()
        {
            if (!MainWindow.main.IsVisible)
            {
                MainWindow.main.Show();
            }

            MainSysIcon.Visibility = Visibility.Collapsed;
        }

        private void MainSysIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.main.IsVisible)
            {
                MainWindow.main.Show();
            }

            MainSysIcon.Visibility = Visibility.Collapsed;
        }
    }
}
