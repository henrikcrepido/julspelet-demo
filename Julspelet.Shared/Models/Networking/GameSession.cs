namespace Julspelet.Shared.Models.Networking;

/// <summary>
/// Represents a P2P game session that players can discover and join.
/// </summary>
public class GameSession
{
    /// <summary>
    /// Unique identifier for the session.
    /// </summary>
    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Human-readable name for the session.
    /// </summary>
    public string SessionName { get; set; } = string.Empty;

    /// <summary>
    /// ID of the host peer.
    /// </summary>
    public string HostId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the host player.
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// Current number of players in the session.
    /// </summary>
    public int CurrentPlayers { get; set; }

    /// <summary>
    /// Maximum number of players allowed.
    /// </summary>
    public int MaxPlayers { get; set; } = 6;

    /// <summary>
    /// Whether the game has already started.
    /// </summary>
    public bool IsStarted { get; set; }

    /// <summary>
    /// Connection information for joining the session.
    /// Format depends on the network transport (IP:Port for sockets, SignalR URL for web, etc.)
    /// </summary>
    public string ConnectionInfo { get; set; } = string.Empty;

    /// <summary>
    /// When the session was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional password for private sessions.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Network type for this session (Local, Internet, SignalR).
    /// </summary>
    public NetworkType NetworkType { get; set; }

    /// <summary>
    /// Whether the session allows spectators.
    /// </summary>
    public bool AllowSpectators { get; set; }
}

/// <summary>
/// Type of network connection for a game session.
/// </summary>
public enum NetworkType
{
    /// <summary>
    /// Local network (WiFi/LAN) using UDP/TCP sockets.
    /// </summary>
    Local,

    /// <summary>
    /// Internet connection using relay server or NAT traversal.
    /// </summary>
    Internet,

    /// <summary>
    /// Web-based connection using SignalR.
    /// </summary>
    SignalR,

    /// <summary>
    /// WebRTC peer-to-peer connection.
    /// </summary>
    WebRTC
}
