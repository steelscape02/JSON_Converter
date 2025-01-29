using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JsonConverter
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Submitting";
            jsonEntry.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string entry);
            if (IsJson(entry))
            {
                //send to JsonReader
            }
            else
            {
                myButton.Content = "JSON Entry not valid";
                //wait 5 sec
                //reset
            }
        }
        private void Menu_Opening(object sender, object e)
        {
            CommandBarFlyout? myFlyout = sender as CommandBarFlyout;
            if (myFlyout.Target == jsonEntry)
            {
                AppBarButton myButton = new AppBarButton
                {
                    Command = new StandardUICommand(StandardUICommandKind.Copy)
                };
                myFlyout.PrimaryCommands.Add(myButton);
            }
        }

        private void jsonEntry_Loaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening += Menu_Opening;
            jsonEntry.ContextFlyout.Opening += Menu_Opening;
        }

        private void jsonEntry_Unloaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening -= Menu_Opening;
            jsonEntry.ContextFlyout.Opening -= Menu_Opening;
        }

        private bool IsJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                JsonDocument.Parse(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
