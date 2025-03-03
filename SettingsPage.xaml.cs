using Microsoft.UI.Xaml.Controls;
using System;

namespace JsonConverter
{
    /// <summary>
    /// Settings page for setting control options
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public static int ThemeIndex = 0;
        public static bool? AllOptional = false;
        public static bool? SuggestCorrs = false;
        public static bool? ValidateMsgs = false;

        public SettingsPage()
        {
            InitializeComponent();
            versionID.Text = TextResources.version;
            RootName.Text = TextResources.baseName;
            themeSelect.SelectedIndex = 0;
        }

        private void Back_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(MainPage));
        }

        private void BaseName_Change(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            LanguageSelector.RootName = RootName.Text;
        }

        private void ThemeSelect_Change(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            ThemeIndex = themeSelect.SelectedIndex;
        }

        private void Options_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RootName.Text = LanguageSelector.RootName;
            themeSelect.SelectedIndex = ThemeIndex;
            allOptional.IsChecked = AllOptional;
            suggestCorrs.IsChecked = SuggestCorrs;
            validateMsgs.IsChecked = ValidateMsgs;
        }

        private void Options_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            LanguageSelector.RootName = RootName.Text;
            ThemeIndex = themeSelect.SelectedIndex;
            AllOptional = allOptional.IsChecked;
            SuggestCorrs = suggestCorrs.IsChecked;
            ValidateMsgs = validateMsgs.IsChecked;
        }
    }
}
