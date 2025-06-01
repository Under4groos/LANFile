using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using BeaconLib;
using Java.Net;
using Java.Util;
using LANFile.Models;
using SuperSimpleTcp;

namespace LANFile.Helper;

[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
[SuppressMessage("Interoperability", "CA1422:Проверка совместимости платформы")]
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class MultiConnectionService : IDisposable
{
    private const string _fileDeviceInfo = "_devices.txt";
    private readonly ObservableCollection<DeviceModel> _devices = [];
    private Beacon? _beacon;
    private List<BeaconLocation> _beaconLocations = [];
    private Probe? _probe;
    public Action<ObservableCollection<DeviceModel>> DevicesUpdated;
    public IPAddress Host;

    #region SimpleTcpServer

    private readonly SimpleTcpServer _tcpServer = null;
    private readonly int _tcpServerPort = 9459;

    #endregion


    public MultiConnectionService()
    {
        Platform = $"{(OperatingSystem.IsAndroid() ? "Android" : "Windows")}";

        var adressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        Host = adressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToArray().Last();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) Host = AndroidGetLocalIp();


        _tcpServer = new SimpleTcpServer($"{Host}:{_tcpServerPort}");
        Debug.WriteLine($"{Host}:{_tcpServerPort}");
    }

    public string NameApplication { get; set; } = $"{Guid.NewGuid().ToString().Substring(0, 8)}";
    public static string Platform { get; set; } = string.Empty;
    public string ConnectionName { get; set; } = "LANFileConnect";

    public void Dispose()
    {
        CloseAll();
        GC.SuppressFinalize(this);
        GC.Collect();
    }


    public void CloseAll()
    {
        _beacon?.Stop();
        _beacon?.Dispose();
        _probe?.Stop();
        _probe?.Dispose();

        if (_tcpServer != null && _tcpServer.IsListening)
            _tcpServer?.Stop();

        _probe = null;
        _beacon = null;
    }

    public void TCPServerStart()
    {
        try
        {
            if (_tcpServer.IsListening)
            {
                _tcpServer.Stop();
            }

            if (!_tcpServer.IsListening)
                _tcpServer?.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public void ProbeStart()
    {
        this.CloseAll();

        _probe = new Probe(ConnectionName, IPAddress.Any);
        _probe.BeaconsUpdated += ProbeOnBeaconsUpdated;


        _probe?.Start();
        InvoceBeginReceivedProbeOrBeacon();
    }


    public void BeaconStart()
    {
        this.CloseAll();
        _beacon = new Beacon(ConnectionName, 4863, IPAddress.Any);
        _beacon.BeaconData =
            $"{Platform};{NameApplication};{(_tcpServer != null ? _tcpServer.IpPort : "0.0.0.0:0000")}";
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
        var beaconLocations = beacons as BeaconLocation[] ?? beacons.ToArray();
        _beaconLocations = _beaconLocations.Union(beaconLocations.Where(b => !string.IsNullOrEmpty(b.Data))).ToList();
        if (_beaconLocations.Any())
        {
            var _listdevices = new List<DeviceModel>();
            foreach (var beaconLocation in _beaconLocations)
            {
                var spData = beaconLocation.Data.Split(";").Select(l => l.Trim()).ToArray();
                if (spData.Count() < 2)
                    continue;
                var ipHostTcp = spData[2];
                var deviceName = spData[1];
                var plaatform = spData[0];


                var device = new DeviceModel
                {
                    Host = beaconLocation.Address.Address.ToString(),
                    Name = deviceName,
                    Os = $"{plaatform.ToLower()}",
                    Port = beaconLocation.Address.Port.ToString(),
                    IpTcpHost = ipHostTcp
                };
                if (device.Host != Host.ToString())
                    _listdevices.Add(device);
            }

            foreach (var deviceModel in _listdevices.Union(_listdevices))
                if (_devices.FirstOrDefault(d =>
                        d.Host == deviceModel.Host && !string.IsNullOrEmpty(deviceModel.Host)) == null)
                    _devices.Add(deviceModel);
        }


        var deviceLines = string.Join("\n", _devices.Select(d => d.ToLineString()));
        StorageHelper.WriteToFile("devices.txt", deviceLines);
        DevicesUpdated?.Invoke(_devices);
    }

    public IPAddress? AndroidGetLocalIp()
    {
        if (OperatingSystem.IsAndroid())
        {
            var allNetworkInterfaces = Collections.List(NetworkInterface.NetworkInterfaces);

            foreach (var interfaces in allNetworkInterfaces)
            {
                var addressInterface = (interfaces as NetworkInterface)?.InterfaceAddresses;
                if (addressInterface != null)
                {
                    var arrayAdress = addressInterface.Where(i => i.Broadcast != null)
                        .Select(i => i.Address?.HostAddress).Where(host => host != null && host.StartsWith("192.168."));
                    var enumerable = arrayAdress as string[] ?? arrayAdress.ToArray();
                    if (!enumerable.Any())
                        continue;

                    if (IPAddress.TryParse(enumerable.First(), out var ip) && ip != null) return ip;
                }
            }
        }

        return null;
    }

    public void ClearFileData()
    {
        StorageHelper.WriteToFile("devices.txt", "");
        _devices.Clear();
        _beaconLocations.Clear();
    }

    public void LoadedLastDevices()
    {
        try
        {
            foreach (var line in StorageHelper.ReadFile("devices.txt").Split('\n').Select(a => a.Trim())
                         .Where(a => !string.IsNullOrWhiteSpace(a))
                    )
            {
                var _device = DeviceModel.Parse(line.Trim());
                if (_devices.FirstOrDefault(d => d.Host == _device.Host && !string.IsNullOrEmpty(_device.Host)) == null)
                    _devices.Add(_device);
            }

            DevicesUpdated?.Invoke(_devices);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}