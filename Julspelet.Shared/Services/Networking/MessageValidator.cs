using Julspelet.Shared.Models;
using Julspelet.Shared.Models.Networking;
using System.Collections.Concurrent;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Implements validation logic for game messages to prevent cheating.
/// Uses server-authoritative validation and rate limiting.
/// </summary>
public class MessageValidator : IMessageValidator
{
    private readonly ScoringService _scoringService;
    private readonly ConcurrentDictionary<string, List<DateTime>> _messageTimestamps = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastMessageTime = new();
    
    private const int MAX_MESSAGES_PER_SECOND = 10;
    private const int MESSAGE_HISTORY_SECONDS = 60;
    private const int MAX_MESSAGE_AGE_SECONDS = 30;
    private const int MIN_DICE_VALUE = 1;
    private const int MAX_DICE_VALUE = 6;
    private const int DICE_COUNT = 5;

    public MessageValidator(ScoringService scoringService)
    {
        _scoringService = scoringService;
    }

    public bool ValidateDiceRoll(DiceRollMessage message, GameState gameState)
    {
        // Validate basic message properties
        if (message == null || gameState == null)
            return false;

        // Validate it's the player's turn
        var currentPlayer = gameState.GetCurrentPlayer();
        if (currentPlayer == null || currentPlayer.Id.ToString() != message.PlayerId)
            return false;

        // Validate roll number (1, 2, or 3)
        if (message.RollNumber < 1 || message.RollNumber > 3)
            return false;

        // Validate dice values are within valid range
        if (message.DiceValues.Count != DICE_COUNT)
            return false;

        foreach (var value in message.DiceValues)
        {
            if (value < MIN_DICE_VALUE || value > MAX_DICE_VALUE)
                return false;
        }

        // Validate held dice list matches dice count
        if (message.HeldDice.Count != DICE_COUNT)
            return false;

        // If this is not the first roll, validate that held dice values match previous roll
        if (message.RollNumber > 1 && gameState.DiceSet != null)
        {
            for (int i = 0; i < DICE_COUNT; i++)
            {
                if (message.HeldDice[i] && 
                    gameState.DiceSet.Dice[i].Value != message.DiceValues[i])
                {
                    // Held dice should not change value
                    return false;
                }
            }
        }

        return true;
    }

    public bool ValidateScoreSelection(ScoreSelectionMessage message, GameState gameState)
    {
        // Validate basic message properties
        if (message == null || gameState == null)
            return false;

        // Validate it's the player's turn
        var currentPlayer = gameState.GetCurrentPlayer();
        if (currentPlayer == null || currentPlayer.Id.ToString() != message.PlayerId)
            return false;

        // Validate the category is valid
        if (!Enum.TryParse<ScoreCategory>(message.Category, out var category))
            return false;

        // Validate the category hasn't been used yet
        if (currentPlayer.ScoreCard.Scores.ContainsKey(category))
            return false;

        // Calculate expected score from current dice
        if (gameState.DiceSet == null)
            return false;

        var diceValues = gameState.DiceSet.Dice.Select(d => d.Value).ToArray();
        var expectedScore = _scoringService.CalculateScore(category, diceValues);

        // Validate the claimed score matches the calculated score
        if (message.Score != expectedScore)
            return false;

        return true;
    }

    public bool ValidatePlayerAction(NetworkMessage message, GameState gameState)
    {
        if (message == null || gameState == null)
            return false;

        // Validate game is in progress
        if (!gameState.IsGameStarted || gameState.IsGameComplete)
            return false;

        // For player-specific actions, validate the sender is in the game
        if (message is DiceRollMessage || message is ScoreSelectionMessage)
        {
            var playerId = message switch
            {
                DiceRollMessage roll => roll.PlayerId,
                ScoreSelectionMessage score => score.PlayerId,
                _ => null
            };

            if (playerId == null)
                return false;

            var player = gameState.Players.FirstOrDefault(p => p.Id.ToString() == playerId);
            if (player == null)
                return false;
        }

        return true;
    }

    public bool CheckRateLimit(string senderId, string messageType)
    {
        if (string.IsNullOrEmpty(senderId))
            return false;

        var now = DateTime.UtcNow;
        var key = $"{senderId}:{messageType}";

        // Get or create timestamp list for this sender
        var timestamps = _messageTimestamps.GetOrAdd(key, _ => new List<DateTime>());

        lock (timestamps)
        {
            // Remove old timestamps
            timestamps.RemoveAll(t => (now - t).TotalSeconds > MESSAGE_HISTORY_SECONDS);

            // Check if rate limit exceeded
            if (timestamps.Count >= MAX_MESSAGES_PER_SECOND)
            {
                return false; // Rate limit exceeded
            }

            // Add new timestamp
            timestamps.Add(now);
        }

        // Also check minimum time between messages of same type (anti-spam)
        if (_lastMessageTime.TryGetValue(key, out var lastTime))
        {
            var timeSinceLastMessage = (now - lastTime).TotalMilliseconds;
            
            // Require at least 100ms between messages of same type
            if (timeSinceLastMessage < 100)
                return false;
        }

        _lastMessageTime[key] = now;

        return true;
    }

    public bool ValidateMessageTiming(NetworkMessage message)
    {
        if (message == null)
            return false;

        var now = DateTime.UtcNow;
        var messageAge = (now - message.Timestamp).TotalSeconds;

        // Reject messages that are too old (possible replay attack)
        if (messageAge > MAX_MESSAGE_AGE_SECONDS)
            return false;

        // Reject messages from the future (clock sync issues or manipulation)
        if (messageAge < -5) // Allow 5 seconds clock skew
            return false;

        return true;
    }

    /// <summary>
    /// Cleans up old rate limit data to prevent memory leaks.
    /// Should be called periodically (e.g., every minute).
    /// </summary>
    public void CleanupOldData()
    {
        var now = DateTime.UtcNow;
        var cutoffTime = now.AddSeconds(-MESSAGE_HISTORY_SECONDS);

        // Clean up timestamp lists
        foreach (var kvp in _messageTimestamps)
        {
            var timestamps = kvp.Value;
            lock (timestamps)
            {
                timestamps.RemoveAll(t => t < cutoffTime);
                
                // Remove empty lists
                if (timestamps.Count == 0)
                {
                    _messageTimestamps.TryRemove(kvp.Key, out _);
                }
            }
        }

        // Clean up last message times
        var oldKeys = _lastMessageTime
            .Where(kvp => (now - kvp.Value).TotalSeconds > MESSAGE_HISTORY_SECONDS)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in oldKeys)
        {
            _lastMessageTime.TryRemove(key, out _);
        }
    }
}
