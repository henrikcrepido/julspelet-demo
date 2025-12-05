namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a single match within a tournament round.
/// In Yatzy, this is one complete game between players.
/// </summary>
public class Match
{
    /// <summary>
    /// Unique identifier for the match.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// The tournament round this match belongs to.
    /// </summary>
    public TournamentRound Round { get; set; }
    
    /// <summary>
    /// Match number within the round (e.g., Match 1, Match 2).
    /// </summary>
    public int MatchNumber { get; set; }
    
    /// <summary>
    /// Players participating in this match.
    /// </summary>
    public List<Player> Players { get; set; } = new();
    
    /// <summary>
    /// The game state for this match.
    /// </summary>
    public GameState? GameState { get; set; }
    
    /// <summary>
    /// Winner of the match (determined after game completion).
    /// </summary>
    public Player? Winner { get; set; }
    
    /// <summary>
    /// Whether this match has been completed.
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Timestamp when the match started.
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// Timestamp when the match was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}
