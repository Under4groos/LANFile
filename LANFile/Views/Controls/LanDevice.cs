using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LANFile.Helper;
using SuperSimpleTcp;

namespace LANFile.Views.Controls;

public class LanDevice : ContentControl
{
    public static readonly StyledProperty<bool> IsVisibilityButtonProperty =
        AvaloniaProperty.Register<LanDevice, bool>(nameof(IsVisibilityButton));
    
    public static readonly StyledProperty<string> ConnectNameProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(ConnectName));


    public static readonly StyledProperty<string> OsProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(OsProperty));


    public static readonly StyledProperty<string> PortProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Port));

    public static readonly StyledProperty<string> HostProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Host));


    public static readonly StyledProperty<string> PingProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Ping), "0ms");

    public static readonly StyledProperty<string> IpTcpHostProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(IpTcpHost), "<null>");


    private Thread? _thread;
    private SimpleTcpClient _tcpClient;

    private WrapPanel? _wrapPanel;

    public bool IsVisibilityButton
    {
        get => GetValue(IsVisibilityButtonProperty);
        set => SetValue(IsVisibilityButtonProperty, value);
    }
        
    public string ConnectName
    {
        get => GetValue(ConnectNameProperty);
        set => SetValue(ConnectNameProperty, value);
    }

    public string Os
    {
        get => GetValue(OsProperty);
        set => SetValue(OsProperty, value);
    }

    public string Port
    {
        get => GetValue(PortProperty);
        set => SetValue(PortProperty, value);
    }

    public string Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    public string Ping
    {
        get => GetValue(PingProperty);
        set => SetValue(PingProperty, value);
    }

    public string IpTcpHost
    {
        get => GetValue(IpTcpHostProperty);
        set => SetValue(IpTcpHostProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);


        _wrapPanel = ControlHelper.FindTemplateControlByName<WrapPanel>(this, "WrapPanel");
        var buttons = ControlHelper.FindTemplateControlsByType<WrapPanel>(this, typeof(Button))
            .Select(a => (Button)a).ToArray();
        foreach (var button in buttons) button.Click += ButtonOnClick;
    }

    void TCPClientConnect()
    {
        try
        {
            // if (_tcpClient != null && _tcpClient.IsConnected)
            // {
            //     _tcpClient.Disconnect();
            //     _tcpClient.Dispose();
            //     _tcpClient = null;
            // }
            //
            //
            // _tcpClient = new SimpleTcpClient($"{Host}:{Port}");
            // _tcpClient.Connect();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void ButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        Console.WriteLine($"{button.Tag} {button.Content} {Host} ");
        switch (button.Tag)
        {
            case "send":


                //TCPClientConnect();

                break;
            case "ping":
                Ping = "...";
                var host = Host;

                button.IsEnabled = false;

                _thread = new Thread(async () =>
                {
                    try
                    {
                        var p1 = new Ping();
                        PingReply PR;

                        do
                        {
                            PR = p1.Send(host);
                            var pingStatus = PR.Status == IPStatus.Success
                                ? $"{PR.RoundtripTime} ms"
                                : PR.Status.ToString();


                            Dispatcher.UIThread.Invoke(() =>
                                {
                                    button.IsEnabled = true;
                                    Ping = pingStatus;
                                },
                                DispatcherPriority.Background);
                        } while (PR.Status != IPStatus.Success);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                });

                _thread.Start();
                break;
        }
    }
}