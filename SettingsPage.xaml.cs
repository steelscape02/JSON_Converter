using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace JsonConverter
{
    /// <summary>
    /// Settings page for setting control options
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            versionID.Text = TextResources.version;
            RootName.Text = TextResources.baseName;
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
            LanguageSelectorHelpers.RootName = RootName.Text;
        }

        private void ThemeSelect_Change(object sender, SelectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            SettingsPageHelpers.ThemeIndex = themeSelect.SelectedIndex;
        }

        //All optional
        private void AllOptional_Checked(object sender, RoutedEventArgs e)
        {
            LanguageSelectorHelpers.AllOptional = true;
        }

        private void AllOptional_Unchecked(object sender, RoutedEventArgs e)
        {
            LanguageSelectorHelpers.AllOptional = false;
        }

        //Suggest corrections
        private void SuggestCorrs_Checked(object sender, RoutedEventArgs e)
        {
            LanguageSelectorHelpers.SuggestCorrs = true;
        }

        private void SuggestCorrs_Unchecked(object sender, RoutedEventArgs e)
        {
            LanguageSelectorHelpers.SuggestCorrs = false;
        }

        //Validation Messages
        private void ValidateMsgs_Checked(object sender, RoutedEventArgs e)
        {
            MainPageHelpers.ValidateMsgs = true;
        }

        private void ValidateMsgs_Unchecked(object sender, RoutedEventArgs e)
        {
            MainPageHelpers.ValidateMsgs = false;
        }

        private void Options_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RootName.Text = LanguageSelectorHelpers.RootName;
            themeSelect.SelectedIndex = SettingsPageHelpers.ThemeIndex;
            allOptional.IsChecked = SettingsPageHelpers.AllOptional;
            suggestCorrs.IsChecked = SettingsPageHelpers.SuggestCorrs;
            validateMsgs.IsChecked = SettingsPageHelpers.ValidateMsgs;
        }

        private void Options_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            LanguageSelectorHelpers.RootName = RootName.Text;
            SettingsPageHelpers.ThemeIndex = themeSelect.SelectedIndex;
            SettingsPageHelpers.AllOptional = allOptional.IsChecked;
            SettingsPageHelpers.SuggestCorrs = suggestCorrs.IsChecked;
            SettingsPageHelpers.ValidateMsgs = validateMsgs.IsChecked;
        }

    }
}
