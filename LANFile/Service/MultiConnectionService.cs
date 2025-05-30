using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using BeaconLib;
using Java.Util;
using LANFile.Models;

namespace LANFile.Helper;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
[SuppressMessage("Interoperability", "CA1422:Проверка совместимости платформы")]
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class MultiConnectionService : IDisposable
{
    private ObservableCollection<DeviceModel> _devices = [];
    private Beacon? _beacon = null;
    private Probe? _probe = null;
    public static string NameApplication { get; set; } = $"{Guid.NewGuid().ToString().Substring(0, 9)}";

    public Action<ObservableCollection<DeviceModel>> DevicesUpdated;
    public IPAddress Host;
    public static string Platform { get; set; } = string.Empty;


    public string ConnectionName { get; set; } = "LANFileApplication";

    public MultiConnectionService()
    {
        Platform = $"{(OperatingSystem.IsAndroid() ? "Android" : "Windows")}";


        var adressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        Host = adressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToArray().Last();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Host = AndroidGetLocalIp();
        }
    }


    public void CloseAll()
    {
        _beacon?.Stop();
        _beacon?.Dispose();
        _probe?.Stop();
        _probe?.Dispose();


        _probe = null;
        _beacon = null;
    }

    public void ProbeStart()
    {
        _probe = new Probe(ConnectionName, Host);
        _probe.BeaconsUpdated += ProbeOnBeaconsUpdated;


        _probe?.Start();
        InvoceBeginReceivedProbeOrBeacon();
    }

    public void BeaconStart()
    {
        _beacon = new Beacon(ConnectionName, 4863, IPAddress.Any);
        _beacon.BeaconData = $"{Platform}-{NameApplication}";
        _beacon?.Start();
        InvoceBeginReceivedProbeOrBeacon();
    }

    public void InvoceBeginReceivedProbeOrBeacon()
    {
        _probe?.BeginReceived();
        _beacon?.BeginReceived();
    }

    public void ClearDevices()
    {
        _devices.Clear();
    }

    private void ProbeOnBeaconsUpdated(IEnumerable<BeaconLocation> beacons)
    {
        if (beacons.Any())
        {
            foreach (var beacon in beacons.Where(b => !string.IsNullOrEmpty(b.Data)).ToArray())
            {
                var spData = beacon.Data.Split("-");
                if (spData.Length < 2)
                    continue;

                var deviceName = spData[1];
                var plaatform = spData[0];

                var device = new DeviceModel
                {
                    Host = beacon.Address.Address.ToString(),
                    Name = deviceName,
                    Os = $"{plaatform.ToLower()}",
                    Port = beacon.Address.Port.ToString(),
                    Ping = 0
                };
                if (_devices.FirstOrDefault(d => d.Host == device.Host && d.Port == device.Port) == null)
                    _devices.Add(device);
            }
        }

        DevicesUpdated?.Invoke(_devices);
    }


    public IPAddress? AndroidGetLocalIp()
    {
        if (OperatingSystem.IsAndroid())
        {
            var allNetworkInterfaces = Collections.List(Java.Net.NetworkInterface.NetworkInterfaces);

            foreach (var interfaces in allNetworkInterfaces)
            {
                var addressInterface = (interfaces as Java.Net.NetworkInterface)?.InterfaceAddresses;
                if (addressInterface != null)
                {
                    var arrayAdress = addressInterface.Where(i => i.Broadcast != null)
                        .Select(i => i.Address?.HostAddress).Where(host => host != null && host.StartsWith("192.168."));
                    var enumerable = arrayAdress as string[] ?? arrayAdress.ToArray();
                    if (!enumerable.Any())
                        continue;

                    if (IPAddress.TryParse(enumerable.First(), out IPAddress? ip) && ip != null)
                    {
                        return ip;
                    }
                }
            }
        }

        return null;
    }


    public void Dispose()
    {
        CloseAll();
        GC.SuppressFinalize(this);
        GC.Collect();
    }
}