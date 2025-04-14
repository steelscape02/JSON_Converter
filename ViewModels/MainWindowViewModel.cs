using System.Collections.ObjectModel;
using System.Reactive;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using JsonArchitect.dm;
using ReactiveUI;

namespace JsonArchitect.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    
    private string _jsonOutput = string.Empty;
    public string JsonOutput
    {
        get => _jsonOutput;
        set
        {
            IsJsonOutput = !string.IsNullOrEmpty(value);
            this.RaiseAndSetIfChanged(ref _jsonOutput, value);
        }
    }

    private string _jsonInput = string.Empty;
    public string JsonInput
    {
        get => _jsonInput;
        set
        {
            IsJsonInput = !string.IsNullOrEmpty(value);
            IsJsonInputValid = IsValidJson(value);
            this.RaiseAndSetIfChanged(ref _jsonInput, value);
        }
    }

    private string _validateMsg = string.Empty;
    public string ValidateMsg
    {
        get => _validateMsg;
        set => this.RaiseAndSetIfChanged(ref _validateMsg, value);
    }
    
    private string _corrMsg = string.Empty;
    public string CorrMsg
    {
        get => _corrMsg;
        set => this.RaiseAndSetIfChanged(ref _corrMsg, value);
    }
    
    private bool _isJsonInputValid;
    public bool IsJsonInputValid
    {
        get => _isJsonInputValid;
        set => this.RaiseAndSetIfChanged(ref _isJsonInputValid, value);
    }
    
    private bool _isJsonInput;
    public bool IsJsonInput
    {
        get => _isJsonInput;
        set => this.RaiseAndSetIfChanged(ref _isJsonInput, value);
    }

    private bool _isJsonOutput;
    public bool IsJsonOutput
    {
        get => _isJsonOutput;
        set => this.RaiseAndSetIfChanged(ref _isJsonOutput, value);
    }
    
    private bool _isPaneOpen;
    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }
    
    private string _selectedLanguage = string.Empty;
    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
    }
    
    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }

    private bool _flashAnimation;
    public bool FlashAnimation
    {
        get => _flashAnimation;
        set => this.RaiseAndSetIfChanged(ref _flashAnimation, value);
    }

    private bool _isSugFlyoutOpen;
    public bool IsSugFlyoutOpen
    {
        get => _isSugFlyoutOpen;
        set => this.RaiseAndSetIfChanged(ref _isSugFlyoutOpen, value);
    }

    private bool _allOptional;
    public bool AllOptional
    {
        get
        {
            var res = _manager.Get(TextResources.allOptional);
            _ = bool.TryParse(res?.ToString(), out _allOptional);
            return _allOptional;
        }
        set
        {
            _manager.Set(TextResources.allOptional,value);
            this.RaiseAndSetIfChanged(ref _allOptional, value);
        }
    }

    private bool _suggestCorrs;
    public bool SuggestCorrs
    {
        get
        {
            var res = _manager.Get(TextResources.suggestCorrs);
            _ = bool.TryParse(res?.ToString(), out _suggestCorrs);
            return _suggestCorrs;
        }
        set
        {
            _manager.Set(TextResources.suggestCorrs, value);
            this.RaiseAndSetIfChanged(ref _suggestCorrs, value);
        }
    }

    private bool _detailedErrors;
    public bool DetailedErrors
    {
        get
        {
            var res = _manager.Get(TextResources.detailedErrors);
            _ = bool.TryParse(res?.ToString(), out _detailedErrors);
            return _detailedErrors;
        }
        set
        {
            _manager.Set(TextResources.detailedErrors, value);
            this.RaiseAndSetIfChanged(ref _detailedErrors, value);
        }
    }

    //ObservableCollection<string> Items { get; } = new ObservableCollection<string>
    public ObservableCollection<string> Languages { get; set; } = ["C#", "C++", "Python"];
    
    private readonly Window _mainWindow;
    private readonly IStorageManager _manager;
    
    public static void SaveSettings()
    {
        var settings = new StorageManager();
        //fill the setting with data.
        var json = JsonSerializer.Serialize(settings.GetAll());
        const string filePath = "settings.json";
        File.WriteAllText(filePath, json);
    }

    public static void LoadSettings()
    {
        var settings = new StorageManager();
        try
        {
            var contents = File.ReadAllText("settings.json");
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(contents);
            if (json != null) settings.SetAll(json);
        }
        catch (FileNotFoundException)
        { }
    }
    
    private static FilePickerFileType JsonFileType { get; } = new("All Images") {
        Patterns = ["*.json"],
        AppleUniformTypeIdentifiers = ["public.json"],
        MimeTypes = ["application/json"]
    };

    private static bool IsValidJson(string str)
    {
        try
        {
            JsonDocument.Parse(str);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private async Task SuggestCorrs_Click(HashSet<Element> elements)
    {
        foreach (var i in elements)
        {
            var reserved = SelectedIndex switch
            {
                0 => CSharpDm.ReservedWords.Any(i.Name.Contains),
                1 => PythonDm.ReservedWords.Any(i.Name.Contains),
                2 => CppDm.ReservedWords.Any(i.Name.Contains),
                _ => false
            };
            var illegalChars = Element._illegal.Any(i.Name.Contains);
            //show popup
            if (reserved || illegalChars)
            {
                //TODO: Show a dialog with the item to correct and options
                //

                // var result = await dialog.ShowAsync();
                // switch (result)
                // {
                //     case ContentDialogResult.Primary:
                //     {
                //         elements.TryGetValue(i, out Element? element);
                //         if(element != null && SuggestCorrsHelpers.corrected_word != null) 
                //         { 
                //             element.Name = SuggestCorrsHelpers.corrected_word;
                //             if(illegalChars && !Element._illegal.Any(element.Name.Contains)) element.Rename = false;
                //         }
                //         break;
                //     }
                //     case ContentDialogResult.Secondary:
                //     {
                //         break;
                //     }
                //     case ContentDialogResult.None:
                //     {
                //         return;
                //     }
                // }
            }

            //recursive search
            if (i.Children.Count > 0)
            {
                await SuggestCorrs_Click(i.Children);
            }
        }
    }
    
    private async Task OpenFilePicker()
    {
        //This can also be applied for SaveFilePicker
        var files = await _mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "JSON File",
            FileTypeFilter = [JsonFileType, FilePickerFileTypes.All]
        });
        
        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            var fileContent = await streamReader.ReadToEndAsync();
            JsonInput = fileContent;
        }
    }
    
    private async Task SaveFilePicker()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var file = await _mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Output", //TODO: Sync lang
            
        });

        if (file is not null)
        {
            // Open writing stream from the file.
            await using var stream = await file.OpenWriteAsync();
            await using var streamWriter = new StreamWriter(stream);
            // Write some content to the file.
            await streamWriter.WriteLineAsync(JsonOutput);
        }
    }

    private async Task CopyOutput()
    {
        var clipboard = _mainWindow.Clipboard;
        var dataObject = new DataObject();
        dataObject.Set(DataFormats.Text, JsonOutput);
        if(clipboard is not null)
            await clipboard.SetDataObjectAsync(dataObject);
    }
    
    private void ClearInput()
    {
        JsonInput = string.Empty;
    }
    
    private void ValidateInput(TextBox textBox)
    {
        IsSugFlyoutOpen = !IsSugFlyoutOpen;
        try
        {
            using (JsonDocument.Parse(JsonInput)) {}
            ValidateMsg = "No error";
        }
        catch (JsonException j)
        {
            var res = _manager.Get(TextResources.detailedErrors);
            _ = bool.TryParse(res?.ToString(), out var detailedError);
            if (detailedError)
            {
                ValidateMsg = string.IsNullOrWhiteSpace(JsonInput) ? "Empty JSON" : j.ToString();
            }
            else
            {
                ValidateMsg = string.IsNullOrWhiteSpace(JsonInput) ? "Empty JSON" : "JSON Error";
            }
        }
        FlyoutBase.ShowAttachedFlyout(textBox);
    }

    private void CancelMsg()
    {
        IsSugFlyoutOpen = false;
    }
    
    // ReSharper disable once AsyncVoidMethod
    private async void FlashControl()
    {
        FlashAnimation = !FlashAnimation;
        await Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5));

            // Switch back to the UI thread to update the property.
            Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
            {
                FlashAnimation = !FlashAnimation; // Toggle the state
            });
        });
    }
    
    private async Task SubmitInput()
    {
        var selector = new LanguageSelector(_manager);
        if (!IsValidJson(JsonInput)) return;
        var reader = new JsonReader(JsonInput);
        var contents = reader.ReadJson();
        
        if (_manager.Get(TextResources.suggestCorrs) as bool? ?? false)
        {
            await SuggestCorrs_Click(contents);
        }
        string dm;
        switch (SelectedIndex)
        {
            case 0:
                dm = selector.Select(LanguageSelector.Languages.CSharp, contents);
                JsonOutput = dm;
                break;
            case 1:
                dm = selector.Select(LanguageSelector.Languages.Cpp, contents);
                JsonOutput = dm;
                break;
            case 2:
                dm = selector.Select(LanguageSelector.Languages.Python, contents);
                JsonOutput = dm;
                break;
            default:
                FlashControl();
                break;
        }
    }
    
    public ReactiveCommand<Unit, Unit> ToggleCommand { get; }
    public ReactiveCommand<Unit, Unit> UploadCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<TextBox, Unit> ValidateCommand { get; }
    public ReactiveCommand<Unit,Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    
    public MainWindowViewModel(Window window)
    {
        //SelectedIndex = 0; //TODO: Not functional
        _mainWindow = window;
        _manager = new StorageManager();
        //pane toggle command
        ToggleCommand= ReactiveCommand.Create(() => {
                IsPaneOpen = !IsPaneOpen;
            });

        UploadCommand = ReactiveCommand.CreateFromTask(OpenFilePicker);
        SaveCommand = ReactiveCommand.CreateFromTask(SaveFilePicker);
        CopyCommand = ReactiveCommand.CreateFromTask(CopyOutput); //TODO: Test
        
        ClearCommand = ReactiveCommand.Create(ClearInput);
        
        ValidateCommand = ReactiveCommand.Create<TextBox>(ValidateInput);
        CancelCommand = ReactiveCommand.Create(CancelMsg);
        SubmitCommand = ReactiveCommand.CreateFromTask(SubmitInput);
    }
}