namespace Julspelet.Models;

/// <summary>
/// Represents a single die in the game.
/// A die has a value (1-6) and can be held between throws.
/// </summary>
public class Die
{
    /// <summary>
    /// The current value showing on the die (1-6).
    /// </summary>
    public int Value { get; set; } = 1;

    /// <summary>
    /// Indicates whether the die is held (won't be rolled on next throw).
    /// </summary>
    public bool IsHeld { get; set; }

    /// <summary>
    /// Rolls the die to get a random value between 1 and 6.
    /// Only rolls if the die is not held.
    /// </summary>
    public void Roll(Random random)
    {
        if (!IsHeld)
        {
            Value = random.Next(1, 7); // Random number from 1 to 6
        }
    }

    /// <summary>
    /// Toggles the held state of the die.
    /// </summary>
    public void ToggleHold()
    {
        IsHeld = !IsHeld;
    }
}
