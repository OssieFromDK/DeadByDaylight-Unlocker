using Steamworks;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FortniteBurger
{
    public partial class Profile : Page
    {
        /* Profile Types */
        internal bool FullProfile = true;
        internal bool Skins_Perks_Only = false;
        internal bool Skins_Only = false;
        internal bool DLC_Only = false;
        internal bool Off = false;

        /* Extras */
        internal bool Currency_Spoof = false;
        internal bool Level_Spoof = false;
        internal bool Break_Skins = false;
        internal bool Disabled = false;

        public Profile()
        {
            InitializeComponent();
        }

        internal void SetProfileType(int profile)
        {
            switch (profile)
            {
                case 0:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Full.Visibility = Visibility.Collapsed;
                        ProfileTypeBox.Text = "Profile Off";
                    }));
                    FullProfile = false;
                    Skins_Perks_Only = false;
                    Skins_Only = false;
                    DLC_Only = false;
                    Off = true;
                    Break_Skins = false;
                    Disabled = false;
                    break;
                case 1:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Full.Visibility = Visibility.Visible;
                        ProfileTypeBox.Text = "Full Profile";
                    }));
                    FullProfile = true;
                    Skins_Perks_Only = false;
                    Skins_Only = false;
                    DLC_Only = false;
                    Off = false;
                    Break_Skins = true;
                    Disabled = true;
                    break;
                case 2:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Full.Visibility = Visibility.Collapsed;
                        ProfileTypeBox.Text = "Skins & Perks";
                    }));
                    FullProfile = false;
                    Skins_Perks_Only = true;
                    Skins_Only = false;
                    DLC_Only = false;
                    Off = false;
                    Break_Skins = true;
                    Disabled = true;
                    break;
                case 3:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Full.Visibility = Visibility.Collapsed;
                        ProfileTypeBox.Text = "Skins Only";
                    }));
                    FullProfile = false;
                    Skins_Perks_Only = false;
                    Skins_Only = true;
                    DLC_Only = false;
                    Off = false;
                    Break_Skins = true;
                    Disabled = true;
                    break;
                case 4:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Full.Visibility = Visibility.Collapsed;
                        ProfileTypeBox.Text = "DLC Only";
                    }));
                    FullProfile = false;
                    Skins_Perks_Only = false;
                    Skins_Only = false;
                    DLC_Only = true;
                    Off = false;
                    Break_Skins = false;
                    Disabled = false;
                    break;
            }
        }

        internal int GetProfileType()
        {
            if(Off)
            {
                return 0;
            }
            else if(FullProfile)
            {
                return 1;
            }
            else if(Skins_Perks_Only)
            {
                return 2;
            }
            else if(Skins_Only)
            {
                return 3;
            }
            else if(DLC_Only)
            {
                return 4;
            }

            return 1;
        }

        private void Switch_Profile(object sender, RoutedEventArgs e)
        {
            if (FullProfile)
            {
                ProfileTypeBox.Text = "Skins & Perks";
                FullProfile = false;
                Full.Visibility = Visibility.Collapsed;
                Skins_Perks_Only = true;
                Break_Skins = true;
                Disabled = true;
            } 
            else if (Skins_Perks_Only)
            {
                ProfileTypeBox.Text = "Skins Only";
                Skins_Perks_Only = false;
                Skins_Only = true;
                Break_Skins = true;
                Disabled = true;
            }
            else if (Skins_Only)
            {
                ProfileTypeBox.Text = "DLC Only";
                Skins_Only = false;
                DLC_Only = true;
                Break_Skins = false;
                Disabled = false;
            }
            else if (DLC_Only)
            {
                ProfileTypeBox.Text = "Profile Off";
                DLC_Only = false;
                Off = true;
                Break_Skins = false;
                Disabled = false;
            } 
            else if (Off)
            {
                ProfileTypeBox.Text = "Full Profile";
                Off = false;
                FullProfile = true;
                Break_Skins = true;
                Disabled = true;
                Full.Visibility = Visibility.Visible;
            }
        }

        private void Currency_Clicked(object sender, RoutedEventArgs e) => Currency_Spoof = !Currency_Spoof;
        private void Level_Clicked(object sender, RoutedEventArgs e) => Level_Spoof = !Level_Spoof;


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
