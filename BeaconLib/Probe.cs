using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BeaconLib;

/// <summary>
///     Counterpart of the beacon, searches for beacons
/// </summary>
/// <remarks>
///     The beacon list event will not be raised on your main thread!
/// </remarks>
public class Probe : IDisposable
{
    /// <summary>
    ///     Remove beacons older than this
    /// </summary>
    private static readonly TimeSpan BeaconTimeout = new(0, 0, 0, 5); // seconds

    private Task _mainTask;
    private readonly UdpClient _udp = new();
    private readonly EventWaitHandle _waitHandle = new(false, EventResetMode.AutoReset);
    private IEnumerable<BeaconLocation> _currentBeacons = Enumerable.Empty<BeaconLocation>();


    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    public Probe(string beaconType, IPAddress endAny = null, bool isBackground = false)
    {
        _udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        BeaconType = beaconType;
        if (endAny != null) _udp.Client.Bind(new IPEndPoint(endAny, 0));

        #region TARGET_WINDOWS

        try
        {
            _udp.AllowNatTraversal(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error switching on NAT traversal: " + ex.Message);
        }

        #endregion

        _udp.BeginReceive(ResponseReceived, null);

        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public string BeaconType { get; }

    public void Dispose()
    {
        try
        {
            Stop();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    public event Action<IEnumerable<BeaconLocation>> BeaconsUpdated;

    public void Start()
    {
        _mainTask = Task.Run(async () =>
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    BroadcastProbe();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                await Task.Delay(2000, _cancellationToken);
                PruneBeacons();
            }

            Console.WriteLine($"{_cancellationTokenSource.Token} stopped.");
        }, _cancellationToken);
    }

    private void ResponseReceived(IAsyncResult ar)
    {
        if (_cancellationToken.IsCancellationRequested || _udp.Client == null)
            return;
        var remote = new IPEndPoint(IPAddress.Any, 0);
        var bytes = _udp.EndReceive(ar, ref remote);

        var typeBytes = Beacon.Encode(BeaconType).ToList();
        Debug.WriteLine(string.Join(", ", typeBytes.Select(_ => (char)_)));
        if (Beacon.HasPrefix(bytes, typeBytes))
            try
            {
                var portBytes = bytes.Skip(typeBytes.Count()).Take(2).ToArray();
                var port = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(portBytes, 0));
                var payload = Beacon.Decode(bytes.Skip(typeBytes.Count() + 2));
                NewBeacon(new BeaconLocation(new IPEndPoint(remote.Address, port), payload, DateTime.Now));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
    }

    public void BeginReceived()
    {
        try
        {
            _udp.BeginReceive(ResponseReceived, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void BroadcastProbe()
    {
        var probe = Beacon.Encode(BeaconType).ToArray();
        _udp.Send(probe, probe.Length, new IPEndPoint(IPAddress.Broadcast, Beacon.DiscoveryPort));
    }

    private void PruneBeacons()
    {
        var cutOff = DateTime.Now - BeaconTimeout;
        var oldBeacons = _currentBeacons.ToList();
        var newBeacons = oldBeacons.Where(_ => _.LastAdvertised >= cutOff).ToList();
        if (EnumsEqual(oldBeacons, newBeacons)) return;

        var u = BeaconsUpdated;
        if (u != null) u(newBeacons);
        _currentBeacons = newBeacons;
    }

    private void NewBeacon(BeaconLocation newBeacon)
    {
        var newBeacons = _currentBeacons
            .Where(_ => !_.Equals(newBeacon))
            .Concat(new[] { newBeacon })
            .OrderBy(_ => _.Data)
            .ThenBy(_ => _.Address, IPEndPointComparer.Instance)
            .ToList();
        var u = BeaconsUpdated;
        if (u != null) u(newBeacons);
        _currentBeacons = newBeacons;
    }

    private static bool EnumsEqual<T>(IEnumerable<T> xs, IEnumerable<T> ys)
    {
        return xs.Zip(ys, (x, y) => x.Equals(y)).Count() == xs.Count();
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _udp?.Dispose();
    }
}