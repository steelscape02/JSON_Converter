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
        private readonly StorageManager manager;
        public SettingsPage()
        {
            manager = new StorageManager();
            InitializeComponent();
            versionID.Text = TextResources.version;
            if(manager.Get(TextResources.rootName) == null)
                manager.Set(TextResources.rootName, TextResources.baseName);
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
            manager.Set(TextResources.rootName, RootName.Text);
        }

        private void ThemeSelect_Change(object sender, SelectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            manager.Set(TextResources.themeIndex, themeSelect.SelectedIndex);
            //implement theme change
        }

        //All optional
        private void AllOptional_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.allOptional, true);
        }

        private void AllOptional_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.allOptional, false);
        }

        //Suggest corrections
        private void SuggestCorrs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.suggestCorrs, true);
        }

        private void SuggestCorrs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.suggestCorrs, false);
        }

        //Validation Messages
        private void ValidateMsgs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.validateMsgs, true);
        }

        private void ValidateMsgs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.validateMsgs, false);
        }

        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
            RootName.Text = manager.Get(TextResources.rootName) as string;
            themeSelect.SelectedIndex = manager.Get(TextResources.themeIndex) as int? ?? 0;
            allOptional.IsChecked = manager.Get(TextResources.allOptional) as bool? ?? false;
            suggestCorrs.IsChecked = manager.Get(TextResources.suggestCorrs) as bool? ?? false;
            validateMsgs.IsChecked = manager.Get(TextResources.validateMsgs) as bool? ?? false;
        }

        private void Options_Unloaded(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.rootName, RootName.Text);
            manager.Set(TextResources.themeIndex, themeSelect.SelectedIndex);
            manager.Set(TextResources.allOptional, allOptional.IsChecked ?? false);
            manager.Set(TextResources.suggestCorrs, suggestCorrs.IsChecked ?? false);
            manager.Set(TextResources.validateMsgs, validateMsgs.IsChecked ?? false);
        }

    }
}
