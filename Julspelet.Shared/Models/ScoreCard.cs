namespace Julspelet.Shared.Models;

/// <summary>
/// Represents a player's scorecard with all scoring categories.
/// Tracks which categories have been scored and calculates totals with bonuses.
/// </summary>
public class ScoreCard
{
    /// <summary>
    /// Dictionary storing scores for each category.
    /// Key: ScoreCategory, Value: Score (null if not yet scored)
    /// </summary>
    public Dictionary<ScoreCategory, int?> Scores { get; set; } = new()
    {
        { ScoreCategory.Ones, null },
        { ScoreCategory.Twos, null },
        { ScoreCategory.Threes, null },
        { ScoreCategory.Fours, null },
        { ScoreCategory.Fives, null },
        { ScoreCategory.Sixes, null },
        { ScoreCategory.OnePair, null },
        { ScoreCategory.TwoPairs, null },
        { ScoreCategory.ThreeOfAKind, null },
        { ScoreCategory.FourOfAKind, null },
        { ScoreCategory.SmallStraight, null },
        { ScoreCategory.LargeStraight, null },
        { ScoreCategory.FullHouse, null },
        { ScoreCategory.Chance, null },
        { ScoreCategory.Yatzy, null }
    };

    /// <summary>
    /// Sets the score for a specific category.
    /// </summary>
    public void SetScore(ScoreCategory category, int score)
    {
        Scores[category] = score;
    }

    /// <summary>
    /// Checks if a category has been scored.
    /// </summary>
    public bool IsCategoryScored(ScoreCategory category)
    {
        return Scores.ContainsKey(category) && Scores[category].HasValue;
    }

    /// <summary>
    /// Gets the upper section total (sum of Ones through Sixes).
    /// </summary>
    public int GetUpperSectionTotal()
    {
        var upperCategories = new[]
        {
            ScoreCategory.Ones, ScoreCategory.Twos, ScoreCategory.Threes,
            ScoreCategory.Fours, ScoreCategory.Fives, ScoreCategory.Sixes
        };

        return upperCategories.Sum(cat => Scores[cat] ?? 0);
    }

    /// <summary>
    /// Gets the upper section bonus (50 points if total >= 63).
    /// In Yatzy, you get a bonus if your upper section sum is 63 or more.
    /// </summary>
    public int GetUpperSectionBonus()
    {
        return GetUpperSectionTotal() >= 63 ? 50 : 0;
    }

    /// <summary>
    /// Gets the lower section total (sum of all combinations).
    /// </summary>
    public int GetLowerSectionTotal()
    {
        var lowerCategories = new[]
        {
            ScoreCategory.OnePair, ScoreCategory.TwoPairs, ScoreCategory.ThreeOfAKind,
            ScoreCategory.FourOfAKind, ScoreCategory.SmallStraight, ScoreCategory.LargeStraight,
            ScoreCategory.FullHouse, ScoreCategory.Chance, ScoreCategory.Yatzy
        };

        return lowerCategories.Sum(cat => Scores[cat] ?? 0);
    }

    /// <summary>
    /// Gets the total score including all bonuses.
    /// </summary>
    public int GetTotalScore()
    {
        return GetUpperSectionTotal() + GetUpperSectionBonus() + GetLowerSectionTotal();
    }

    /// <summary>
    /// Checks if all categories have been scored (game is complete for this player).
    /// </summary>
    public bool IsComplete()
    {
        return Scores.Values.All(score => score.HasValue);
    }
}
