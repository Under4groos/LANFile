using System;
using System.Collections.Generic;
using System.Net;

namespace BeaconLib;

internal class IPEndPointComparer : IComparer<IPEndPoint>
{
    public static readonly IPEndPointComparer Instance = new();

    public int Compare(IPEndPoint x, IPEndPoint y)
    {
        var c = string.Compare(x.Address.ToString(), y.Address.ToString(), StringComparison.Ordinal);
        if (c != 0) return c;
        return y.Port - x.Port;
    }
}