using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JsonArchitect.ViewModels;

namespace JsonArchitect;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                await OnAppStart();
            
                desktop.MainWindow = new Views.MainWindow();
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                desktop.Exit += OnAppExit;
            
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
    private static void OnAppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        MainWindowViewModel.SaveSettings();
    }

    private static Task OnAppStart()
    {
        MainWindowViewModel.LoadSettings();
        return Task.CompletedTask;
    }
}
