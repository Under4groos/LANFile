using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using LANFile.Helper;
using LANFile.Models;
using SuperSimpleTcp;

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


    public static readonly StyledProperty<string> PingProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(Ping), "0ms");

    public string Ping
    {
        get => GetValue(PingProperty);
        set => SetValue(PingProperty, value);
    }

    public static readonly StyledProperty<string> HttpResultProperty =
        AvaloniaProperty.Register<LanDevice, string>(nameof(HttpResult), "<null>");

    public string HttpResult
    {
        get => GetValue(HttpResultProperty);
        set => SetValue(HttpResultProperty, value);
    }


    private WrapPanel? _wrapPanel;
    private Thread? _thread;
    private SimpleTcpClient? _simpleTcpClient;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);


        _wrapPanel = ControlHelper.FindTemplateControlByName<WrapPanel>(this, "WrapPanel");
        var buttons = ControlHelper.FindTemplateControlsByType<WrapPanel>(this, typeof(Button))
            .Select(a => (Button)a).ToArray();
        foreach (var button in buttons) button.Click += ButtonOnClick;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _simpleTcpClient?.Disconnect();
        _simpleTcpClient?.Dispose();
        _simpleTcpClient = null;
        base.OnUnloaded(e);
    }

    private void ButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        Console.WriteLine($"{button.Tag} {button.Content} {Host} ");
        switch (button.Tag)
        {
            case "send":

                break;
            case "tcp":
            {
                try
                {
                    if (_simpleTcpClient != null)
                    {
                        if (_simpleTcpClient.IsConnected)
                        {
                            _simpleTcpClient.Send("Hello, world!");
                        }
                        else
                        {
                            _simpleTcpClient?.Dispose();
                            _simpleTcpClient = null;
                        }
                    }

                    if (_simpleTcpClient == null)
                    {
                        _simpleTcpClient = new SimpleTcpClient($"{Host}:9000");

                        _simpleTcpClient.Connect();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }


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