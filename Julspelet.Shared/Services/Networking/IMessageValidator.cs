using Julspelet.Shared.Models;
using Julspelet.Shared.Models.Networking;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Service for validating game messages and actions to prevent cheating.
/// Implements server-authoritative validation for all game state changes.
/// </summary>
public interface IMessageValidator
{
    /// <summary>
    /// Validates a dice roll message to ensure it's legitimate.
    /// </summary>
    bool ValidateDiceRoll(DiceRollMessage message, GameState gameState);

    /// <summary>
    /// Validates a score selection to ensure it matches the dice values.
    /// </summary>
    bool ValidateScoreSelection(ScoreSelectionMessage message, GameState gameState);

    /// <summary>
    /// Validates that a player can perform an action (turn order, game state).
    /// </summary>
    bool ValidatePlayerAction(NetworkMessage message, GameState gameState);

    /// <summary>
    /// Checks if a message rate limit has been exceeded.
    /// </summary>
    bool CheckRateLimit(string senderId, string messageType);

    /// <summary>
    /// Validates message timing to prevent replay attacks.
    /// </summary>
    bool ValidateMessageTiming(NetworkMessage message);
}
