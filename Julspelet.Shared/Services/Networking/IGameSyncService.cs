using Julspelet.Shared.Models;
using Julspelet.Shared.Models.Networking;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Service for synchronizing game state across peers in a P2P network.
/// Handles conflict resolution and ensures all players see consistent game state.
/// </summary>
public interface IGameSyncService
{
    /// <summary>
    /// Event raised when the game state is updated from the network.
    /// </summary>
    event EventHandler<GameState>? GameStateUpdated;

    /// <summary>
    /// Synchronizes the local game state with the network.
    /// Called when a player performs an action (roll dice, select score, etc.)
    /// </summary>
    /// <param name="gameState">The current game state</param>
    Task SyncGameStateAsync(GameState gameState);

    /// <summary>
    /// Requests the full game state from the host.
    /// Used when joining a game in progress or after reconnecting.
    /// </summary>
    Task<GameState?> RequestGameStateAsync();

    /// <summary>
    /// Applies a network message to the local game state.
    /// </summary>
    /// <param name="message">The network message to apply</param>
    /// <param name="currentState">The current game state</param>
    /// <returns>Updated game state, or null if message couldn't be applied</returns>
    GameState? ApplyMessage(NetworkMessage message, GameState currentState);

    /// <summary>
    /// Validates that a game action from a peer is legal.
    /// Host uses this to prevent cheating.
    /// </summary>
    /// <param name="message">The action message to validate</param>
    /// <param name="currentState">The current game state</param>
    /// <returns>True if the action is valid</returns>
    bool ValidateAction(NetworkMessage message, GameState currentState);

    /// <summary>
    /// Resolves conflicts when multiple peers make changes simultaneously.
    /// Uses host as authoritative source for conflict resolution.
    /// </summary>
    /// <param name="localState">The local game state</param>
    /// <param name="networkState">The game state from the network</param>
    /// <returns>The resolved game state</returns>
    GameState ResolveConflict(GameState localState, GameState networkState);
}
