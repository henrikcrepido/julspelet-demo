using System.Text.Json.Serialization;

namespace Julspelet.Shared.Models.Networking;

/// <summary>
/// Base class for all network messages exchanged between peers.
/// Uses polymorphic serialization to support different message types.
/// </summary>
[JsonDerivedType(typeof(PlayerJoinedMessage), typeDiscriminator: "player_joined")]
[JsonDerivedType(typeof(PlayerLeftMessage), typeDiscriminator: "player_left")]
[JsonDerivedType(typeof(GameStateUpdateMessage), typeDiscriminator: "game_state")]
[JsonDerivedType(typeof(DiceRollMessage), typeDiscriminator: "dice_roll")]
[JsonDerivedType(typeof(ScoreSelectionMessage), typeDiscriminator: "score_selection")]
[JsonDerivedType(typeof(TurnChangeMessage), typeDiscriminator: "turn_change")]
[JsonDerivedType(typeof(GameStartMessage), typeDiscriminator: "game_start")]
[JsonDerivedType(typeof(GameEndMessage), typeDiscriminator: "game_end")]
[JsonDerivedType(typeof(ChatMessage), typeDiscriminator: "chat")]
[JsonDerivedType(typeof(HeartbeatMessage), typeDiscriminator: "heartbeat")]
public abstract class NetworkMessage
{
    /// <summary>
    /// Unique identifier for this message.
    /// </summary>
    public string MessageId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ID of the sender peer.
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Sequence number for ordering messages from the same sender.
    /// </summary>
    public long SequenceNumber { get; set; }
}

/// <summary>
/// Message sent when a new player joins the game session.
/// </summary>
public class PlayerJoinedMessage : NetworkMessage
{
    public string PlayerName { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
}

/// <summary>
/// Message sent when a player leaves the game session.
/// </summary>
public class PlayerLeftMessage : NetworkMessage
{
    public string PlayerId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Message containing a full game state update.
/// Used for synchronization when players join or reconnect.
/// </summary>
public class GameStateUpdateMessage : NetworkMessage
{
    public string GameStateJson { get; set; } = string.Empty;
}

/// <summary>
/// Message sent when a player rolls the dice.
/// </summary>
public class DiceRollMessage : NetworkMessage
{
    public string PlayerId { get; set; } = string.Empty;
    public List<int> DiceValues { get; set; } = new();
    public List<bool> HeldDice { get; set; } = new();
    public int RollNumber { get; set; }
}

/// <summary>
/// Message sent when a player selects a score category.
/// </summary>
public class ScoreSelectionMessage : NetworkMessage
{
    public string PlayerId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Score { get; set; }
}

/// <summary>
/// Message sent when the turn changes to the next player.
/// </summary>
public class TurnChangeMessage : NetworkMessage
{
    public string CurrentPlayerId { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
}

/// <summary>
/// Message sent when the game starts.
/// </summary>
public class GameStartMessage : NetworkMessage
{
    public List<string> PlayerOrder { get; set; } = new();
    public string GameMode { get; set; } = string.Empty;
}

/// <summary>
/// Message sent when the game ends.
/// </summary>
public class GameEndMessage : NetworkMessage
{
    public string WinnerId { get; set; } = string.Empty;
    public Dictionary<string, int> FinalScores { get; set; } = new();
}

/// <summary>
/// Message for chat communication between players.
/// </summary>
public class ChatMessage : NetworkMessage
{
    public string PlayerName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Heartbeat message to maintain connection and detect disconnections.
/// </summary>
public class HeartbeatMessage : NetworkMessage
{
    public int PeersConnected { get; set; }
}
