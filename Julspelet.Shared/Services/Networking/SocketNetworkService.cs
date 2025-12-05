using Julspelet.Shared.Models.Networking;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Socket-based network service for native MAUI platforms.
/// Uses UDP for discovery and TCP for reliable game communication.
/// This provides better performance and works offline on local networks.
/// </summary>
public class SocketNetworkService : INetworkService
{
    private const int DISCOVERY_PORT = 47777;
    private const int DEFAULT_GAME_PORT = 47778;
    private const string DISCOVERY_MESSAGE = "JULSPELET_DISCOVERY";
    private const string DISCOVERY_RESPONSE = "JULSPELET_RESPONSE";

    private GameSession? _currentSession;
    private readonly ConcurrentDictionary<string, PeerInfo> _connectedPeers = new();
    private readonly ConcurrentDictionary<string, TcpClient> _peerConnections = new();
    private ConnectionState _connectionState = ConnectionState.Disconnected;
    private long _messageSequence = 0;

    private UdpClient? _discoveryListener;
    private TcpListener? _gameServer;
    private TcpClient? _gameClient;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _discoveryTask;
    private Task? _serverTask;

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

    public async Task<List<GameSession>> DiscoverSessionsAsync(int timeoutSeconds = 5)
    {
        var discoveredSessions = new ConcurrentBag<GameSession>();
        
        try
        {
            using var udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            
            // Send discovery broadcast
            var discoveryBytes = Encoding.UTF8.GetBytes(DISCOVERY_MESSAGE);
            var broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, DISCOVERY_PORT);
            await udpClient.SendAsync(discoveryBytes, discoveryBytes.Length, broadcastEndpoint);

            // Listen for responses with timeout
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var receiveTask = udpClient.ReceiveAsync();
                    var completedTask = await Task.WhenAny(receiveTask, Task.Delay(100, cts.Token));
                    
                    if (completedTask == receiveTask)
                    {
                        var result = await receiveTask;
                        var message = Encoding.UTF8.GetString(result.Buffer);
                        
                        if (message.StartsWith(DISCOVERY_RESPONSE))
                        {
                            // Parse session info from response
                            var jsonPart = message.Substring(DISCOVERY_RESPONSE.Length);
                            var session = JsonSerializer.Deserialize<GameSession>(jsonPart);
                            
                            if (session != null)
                            {
                                // Update connection info with actual host IP
                                session.ConnectionInfo = $"{result.RemoteEndPoint.Address}:{DEFAULT_GAME_PORT}";
                                discoveredSessions.Add(session);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // Ignore individual receive errors
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Discovery error: {ex.Message}");
        }

        return discoveredSessions.ToList();
    }

    public async Task<GameSession> CreateSessionAsync(string hostName, int maxPlayers, NetworkType networkType)
    {
        IsHost = true;
        _cancellationTokenSource = new CancellationTokenSource();

        // Start TCP server for game communication
        _gameServer = new TcpListener(IPAddress.Any, DEFAULT_GAME_PORT);
        _gameServer.Start();

        // Get local IP address
        var localIp = GetLocalIPAddress();

        _currentSession = new GameSession
        {
            SessionId = Guid.NewGuid().ToString(),
            SessionName = $"{hostName}'s Game",
            HostId = PeerId,
            HostName = hostName,
            MaxPlayers = maxPlayers,
            CurrentPlayers = 1,
            NetworkType = networkType,
            ConnectionInfo = $"{localIp}:{DEFAULT_GAME_PORT}",
            LocalPeerId = PeerId,
            IsHost = true,
            CreatedAt = DateTime.UtcNow
        };

        // Start accepting client connections
        _serverTask = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));

        // Start UDP discovery responder
        _discoveryTask = Task.Run(() => RespondToDiscoveryAsync(_cancellationTokenSource.Token));

        SetConnectionState(ConnectionState.Connected);

        return _currentSession;
    }

    public async Task JoinSessionAsync(string sessionId)
    {
        // Discover sessions and find the one with matching ID
        var sessions = await DiscoverSessionsAsync();
        var session = sessions.FirstOrDefault(s => s.SessionId == sessionId);
        
        if (session == null)
            throw new InvalidOperationException($"Session {sessionId} not found on local network");

        await JoinSessionAsync(session, "Player", null);
    }

    public async Task JoinSessionAsync(GameSession session, string playerName, string? password = null)
    {
        if (!string.IsNullOrEmpty(session.Password) && session.Password != password)
            throw new UnauthorizedAccessException("Invalid password");

        IsHost = false;
        _cancellationTokenSource = new CancellationTokenSource();

        // Parse host IP and port from ConnectionInfo
        var parts = session.ConnectionInfo.Split(':');
        var hostIp = parts[0];
        var port = int.Parse(parts[1]);

        // Connect to host via TCP
        _gameClient = new TcpClient();
        await _gameClient.ConnectAsync(hostIp, port);

        _currentSession = session;
        _currentSession.LocalPeerId = PeerId;
        _currentSession.IsHost = false;

        // Send join message
        var joinMessage = new PlayerJoinedMessage
        {
            SenderId = PeerId,
            PlayerId = PeerId,
            PlayerName = playerName
        };

        await SendMessageAsync(joinMessage);

        // Start receiving messages
        _ = Task.Run(() => ReceiveMessagesAsync(_gameClient, _cancellationTokenSource.Token));

        SetConnectionState(ConnectionState.Connected);
    }

    public async Task LeaveSessionAsync()
    {
        // Cancel all background tasks
        _cancellationTokenSource?.Cancel();

        // Send leave message before disconnecting
        if (IsConnected)
        {
            var leaveMessage = new PlayerLeftMessage
            {
                SenderId = PeerId,
                PlayerId = PeerId
            };

            try
            {
                await SendMessageAsync(leaveMessage);
            }
            catch
            {
                // Ignore errors when leaving
            }
        }

        // Close all connections
        _gameClient?.Close();
        _gameServer?.Stop();
        _discoveryListener?.Close();

        foreach (var connection in _peerConnections.Values)
        {
            connection?.Close();
        }

        _peerConnections.Clear();
        _connectedPeers.Clear();
        _currentSession = null;

        SetConnectionState(ConnectionState.Disconnected);
    }

    public async Task SendMessageAsync(NetworkMessage message)
    {
        message.SenderId = PeerId;
        message.SequenceNumber = Interlocked.Increment(ref _messageSequence);

        var json = JsonSerializer.Serialize(message, message.GetType());
        var bytes = Encoding.UTF8.GetBytes(json);
        var lengthPrefix = BitConverter.GetBytes(bytes.Length);

        if (IsHost)
        {
            // Host sends to all connected clients
            foreach (var connection in _peerConnections.Values)
            {
                try
                {
                    var stream = connection.GetStream();
                    await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                }
                catch
                {
                    // Handle disconnected peer
                }
            }
        }
        else if (_gameClient?.Connected == true)
        {
            // Client sends to host
            var stream = _gameClient.GetStream();
            await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            await stream.FlushAsync();
        }
    }

    public async Task SendMessageAsync(NetworkMessage message, string peerId)
    {
        message.SenderId = PeerId;
        message.SequenceNumber = Interlocked.Increment(ref _messageSequence);

        var json = JsonSerializer.Serialize(message, message.GetType());
        var bytes = Encoding.UTF8.GetBytes(json);
        var lengthPrefix = BitConverter.GetBytes(bytes.Length);

        if (_peerConnections.TryGetValue(peerId, out var connection) && connection.Connected)
        {
            var stream = connection.GetStream();
            await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
            await stream.FlushAsync();
        }
    }

    public List<PeerInfo> GetConnectedPeers()
    {
        return _connectedPeers.Values.ToList();
    }

    public Task<GameSession?> GetCurrentSessionAsync()
    {
        return Task.FromResult(_currentSession);
    }

    public Task StartAsync()
    {
        SetConnectionState(ConnectionState.Disconnected);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        await LeaveSessionAsync();
        _cancellationTokenSource?.Dispose();
    }

    private void SetConnectionState(ConnectionState newState)
    {
        if (_connectionState != newState)
        {
            _connectionState = newState;
            ConnectionStateChanged?.Invoke(this, newState);
        }
    }

    // Helper methods for socket operations

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        if (_gameServer == null) return;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await _gameServer.AcceptTcpClientAsync();
                var peerId = Guid.NewGuid().ToString();
                
                _peerConnections.TryAdd(peerId, client);

                var peerInfo = new PeerInfo
                {
                    PeerId = peerId,
                    PlayerName = "Unknown",
                    State = ConnectionState.Connected,
                    ConnectionEndpoint = client.Client.RemoteEndPoint?.ToString() ?? ""
                };

                _connectedPeers.TryAdd(peerId, peerInfo);
                PeerConnected?.Invoke(this, peerInfo);

                // Start receiving messages from this client
                _ = Task.Run(() => ReceiveMessagesAsync(client, cancellationToken));

                // Update session player count
                if (_currentSession != null)
                {
                    _currentSession.CurrentPlayers = _connectedPeers.Count + 1; // +1 for host
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task ReceiveMessagesAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var stream = client.GetStream();
        var lengthBuffer = new byte[4];

        while (!cancellationToken.IsCancellationRequested && client.Connected)
        {
            try
            {
                // Read message length prefix
                var bytesRead = await stream.ReadAsync(lengthBuffer, 0, 4, cancellationToken);
                if (bytesRead != 4) break;

                var messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                if (messageLength <= 0 || messageLength > 1024 * 1024) break; // Max 1MB

                // Read message content
                var messageBuffer = new byte[messageLength];
                var totalRead = 0;

                while (totalRead < messageLength)
                {
                    bytesRead = await stream.ReadAsync(
                        messageBuffer, 
                        totalRead, 
                        messageLength - totalRead, 
                        cancellationToken);
                    
                    if (bytesRead == 0) break;
                    totalRead += bytesRead;
                }

                if (totalRead == messageLength)
                {
                    var json = Encoding.UTF8.GetString(messageBuffer);
                    var message = JsonSerializer.Deserialize<NetworkMessage>(json);

                    if (message != null)
                    {
                        MessageReceived?.Invoke(this, message);

                        // If we're the host, broadcast to all other clients
                        if (IsHost)
                        {
                            foreach (var kvp in _peerConnections)
                            {
                                if (kvp.Value != client) // Don't send back to sender
                                {
                                    try
                                    {
                                        var peerStream = kvp.Value.GetStream();
                                        await peerStream.WriteAsync(lengthBuffer, 0, 4, cancellationToken);
                                        await peerStream.WriteAsync(messageBuffer, 0, messageLength, cancellationToken);
                                        await peerStream.FlushAsync(cancellationToken);
                                    }
                                    catch
                                    {
                                        // Peer disconnected
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                break;
            }
        }

        // Clean up disconnected peer
        var disconnectedPeerId = _peerConnections.FirstOrDefault(kvp => kvp.Value == client).Key;
        if (!string.IsNullOrEmpty(disconnectedPeerId))
        {
            _peerConnections.TryRemove(disconnectedPeerId, out _);
            
            if (_connectedPeers.TryRemove(disconnectedPeerId, out var peerInfo))
            {
                PeerDisconnected?.Invoke(this, peerInfo);
            }

            if (_currentSession != null)
            {
                _currentSession.CurrentPlayers = _connectedPeers.Count + 1;
            }
        }
    }

    private async Task RespondToDiscoveryAsync(CancellationToken cancellationToken)
    {
        try
        {
            _discoveryListener = new UdpClient(DISCOVERY_PORT);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _discoveryListener.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(result.Buffer);

                    if (message == DISCOVERY_MESSAGE && _currentSession != null)
                    {
                        // Respond with session info
                        var sessionJson = JsonSerializer.Serialize(_currentSession);
                        var response = DISCOVERY_RESPONSE + sessionJson;
                        var responseBytes = Encoding.UTF8.GetBytes(response);

                        await _discoveryListener.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Discovery response error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Discovery listener error: {ex.Message}");
        }
    }

    private string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch
        {
            // Fallback
        }

        return "127.0.0.1";
    }
}
