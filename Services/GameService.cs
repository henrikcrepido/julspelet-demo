using Julspelet.Models;

namespace Julspelet.Services;

/// <summary>
/// Service for managing the game state and game flow.
/// Handles player turns, dice rolling, scoring, and game progression.
/// </summary>
public class GameService
{
    private readonly ScoringService _scoringService;

    /// <summary>
    /// The current game state.
    /// </summary>
    public GameState GameState { get; private set; }

    /// <summary>
    /// Event raised when the game state changes.
    /// Used to notify UI components to re-render.
    /// </summary>
    public event Action? OnGameStateChanged;

    public GameService(ScoringService scoringService)
    {
        _scoringService = scoringService;
        GameState = new GameState();
    }

    /// <summary>
    /// Creates a new game, resetting all state.
    /// </summary>
    public void NewGame()
    {
        GameState = new GameState();
        NotifyStateChanged();
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    public bool AddPlayer(string name)
    {
        var result = GameState.AddPlayer(name);
        if (result)
        {
            NotifyStateChanged();
        }
        return result;
    }

    /// <summary>
    /// Removes a player from the game (only before game starts).
    /// </summary>
    public bool RemovePlayer(Guid playerId)
    {
        if (GameState.IsGameStarted)
            return false;

        var player = GameState.Players.FirstOrDefault(p => p.Id == playerId);
        if (player != null)
        {
            GameState.Players.Remove(player);
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public bool StartGame()
    {
        var result = GameState.StartGame();
        if (result)
        {
            NotifyStateChanged();
        }
        return result;
    }

    /// <summary>
    /// Rolls the dice for the current turn.
    /// </summary>
    public void RollDice()
    {
        GameState.Roll();
        NotifyStateChanged();
    }

    /// <summary>
    /// Toggles the hold state of a die at the specified index.
    /// </summary>
    public void ToggleDieHold(int index)
    {
        if (GameState.RollsThisTurn > 0) // Can only hold dice after first roll
        {
            GameState.DiceSet.ToggleHold(index);
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Scores the current dice for a specific category and moves to next player.
    /// </summary>
    public bool ScoreCategory(ScoreCategory category)
    {
        var currentPlayer = GameState.GetCurrentPlayer();
        if (currentPlayer == null || GameState.RollsThisTurn == 0)
            return false;

        // Check if category is already scored
        if (currentPlayer.ScoreCard.IsCategoryScored(category))
            return false;

        // Calculate and set the score
        var diceValues = GameState.DiceSet.GetValues();
        var score = _scoringService.CalculateScore(category, diceValues);
        currentPlayer.ScoreCard.SetScore(category, score);

        // Move to next player
        GameState.NextPlayer();
        NotifyStateChanged();

        return true;
    }

    /// <summary>
    /// Gets the potential score for a category with current dice (without committing).
    /// </summary>
    public int GetPotentialScore(ScoreCategory category)
    {
        if (GameState.RollsThisTurn == 0)
            return 0;

        var diceValues = GameState.DiceSet.GetValues();
        return _scoringService.CalculateScore(category, diceValues);
    }

    /// <summary>
    /// Gets available categories for the current player.
    /// </summary>
    public List<ScoreCategory> GetAvailableCategories()
    {
        var currentPlayer = GameState.GetCurrentPlayer();
        if (currentPlayer == null)
            return new List<ScoreCategory>();

        return _scoringService.GetAvailableCategories(currentPlayer.ScoreCard);
    }

    /// <summary>
    /// Notifies subscribers that the game state has changed.
    /// This triggers UI updates in Blazor components.
    /// </summary>
    private void NotifyStateChanged()
    {
        OnGameStateChanged?.Invoke();
    }
}
