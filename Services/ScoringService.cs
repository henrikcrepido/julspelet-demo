using Julspelet.Models;

namespace Julspelet.Services;

/// <summary>
/// Service for calculating Yatzy scores based on dice values.
/// Implements all Yatzy scoring rules for different categories.
/// </summary>
public class ScoringService
{
    /// <summary>
    /// Calculates the score for a given category based on the dice values.
    /// </summary>
    /// <param name="category">The scoring category to calculate</param>
    /// <param name="diceValues">Array of 5 dice values (1-6)</param>
    /// <returns>The score for the category, or 0 if the combination doesn't match</returns>
    public int CalculateScore(ScoreCategory category, int[] diceValues)
    {
        if (diceValues == null || diceValues.Length != 5)
            return 0;

        return category switch
        {
            ScoreCategory.Ones => SumOfNumber(diceValues, 1),
            ScoreCategory.Twos => SumOfNumber(diceValues, 2),
            ScoreCategory.Threes => SumOfNumber(diceValues, 3),
            ScoreCategory.Fours => SumOfNumber(diceValues, 4),
            ScoreCategory.Fives => SumOfNumber(diceValues, 5),
            ScoreCategory.Sixes => SumOfNumber(diceValues, 6),
            ScoreCategory.OnePair => CalculateOnePair(diceValues),
            ScoreCategory.TwoPairs => CalculateTwoPairs(diceValues),
            ScoreCategory.ThreeOfAKind => CalculateThreeOfAKind(diceValues),
            ScoreCategory.FourOfAKind => CalculateFourOfAKind(diceValues),
            ScoreCategory.SmallStraight => CalculateSmallStraight(diceValues),
            ScoreCategory.LargeStraight => CalculateLargeStraight(diceValues),
            ScoreCategory.FullHouse => CalculateFullHouse(diceValues),
            ScoreCategory.Chance => CalculateChance(diceValues),
            ScoreCategory.Yatzy => CalculateYatzy(diceValues),
            _ => 0
        };
    }

    /// <summary>
    /// Calculates sum of all dice showing a specific number.
    /// Used for Ones, Twos, Threes, Fours, Fives, Sixes.
    /// </summary>
    private int SumOfNumber(int[] diceValues, int number)
    {
        return diceValues.Where(d => d == number).Sum();
    }

    /// <summary>
    /// Calculates score for One Pair (sum of the highest pair).
    /// </summary>
    private int CalculateOnePair(int[] diceValues)
    {
        var counts = GetDiceCounts(diceValues);
        
        // Find the highest value that appears at least twice
        for (int value = 6; value >= 1; value--)
        {
            if (counts[value] >= 2)
                return value * 2;
        }
        
        return 0;
    }

    /// <summary>
    /// Calculates score for Two Pairs (sum of both pairs).
    /// </summary>
    private int CalculateTwoPairs(int[] diceValues)
    {
        var counts = GetDiceCounts(diceValues);
        var pairs = new List<int>();

        // Find all pairs
        for (int value = 6; value >= 1; value--)
        {
            if (counts[value] >= 2)
                pairs.Add(value);
        }

        // Need exactly 2 different pairs
        if (pairs.Count >= 2)
            return (pairs[0] * 2) + (pairs[1] * 2);

        return 0;
    }

    /// <summary>
    /// Calculates score for Three of a Kind (sum of the three dice).
    /// </summary>
    private int CalculateThreeOfAKind(int[] diceValues)
    {
        var counts = GetDiceCounts(diceValues);

        // Find the highest value that appears at least three times
        for (int value = 6; value >= 1; value--)
        {
            if (counts[value] >= 3)
                return value * 3;
        }

        return 0;
    }

    /// <summary>
    /// Calculates score for Four of a Kind (sum of the four dice).
    /// </summary>
    private int CalculateFourOfAKind(int[] diceValues)
    {
        var counts = GetDiceCounts(diceValues);

        // Find the value that appears at least four times
        for (int value = 6; value >= 1; value--)
        {
            if (counts[value] >= 4)
                return value * 4;
        }

        return 0;
    }

    /// <summary>
    /// Calculates score for Small Straight (1-2-3-4-5 = 15 points).
    /// </summary>
    private int CalculateSmallStraight(int[] diceValues)
    {
        var sorted = diceValues.OrderBy(d => d).ToArray();
        var smallStraight = new[] { 1, 2, 3, 4, 5 };

        if (sorted.SequenceEqual(smallStraight))
            return 15;

        return 0;
    }

    /// <summary>
    /// Calculates score for Large Straight (2-3-4-5-6 = 20 points).
    /// </summary>
    private int CalculateLargeStraight(int[] diceValues)
    {
        var sorted = diceValues.OrderBy(d => d).ToArray();
        var largeStraight = new[] { 2, 3, 4, 5, 6 };

        if (sorted.SequenceEqual(largeStraight))
            return 20;

        return 0;
    }

    /// <summary>
    /// Calculates score for Full House (three of a kind + pair = sum of all dice).
    /// </summary>
    private int CalculateFullHouse(int[] diceValues)
    {
        var counts = GetDiceCounts(diceValues);
        bool hasThree = false;
        bool hasPair = false;

        foreach (var count in counts.Values)
        {
            if (count == 3)
                hasThree = true;
            else if (count == 2)
                hasPair = true;
        }

        if (hasThree && hasPair)
            return diceValues.Sum();

        return 0;
    }

    /// <summary>
    /// Calculates score for Chance (sum of all dice, no requirements).
    /// </summary>
    private int CalculateChance(int[] diceValues)
    {
        return diceValues.Sum();
    }

    /// <summary>
    /// Calculates score for Yatzy (all five dice the same = 50 points).
    /// </summary>
    private int CalculateYatzy(int[] diceValues)
    {
        if (diceValues.Distinct().Count() == 1)
            return 50;

        return 0;
    }

    /// <summary>
    /// Helper method to count occurrences of each die value (1-6).
    /// </summary>
    private Dictionary<int, int> GetDiceCounts(int[] diceValues)
    {
        var counts = new Dictionary<int, int>();
        
        for (int i = 1; i <= 6; i++)
        {
            counts[i] = diceValues.Count(d => d == i);
        }

        return counts;
    }

    /// <summary>
    /// Gets all available categories that haven't been scored yet.
    /// </summary>
    public List<ScoreCategory> GetAvailableCategories(ScoreCard scoreCard)
    {
        return Enum.GetValues<ScoreCategory>()
            .Where(cat => !scoreCard.IsCategoryScored(cat))
            .ToList();
    }
}
