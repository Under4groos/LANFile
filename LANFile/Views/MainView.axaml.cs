using Avalonia.Controls;
using Avalonia.Interactivity;
using BeaconLib;
using LANFile.ViewModels;

namespace LANFile.Views;

public partial class MainView : UserControl
{
    private readonly Beacon? _beacon = null;

    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        
        base.OnLoaded(e);
    }

    private void ButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        switch ((button.Tag ?? string.Empty).ToString())
        {
            case "beacon":
                break;
            case "probe":
                break;
            case "dispose":
                break;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _beacon?.Dispose();
        base.OnUnloaded(e);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MainViewModel mv)
        {
            mv.Refresh();
        }
    }
}