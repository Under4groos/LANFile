using CommunityToolkit.Mvvm.ComponentModel;
using LANFile.ViewModels;

namespace LANFile.Models;

public partial class DeviceModel : ViewModelBase
{
    [ObservableProperty] private string? _host = string.Empty;
    [ObservableProperty] private string? _name = string.Empty;
    [ObservableProperty] private string? _os = string.Empty;
    [ObservableProperty] private int? _ping = 0;
    [ObservableProperty] private string? _port = string.Empty;


    public string ToLineString()
    {
        return @$"{Name}\_/{Host}\_/{Port}\_/{Os}";
    }

    public static DeviceModel Parse(string line)
    {
        var device = new DeviceModel();
        if (string.IsNullOrEmpty(line))
            return device;

        var lines = line.Trim().Split(@"\_/");
        if (lines.Length == 4)
        {
            device.Name = lines[0];
            device.Host = lines[1];
            device.Port = lines[2];
            device.Os = lines[3];
        }

        return device;
    }
}