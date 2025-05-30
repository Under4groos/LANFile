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
using LANFile.Helper;
using LANFile.Models;
using LANFile.Server;
using LANFile.ViewModels;
using LANFile.Views.Controls;
using SuperSimpleTcp;


namespace LANFile.Views;

public partial class MainView : UserControl
{
    private MainViewModel _mainViewModel;
    private MultiConnectionService _multiConnectionService = new MultiConnectionService();

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            _mainViewModel.Host = _multiConnectionService.Host.ToString();
            _mainViewModel.Title = _multiConnectionService.ConnectionName;
        }
        
        _multiConnectionService.DevicesUpdated += BeaconsUpdated;
        base.OnLoaded(e);
    }

    private void BeaconsUpdated(ObservableCollection<DeviceModel> deviceModels)
    {
        _mainViewModel.Devices = deviceModels;
    }


    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not TabControl tabControl || tabControl.SelectedItem is not IconTabItem iconTabItem)
            return;

        var header = (iconTabItem.Header?.ToString() ?? string.Empty).ToLower();
        
        _multiConnectionService.CloseAll();
        switch (header)
        {
            case "upload":
                _multiConnectionService.ProbeStart();
                break;
            case "download":
                _multiConnectionService.BeaconStart();
                break;
        }
    }
}