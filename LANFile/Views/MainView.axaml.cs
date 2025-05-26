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
using SuperSimpleTcp;


namespace LANFile.Views;

public partial class MainView : UserControl
{
    private Beacon? _beacon = null;
    private Probe? _probe = null;
    private MainViewModel _mainViewModel;
    private SimpleTcpServer? _simpleTcp;
    private SimpleTcpClient? _simpleTcpClient;

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _mainViewModel = this.DataContext as MainViewModel;

        var buttons = StackPanelTabs.Children.ToArray().Where(a => a.GetType() == typeof(Button)).Select(a => a as Button).ToArray();
        foreach (var button in buttons)
        {
            button.Click += TabOnClick;
        }

        base.OnLoaded(e);
    }

    private void TabOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            foreach (var tabsChild in Tabs.Children)
            {
                if (tabsChild.Tag == tag)
                {
                    tabsChild.IsVisible = true;
                }
                else
                {
                    tabsChild.IsVisible = false;
                }
            }
        }
       
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
        
        _simpleTcp?.DisconnectClient($"{_simpleTcp.IpAddress}:{_simpleTcp.Port}" );
        _simpleTcp?.Dispose();
        
        
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

    private void ClientEventsOnConnected(object? sender, ConnectionEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonClickBeacon(object? sender, RoutedEventArgs e)
    {
        DisposeAll();
        _simpleTcp = new SimpleTcpServer($"{App.GetLocalIPAddress()}:9000");
        _simpleTcp.Events.ClientConnected += ServerEventsOnClientConnected;
        _simpleTcp.Events.ClientDisconnected += (o, args) =>
        {
            LogAdd($"[{args.IpPort}] client disconnected");
        };
        _simpleTcp.Events.DataReceived += (o, args) =>
        {
            LogAdd($"[{args.IpPort}]: {System.Text.Encoding.UTF8.GetString(args.Data)}");
        };
        _simpleTcp.Start();
        LogAdd($"Starting client {_simpleTcp.IpAddress}:{_simpleTcp.Port}");

        _beacon = new Beacon(nameof(MainView), (ushort)(4000 + App.R.Next(0, 400)));
        _beacon.BeaconData = $"{App.Platform}-{App.NameApplication}";
        _beacon?.Start();
    }

    private void ServerEventsOnClientConnected(object? sender, ConnectionEventArgs e)
    {
        Console.WriteLine($"[{e.IpPort}] client connected");
        LogAdd($"[{e.IpPort}] client connected");
        _simpleTcp?.Send(e.IpPort, "Hello, world!");
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

    private void LogAdd(string? message)
    {
        Dispatcher.UIThread.Invoke(() => {  logconsole.Children.Add(new TextBlock(){Text = message}); });
       
    }
}