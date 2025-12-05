namespace Julspelet.Shared.Models.Networking;

/// <summary>
/// Information about a connected peer in the P2P network.
/// </summary>
public class PeerInfo
{
    /// <summary>
    /// Unique identifier for the peer.
    /// </summary>
    public string PeerId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Player name associated with this peer.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Display name for UI (defaults to PlayerName).
    /// </summary>
    public string DisplayName => string.IsNullOrWhiteSpace(PlayerName) ? PeerId.Substring(0, 8) : PlayerName;

    /// <summary>
    /// Connection endpoint (IP address, SignalR connection ID, etc.).
    /// </summary>
    public string ConnectionEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Whether this peer is the host of the session.
    /// </summary>
    public bool IsHost { get; set; }

    /// <summary>
    /// Last time a heartbeat was received from this peer.
    /// </summary>
    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current network latency to this peer in milliseconds.
    /// </summary>
    public int Latency { get; set; }

    /// <summary>
    /// Connection state of the peer.
    /// </summary>
    public ConnectionState State { get; set; } = ConnectionState.Connecting;

    /// <summary>
    /// Platform the peer is running on (Web, Android, iOS, Windows, macOS).
    /// </summary>
    public string Platform { get; set; } = string.Empty;
}

/// <summary>
/// Connection state of a peer.
/// </summary>
public enum ConnectionState
{
    /// <summary>
    /// Initial connection in progress.
    /// </summary>
    Connecting,

    /// <summary>
    /// Successfully connected and communicating.
    /// </summary>
    Connected,

    /// <summary>
    /// Connection lost but may reconnect.
    /// </summary>
    Disconnected,

    /// <summary>
    /// Attempting to reconnect.
    /// </summary>
    Reconnecting,

    /// <summary>
    /// Permanently disconnected.
    /// </summary>
    Failed
}
