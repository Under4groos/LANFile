using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using BeaconLib;
using LANFile.Models;
using LANFile.ViewModels;

namespace LANFile.Views;

public partial class MainView : UserControl
{
    private Beacon? _beacon = null;
    private Probe? _probe = null;

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        DisposeAll();
        base.OnUnloaded(e);
    }

    void DisposeAll()
    {
        _beacon?.Dispose();
        _beacon = null;
        _probe?.Dispose();
        _probe = null;

        if (this.DataContext is MainViewModel vm)
        {
            vm.Devices = new ObservableCollection<DeviceModel>();
        }
    }


    private void ButtonClickRefresh(object? sender, RoutedEventArgs e)
    {
        _probe?.BeginReceived();
        _beacon?.BeginReceived();
    }

    private void ButtonClickFindDevice(object? sender, RoutedEventArgs e)
    {
        DisposeAll();


        _probe = new Probe(nameof(MainView));
        _probe.BeaconsUpdated += (beacons) =>
        {
            ObservableCollection<DeviceModel> devices = [];
            if (beacons.Any())
            {
                foreach (var beacon in beacons.Where(b => !string.IsNullOrEmpty(b.Data)).ToArray() )
                {
                    string[] spData = beacon.Data.Split("-");
                    if (spData.Length < 2)
                        continue;

                    string deviceName = spData[1];
                    string plaatform = spData[0];
                    devices.Add(new DeviceModel()
                    {
                        Host = beacon.Address.ToString(),
                        Name = deviceName,
                        Os = $"{plaatform.ToLower()}",
                        Port = beacon.Address.Port.ToString(),
                        Ping = App.R.Next(10, 1000)
                    });


                    Console.WriteLine(beacon.Address + ": " + beacon.Data);
                }
                
                Dispatcher.UIThread.Invoke(() =>
                {
                    if (this.DataContext is MainViewModel vm)
                    {
                        vm.Devices = devices;
                    }
                });
            }


            
        };

        _probe?.Start();
        _probe?.BeginReceived();
    }

    private void ButtonClickBeacon(object? sender, RoutedEventArgs e)
    {
        DisposeAll();
       
        _beacon = new Beacon(nameof(MainView), (ushort)(4000 + App.R.Next(0, 400)));
        _beacon.BeaconData = $"{App.Platform}-{App.NameApplication}";
        _beacon?.Start();
    }

    private void ButtonClickDispose(object? sender, RoutedEventArgs e)
    {
        DisposeAll();
    }
}