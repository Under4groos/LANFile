using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LANFile.Models;

namespace LANFile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
    [ObservableProperty] private ObservableCollection<DeviceModel> _devices = [];


    public MainViewModel()
    {
        
    }

    public void Refresh()
    {
        ObservableCollection<DeviceModel> devices = [];
        for (var i = 0; i < 15; i++)
        {
            devices.Add(new DeviceModel()
            {
                Host = $"{App.R.Next(0, 255)}.{App.R.Next(0, 255)}.{App.R.Next(0, 255)}.{App.R.Next(0, 255)}",
                Name = $"{Guid.NewGuid()}".Substring(10),
                Os = $"{(i / 2 == 1 ? "android" : "windows")}",
                Port = $"{App.R.Next(0, 255)}{App.R.Next(0, 255)}{App.R.Next(0, 255)}{App.R.Next(0, 255)}",
                Ping = App.R.Next(10, 1000)
            });
        }
        this.Devices = devices;
    }
}