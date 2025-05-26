using System;
using System.Linq;
using Android.App;
using Android.OS;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LANFile.ViewModels;
using LANFile.Views;
using Application = Avalonia.Application;

namespace LANFile;

public class App : Application
{
    public static Random R = new Random();

    public static string NameApplication { get; set; }
        

    
    public static string Platform { get; set; } =
        $"{(OperatingSystem.IsAndroid() ? "Android" : "Windows")}";
    
    
    public override void Initialize()
    {
        if (OperatingSystem.IsAndroid())
        {
            NameApplication = $"{Build.Model ?? "Error"}";
        }
        else
        {
            NameApplication = $"{System.Security.Principal.WindowsIdentity.GetCurrent().Name}";
        }

        NameApplication += $"_{Guid.NewGuid().ToString().Substring(5)}";
        
        
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}