namespace Julspelet.Shared.Models;

/// <summary>
/// Defines all possible scoring categories in Yatzy.
/// Each category has specific rules for calculating scores.
/// </summary>
public enum ScoreCategory
{
    // Upper section (number categories)
    Ones,
    Twos,
    Threes,
    Fours,
    Fives,
    Sixes,

    // Lower section (combination categories)
    OnePair,
    TwoPairs,
    ThreeOfAKind,
    FourOfAKind,
    SmallStraight,  // 1-2-3-4-5
    LargeStraight,  // 2-3-4-5-6
    FullHouse,      // Three of a kind + pair
    Chance,         // Sum of all dice
    Yatzy           // Five of a kind (50 points)
}

/// <summary>
/// Extension methods for ScoreCategory enum.
/// </summary>
public static class ScoreCategoryExtensions
{
    /// <summary>
    /// Gets a human-readable display name for the category.
    /// </summary>
    public static string GetDisplayName(this ScoreCategory category)
    {
        return category switch
        {
            ScoreCategory.Ones => "Ones",
            ScoreCategory.Twos => "Twos",
            ScoreCategory.Threes => "Threes",
            ScoreCategory.Fours => "Fours",
            ScoreCategory.Fives => "Fives",
            ScoreCategory.Sixes => "Sixes",
            ScoreCategory.OnePair => "One Pair",
            ScoreCategory.TwoPairs => "Two Pairs",
            ScoreCategory.ThreeOfAKind => "Three of a Kind",
            ScoreCategory.FourOfAKind => "Four of a Kind",
            ScoreCategory.SmallStraight => "Small Straight (1-5)",
            ScoreCategory.LargeStraight => "Large Straight (2-6)",
            ScoreCategory.FullHouse => "Full House",
            ScoreCategory.Chance => "Chance",
            ScoreCategory.Yatzy => "YATZY!",
            _ => category.ToString()
        };
    }

    /// <summary>
    /// Determines if the category is in the upper section (number categories).
    /// </summary>
    public static bool IsUpperSection(this ScoreCategory category)
    {
        return category is ScoreCategory.Ones or ScoreCategory.Twos or 
               ScoreCategory.Threes or ScoreCategory.Fours or 
               ScoreCategory.Fives or ScoreCategory.Sixes;
    }
}
