using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LANFile.Helper;
using LANFile.Models;
using LANFile.ViewModels;
using LANFile.Views.Controls;

namespace LANFile.Views;

public partial class MainView : UserControl
{
    private readonly MultiConnectionService _multiConnectionService = new();
    private MainViewModel _mainViewModel;

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
            _mainViewModel.Title = _multiConnectionService.NameApplication;
        }

        _multiConnectionService.DevicesUpdated += BeaconsUpdated;
        _multiConnectionService.LoadedLastDevices();

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

    private void ButtonClickStartUdpServer(object? sender, RoutedEventArgs e)
    {
        _multiConnectionService.TCPServerStart();
    }

    private void ButtonClickResetAll(object? sender, RoutedEventArgs e)
    {
        _multiConnectionService.CloseAll();
        _multiConnectionService.ClearFileData();
        _mainViewModel.Devices = new ObservableCollection<DeviceModel>();
    }
}