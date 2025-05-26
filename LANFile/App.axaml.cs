using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Android.App;
using Android.Net.Wifi;
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
    public static string NameApplication { get; set; } = $"{Guid.NewGuid().ToString().Substring(0, 9)}";

    public static string Platform { get; set; } =
        $"{(OperatingSystem.IsAndroid() ? "Android" : "Windows")}";

    public static string GetLocalIPAddress()
    {
        try
        {
            if (OperatingSystem.IsAndroid())
            {
                WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
                int ipaddress = wifiManager.ConnectionInfo.IpAddress;
                IPAddress ipAddr = new IPAddress(ipaddress);
       
                return ipAddr.ToString();
            }
            


            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public override void Initialize()
    {
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