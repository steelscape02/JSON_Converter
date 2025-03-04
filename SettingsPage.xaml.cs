using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace JsonConverter
{
    /// <summary>
    /// Settings page for setting control options
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        StorageManager manager;
        public SettingsPage()
        {
            manager = new StorageManager();
            InitializeComponent();
            versionID.Text = TextResources.version;
            if(manager.Get("RootName") == null)
                manager.Save("RootName", TextResources.baseName);
            themeSelect.SelectedIndex = 0;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            _ = Frame.Navigate(typeof(MainPage));
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            if (!allOptional_TeachingTip.IsOpen && !suggestCorrs_TeachingTip.IsOpen && !validateMsgs_TeachingTip.IsOpen)
            {
                allOptional_TeachingTip.IsOpen = true;
                suggestCorrs_TeachingTip.IsOpen = true;
                validateMsgs_TeachingTip.IsOpen = true;
            }
            else
            {
                allOptional_TeachingTip.IsOpen = false;
                suggestCorrs_TeachingTip.IsOpen = false;
                validateMsgs_TeachingTip.IsOpen = false;
            }
        }

        private void BaseName_Change(object sender, TextChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            manager.Save("RootName", RootName.Text);
        }

        private void ThemeSelect_Change(object sender, SelectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            manager.Save("ThemeIndex", themeSelect.SelectedIndex);
            //implement theme change
        }

        //All optional
        private void AllOptional_Checked(object sender, RoutedEventArgs e)
        {
            manager.Save("AllOptional", true);
        }

        private void AllOptional_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Save("AllOptional", false);
        }

        //Suggest corrections
        private void SuggestCorrs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Save("SuggestCorrs", true);
        }

        private void SuggestCorrs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Save("SuggestCorrs", false);
        }

        //Validation Messages
        private void ValidateMsgs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Save("ValidateMsgs", true);
        }

        private void ValidateMsgs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Save("ValidateMsgs", false);
        }

        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
            RootName.Text = manager.Get("RootName") as string;
            themeSelect.SelectedIndex = manager.Get("ThemeIndex") as int? ?? 0;
            allOptional.IsChecked = manager.Get("AllOptional") as bool? ?? false;
            suggestCorrs.IsChecked = manager.Get("SuggestCorrs") as bool? ?? false;
            validateMsgs.IsChecked = manager.Get("ValidateMsgs") as bool? ?? false;
        }

        private void Options_Unloaded(object sender, RoutedEventArgs e)
        {
            manager.Save("RootName", RootName.Text);
            manager.Save("ThemeIndex", themeSelect.SelectedIndex);
            manager.Save("AllOptional", allOptional.IsChecked ?? false);
            manager.Save("SuggestCorrs", suggestCorrs.IsChecked ?? false);
            manager.Save("ValidateMsgs", validateMsgs.IsChecked ?? false);
        }

    }
}
