using Julspelet.Shared.Models.Networking;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Interface for network communication services.
/// Abstracts the underlying transport (sockets, SignalR, WebRTC) so the same
/// game logic can work across web and native platforms.
/// </summary>
public interface INetworkService
{
    /// <summary>
    /// Gets the current peer ID for this instance.
    /// </summary>
    string PeerId { get; }

    /// <summary>
    /// Gets whether this peer is currently connected to a session.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets whether this peer is the host of the current session.
    /// </summary>
    bool IsHost { get; }

    /// <summary>
    /// Event raised when a message is received from a peer.
    /// </summary>
    event EventHandler<NetworkMessage>? MessageReceived;

    /// <summary>
    /// Event raised when a peer connects to the session.
    /// </summary>
    event EventHandler<PeerInfo>? PeerConnected;

    /// <summary>
    /// Event raised when a peer disconnects from the session.
    /// </summary>
    event EventHandler<PeerInfo>? PeerDisconnected;

    /// <summary>
    /// Event raised when the connection state changes.
    /// </summary>
    event EventHandler<ConnectionState>? ConnectionStateChanged;

    /// <summary>
    /// Discovers available game sessions on the network.
    /// </summary>
    /// <param name="timeoutSeconds">How long to search for sessions</param>
    /// <returns>List of discovered sessions</returns>
    Task<List<GameSession>> DiscoverSessionsAsync(int timeoutSeconds = 5);

    /// <summary>
    /// Creates a new game session as the host.
    /// </summary>
    /// <param name="sessionName">Name for the session</param>
    /// <param name="maxPlayers">Maximum number of players</param>
    /// <param name="password">Optional password for private sessions</param>
    /// <returns>The created game session</returns>
    Task<GameSession> CreateSessionAsync(string sessionName, int maxPlayers = 6, string? password = null);

    /// <summary>
    /// Joins an existing game session.
    /// </summary>
    /// <param name="session">The session to join</param>
    /// <param name="playerName">Name of the joining player</param>
    /// <param name="password">Password if the session is private</param>
    Task JoinSessionAsync(GameSession session, string playerName, string? password = null);

    /// <summary>
    /// Leaves the current session.
    /// </summary>
    Task LeaveSessionAsync();

    /// <summary>
    /// Sends a message to all connected peers.
    /// </summary>
    /// <param name="message">The message to send</param>
    Task SendMessageAsync(NetworkMessage message);

    /// <summary>
    /// Sends a message to a specific peer.
    /// </summary>
    /// <param name="peerId">ID of the target peer</param>
    /// <param name="message">The message to send</param>
    Task SendMessageToAsync(string peerId, NetworkMessage message);

    /// <summary>
    /// Gets information about all connected peers.
    /// </summary>
    List<PeerInfo> GetConnectedPeers();

    /// <summary>
    /// Gets the current session information.
    /// </summary>
    GameSession? GetCurrentSession();

    /// <summary>
    /// Starts the network service and begins listening for connections (for hosts).
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Stops the network service and closes all connections.
    /// </summary>
    Task StopAsync();
}
