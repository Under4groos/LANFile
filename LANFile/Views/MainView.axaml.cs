using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using BeaconLib;
using LANFile.Models;
using LANFile.Server;
using LANFile.ViewModels;


namespace LANFile.Views;

public partial class MainView : UserControl
{
    private Beacon? _beacon = null;
    private Probe? _probe = null;
    private MainViewModel _mainViewModel;
    private HttpServer _httpServer = new HttpServer();

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _mainViewModel = this.DataContext as MainViewModel;

        _httpServer.OnHttpListenerResponse += OnHttpListenerResponse;
        _httpServer.Start();


        base.OnLoaded(e);
    }

    private void OnHttpListenerResponse(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> query, string httpMethod, Uri? userHostName)
    {
        
        
        
    }


    protected override void OnUnloaded(RoutedEventArgs e)
    {
        DisposeAll();
        base.OnUnloaded(e);
    }

    void DisposeAll()
    {
        _beacon?.Stop();
        _beacon?.Dispose();
        _probe?.Stop();
        _probe?.Dispose();

        GC.SuppressFinalize(this);
        GC.Collect();

        _probe = null;
        _beacon = null;
        _mainViewModel.Devices = new ObservableCollection<DeviceModel>();
        _mainViewModel.Title = App.NameApplication;
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
                foreach (var beacon in beacons.Where(b => !string.IsNullOrEmpty(b.Data)).ToArray())
                {
                    string[] spData = beacon.Data.Split("-");
                    if (spData.Length < 2)
                        continue;

                    string deviceName = spData[1];
                    string plaatform = spData[0];
                    devices.Add(new DeviceModel()
                    {
                        Host = beacon.Address.Address.ToString(),
                        Name = deviceName,
                        Os = $"{plaatform.ToLower()}",
                        Port = beacon.Address.Port.ToString(),
                        Ping = App.R.Next(10, 1000)
                    });


                    Console.WriteLine(beacon.Address + ": " + beacon.Data);
                }

                Dispatcher.UIThread.Invoke(() => { _mainViewModel.Devices = devices; });
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

    private void ButtonClickRandom(object? sender, RoutedEventArgs e)
    {
        DisposeAll();
        _mainViewModel.Random();
    }
}