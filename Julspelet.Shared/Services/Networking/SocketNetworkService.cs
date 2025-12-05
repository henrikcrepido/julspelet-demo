using Julspelet.Shared.Models.Networking;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Socket-based network service for native MAUI platforms.
/// Uses UDP for discovery and TCP for reliable game communication.
/// This provides better performance and works offline on local networks.
/// 
/// Platform-specific implementations will be in the MAUI project's Platforms folder.
/// </summary>
public class SocketNetworkService : INetworkService
{
    private GameSession? _currentSession;
    private readonly List<PeerInfo> _connectedPeers = new();
    private ConnectionState _connectionState = ConnectionState.Disconnected;
    private long _messageSequence = 0;

    public string PeerId { get; private set; } = Guid.NewGuid().ToString();
    public bool IsConnected => _connectionState == ConnectionState.Connected;
    public bool IsHost { get; private set; }

    public event EventHandler<NetworkMessage>? MessageReceived;
    public event EventHandler<PeerInfo>? PeerConnected;
    public event EventHandler<PeerInfo>? PeerDisconnected;
    public event EventHandler<ConnectionState>? ConnectionStateChanged;

    public Task InitializeAsync()
    {
        // Socket initialization happens in StartAsync
        return Task.CompletedTask;
    }

    public Task<List<GameSession>> DiscoverSessionsAsync(int timeoutSeconds = 5)
    {
        // TODO: Implement UDP multicast/broadcast discovery
        // Send discovery packets on the local network
        // Listen for responses from hosts
        // Return list of discovered sessions

        // For now, return empty list - platform-specific implementation needed
        return Task.FromResult(new List<GameSession>());
    }

    public Task<GameSession> CreateSessionAsync(string hostName, int maxPlayers, NetworkType networkType)
    {
        // TODO: Implement TCP server socket
        // Start listening on a port
        // Begin advertising the session via UDP broadcast

        _currentSession = new GameSession
        {
            SessionId = Guid.NewGuid().ToString(),
            SessionName = $"{hostName}'s Game",
            HostId = PeerId,
            HostName = hostName,
            MaxPlayers = maxPlayers,
            NetworkType = networkType,
            ConnectionInfo = $"0.0.0.0:0", // TODO: Set actual IP and port
            LocalPeerId = PeerId,
            IsHost = true
        };

        IsHost = true;
        SetConnectionState(ConnectionState.Connected);

        return Task.FromResult(_currentSession);
    }

    public Task JoinSessionAsync(string sessionId)
    {
        // TODO: Implement joining by session ID
        // This would typically involve discovering the session first
        throw new NotImplementedException("Session discovery required for socket-based joining");
    }

    public Task JoinSessionAsync(GameSession session, string playerName, string? password = null)
    {
        // TODO: Implement TCP client socket
        // Connect to the host's IP and port
        // Send join request

        if (!string.IsNullOrEmpty(session.Password) && session.Password != password)
            throw new UnauthorizedAccessException("Invalid password");

        _currentSession = session;
        IsHost = false;
        SetConnectionState(ConnectionState.Connected);

        return Task.CompletedTask;
    }

    public Task LeaveSessionAsync()
    {
        // TODO: Close all socket connections
        // Stop UDP discovery if host

        _currentSession = null;
        _connectedPeers.Clear();
        SetConnectionState(ConnectionState.Disconnected);

        return Task.CompletedTask;
    }

    public Task SendMessageAsync(NetworkMessage message)
    {
        // TODO: Serialize and send message to all connected peers via TCP
        message.SenderId = PeerId;
        message.SequenceNumber = Interlocked.Increment(ref _messageSequence);

        return Task.CompletedTask;
    }

    public Task SendMessageAsync(NetworkMessage message, string peerId)
    {
        // TODO: Send message to specific peer via TCP
        message.SenderId = PeerId;
        message.SequenceNumber = Interlocked.Increment(ref _messageSequence);

        return Task.CompletedTask;
    }

    public List<PeerInfo> GetConnectedPeers()
    {
        return new List<PeerInfo>(_connectedPeers);
    }

    public Task<GameSession?> GetCurrentSessionAsync()
    {
        return Task.FromResult(_currentSession);
    }

    public Task StartAsync()
    {
        // TODO: Initialize socket infrastructure
        SetConnectionState(ConnectionState.Disconnected);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        // TODO: Clean up all sockets and resources
        SetConnectionState(ConnectionState.Disconnected);
        return Task.CompletedTask;
    }

    private void SetConnectionState(ConnectionState newState)
    {
        if (_connectionState != newState)
        {
            _connectionState = newState;
            ConnectionStateChanged?.Invoke(this, newState);
        }
    }
}
