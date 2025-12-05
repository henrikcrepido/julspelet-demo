using NUnit.Framework;
using Julspelet.Shared.Models;
using Julspelet.Shared.Services;

namespace Julspelet.Tests;

/// <summary>
/// Integration tests for GameService.
/// Tests the complete game flow with multiple players.
/// </summary>
[TestFixture]
public class GameServiceIntegrationTests
{
    private GameService _gameService = null!;
    private ScoringService _scoringService = null!;

    [SetUp]
    public void Setup()
    {
        _scoringService = new ScoringService();
        _gameService = new GameService(_scoringService);
    }

    [Test]
    public void NewGame_InitializesCorrectly()
    {
        // Act
        _gameService.NewGame();
        var gameState = _gameService.GameState;

        // Assert
        Assert.That(gameState, Is.Not.Null, "Game state should be initialized");
        Assert.That(gameState.Players.Count, Is.EqualTo(0), "Should start with no players");
        Assert.That(gameState.IsGameStarted, Is.False, "Game should not be started");
    }

    [Test]
    public void AddPlayer_MultiplePlayers_AddsSuccessfully()
    {
        // Arrange
        _gameService.NewGame();

        // Act
        var result1 = _gameService.AddPlayer("Alice");
        var result2 = _gameService.AddPlayer("Bob");
        var result3 = _gameService.AddPlayer("Charlie");
        var gameState = _gameService.GameState;

        // Assert
        Assert.That(result1, Is.True, "First player should be added");
        Assert.That(result2, Is.True, "Second player should be added");
        Assert.That(result3, Is.True, "Third player should be added");
        Assert.That(gameState.Players.Count, Is.EqualTo(3), "Should have 3 players");
    }

    [Test]
    public void StartGame_WithPlayers_StartsSuccessfully()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2");

        // Act
        var result = _gameService.StartGame(GameMode.Multiplayer);
        var gameState = _gameService.GameState;

        // Assert
        Assert.That(result, Is.True, "Game should start successfully");
        Assert.That(gameState.IsGameStarted, Is.True, "Game should be marked as started");
        Assert.That(gameState.CurrentPlayerIndex, Is.EqualTo(0), "Should start with player 0");
    }

    [Test]
    public void RollDice_ThreeTimes_IncreasesRollsCorrectly()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2"); // Need at least 2 players for multiplayer
        var started = _gameService.StartGame();
        
        Assert.That(started, Is.True, "Game should start successfully");
        Assert.That(_gameService.GameState.IsGameStarted, Is.True, "Game should be marked as started");

        // Act & Assert - RollsThisTurn starts at 0 before any rolls
        Assert.That(_gameService.GameState.RollsThisTurn, Is.EqualTo(0), "Should start with 0 rolls");
        Assert.That(_gameService.GameState.CanRoll(), Is.True, "Should be able to roll");
        
        _gameService.RollDice();
        Assert.That(_gameService.GameState.RollsThisTurn, Is.EqualTo(1), "Should have 1 roll after first roll");
        
        _gameService.RollDice();
        Assert.That(_gameService.GameState.RollsThisTurn, Is.EqualTo(2), "Should have 2 rolls after second roll");
        
        _gameService.RollDice();
        Assert.That(_gameService.GameState.RollsThisTurn, Is.EqualTo(3), "Should have 3 rolls after third roll");
        
        Assert.That(_gameService.GameState.CanRoll(), Is.False, "Should not be able to roll after 3 rolls");
    }

    [Test]
    public void HoldDice_BetweenRolls_PreservesDiceValues()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.StartGame();
        _gameService.RollDice();
        
        // Act - Hold first two dice
        var firstRollDice = _gameService.GameState.DiceSet.GetValues();
        _gameService.ToggleDieHold(0);
        _gameService.ToggleDieHold(1);
        
        _gameService.RollDice();
        var secondRollDice = _gameService.GameState.DiceSet.GetValues();

        // Assert
        Assert.That(secondRollDice[0], Is.EqualTo(firstRollDice[0]), "First die should be preserved");
        Assert.That(secondRollDice[1], Is.EqualTo(firstRollDice[1]), "Second die should be preserved");
    }

    [Test]
    public void ScoreCategory_AfterRolling_AdvancesToNextPlayer()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2");
        _gameService.StartGame();
        
        // Act
        _gameService.RollDice();
        var result = _gameService.ScoreCategory(ScoreCategory.Ones);

        var gameState = _gameService.GameState;

        // Assert
        Assert.That(result, Is.True, "Should successfully score");
        Assert.That(gameState.CurrentPlayerIndex, Is.EqualTo(1), "Should advance to second player");
        Assert.That(gameState.RollsThisTurn, Is.EqualTo(0), "Should reset rolls for next player");
    }

    [Test]
    public void ScoreCategory_BeforeRolling_ReturnsFalse()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.StartGame();

        // Act
        var result = _gameService.ScoreCategory(ScoreCategory.Ones);

        // Assert
        Assert.That(result, Is.False, "Should not allow selecting score without rolling dice");
    }

    [Test]
    public void ScoreCategory_AlreadyUsedCategory_ReturnsFalse()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.StartGame();
        _gameService.RollDice();
        _gameService.ScoreCategory(ScoreCategory.Ones);

        // Act - Try to use Ones again
        _gameService.RollDice();
        var result = _gameService.ScoreCategory(ScoreCategory.Ones);

        // Assert
        Assert.That(result, Is.False, "Should not allow selecting already used category");
    }

    [Test]
    public void GetPotentialScore_AfterRolling_ReturnsCorrectValue()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.StartGame();
        _gameService.RollDice();
        
        // Act
        var score = _gameService.GetPotentialScore(ScoreCategory.Ones);

        // Assert
        Assert.That(score, Is.GreaterThanOrEqualTo(0), "Should return valid potential score");
    }

    [Test]
    public void GetAvailableCategories_ReturnsUnscoredCategories()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2"); // Add second player so we stay in game after scoring
        _gameService.StartGame();
        _gameService.RollDice();
        _gameService.ScoreCategory(ScoreCategory.Ones);

        // Now it's player 2's turn - they should have all categories available
        var player2Available = _gameService.GetAvailableCategories();
        Assert.That(player2Available.Count, Is.EqualTo(15), "Player 2 should have all 15 categories available");

        // Roll and score for player 2, then check player 1's available categories
        _gameService.RollDice();
        _gameService.ScoreCategory(ScoreCategory.Twos);
        
        // Now it's back to player 1's turn
        var player1Available = _gameService.GetAvailableCategories();

        // Assert
        Assert.That(player1Available, Does.Not.Contain(ScoreCategory.Ones), "Ones should not be available for player 1");
        Assert.That(player1Available.Count, Is.EqualTo(14), "Player 1 should have 14 remaining categories");
    }

    [Test]
    public void GameFlow_CompleteTurn_MaintainsCorrectState()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2");
        _gameService.StartGame();

        // Act - Complete first player's turn
        var initialState = _gameService.GameState;
        Assert.That(initialState.CurrentPlayerIndex, Is.EqualTo(0), "Should start with player 1");

        _gameService.RollDice();
        _gameService.ToggleDieHold(0);
        _gameService.RollDice();
        _gameService.ScoreCategory(ScoreCategory.Ones);

        var afterFirstTurn = _gameService.GameState;

        // Assert
        Assert.That(afterFirstTurn.CurrentPlayerIndex, Is.EqualTo(1), "Should advance to player 2");
        Assert.That(afterFirstTurn.RollsThisTurn, Is.EqualTo(0), "Should reset rolls");
        Assert.That(afterFirstTurn.Players[0].ScoreCard.IsCategoryScored(ScoreCategory.Ones), 
            Is.True, "Player 1 should have scored Ones");
    }

    [Test]
    public void RemovePlayer_BeforeGameStarts_RemovesSuccessfully()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2");
        var player1Id = _gameService.GameState.Players[0].Id;

        // Act
        var result = _gameService.RemovePlayer(player1Id);

        // Assert
        Assert.That(result, Is.True, "Should remove player successfully");
        Assert.That(_gameService.GameState.Players.Count, Is.EqualTo(1), "Should have 1 player left");
    }

    [Test]
    public void RemovePlayer_AfterGameStarts_ReturnsFalse()
    {
        // Arrange
        _gameService.NewGame();
        _gameService.AddPlayer("Player1");
        _gameService.AddPlayer("Player2");
        var player1Id = _gameService.GameState.Players[0].Id;
        _gameService.StartGame();

        // Act
        var result = _gameService.RemovePlayer(player1Id);

        // Assert
        Assert.That(result, Is.False, "Should not remove player after game starts");
        Assert.That(_gameService.GameState.Players.Count, Is.EqualTo(2), "Should still have 2 players");
    }
}
