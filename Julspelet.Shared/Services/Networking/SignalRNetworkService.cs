using System.Text.Json;
using Julspelet.Shared.Models.Networking;
using Microsoft.AspNetCore.SignalR.Client;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// SignalR-based network service for web platforms.
/// Uses ASP.NET Core SignalR for real-time communication.
/// This works for both web browsers and can be used as a relay for MAUI apps.
/// </summary>
public class SignalRNetworkService : INetworkService
{
    private HubConnection? _hubConnection;
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

    public async Task<List<GameSession>> DiscoverSessionsAsync(int timeoutSeconds = 5)
    {
        // For SignalR, discovery happens through a central server
        // The server maintains a list of active sessions
        if (_hubConnection == null)
            return new List<GameSession>();

        try
        {
            var sessions = await _hubConnection.InvokeAsync<List<GameSession>>(
                "GetAvailableSessions",
                CancellationToken.None);
            return sessions ?? new List<GameSession>();
        }
        catch
        {
            return new List<GameSession>();
        }
    }

    public async Task<GameSession> CreateSessionAsync(string sessionName, int maxPlayers = 6, string? password = null)
    {
        if (_hubConnection == null)
            throw new InvalidOperationException("Network service not started");

        _currentSession = new GameSession
        {
            SessionId = Guid.NewGuid().ToString(),
            SessionName = sessionName,
            HostId = PeerId,
            HostName = sessionName, // Will be set by the caller
            MaxPlayers = maxPlayers,
            Password = password,
            NetworkType = NetworkType.SignalR
        };

        IsHost = true;

        // Register session with SignalR hub
        await _hubConnection.InvokeAsync("CreateSession", _currentSession);

        SetConnectionState(ConnectionState.Connected);
        return _currentSession;
    }

    public async Task JoinSessionAsync(GameSession session, string playerName, string? password = null)
    {
        if (_hubConnection == null)
            throw new InvalidOperationException("Network service not started");

        // Validate password if required
        if (!string.IsNullOrEmpty(session.Password) && session.Password != password)
            throw new UnauthorizedAccessException("Invalid password");

        _currentSession = session;
        IsHost = false;

        // Join the SignalR group for this session
        await _hubConnection.InvokeAsync("JoinSession", session.SessionId, playerName);

        SetConnectionState(ConnectionState.Connected);

        // Notify peers that we joined
        var joinMessage = new PlayerJoinedMessage
        {
            SenderId = PeerId,
            PlayerName = playerName,
            PlayerId = PeerId,
            SequenceNumber = Interlocked.Increment(ref _messageSequence)
        };

        await SendMessageAsync(joinMessage);
    }

    public async Task LeaveSessionAsync()
    {
        if (_hubConnection == null || _currentSession == null)
            return;

        var leaveMessage = new PlayerLeftMessage
        {
            SenderId = PeerId,
            PlayerId = PeerId,
            Reason = "Player left",
            SequenceNumber = Interlocked.Increment(ref _messageSequence)
        };

        await SendMessageAsync(leaveMessage);
        await _hubConnection.InvokeAsync("LeaveSession", _currentSession.SessionId);

        _currentSession = null;
        _connectedPeers.Clear();
        SetConnectionState(ConnectionState.Disconnected);
    }

    public async Task SendMessageAsync(NetworkMessage message)
    {
        if (_hubConnection == null || _currentSession == null)
            return;

        message.SenderId = PeerId;
        var json = JsonSerializer.Serialize(message, message.GetType());
        await _hubConnection.InvokeAsync("SendMessage", _currentSession.SessionId, json);
    }

    public async Task SendMessageToAsync(string peerId, NetworkMessage message)
    {
        if (_hubConnection == null)
            return;

        message.SenderId = PeerId;
        var json = JsonSerializer.Serialize(message, message.GetType());
        await _hubConnection.InvokeAsync("SendMessageToPeer", peerId, json);
    }

    public List<PeerInfo> GetConnectedPeers()
    {
        return new List<PeerInfo>(_connectedPeers);
    }

    public GameSession? GetCurrentSession()
    {
        return _currentSession;
    }

    public async Task StartAsync()
    {
        // This would connect to the SignalR hub
        // The hub URL should be configured through dependency injection
        // For now, this is a placeholder that would need the actual hub URL
        var hubUrl = "https://localhost:5027/gamehub"; // TODO: Make this configurable

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        // Register message handlers
        _hubConnection.On<string>("ReceiveMessage", (json) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<NetworkMessage>(json);
                if (message != null)
                {
                    MessageReceived?.Invoke(this, message);
                }
            }
            catch (JsonException)
            {
                // Invalid message format
            }
        });

        _hubConnection.On<string, string>("PeerJoined", (peerId, playerName) =>
        {
            var peer = new PeerInfo
            {
                PeerId = peerId,
                PlayerName = playerName,
                State = ConnectionState.Connected,
                Platform = "Web"
            };

            _connectedPeers.Add(peer);
            PeerConnected?.Invoke(this, peer);
        });

        _hubConnection.On<string>("PeerLeft", (peerId) =>
        {
            var peer = _connectedPeers.FirstOrDefault(p => p.PeerId == peerId);
            if (peer != null)
            {
                _connectedPeers.Remove(peer);
                PeerDisconnected?.Invoke(this, peer);
            }
        });

        _hubConnection.Reconnecting += error =>
        {
            SetConnectionState(ConnectionState.Reconnecting);
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            SetConnectionState(ConnectionState.Connected);
            return Task.CompletedTask;
        };

        _hubConnection.Closed += error =>
        {
            SetConnectionState(ConnectionState.Failed);
            return Task.CompletedTask;
        };

        SetConnectionState(ConnectionState.Connecting);
        await _hubConnection.StartAsync();
        SetConnectionState(ConnectionState.Connected);
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }

        SetConnectionState(ConnectionState.Disconnected);
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
