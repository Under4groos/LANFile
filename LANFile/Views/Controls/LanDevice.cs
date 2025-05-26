using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using LANFile.Helper;
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

    private WrapPanel? _wrapPanel;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        
        base.OnApplyTemplate(e);
        
        
        _wrapPanel = ControlHelper.FindTemplateControlByName<WrapPanel>(this, "WrapPanel");
        if (_wrapPanel != null)
        {
            foreach (var button in  _wrapPanel.Children)
            {
                
            }
        }
    }
}