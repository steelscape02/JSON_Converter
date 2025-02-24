using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JsonConverter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private new string Language = "";

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }


        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            jsonEntry.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string entry);
            if (IsJson(entry))
            {
                var reader = new JsonReader(entry);
                var contents = new HashSet<Element>();
                contents = reader.ReadJson();
                string dm = "";
                switch (Language)
                {
                    case "C#":
                        dm = CSharpDm.BuildRoot(contents);
                        outputBox.Text = dm;
                        break;
                    case "Python":
                        dm = PythonDm.BuildRoot(contents);
                        outputBox.Text = dm;
                        break;
                    case "C++":
                        dm = CppDm.BuildRoot(contents);
                        outputBox.Text = dm;
                        break;
                    default:
                        FlashComboBox();
                        break;
                }


            }
        }

        private void Clear_input(object sender, RoutedEventArgs e)
        {
            jsonEntry.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "");
            outputBox.Text = "";
        }

        private async void Upload_json(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".json");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Get your main window handle
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var contents = await ReadFileContentsAsync(file);

                if (jsonEntry != null)
                {
                    // Set the text of the RichEditBox
                    jsonEntry.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, contents);
                }

            }
            else
            {
                //error msg
            }
        }

        private static async Task<string> ReadFileContentsAsync(StorageFile file)
        {
            return await FileIO.ReadTextAsync(file);
        }

        private async void SaveJson(object sender, RoutedEventArgs e)
        {

            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("C# Source File", new List<string>() { ".cs" }); //ONLY CSHARP
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "json_model";
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Get your main window handle
            InitializeWithWindow.Initialize(savePicker, hwnd);
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, outputBox.Text);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    this.outputBox.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    this.outputBox.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                this.outputBox.Text = "Operation cancelled.";
            }
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(outputBox.Text); // Copy the text from the TextBox
            Clipboard.SetContent(dataPackage);
        }

        private void Menu_Opening(object? sender, object e)
        {
            CommandBarFlyout? myFlyout = sender as CommandBarFlyout;
            if (myFlyout != null && myFlyout.Target == jsonEntry)
            {
                AppBarButton submitBtn = new AppBarButton
                {
                    Command = new StandardUICommand(StandardUICommandKind.Copy)
                };
                myFlyout.PrimaryCommands.Add(submitBtn);
            }
        }

        private void JsonEntry_Loaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening += Menu_Opening;
            jsonEntry.ContextFlyout.Opening += Menu_Opening;
        }

        private void JsonEntry_Unloaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening -= Menu_Opening;
            jsonEntry.ContextFlyout.Opening -= Menu_Opening;
        }

        private static bool IsJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                using (JsonDocument.Parse(input)) { }
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private void Language_Select(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is not null)
            {
                Language = (string)comboBox.SelectedItem;
            }
        }

        private async void FlashComboBox()
        {
            var originalBrush = LanguageSelect.Background;
            var brush = new SolidColorBrush(Microsoft.UI.Colors.Red);
            LanguageSelect.Background = brush;

            await Task.Delay(500); // Wait for fade animation to complete
            LanguageSelect.Background = originalBrush; // Restore original background
        }
    }
}
