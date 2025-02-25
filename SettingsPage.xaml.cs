using Microsoft.UI.Xaml.Controls;

namespace JsonConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            versionID.Text = TextResources.version;
        }

        private void Back_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _ = Frame.Navigate(typeof(MainPage));
        }
    }
}
