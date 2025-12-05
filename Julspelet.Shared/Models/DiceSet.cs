namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a set of 5 dice used in Yatzy.
/// Manages rolling all dice and tracking held dice.
/// </summary>
public class DiceSet
{
    private readonly Random _random = new();

    /// <summary>
    /// The five dice in the set.
    /// </summary>
    public List<Die> Dice { get; set; } = new()
    {
        new Die(),
        new Die(),
        new Die(),
        new Die(),
        new Die()
    };

    /// <summary>
    /// Gets the values of all dice as an array.
    /// </summary>
    public int[] GetValues() => Dice.Select(d => d.Value).ToArray();

    /// <summary>
    /// Rolls all dice that are not held.
    /// </summary>
    public void RollAll()
    {
        foreach (var die in Dice)
        {
            die.Roll(_random);
        }
    }

    /// <summary>
    /// Resets all dice to not held and rolls them.
    /// Used at the start of a new turn.
    /// </summary>
    public void ResetAndRoll()
    {
        foreach (var die in Dice)
        {
            die.IsHeld = false;
        }
        RollAll();
    }

    /// <summary>
    /// Toggles the held state of a die at the specified index.
    /// </summary>
    public void ToggleHold(int index)
    {
        if (index >= 0 && index < Dice.Count)
        {
            Dice[index].ToggleHold();
        }
    }

    /// <summary>
    /// Releases all held dice.
    /// </summary>
    public void ReleaseAllHolds()
    {
        foreach (var die in Dice)
        {
            die.IsHeld = false;
        }
    }
}
