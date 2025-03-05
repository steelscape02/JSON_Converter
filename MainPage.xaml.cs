using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using WinRT.Interop;


namespace JsonConverter
{
    /// <summary>
    /// Main page for performing JSON conversion
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly StorageManager manager;

        public MainPage()
        {
            InitializeComponent();
            manager = new StorageManager();
        }

        private new string Language = "";

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        private void Validate_JSON(object sender, RoutedEventArgs e)
        {

            validateBtn.Focus(FocusState.Programmatic);
            jsonEntry.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string entry);

            
            try
            {
                using (JsonDocument.Parse(entry)) { }
                validateErr_msg.Text = "No error";
                valid_validate_start.Begin();
                valid_validate_end.Begin();
            }
            catch (JsonException j)
            {
                if (manager.Get("ValidateMsgs") as bool? ?? false)
                {
                    validateErr_msg.Text = j.ToString();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(entry))
                        validateErr_msg.Text = "Empty JSON";
                    else
                        validateErr_msg.Text = "JSON Error";
                }
                
                invalid_validate_start.Begin();
                invalid_validate_end.Begin();
            }
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            
            var _selector = new LanguageSelector(manager);
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
                        dm = _selector.Select(LanguageSelector.Languages.CSharp, contents);
                        outputBox.Text = dm;
                        break;
                    case "Python":
                        dm = _selector.Select(LanguageSelector.Languages.Python, contents);
                        outputBox.Text = dm;
                        break;
                    case "C++":
                        dm = _selector.Select(LanguageSelector.Languages.Cpp, contents);
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
            if(Language == "")
            {
                return; //TODO: Add an error here
            }
            Windows.Storage.Pickers.FileSavePicker savePicker = new()
            {
                SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            // Dropdown of file types the user can save the file as
            string ext;
            string exp;
            switch (Language)
            {
                case "C#":
                    {
                        ext = ".cs";
                        exp = "C# Source File";
                        break;
                    }
                case "Python":
                    {
                        ext = ".py";
                        exp = "Python Source File";
                        break;
                    }
                case "C++":
                    {
                        ext = ".cpp";
                        exp = "C++ Source File";
                        break;
                    }
                default:
                    {
                        FlashComboBox();
                        return;
                    }
            }
            
            savePicker.FileTypeChoices.Add(exp, [ext]);
            savePicker.SuggestedFileName = "json_model";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this); // Get your main window handle
            InitializeWithWindow.Initialize(savePicker, hwnd);
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, outputBox.Text);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);
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

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening += Menu_Opening;
            jsonEntry.ContextFlyout.Opening += Menu_Opening;
            jsonEntry.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, manager.Get(TextResources.jsonContents) as string);

            outputBox.Text = manager.Get(TextResources.jsonOutput) as string;
            LanguageSelect.SelectedValue = manager.Get(TextResources.selectedLang) as string;
        }

        private void Main_Unloaded(object sender, RoutedEventArgs e)
        {
            jsonEntry.SelectionFlyout.Opening -= Menu_Opening;
            jsonEntry.ContextFlyout.Opening -= Menu_Opening;

            jsonEntry.Document.GetText(Microsoft.UI.Text.TextGetOptions.None, out string jsonContents);
            manager.Set(TextResources.jsonContents, jsonContents);
            manager.Set(TextResources.jsonOutput, outputBox.Text);
            manager.Set(TextResources.selectedLang, LanguageSelect.SelectedValue);
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

        private void FlashComboBox()
        {
            flash_start.Begin();
            flash_end.Begin();
        }
    }
}
