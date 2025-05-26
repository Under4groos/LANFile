using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LANFile.Enums;
using LANFile.Events;
using LANFile.Extentions;

namespace LANFile.Server
{
    public class HttpServer : IDisposable
    {
        public HttpListener? listener;
        public string? Host { get; private set; }
        public bool RunServer
        {
            get; private set;
        }
        public HttpEvents.EventHttpListenerResponse? OnHttpListenerResponse;
      

        public void Start(string host = "http://localhost:8080/")
        {
            Host = host;
            listener = new HttpListener();
            
            listener.Prefixes.Add(Host);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", Host);
            Task listenTask = HandleIncomingConnections();
        }



        public async Task HandleIncomingConnections()
        {

            if (listener == null)
            {
                Console.WriteLine($"Listener is null.");
                return;
            }
            RunServer = true;

            while (RunServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                try
                {

                    Dictionary<string, string> Query = req.QueryString.ToDictionary();
                    string method = req.HttpMethod;

                     
                    Console.WriteLine($"{req.Url.AbsolutePath}");
                    OnHttpListenerResponse?.Invoke(req, resp, Query, method, req.Url);
                }
                catch (Exception e)
                {
                    await resp.WriteAsyncString(e.Message, ContentTypes.Json);

                }
                resp.Close();


            }
        }

        public void Dispose()
        {

            listener?.Close();
        }
    }
}
