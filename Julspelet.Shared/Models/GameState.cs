namespace Julspelet.Shared.Models;

/// <summary>
/// Game mode enumeration.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Single player mode - play alone to achieve the highest score.
    /// </summary>
    SinglePlayer,
    
    /// <summary>
    /// Multiplayer mode - compete against other players.
    /// </summary>
    Multiplayer
}

/// <summary>
/// Represents the current state of the game.
/// Manages players, current turn, dice rolls, and game progress.
/// </summary>
public class GameState
{
    /// <summary>
    /// List of all players in the game.
    /// </summary>
    public List<Player> Players { get; set; } = new();

    /// <summary>
    /// The set of dice being used in the game.
    /// </summary>
    public DiceSet DiceSet { get; set; } = new();

    /// <summary>
    /// Index of the current player (0-based).
    /// </summary>
    public int CurrentPlayerIndex { get; set; } = 0;

    /// <summary>
    /// Number of rolls taken in the current turn (0-3).
    /// Players get 3 rolls per turn.
    /// </summary>
    public int RollsThisTurn { get; set; } = 0;

    /// <summary>
    /// Maximum number of rolls allowed per turn.
    /// </summary>
    public const int MaxRollsPerTurn = 3;

    /// <summary>
    /// Indicates whether the game has started.
    /// </summary>
    public bool IsGameStarted { get; set; }

    /// <summary>
    /// Indicates whether the game has ended.
    /// </summary>
    public bool IsGameComplete { get; set; }

    /// <summary>
    /// The game mode (single player or multiplayer).
    /// </summary>
    public GameMode Mode { get; set; } = GameMode.Multiplayer;

    /// <summary>
    /// Gets the current player whose turn it is.
    /// </summary>
    public Player? GetCurrentPlayer()
    {
        if (Players.Count == 0 || CurrentPlayerIndex >= Players.Count)
            return null;

        return Players[CurrentPlayerIndex];
    }

    /// <summary>
    /// Adds a new player to the game.
    /// Can only add players before the game starts.
    /// </summary>
    public bool AddPlayer(string name)
    {
        if (IsGameStarted || string.IsNullOrWhiteSpace(name))
            return false;

        if (Players.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return false; // Player name must be unique

        Players.Add(new Player { Name = name.Trim() });
        return true;
    }

    /// <summary>
    /// Starts the game with the specified game mode.
    /// </summary>
    /// <param name="gameMode">The game mode to start (SinglePlayer or Multiplayer).</param>
    public bool StartGame(GameMode gameMode = GameMode.Multiplayer)
    {
        Mode = gameMode;
        
        // Single player requires 1 player, multiplayer requires 2+
        var minPlayers = Mode == GameMode.SinglePlayer ? 1 : 2;
        if (Players.Count < minPlayers)
            return false;

        IsGameStarted = true;
        CurrentPlayerIndex = 0;
        Players[0].IsCurrentTurn = true;
        RollsThisTurn = 0;

        return true;
    }

    /// <summary>
    /// Moves to the next player's turn.
    /// </summary>
    public void NextPlayer()
    {
        Players[CurrentPlayerIndex].IsCurrentTurn = false;
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        Players[CurrentPlayerIndex].IsCurrentTurn = true;
        RollsThisTurn = 0;
        DiceSet.ResetAndRoll();

        // Check if game is complete
        CheckGameComplete();
    }

    /// <summary>
    /// Checks if all players have completed their scorecards.
    /// </summary>
    private void CheckGameComplete()
    {
        IsGameComplete = Players.All(p => p.ScoreCard.IsComplete());
    }

    /// <summary>
    /// Gets the winner(s) of the game.
    /// </summary>
    public List<Player> GetWinners()
    {
        if (!IsGameComplete || Players.Count == 0)
            return new List<Player>();

        var maxScore = Players.Max(p => p.TotalScore);
        return Players.Where(p => p.TotalScore == maxScore).ToList();
    }

    /// <summary>
    /// Checks if the current player can roll the dice.
    /// </summary>
    public bool CanRoll()
    {
        return IsGameStarted && !IsGameComplete && RollsThisTurn < MaxRollsPerTurn;
    }

    /// <summary>
    /// Performs a dice roll for the current turn.
    /// </summary>
    public void Roll()
    {
        if (CanRoll())
        {
            if (RollsThisTurn == 0)
            {
                DiceSet.ResetAndRoll();
            }
            else
            {
                DiceSet.RollAll();
            }
            RollsThisTurn++;
        }
    }
}
