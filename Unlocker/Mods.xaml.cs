using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace FortniteBurger
{
    public partial class Mods : Page
    {
        private Dictionary<string, dynamic> Values = new Dictionary<string, dynamic>();


        public Mods()
        {
            InitializeComponent();

            Values["FOV_Desc_Label"] = FOV_Desc_Label;
            Values["FOV_InstallButton"] = FOV_InstallButton;
            Values["FOV_AfterInstall"] = FOV_AfterInstall;
            Values["FOV_Config"] = FOV_Config;
            Values["FOV_Input1"] = FOV_Input1;
            Values["FOV_Input2"] = FOV_Input2;

            Values["KR_InstallButton"] = KR_InstallButton;
            Values["KR_AfterInstall"] = KR_AfterInstall;

            Values["RS_Desc_Label"] = RS_Desc_Label;
            Values["RS_InstallButton"] = RS_InstallButton;
            Values["RS_AfterInstall"] = RS_AfterInstall;
            Values["RS_Config"] = RS_Config;
            Values["RS_Input1_Check"] = RS_Input1_Check;
            Values["RS_Input1"] = RS_Input1;
            Values["RS_Input2"] = RS_Input2;

            Values["RES_Desc_Label"] = RES_Desc_Label;
            Values["RES_InstallButton"] = RES_InstallButton;
            Values["RES_AfterInstall"] = RES_AfterInstall;
            Values["RES_Config"] = RES_Config;
            Values["RES_Input1"] = RES_Input1;
            Values["RES_Input2"] = RES_Input2;

            Values["Corn_InstallButton"] = Corn_InstallButton;
            Values["Corn_AfterInstall"] = Corn_AfterInstall;

            Values["OA_InstallButton"] = OA_InstallButton;
            Values["OA_AfterInstall"] = OA_AfterInstall;
        }

        internal void CheckForMS()
        {
            if (MainWindow.CurrentType == "MS")
            {
                MS.Visibility = Visibility.Visible;
                Allowed.Visibility = Visibility.Collapsed;
            }
            else
            {
                MS.Visibility = Visibility.Collapsed;
                Allowed.Visibility = Visibility.Visible;
            }
        }

        internal void SetIsInstalled(string ModName, Dictionary<string,string> ConfigValues = null)
        {
            if (string.IsNullOrEmpty(ModName)) return;

            if (ModName == "Core") return;

            Values[ModName + "_InstallButton"].Visibility = Visibility.Hidden;
            Values[ModName + "_AfterInstall"].Visibility = Visibility.Visible;

            if (ConfigValues != null) 
            {
                foreach (KeyValuePair<string, string> kvp in ConfigValues)
                {

                    if (Values.ContainsKey(ModName + "_Input1"))
                    {
                        if (Values[ModName + "_Input1"].Tag == kvp.Key)
                            Values[ModName + "_Input1"].Text = kvp.Value;
                    }

                    if (Values.ContainsKey(ModName + "_Input2"))
                    {
                        if (Values[ModName + "_Input2"].Tag == kvp.Key)
                            Values[ModName + "_Input2"].Text = kvp.Value;
                    }

                    if (Values.ContainsKey(ModName + "_Input1_Check"))
                    {
                        if (Values[ModName + "_Input1_Check"].Tag == kvp.Key)
                            Values[ModName + "_Input1_Check"].IsChecked = kvp.Value == "True";
                    }
                }
            }
        }

        private void Install(object sender, RoutedEventArgs e)
        {
            Button Btn = sender as Button;

            string InstallType = Btn.Tag as string;

            if (string.IsNullOrEmpty(InstallType)) return;

            Classes.Mods.ModManager.InstallMod(InstallType);

            Values[InstallType + "_InstallButton"].Visibility = Visibility.Hidden;
            Values[InstallType + "_AfterInstall"].Visibility = Visibility.Visible;
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Title = "Fortnite Burger Custom Mods";
            openFileDlg.DefaultExt = ".pak";
            openFileDlg.Filter = "Pak/Bak Files|*.pak;*.bak";
            openFileDlg.InitialDirectory = Environment.CurrentDirectory;
            openFileDlg.Multiselect = true;
            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                foreach (string File in openFileDlg.FileNames)
                {
                    Classes.Mods.ModManager.AddCustomMod(File);
                }
            }
        }

        private void Configure(object sender, RoutedEventArgs e)
        {
            Button Btn = sender as Button;

            string InstallType = Btn.Tag as string;

            if (string.IsNullOrEmpty(InstallType)) return;

            Values[InstallType + "_Desc_Label"].Visibility = Visibility.Hidden;
            Values[InstallType + "_AfterInstall"].Visibility = Visibility.Hidden;
            Values[InstallType + "_Config"].Visibility = Visibility.Visible;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Button Btn = sender as Button;

            string InstallType = Btn.Tag as string;

            if (string.IsNullOrEmpty(InstallType)) return;

            Values[InstallType + "_Desc_Label"].Visibility = Visibility.Visible;
            Values[InstallType + "_AfterInstall"].Visibility = Visibility.Visible;
            Values[InstallType + "_Config"].Visibility = Visibility.Hidden;

            Dictionary<string, string> NewINI = new Dictionary<string, string>();

            if (InstallType != "RES")
            {
                if (Values.ContainsKey(InstallType + "_Input1"))
                    NewINI[Values[InstallType + "_Input1"].Tag as string] = Values[InstallType + "_Input1"].Text;

                if (Values.ContainsKey(InstallType + "_Input2"))
                    NewINI[Values[InstallType + "_Input2"].Tag as string] = Values[InstallType + "_Input2"].Text;

                if (Values.ContainsKey(InstallType + "_Input1_Check"))
                    NewINI[Values[InstallType + "_Input1_Check"].Tag as string] = Values[InstallType + "_Input1_Check"].IsChecked.ToString();
            }
            else
            {
                NewINI["ScreenResolutionSettings"] = Values[InstallType + "_Input1"].Text + "x" + Values[InstallType + "_Input2"].Text + "f";
            }

            Classes.Mods.ModManager.EditINI(InstallType, NewINI);
        }

        private void UnInstall(object sender, RoutedEventArgs e)
        {
            Button Btn = sender as Button;

            string InstallType = Btn.Tag as string;

            if (string.IsNullOrEmpty(InstallType)) return;

            Classes.Mods.ModManager.DeleteMod(InstallType);

            Values[InstallType + "_AfterInstall"].Visibility = Visibility.Hidden;
            Values[InstallType + "_InstallButton"].Visibility = Visibility.Visible;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
