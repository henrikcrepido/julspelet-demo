using System.Text.Json;
using Julspelet.Shared.Models;
using Julspelet.Shared.Models.Networking;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Implementation of game state synchronization service.
/// Works with any INetworkService implementation to sync game state across peers.
/// Includes validation and anti-cheat protection.
/// </summary>
public class GameSyncService : IGameSyncService
{
    private readonly INetworkService _networkService;
    private readonly ScoringService _scoringService;
    private readonly IMessageValidator _validator;
    private long _messageSequence = 0;

    public event EventHandler<GameState>? GameStateUpdated;

    public GameSyncService(
        INetworkService networkService, 
        ScoringService scoringService,
        IMessageValidator validator)
    {
        _networkService = networkService;
        _scoringService = scoringService;
        _validator = validator;

        // Subscribe to network messages
        _networkService.MessageReceived += OnMessageReceived;
    }

    /// <summary>
    /// Handles incoming network messages and updates game state accordingly.
    /// </summary>
    private void OnMessageReceived(object? sender, NetworkMessage message)
    {
        // Messages will be processed by the game service which calls ApplyMessage
        // This is just for additional logging or validation if needed
    }

    public async Task SyncGameStateAsync(GameState gameState)
    {
        // Serialize and send the full game state
        var json = JsonSerializer.Serialize(gameState);
        var message = new GameStateUpdateMessage
        {
            SenderId = _networkService.PeerId,
            GameStateJson = json,
            SequenceNumber = Interlocked.Increment(ref _messageSequence)
        };

        await _networkService.SendMessageAsync(message);
    }

    public async Task<GameState?> RequestGameStateAsync()
    {
        if (!_networkService.IsConnected)
            return null;

        // In a real implementation, this would send a request message
        // and wait for the host to respond with the game state
        // For now, return null to indicate the caller should use local state
        return null;
    }

    public GameState? ApplyMessage(NetworkMessage message, GameState currentState)
    {
        // Validate message timing (prevent replay attacks)
        if (!_validator.ValidateMessageTiming(message))
        {
            Console.WriteLine($"Message rejected: Invalid timing from {message.SenderId}");
            return currentState;
        }

        // Check rate limiting
        var messageType = message.GetType().Name;
        if (!_validator.CheckRateLimit(message.SenderId, messageType))
        {
            Console.WriteLine($"Message rejected: Rate limit exceeded for {message.SenderId}");
            return currentState;
        }

        // Validate player action (if applicable)
        if (!_validator.ValidatePlayerAction(message, currentState))
        {
            Console.WriteLine($"Message rejected: Invalid player action from {message.SenderId}");
            return currentState;
        }

        try
        {
            return message switch
            {
                GameStateUpdateMessage update => ApplyGameStateUpdate(update, currentState),
                DiceRollMessage roll => ApplyDiceRollWithValidation(roll, currentState),
                ScoreSelectionMessage score => ApplyScoreSelectionWithValidation(score, currentState),
                TurnChangeMessage turn => ApplyTurnChange(turn, currentState),
                _ => currentState // Unknown message type, return current state
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying message: {ex.Message}");
            // If there's an error applying the message, return the current state
            return currentState;
        }
    }

    private GameState ApplyGameStateUpdate(GameStateUpdateMessage message, GameState currentState)
    {
        try
        {
            var newState = JsonSerializer.Deserialize<GameState>(message.GameStateJson);
            if (newState != null)
            {
                GameStateUpdated?.Invoke(this, newState);
                return newState;
            }
        }
        catch (JsonException)
        {
            // Failed to deserialize, keep current state
        }

        return currentState;
    }

    private GameState ApplyDiceRollWithValidation(DiceRollMessage message, GameState currentState)
    {
        // Validate the dice roll for anti-cheat
        if (!_validator.ValidateDiceRoll(message, currentState))
        {
            Console.WriteLine($"Invalid dice roll rejected from player {message.PlayerId}");
            return currentState;
        }

        // Update the dice values for the current player
        var currentPlayer = currentState.Players.ElementAtOrDefault(currentState.CurrentPlayerIndex);
        if (currentPlayer?.Id.ToString() == message.PlayerId)
        {
            for (int i = 0; i < message.DiceValues.Count && i < currentState.DiceSet.Dice.Count; i++)
            {
                currentState.DiceSet.Dice[i].Value = message.DiceValues[i];
                if (i < message.HeldDice.Count)
                {
                    currentState.DiceSet.Dice[i].IsHeld = message.HeldDice[i];
                }
            }
            currentState.RollsThisTurn = message.RollNumber;

            GameStateUpdated?.Invoke(this, currentState);
        }

        return currentState;
    }

    private GameState ApplyDiceRoll(DiceRollMessage message, GameState currentState)
    {
        // Legacy method for compatibility - calls validated version
        return ApplyDiceRollWithValidation(message, currentState);
    }

    private GameState ApplyScoreSelectionWithValidation(ScoreSelectionMessage message, GameState currentState)
    {
        // Validate the score selection for anti-cheat
        if (!_validator.ValidateScoreSelection(message, currentState))
        {
            Console.WriteLine($"Invalid score selection rejected from player {message.PlayerId}");
            return currentState;
        }

        // Find the player and update their score
        var player = currentState.Players.FirstOrDefault(p => p.Id.ToString() == message.PlayerId);
        if (player != null)
        {
            // Apply the score to the appropriate category
            // The category name should match the ScoreCategory enum
            if (Enum.TryParse<ScoreCategory>(message.Category, out var category))
            {
                player.ScoreCard.Scores[category] = message.Score;
            }

            GameStateUpdated?.Invoke(this, currentState);
        }

        return currentState;
    }

    private GameState ApplyScoreSelection(ScoreSelectionMessage message, GameState currentState)
    {
        // Legacy method for compatibility - calls validated version
        return ApplyScoreSelectionWithValidation(message, currentState);
    }

    private GameState ApplyTurnChange(TurnChangeMessage message, GameState currentState)
    {
        // Update current player index
        var nextPlayerIndex = currentState.Players.FindIndex(p => p.Id.ToString() == message.CurrentPlayerId);
        if (nextPlayerIndex >= 0)
        {
            currentState.CurrentPlayerIndex = nextPlayerIndex;
            currentState.RollsThisTurn = 0;

            // Reset dice
            foreach (var die in currentState.DiceSet.Dice)
            {
                die.IsHeld = false;
                die.Value = 1;
            }

            GameStateUpdated?.Invoke(this, currentState);
        }

        return currentState;
    }

    public bool ValidateAction(NetworkMessage message, GameState currentState)
    {
        // Only the host validates actions to prevent cheating
        if (!_networkService.IsHost)
            return true;

        return message switch
        {
            DiceRollMessage roll => ValidateDiceRoll(roll, currentState),
            ScoreSelectionMessage score => ValidateScoreSelection(score, currentState),
            _ => true // Other message types don't need validation
        };
    }

    private bool ValidateDiceRoll(DiceRollMessage message, GameState currentState)
    {
        // Check if it's the correct player's turn
        var currentPlayer = currentState.Players.ElementAtOrDefault(currentState.CurrentPlayerIndex);
        if (currentPlayer?.Id.ToString() != message.PlayerId)
            return false;

        // Check if player has rolls remaining
        if (currentState.RollsThisTurn >= GameState.MaxRollsPerTurn)
            return false;

        // Check if roll number is valid
        if (message.RollNumber < 1 || message.RollNumber > 3)
            return false;

        return true;
    }

    private bool ValidateScoreSelection(ScoreSelectionMessage message, GameState currentState)
    {
        // Check if it's the correct player's turn
        var currentPlayer = currentState.Players.ElementAtOrDefault(currentState.CurrentPlayerIndex);
        if (currentPlayer?.Id.ToString() != message.PlayerId)
            return false;

        // Check if the category hasn't been used yet
        var player = currentState.Players.FirstOrDefault(p => p.Id.ToString() == message.PlayerId);
        if (player == null)
            return false;

        if (Enum.TryParse<ScoreCategory>(message.Category, out var category))
        {
            if (player.ScoreCard.Scores.ContainsKey(category) && player.ScoreCard.Scores[category] != null)
                return false; // Category already used

            // Validate the score matches what the dice would give
            var diceValues = currentState.DiceSet.Dice.Select(d => d.Value).ToArray();
            var calculatedScore = _scoringService.CalculateScore(category, diceValues);
            return calculatedScore == message.Score;
        }

        return false;
    }

    public GameState ResolveConflict(GameState localState, GameState networkState)
    {
        // Host state is always authoritative
        if (_networkService.IsHost)
            return localState;

        // Clients always accept the host's state
        return networkState;
    }
}
