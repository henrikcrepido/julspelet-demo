using NUnit.Framework;
using Julspelet.Shared.Models;
using Julspelet.Shared.Models.Networking;
using Julspelet.Shared.Services;
using Julspelet.Shared.Services.Networking;
using System.Text.Json;

namespace Julspelet.Tests;

/// <summary>
/// Integration tests for multiplayer networking models and message serialization.
/// Tests message structures, session management, and network communication patterns.
/// </summary>
[TestFixture]
public class MultiplayerIntegrationTests
{
    private ScoringService _scoringService = null!;
    private MessageValidator _messageValidator = null!;
    private MessageAuthenticator _messageAuthenticator = null!;

    [SetUp]
    public void Setup()
    {
        _scoringService = new ScoringService();
        _messageValidator = new MessageValidator(_scoringService);
        _messageAuthenticator = new MessageAuthenticator();
    }

    [Test]
    public void GameSession_Creation_InitializesCorrectly()
    {
        // Arrange
        var hostName = "Host Player";
        var maxPlayers = 4;

        // Act
        var session = new GameSession
        {
            SessionId = Guid.NewGuid().ToString(),
            HostName = hostName,
            MaxPlayers = maxPlayers,
            NetworkType = NetworkType.SignalR,
            LocalPeerId = Guid.NewGuid().ToString(),
            IsHost = true
        };

        // Assert
        Assert.That(session.SessionId, Is.Not.Null.And.Not.Empty, "Session ID should be set");
        Assert.That(session.HostName, Is.EqualTo(hostName), "Host name should match");
        Assert.That(session.MaxPlayers, Is.EqualTo(maxPlayers), "Max players should match");
        Assert.That(session.IsHost, Is.True, "Should be marked as host");
    }

    [Test]
    public void PeerInfo_Creation_ContainsRequiredFields()
    {
        // Arrange & Act
        var peer = new PeerInfo
        {
            PeerId = Guid.NewGuid().ToString(),
            PlayerName = "Test Player",
            State = ConnectionState.Connected,
            LastHeartbeat = DateTime.UtcNow
        };

        // Assert
        Assert.That(peer.PeerId, Is.Not.Null.And.Not.Empty, "Peer ID should be set");
        Assert.That(peer.DisplayName, Is.EqualTo("Test Player"), "Display name should match");
        Assert.That(peer.State, Is.EqualTo(ConnectionState.Connected), "Should be connected");
        Assert.That(peer.LastHeartbeat, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)), "Last heartbeat should be recent");
    }

    [Test]
    public void NetworkMessage_Serialization_PreservesPolymorphicType()
    {
        // Arrange
        var originalMessage = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, true, false, true, false },
            RollNumber = 1
        };

        // Act
        var json = JsonSerializer.Serialize<NetworkMessage>(originalMessage);
        var deserializedMessage = JsonSerializer.Deserialize<NetworkMessage>(json);

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null, "Deserialized message should not be null");
        Assert.That(deserializedMessage, Is.TypeOf<DiceRollMessage>(), "Should deserialize to correct type");
        
        var diceRoll = deserializedMessage as DiceRollMessage;
        Assert.That(diceRoll!.PlayerId, Is.EqualTo("player1"), "Player ID should match");
        Assert.That(diceRoll.DiceValues, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }), "Dice values should match");
        Assert.That(diceRoll.RollNumber, Is.EqualTo(1), "Roll number should match");
    }

    [Test]
    public void DiceRollMessage_Serialization_PreservesAllProperties()
    {
        // Arrange
        var originalMessage = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, true, false, true, false },
            RollNumber = 1
        };

        // Act
        var json = JsonSerializer.Serialize<NetworkMessage>(originalMessage);
        var deserializedMessage = JsonSerializer.Deserialize<NetworkMessage>(json) as DiceRollMessage;

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null, "Deserialized message should not be null");
        Assert.That(deserializedMessage!.PlayerId, Is.EqualTo("player1"), "Player ID should match");
        Assert.That(deserializedMessage.DiceValues, Is.EqualTo(new List<int> { 1, 2, 3, 4, 5 }), "Dice values should match");
        Assert.That(deserializedMessage.RollNumber, Is.EqualTo(1), "Roll number should match");
    }

    [Test]
    public void ScoreSelectionMessage_Serialization_PreservesCategory()
    {
        // Arrange
        var originalMessage = new ScoreSelectionMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            Category = "Ones",
            Score = 5
        };

        // Act
        var json = JsonSerializer.Serialize<NetworkMessage>(originalMessage);
        var deserializedMessage = JsonSerializer.Deserialize<NetworkMessage>(json) as ScoreSelectionMessage;

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null, "Deserialized message should not be null");
        Assert.That(deserializedMessage!.PlayerId, Is.EqualTo("player1"), "Player ID should match");
        Assert.That(deserializedMessage.Category, Is.EqualTo("Ones"), "Category should match");
        Assert.That(deserializedMessage.Score, Is.EqualTo(5), "Score should match");
    }

    [Test]
    public void PlayerJoinedMessage_Serialization_PreservesPlayerInfo()
    {
        // Arrange
        var originalMessage = new PlayerJoinedMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player2",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player2",
            PlayerName = "Player Two"
        };

        // Act
        var json = JsonSerializer.Serialize<NetworkMessage>(originalMessage);
        var deserializedMessage = JsonSerializer.Deserialize<NetworkMessage>(json) as PlayerJoinedMessage;

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null, "Deserialized message should not be null");
        Assert.That(deserializedMessage!.PlayerId, Is.EqualTo("player2"), "Player ID should match");
        Assert.That(deserializedMessage.PlayerName, Is.EqualTo("Player Two"), "Player name should match");
    }

    [Test]
    public void GameStartMessage_Serialization_PreservesPlayerList()
    {
        // Arrange
        var originalMessage = new GameStartMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "host",
            Timestamp = DateTime.UtcNow,
            PlayerIds = new List<string> { "player1", "player2", "player3" },
            GameMode = GameMode.Classic
        };

        // Act
        var json = JsonSerializer.Serialize<NetworkMessage>(originalMessage);
        var deserializedMessage = JsonSerializer.Deserialize<NetworkMessage>(json) as GameStartMessage;

        // Assert
        Assert.That(deserializedMessage, Is.Not.Null, "Deserialized message should not be null");
        Assert.That(deserializedMessage!.PlayerIds.Count, Is.EqualTo(3), "Should have 3 players");
        Assert.That(deserializedMessage.GameMode, Is.EqualTo(GameMode.Classic), "Game mode should match");
    }

    [Test]
    public void MessageAuthenticator_SignAndVerify_WorksCorrectly()
    {
        // Arrange
        var secret = _messageAuthenticator.GenerateSessionSecret();
        var message = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, false, false, false, false },
            RollNumber = 1
        };

        // Act
        var signature = _messageAuthenticator.SignMessage(message, secret);
        var isValid = _messageAuthenticator.VerifySignature(message, signature, secret);

        // Assert
        Assert.That(signature, Is.Not.Null.And.Not.Empty, "Signature should be generated");
        Assert.That(isValid, Is.True, "Valid signature should verify successfully");
    }

    [Test]
    public void MessageAuthenticator_TamperedMessage_FailsVerification()
    {
        // Arrange
        var secret = _messageAuthenticator.GenerateSessionSecret();
        var message = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, false, false, false, false },
            RollNumber = 1
        };

        var signature = _messageAuthenticator.SignMessage(message, secret);
        
        // Tamper with message
        message.DiceValues = new List<int> { 6, 6, 6, 6, 6 };

        // Act
        var isValid = _messageAuthenticator.VerifySignature(message, signature, secret);

        // Assert
        Assert.That(isValid, Is.False, "Tampered message should fail verification");
    }

    [Test]
    public void MessageValidator_ValidatesBasicRules()
    {
        // Arrange
        var message = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow,
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, false, false, false, false },
            RollNumber = 1
        };

        // Act - Validate message timing (should pass for recent message)
        var isTimingValid = _messageValidator.ValidateMessageTiming(message);

        // Assert
        Assert.That(isTimingValid, Is.True, "Recent message should pass timing validation");
    }

    [Test]
    public void MessageValidator_RejectsOldMessages()
    {
        // Arrange
        var message = new DiceRollMessage
        {
            MessageId = Guid.NewGuid().ToString(),
            SenderId = "player1",
            Timestamp = DateTime.UtcNow.AddMinutes(-2), // 2 minutes old
            PlayerId = "player1",
            DiceValues = new List<int> { 1, 2, 3, 4, 5 },
            HeldDice = new List<bool> { false, false, false, false, false },
            RollNumber = 1
        };

        // Act
        var isTimingValid = _messageValidator.ValidateMessageTiming(message);

        // Assert
        Assert.That(isTimingValid, Is.False, "Old message should fail timing validation");
    }

    [Test]
    public void MessageValidator_RateLimiting_EnforcesLimits()
    {
        // Arrange
        var senderId = "player1";
        var messageType = "dice_roll";

        // Act - Check rate limit 15 times rapidly
        var results = new List<bool>();
        for (int i = 0; i < 15; i++)
        {
            results.Add(_messageValidator.CheckRateLimit(senderId, messageType));
        }

        // Assert
        var allowedCount = results.Count(r => r);
        Assert.That(allowedCount, Is.LessThan(15), "Should enforce rate limiting after multiple rapid messages");
        Assert.That(allowedCount, Is.GreaterThan(0), "Should allow some messages");
    }

    [Test]
    public void ScoringService_Integration_CalculatesCorrectScores()
    {
        // Arrange
        var dice = new int[] { 1, 1, 3, 4, 5 };

        // Act
        var onesScore = _scoringService.CalculateScore(ScoreCategory.Ones, dice);
        var threesScore = _scoringService.CalculateScore(ScoreCategory.Threes, dice);
        var chanceScore = _scoringService.CalculateScore(ScoreCategory.Chance, dice);

        // Assert
        Assert.That(onesScore, Is.EqualTo(2), "Should sum ones correctly");
        Assert.That(threesScore, Is.EqualTo(3), "Should sum threes correctly");
        Assert.That(chanceScore, Is.EqualTo(14), "Chance should sum all dice");
    }

    [Test]
    public void GameSession_Initialization_SetsProperties()
    {
        // Arrange & Act
        var session = new GameSession
        {
            SessionId = "test-123",
            HostName = "Test Host",
            MaxPlayers = 4,
            CurrentPlayers = 2,
            NetworkType = NetworkType.SignalR,
            LocalPeerId = "peer-123",
            IsHost = true
        };

        // Assert
        Assert.That(session.SessionId, Is.EqualTo("test-123"), "Session ID should match");
        Assert.That(session.HostName, Is.EqualTo("Test Host"), "Host name should match");
        Assert.That(session.MaxPlayers, Is.EqualTo(4), "Max players should match");
        Assert.That(session.CurrentPlayers, Is.EqualTo(2), "Current players should match");
        Assert.That(session.IsHost, Is.True, "Should be host");
    }

    [Test]
    public void PeerInfo_Properties_WorkCorrectly()
    {
        // Arrange & Act
        var peer = new PeerInfo
        {
            PeerId = "peer-456",
            PlayerName = "Test Player",
            State = ConnectionState.Connected,
            LastHeartbeat = DateTime.UtcNow
        };

        // Assert
        Assert.That(peer.PeerId, Is.EqualTo("peer-456"), "Peer ID should match");
        Assert.That(peer.DisplayName, Is.EqualTo("Test Player"), "Display name should match");
        Assert.That(peer.State, Is.EqualTo(ConnectionState.Connected), "Should be connected");
    }
}
