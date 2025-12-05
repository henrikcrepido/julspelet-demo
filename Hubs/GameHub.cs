using Microsoft.AspNetCore.SignalR;
using Julspelet.Shared.Models.Networking;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Julspelet.Hubs;

/// <summary>
/// SignalR hub for managing multiplayer game sessions.
/// Handles session creation, joining, leaving, and message broadcasting.
/// </summary>
public class GameHub : Hub
{
    // In-memory storage for game sessions (for demo purposes)
    // In production, consider using distributed cache (Redis) for multi-server scenarios
    private static readonly ConcurrentDictionary<string, GameSession> _sessions = new();
    
    // Track which connection is in which session
    private static readonly ConcurrentDictionary<string, string> _connectionToSession = new();

    /// <summary>
    /// Creates a new game session and adds the host to it.
    /// </summary>
    /// <param name="session">The game session to create</param>
    /// <returns>The created session with updated information</returns>
    public async Task<GameSession> CreateSession(GameSession session)
    {
        // Ensure session has a unique ID
        if (string.IsNullOrEmpty(session.SessionId))
        {
            session.SessionId = GenerateSessionCode();
        }

        // Set the host peer ID to the current connection
        session.LocalPeerId = Context.ConnectionId;
        session.IsHost = true;
        session.CreatedAt = DateTime.UtcNow;

        // Store the session
        if (!_sessions.TryAdd(session.SessionId, session))
        {
            throw new HubException("Failed to create session. Session ID already exists.");
        }

        // Add host to SignalR group
        await Groups.AddToGroupAsync(Context.ConnectionId, session.SessionId);
        
        // Track connection
        _connectionToSession.TryAdd(Context.ConnectionId, session.SessionId);

        return session;
    }

    /// <summary>
    /// Joins an existing game session.
    /// </summary>
    /// <param name="sessionId">The session ID to join</param>
    /// <param name="playerName">The name of the player joining</param>
    /// <returns>The session that was joined</returns>
    public async Task<GameSession> JoinSession(string sessionId, string playerName)
    {
        if (!_sessions.TryGetValue(sessionId, out var session))
        {
            throw new HubException("Session not found.");
        }

        // Check if session is full
        if (session.CurrentPlayers >= session.MaxPlayers)
        {
            throw new HubException("Session is full.");
        }

        // Add player to SignalR group
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        
        // Track connection
        _connectionToSession.TryAdd(Context.ConnectionId, sessionId);

        // Increment player count
        session.CurrentPlayers++;

        // Notify others in the session that a new player joined
        var joinMessage = new PlayerJoinedMessage
        {
            SenderId = Context.ConnectionId,
            PlayerId = Context.ConnectionId,
            PlayerName = playerName,
            Timestamp = DateTime.UtcNow
        };

        await Clients.Group(sessionId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(joinMessage));

        return session;
    }

    /// <summary>
    /// Leaves the current game session.
    /// </summary>
    /// <param name="sessionId">The session ID to leave</param>
    public async Task LeaveSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            // Decrement player count
            session.CurrentPlayers = Math.Max(0, session.CurrentPlayers - 1);

            // Remove from group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            
            // Remove connection tracking
            _connectionToSession.TryRemove(Context.ConnectionId, out _);

            // Notify others that player left
            var leaveMessage = new PlayerLeftMessage
            {
                SenderId = Context.ConnectionId,
                PlayerId = Context.ConnectionId,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(sessionId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(leaveMessage));

            // If session is empty and not the host, remove it
            if (session.CurrentPlayers == 0)
            {
                _sessions.TryRemove(sessionId, out _);
            }
        }
    }

    /// <summary>
    /// Sends a message to all players in a session.
    /// </summary>
    /// <param name="sessionId">The session ID</param>
    /// <param name="messageJson">The serialized message to send</param>
    public async Task SendMessage(string sessionId, string messageJson)
    {
        if (!_sessions.ContainsKey(sessionId))
        {
            throw new HubException("Session not found.");
        }

        // Broadcast message to all players in the session
        await Clients.Group(sessionId).SendAsync("ReceiveMessage", messageJson);
    }

    /// <summary>
    /// Sends a message to a specific peer in a session.
    /// </summary>
    /// <param name="sessionId">The session ID</param>
    /// <param name="peerId">The peer ID (connection ID) to send to</param>
    /// <param name="messageJson">The serialized message to send</param>
    public async Task SendMessageToPeer(string sessionId, string peerId, string messageJson)
    {
        if (!_sessions.ContainsKey(sessionId))
        {
            throw new HubException("Session not found.");
        }

        // Send message to specific peer
        await Clients.Client(peerId).SendAsync("ReceiveMessage", messageJson);
    }

    /// <summary>
    /// Gets all available sessions that can be joined.
    /// </summary>
    /// <returns>List of available game sessions</returns>
    public Task<List<GameSession>> GetAvailableSessions()
    {
        var availableSessions = _sessions.Values
            .Where(s => s.CurrentPlayers < s.MaxPlayers)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        return Task.FromResult(availableSessions);
    }

    /// <summary>
    /// Gets a specific session by ID.
    /// </summary>
    /// <param name="sessionId">The session ID to retrieve</param>
    /// <returns>The game session, or null if not found</returns>
    public Task<GameSession?> GetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    /// <summary>
    /// Called when a client disconnects. Automatically removes them from their session.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectionToSession.TryGetValue(Context.ConnectionId, out var sessionId))
        {
            await LeaveSession(sessionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Generates a random 6-character session code.
    /// </summary>
    /// <returns>A random session code</returns>
    private static string GenerateSessionCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude similar looking characters
        var random = new Random();
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }
}
