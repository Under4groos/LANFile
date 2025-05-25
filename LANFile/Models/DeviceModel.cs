using CommunityToolkit.Mvvm.ComponentModel;
using LANFile.ViewModels;

namespace LANFile.Models;

public partial class DeviceModel : ViewModelBase
{
    [ObservableProperty] private string? _name = string.Empty;
    [ObservableProperty] private string? _host = string.Empty;
    [ObservableProperty] private string? _os = string.Empty;
    [ObservableProperty] private int? _ping = 0;
}