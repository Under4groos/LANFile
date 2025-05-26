using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.OS;
using CommunityToolkit.Mvvm.ComponentModel;
using LANFile.Models;

namespace LANFile.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string? _Title;
    [ObservableProperty] private ObservableCollection<DeviceModel> _devices = [];


    public MainViewModel()
    {
        this.Title = "LANFile-Debug";
        
    }
    public void Random()
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