using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LANFile.Models;

namespace LANFile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Random R = new();
    [ObservableProperty] private ObservableCollection<DeviceModel> _devices = [];
    [ObservableProperty] private string? _Host;
    [ObservableProperty] private string? _Title;

    public MainViewModel()
    {
        Title = "LANFile-Debug";
    }

    public void Random()
    {
        ObservableCollection<DeviceModel> devices = [];
        for (var i = 0; i < 15; i++)
            devices.Add(new DeviceModel
            {
                Host = $"{R.Next(0, 255)}.{R.Next(0, 255)}.{R.Next(0, 255)}.{R.Next(0, 255)}",
                Name = $"{Guid.NewGuid()}".Substring(10),
                Os = $"{(i / 2 == 1 ? "android" : "windows")}",
                Port = $"{R.Next(0, 255)}{R.Next(0, 255)}{R.Next(0, 255)}{R.Next(0, 255)}",
                Ping = R.Next(10, 1000)
            });
        Devices = devices;
    }
}