namespace SuperSimpleTcp;

using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

internal class ClientMetadata : IDisposable
{
    #region Public-Members

    internal TcpClient Client => _tcpClient;

    internal NetworkStream NetworkStream => _networkStream;

    internal SslStream SslStream
    {
        get => _sslStream;
        set => _sslStream = value;
    }

    internal string IpPort => _ipPort;

    internal SemaphoreSlim SendLock = new(1, 1);
    internal SemaphoreSlim ReceiveLock = new(1, 1);

    internal CancellationTokenSource TokenSource { get; set; }

    internal CancellationToken Token { get; set; }

    #endregion

    #region Private-Members

    private TcpClient _tcpClient = null;
    private NetworkStream _networkStream = null;
    private SslStream _sslStream = null;
    private string _ipPort = null;

    #endregion

    #region Constructors-and-Factories

    internal ClientMetadata(TcpClient tcp)
    {
        if (tcp == null) throw new ArgumentNullException(nameof(tcp));

        _tcpClient = tcp;
        _networkStream = tcp.GetStream();
        _ipPort = tcp.Client.RemoteEndPoint.ToString();
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

        if (_sslStream != null) _sslStream.Close();

        if (_networkStream != null) _networkStream.Close();

        if (_tcpClient != null)
        {
            _tcpClient.Close();
            _tcpClient.Dispose();
        }

        SendLock.Dispose();
        ReceiveLock.Dispose();
    }

    #endregion
}