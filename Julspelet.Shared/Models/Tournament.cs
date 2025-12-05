namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a Yatzy tournament with multiple rounds.
/// Supports quarter-finals, semi-finals, and final.
/// </summary>
public class Tournament
{
    /// <summary>
    /// Unique identifier for the tournament.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Display name of the tournament.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The game master who created and manages this tournament.
    /// </summary>
    public GameMaster GameMaster { get; set; } = new();
    
    /// <summary>
    /// Players registered for the tournament.
    /// </summary>
    public List<Player> RegisteredPlayers { get; set; } = new();
    
    /// <summary>
    /// Current round of the tournament.
    /// </summary>
    public TournamentRound CurrentRound { get; set; } = TournamentRound.QuarterFinals;
    
    /// <summary>
    /// All matches in the tournament (across all rounds).
    /// </summary>
    public List<Match> Matches { get; set; } = new();
    
    /// <summary>
    /// Whether the tournament has started.
    /// </summary>
    public bool HasStarted { get; set; }
    
    /// <summary>
    /// Whether the tournament is completed.
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Maximum number of players allowed (typically 4, 8, or 16).
    /// </summary>
    public int MaxPlayers { get; set; } = 8;
    
    /// <summary>
    /// Minimum number of players required to start.
    /// </summary>
    public int MinPlayers { get; set; } = 4;
    
    /// <summary>
    /// Tournament winner (set when tournament is completed).
    /// </summary>
    public Player? Winner { get; set; }
    
    /// <summary>
    /// Timestamp when the tournament was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Timestamp when the tournament started.
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// Timestamp when the tournament was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Gets whether the tournament is accepting new players.
    /// </summary>
    public bool IsAcceptingPlayers => !HasStarted && RegisteredPlayers.Count < MaxPlayers;
    
    /// <summary>
    /// Gets whether the tournament has enough players to start.
    /// </summary>
    public bool CanStart => !HasStarted && RegisteredPlayers.Count >= MinPlayers && 
                           RegisteredPlayers.Count <= MaxPlayers;
}
