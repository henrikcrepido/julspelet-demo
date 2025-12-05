namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a player in the Yatzy game.
/// Each player has a unique ID, name, and scorecard to track their scores.
/// </summary>
public class Player
{
    /// <summary>
    /// Unique identifier for the player.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The player's display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The player's scorecard containing all their scores for different categories.
    /// </summary>
    public ScoreCard ScoreCard { get; set; } = new();

    /// <summary>
    /// Indicates whether it's currently this player's turn.
    /// </summary>
    public bool IsCurrentTurn { get; set; }

    /// <summary>
    /// Gets the total score for the player including bonuses.
    /// </summary>
    public int TotalScore => ScoreCard.GetTotalScore();
}
