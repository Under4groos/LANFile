using System;
using System.Collections.Generic;
using System.Net;

namespace LANFile.Events;

public class HttpEvents
{
    public delegate void EventHttpListenerResponse(
        HttpListenerRequest request, // Изменено на строчную букву
        HttpListenerResponse response, // Изменено на строчную букву
        Dictionary<string, string> query, // Изменено на строчную букву
        string httpMethod, // Изменено на строчную букву
        Uri? userHostName
    );
}