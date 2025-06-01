using System;

namespace SuperSimpleTcp;

/// <summary>
///     SimpleTcp statistics.
/// </summary>
public class SimpleTcpStatistics
{
    #region Constructors-and-Factories

    /// <summary>
    ///     Initialize the statistics object.
    /// </summary>
    public SimpleTcpStatistics()
    {
    }

    #endregion

    #region Public-Members

    /// <summary>
    ///     The time at which the client or server was started.
    /// </summary>
    public DateTime StartTime { get; } = DateTime.Now.ToUniversalTime();

    /// <summary>
    ///     The amount of time which the client or server has been up.
    /// </summary>
    public TimeSpan UpTime => DateTime.Now.ToUniversalTime() - StartTime;

    /// <summary>
    ///     The number of bytes received.
    /// </summary>
    public long ReceivedBytes { get; internal set; }

    /// <summary>
    ///     The number of bytes sent.
    /// </summary>
    public long SentBytes { get; internal set; }

    #endregion

    #region Private-Members

    #endregion

    #region Public-Methods

    /// <summary>
    ///     Return human-readable version of the object.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var ret =
            "--- Statistics ---" + Environment.NewLine +
            "    Started        : " + StartTime + Environment.NewLine +
            "    Uptime         : " + UpTime + Environment.NewLine +
            "    Received bytes : " + ReceivedBytes + Environment.NewLine +
            "    Sent bytes     : " + SentBytes + Environment.NewLine;
        return ret;
    }

    /// <summary>
    ///     Reset statistics other than StartTime and UpTime.
    /// </summary>
    public void Reset()
    {
        ReceivedBytes = 0;
        SentBytes = 0;
    }

    #endregion
}