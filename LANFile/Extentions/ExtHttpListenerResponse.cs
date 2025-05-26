using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LANFile.Enums;

namespace LANFile.Extentions;

public static class ExtHttpListenerResponse
{
    public static async Task WriteAsyncString(
        this HttpListenerResponse httpListenerResponse,
        string Data,
        string ContentType = ContentTypes.Json
    )
    {
        Console.WriteLine($"Send: {Data}");
        byte[] data = Encoding.UTF8.GetBytes(Data);
        httpListenerResponse.ContentType = ContentType;
        httpListenerResponse.ContentEncoding = Encoding.UTF8;
        httpListenerResponse.ContentLength64 = data.LongLength;


        await httpListenerResponse.OutputStream.WriteAsync(data, 0, data.Length);
    }

    public static async Task WriteAsyncString(
        this HttpListenerResponse httpListenerResponse,
        object Data,
        string ContentType = ContentTypes.Json
    )
    {
        if (Data != null)
            await httpListenerResponse.WriteAsyncString(Data.ToString());
    }

    public static async Task WriteAsyncArray(
        this HttpListenerResponse httpListenerResponse,
        List<string> Data,
        string ContentType = ContentTypes.Json
    )
    {
        await httpListenerResponse.WriteAsyncString(string.Join(":-:", Data));
    }

    public static async Task WriteAsyncDictionary(
        this HttpListenerResponse httpListenerResponse,
        Dictionary<string, string> Data,
        string ContentType = ContentTypes.Json
    )
    {
        string[] list = Data.Select(a => $"[{a.Key}]:{a.Value}").ToArray();

        await httpListenerResponse.WriteAsyncString(string.Join("\n", list));
    }
}