namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a game master who creates and manages tournaments.
/// </summary>
public class GameMaster
{
    /// <summary>
    /// Unique identifier for the game master.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Display name of the game master.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Connection ID for SignalR communication.
    /// </summary>
    public string? ConnectionId { get; set; }
    
    /// <summary>
    /// Timestamp when the game master was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
