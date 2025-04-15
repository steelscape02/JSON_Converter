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
        //create instance of storage manager for this page
        private readonly StorageManager manager;
        public SettingsPage()
        {
            manager = new StorageManager();
            InitializeComponent();
            versionID.Text = TextResources.version;
            if(manager.Get(TextResources.rootName) == null)
                manager.Set(TextResources.rootName, TextResources.baseName);
        }

        /// <summary>
        /// Backbutton event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            _ = Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Help button event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
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

        /// <summary>
        /// Base name (Root class) change event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void BaseName_Change(object sender, TextChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);
            manager.Set(TextResources.rootName, RootName.Text);
        }

        /// <summary>
        /// All Optional <b>checked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void AllOptional_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.allOptional, true);
        }

        /// <summary>
        /// All Optional <b>unchecked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void AllOptional_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.allOptional, false);
        }

        /// <summary>
        /// Suggest corrections <b>checked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void SuggestCorrs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.suggestCorrs, true);
        }

        /// <summary>
        /// Suggest corrections <b>unchecked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void SuggestCorrs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.suggestCorrs, false);
        }

        /// <summary>
        /// Validation messages <b>checked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void ValidateMsgs_Checked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.validateMsgs, true);
        }

        /// <summary>
        /// Validation messages <b>unchecked</b> event handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void ValidateMsgs_Unchecked(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.validateMsgs, false);
        }

        /// <summary>
        /// Options grid loaded handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
            RootName.Text = manager.Get(TextResources.rootName) as string;
            allOptional.IsChecked = manager.Get(TextResources.allOptional) as bool? ?? false;
            suggestCorrs.IsChecked = manager.Get(TextResources.suggestCorrs) as bool? ?? false;
            validateMsgs.IsChecked = manager.Get(TextResources.validateMsgs) as bool? ?? false;
        }

        /// <summary>
        /// Options grid unloaded handler
        /// </summary>
        /// <param name="sender">The object that raised this event</param>
        /// <param name="e">Additional event information</param>
        private void Options_Unloaded(object sender, RoutedEventArgs e)
        {
            manager.Set(TextResources.rootName, RootName.Text);
            manager.Set(TextResources.allOptional, allOptional.IsChecked ?? false);
            manager.Set(TextResources.suggestCorrs, suggestCorrs.IsChecked ?? false);
            manager.Set(TextResources.validateMsgs, validateMsgs.IsChecked ?? false);
        }

    }
}
