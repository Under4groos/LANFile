using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace SuperSimpleTcp;

internal class ClientMetadata : IDisposable
{
    #region Constructors-and-Factories

    internal ClientMetadata(TcpClient tcp)
    {
        if (tcp == null) throw new ArgumentNullException(nameof(tcp));

        Client = tcp;
        NetworkStream = tcp.GetStream();
        IpPort = tcp.Client.RemoteEndPoint.ToString();
        TokenSource = new CancellationTokenSource();
        Token = TokenSource.Token;
    }

    #endregion

    #region Public-Methods

    public void Dispose()
    {
        if (TokenSource != null)
            if (!TokenSource.IsCancellationRequested)
            {
                TokenSource.Cancel();
                TokenSource.Dispose();
            }

        if (SslStream != null) SslStream.Close();

        if (NetworkStream != null) NetworkStream.Close();

        if (Client != null)
        {
            Client.Close();
            Client.Dispose();
        }

        SendLock.Dispose();
        ReceiveLock.Dispose();
    }

    #endregion

    #region Public-Members

    internal TcpClient Client { get; }

    internal NetworkStream NetworkStream { get; }

    internal SslStream SslStream { get; set; }

    internal string IpPort { get; }

    internal SemaphoreSlim SendLock = new(1, 1);
    internal SemaphoreSlim ReceiveLock = new(1, 1);

    internal CancellationTokenSource TokenSource { get; set; }

    internal CancellationToken Token { get; set; }

    #endregion

    #region Private-Members

    #endregion
}