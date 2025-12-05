using NUnit.Framework;
using Julspelet.Shared.Models;
using Julspelet.Shared.Services;

namespace Julspelet.Tests;

/// <summary>
/// Unit tests for the ScoringService.
/// Tests all Yatzy scoring rules to ensure correct score calculation.
/// </summary>
[TestFixture]
public class ScoringServiceTests
{
    private ScoringService _scoringService = null!;

    [SetUp]
    public void Setup()
    {
        _scoringService = new ScoringService();
    }

    #region Upper Section Tests

    [Test]
    public void CalculateScore_Ones_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 1, 1, 3, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Ones, dice);

        // Assert
        Assert.That(score, Is.EqualTo(2), "Should sum all ones: 1 + 1 = 2");
    }

    [Test]
    public void CalculateScore_Twos_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 2, 2, 2, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Twos, dice);

        // Assert
        Assert.That(score, Is.EqualTo(6), "Should sum all twos: 2 + 2 + 2 = 6");
    }

    [Test]
    public void CalculateScore_Sixes_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 6, 6, 6, 6, 1 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Sixes, dice);

        // Assert
        Assert.That(score, Is.EqualTo(24), "Should sum all sixes: 6 + 6 + 6 + 6 = 24");
    }

    [Test]
    public void CalculateScore_UpperSection_NoMatchingDice_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 2, 3, 4, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Ones, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 when no ones are present");
    }

    #endregion

    #region Pair Tests

    [Test]
    public void CalculateScore_OnePair_ReturnsHighestPair()
    {
        // Arrange
        var dice = new int[] { 3, 3, 5, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.OnePair, dice);

        // Assert
        Assert.That(score, Is.EqualTo(10), "Should return highest pair: 5 + 5 = 10");
    }

    [Test]
    public void CalculateScore_OnePair_NoPair_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 1, 2, 3, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.OnePair, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 when no pairs exist");
    }

    [Test]
    public void CalculateScore_TwoPairs_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 2, 2, 4, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.TwoPairs, dice);

        // Assert
        Assert.That(score, Is.EqualTo(12), "Should sum both pairs: (2+2) + (4+4) = 12");
    }

    [Test]
    public void CalculateScore_TwoPairs_OnlyOnePair_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 2, 2, 3, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.TwoPairs, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 when only one pair exists");
    }

    #endregion

    #region Three and Four of a Kind Tests

    [Test]
    public void CalculateScore_ThreeOfAKind_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 4, 4, 4, 2, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.ThreeOfAKind, dice);

        // Assert
        Assert.That(score, Is.EqualTo(12), "Should sum three of a kind: 4 + 4 + 4 = 12");
    }

    [Test]
    public void CalculateScore_ThreeOfAKind_FourOfAKind_ReturnsThree()
    {
        // Arrange
        var dice = new int[] { 5, 5, 5, 5, 2 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.ThreeOfAKind, dice);

        // Assert
        Assert.That(score, Is.EqualTo(15), "Should work with four of a kind: 5 + 5 + 5 = 15");
    }

    [Test]
    public void CalculateScore_FourOfAKind_ReturnsCorrectSum()
    {
        // Arrange
        var dice = new int[] { 3, 3, 3, 3, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.FourOfAKind, dice);

        // Assert
        Assert.That(score, Is.EqualTo(12), "Should sum four of a kind: 3 + 3 + 3 + 3 = 12");
    }

    [Test]
    public void CalculateScore_FourOfAKind_OnlyThree_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 2, 2, 2, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.FourOfAKind, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 when only three of a kind");
    }

    #endregion

    #region Straight Tests

    [Test]
    public void CalculateScore_SmallStraight_Valid_Returns15()
    {
        // Arrange
        var dice = new int[] { 1, 2, 3, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.SmallStraight, dice);

        // Assert
        Assert.That(score, Is.EqualTo(15), "Small straight (1-5) should equal 15");
    }

    [Test]
    public void CalculateScore_SmallStraight_Invalid_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 1, 2, 3, 4, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.SmallStraight, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 for invalid small straight");
    }

    [Test]
    public void CalculateScore_LargeStraight_Valid_Returns20()
    {
        // Arrange
        var dice = new int[] { 2, 3, 4, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.LargeStraight, dice);

        // Assert
        Assert.That(score, Is.EqualTo(20), "Large straight (2-6) should equal 20");
    }

    [Test]
    public void CalculateScore_LargeStraight_Invalid_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 1, 3, 4, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.LargeStraight, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 for invalid large straight");
    }

    #endregion

    #region Full House Tests

    [Test]
    public void CalculateScore_FullHouse_Valid_ReturnsSumOfAll()
    {
        // Arrange
        var dice = new int[] { 3, 3, 3, 5, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.FullHouse, dice);

        // Assert
        Assert.That(score, Is.EqualTo(19), "Full house should sum all dice: 3+3+3+5+5 = 19");
    }

    [Test]
    public void CalculateScore_FullHouse_Invalid_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 3, 3, 4, 5, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.FullHouse, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 for invalid full house");
    }

    [Test]
    public void CalculateScore_FullHouse_AllSame_ReturnsZero()
    {
        // Arrange - Yatzy is not a full house
        var dice = new int[] { 4, 4, 4, 4, 4 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.FullHouse, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Yatzy (all same) is not a valid full house");
    }

    #endregion

    #region Chance and Yatzy Tests

    [Test]
    public void CalculateScore_Chance_ReturnsSumOfAll()
    {
        // Arrange
        var dice = new int[] { 1, 2, 3, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Chance, dice);

        // Assert
        Assert.That(score, Is.EqualTo(15), "Chance should sum all dice: 1+2+3+4+5 = 15");
    }

    [Test]
    public void CalculateScore_Chance_AllSixes_Returns30()
    {
        // Arrange
        var dice = new int[] { 6, 6, 6, 6, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Chance, dice);

        // Assert
        Assert.That(score, Is.EqualTo(30), "Chance with all sixes: 6+6+6+6+6 = 30");
    }

    [Test]
    public void CalculateScore_Yatzy_AllSame_Returns50()
    {
        // Arrange
        var dice = new int[] { 4, 4, 4, 4, 4 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Yatzy, dice);

        // Assert
        Assert.That(score, Is.EqualTo(50), "Yatzy (all same) should return 50 points");
    }

    [Test]
    public void CalculateScore_Yatzy_NotAllSame_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { 4, 4, 4, 4, 5 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Yatzy, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Should return 0 when not all dice are the same");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void CalculateScore_EmptyDiceList_ReturnsZero()
    {
        // Arrange
        var dice = new int[] { };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.Chance, dice);

        // Assert
        Assert.That(score, Is.EqualTo(0), "Empty dice list should return 0");
    }

    [Test]
    public void CalculateScore_ThreeOfAKind_WithYatzy_ReturnsCorrectSum()
    {
        // Arrange - Yatzy should also count as three of a kind
        var dice = new int[] { 6, 6, 6, 6, 6 };

        // Act
        var score = _scoringService.CalculateScore(ScoreCategory.ThreeOfAKind, dice);

        // Assert
        Assert.That(score, Is.EqualTo(18), "Yatzy should count as three of a kind: 6+6+6 = 18");
    }

    #endregion

    #region Available Categories Tests

    [Test]
    public void GetAvailableCategories_EmptyScoreCard_ReturnsAll15Categories()
    {
        // Arrange
        var scoreCard = new ScoreCard();

        // Act
        var available = _scoringService.GetAvailableCategories(scoreCard);

        // Assert
        Assert.That(available.Count, Is.EqualTo(15), "Empty scorecard should have all 15 categories available");
    }

    [Test]
    public void GetAvailableCategories_SomeCategoriesScored_ReturnsOnlyUnscored()
    {
        // Arrange
        var scoreCard = new ScoreCard();
        scoreCard.SetScore(ScoreCategory.Ones, 3);
        scoreCard.SetScore(ScoreCategory.Twos, 6);
        scoreCard.SetScore(ScoreCategory.Yatzy, 50);

        // Act
        var available = _scoringService.GetAvailableCategories(scoreCard);

        // Assert
        Assert.That(available.Count, Is.EqualTo(12), "Should return 12 unscored categories (15 - 3)");
        Assert.That(available, Does.Not.Contain(ScoreCategory.Ones));
        Assert.That(available, Does.Not.Contain(ScoreCategory.Twos));
        Assert.That(available, Does.Not.Contain(ScoreCategory.Yatzy));
    }

    [Test]
    public void GetAvailableCategories_AllCategoriesScored_ReturnsEmpty()
    {
        // Arrange
        var scoreCard = new ScoreCard();
        foreach (ScoreCategory category in Enum.GetValues(typeof(ScoreCategory)))
        {
            scoreCard.SetScore(category, 10);
        }

        // Act
        var available = _scoringService.GetAvailableCategories(scoreCard);

        // Assert
        Assert.That(available.Count, Is.EqualTo(0), "Fully scored card should have no available categories");
    }

    #endregion
}
