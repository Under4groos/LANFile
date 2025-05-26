using Avalonia;
using Avalonia.Controls;
using LANFile.Models;

namespace LANFile.Views.Controls;

public class LanDevice : ContentControl
{
    public static readonly StyledProperty<string> ConnectNameProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(ConnectName));
    public string ConnectName
    {
        get => GetValue(ConnectNameProperty);
        set => SetValue(ConnectNameProperty, value);
    }
    
    
    
    public static readonly StyledProperty<string> OsProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(OsProperty));
    public string Os
    {
        get => GetValue(OsProperty);
        set => SetValue(OsProperty, value);
    }
    
    
    public static readonly StyledProperty<string> PortProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Port));
    public string Port
    {
        get => GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }
    
    public static readonly StyledProperty<string> HostProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Host));
    public string Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }
}