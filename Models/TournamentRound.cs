namespace Julspelet.Models;

/// <summary>
/// Represents the different rounds in a tournament.
/// </summary>
public enum TournamentRound
{
    /// <summary>
    /// Quarter-finals (8 players, 4 matches)
    /// </summary>
    QuarterFinals,
    
    /// <summary>
    /// Semi-finals (4 players, 2 matches)
    /// </summary>
    SemiFinals,
    
    /// <summary>
    /// Final round (2 players, 1 match)
    /// </summary>
    Final,
    
    /// <summary>
    /// Tournament completed
    /// </summary>
    Completed
}
