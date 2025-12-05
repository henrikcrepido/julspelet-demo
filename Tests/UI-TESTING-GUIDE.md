# UI Testing Guide for Julspelet

This document describes the testing strategy and setup for automated UI testing of the Julspelet Yatzy game.

## Testing Strategy

The application uses a multi-layered testing approach:

### 1. Unit Tests ✅ (Implemented)
- **Framework**: NUnit
- **Coverage**: All scoring logic and business rules
- **Files**: `Tests/ScoringServiceTests.cs`
- **Status**: 28 tests, all passing

### 2. Integration Tests ✅ (Implemented)
- **Framework**: NUnit
- **Coverage**: Game service integration, multiplayer networking
- **Files**: `Tests/GameServiceIntegrationTests.cs`, `Tests/MultiplayerIntegrationTests.cs`
- **Status**: 28 tests, all passing

### 3. Component Tests (Recommended - Advanced)
- **Framework**: bUnit
- **Purpose**: Test Blazor components in isolation
- **Complexity**: High (requires mocking JSInterop, MudBlazor, and navigation)

### 4. End-to-End Tests (Recommended)
- **Framework**: Playwright or Selenium
- **Purpose**: Test complete user workflows
- **Complexity**: Medium

### 5. Multiplayer Testing ✅ (Documented)
- **Coverage**: SignalR web multiplayer, MAUI P2P, cross-platform scenarios
- **Documentation**: `Tests/MULTIPLAYER-TESTING.md`
- **Status**: Manual testing guide with comprehensive test scenarios

## Current Test Coverage

### ✅ Unit Tests (Scoring Logic)
All Yatzy scoring rules are thoroughly tested:
- Upper section categories (Ones through Sixes)
- Lower section categories (Pairs, Straights, Full House, etc.)
- Edge cases and validation
- Bonus calculations

```bash
dotnet test --filter "FullyQualifiedName~ScoringServiceTests"
# Result: 28/28 tests passing
```

### ✅ Integration Tests (Game Logic & Networking)
Game service and multiplayer networking thoroughly tested:
- Game initialization and player management
- Dice rolling and turn progression
- Score selection and game flow
- Network message serialization/deserialization
- Message authentication and validation
- Anti-cheat protection and rate limiting

```bash
dotnet test --filter "FullyQualifiedName~GameServiceIntegrationTests|FullyQualifiedName~MultiplayerIntegrationTests"
# Result: 28/28 tests passing
```

### Total Automated Test Coverage
```bash
dotnet test
# Result: 56/56 tests passing
```

## Recommended Approach: End-to-End Testing with Playwright

For UI testing of Blazor Server applications, E2E tests are often more practical than component tests.

### Setup Playwright for .NET

1. **Install Playwright package**:
```bash
dotnet add package Microsoft.Playwright.NUnit
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

2. **Create E2E test file**: `Tests/E2ETests.cs`

### Sample E2E Test Structure

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[TestFixture]
public class YatzyE2ETests : PageTest
{
    private const string BaseUrl = "http://localhost:5027";

    [Test]
    public async Task CanJoinGameWithTwoPlayers()
    {
        // Arrange - Navigate to the game
        await Page.GotoAsync(BaseUrl);

        // Act - Add two players
        await Page.FillAsync("input[placeholder='Player Name']", "Alice");
        await Page.ClickAsync("button:has-text('Add Player')");
        
        await Page.FillAsync("input[placeholder='Player Name']", "Bob");
        await Page.ClickAsync("button:has-text('Add Player')");

        // Assert - Start button should be enabled
        var startButton = Page.Locator("button:has-text('Start Game')");
        await Expect(startButton).ToBeEnabledAsync();
    }

    [Test]
    public async Task CanRollDiceAndScore()
    {
        // Setup game with players
        await SetupGameWithPlayers("Player1", "Player2");

        // Act - Roll dice
        await Page.ClickAsync("button:has-text('Roll')");

        // Assert - Dice should be visible
        await Expect(Page.Locator("text=/⚀|⚁|⚂|⚃|⚄|⚅/")).ToBeVisibleAsync();
        
        // Act - Score in Chance category
        await Page.ClickAsync("text=Chance >> button");

        // Assert - Should move to next player
        await Expect(Page.Locator("text=Player2")).ToBeVisibleAsync();
    }

    private async Task SetupGameWithPlayers(params string[] playerNames)
    {
        await Page.GotoAsync(BaseUrl);
        foreach (var name in playerNames)
        {
            await Page.FillAsync("input", name);
            await Page.ClickAsync("button:has-text('Add Player')");
        }
        await Page.ClickAsync("button:has-text('Start Game')");
    }
}
```

## Manual Testing Checklist

Until automated UI tests are implemented, use this checklist:

### Player Join Flow
- [ ] Can add players with valid names
- [ ] Cannot add duplicate player names
- [ ] Can remove players before game starts
- [ ] Start button disabled with < 2 players
- [ ] Start button enabled with 2+ players
- [ ] Successfully navigates to game page

### Game Flow
- [ ] Displays current player name
- [ ] Shows rolls remaining (3, 2, 1, 0)
- [ ] Can roll dice (up to 3 times)
- [ ] Dice display with emoji faces
- [ ] Can hold/release individual dice
- [ ] Held dice maintain value between rolls
- [ ] Roll button disabled after 3 rolls

### Scoring
- [ ] All 15 categories displayed
- [ ] Potential scores shown for available categories
- [ ] Can select and score a category
- [ ] Category becomes unavailable after scoring
- [ ] Score updates on scorecard
- [ ] Turn moves to next player after scoring

### Scoreboard
- [ ] Shows all players and their scores
- [ ] Upper section bonus calculation (63+ = 50 pts)
- [ ] Running totals update correctly
- [ ] Current player highlighted

### Game Completion
- [ ] Game ends when all categories scored by all players
- [ ] Winner display shows correct player
- [ ] Winner has highest score
- [ ] Handles ties correctly
- [ ] New Game button navigates to player join

## Running the Application for Testing

```bash
# Terminal 1: Start the application
dotnet run

# Terminal 2: Run tests (unit tests only currently)
dotnet test

# For manual testing
# Open browser to: http://localhost:5027
```

## Future Improvements

1. **Implement Playwright E2E Tests**
   - Complete user workflows
   - Cross-browser testing
   - Screenshot comparison

2. **Add Performance Tests**
   - Page load times
   - SignalR connection stability
   - Concurrent user handling

3. **Accessibility Testing**
   - Screen reader compatibility
   - Keyboard navigation
   - ARIA labels and roles

4. **Visual Regression Testing**
   - Screenshot comparison
   - CSS changes detection
   - Responsive design verification

## Test Execution CI/CD

For continuous integration, add to your pipeline:

```yaml
# GitHub Actions example
- name: Run Unit Tests
  run: dotnet test --no-build --verbosity normal

- name: Run E2E Tests (future)
  run: |
    dotnet run --project Julspelet.csproj &
    sleep 5
    dotnet test Tests/E2ETests.cs
```

## Notes

- **bUnit limitations**: MudBlazor components require extensive JSInterop mocking
- **Playwright advantages**: Tests real browser behavior, easier to maintain
- **Current focus**: Unit tests provide excellent coverage of business logic
- **UI confidence**: Manual testing checklist ensures UI quality

## Resources

- [bUnit Documentation](https://bunit.dev/)
- [Playwright for .NET](https://playwright.dev/dotnet/)
- [Blazor Testing Best Practices](https://learn.microsoft.com/en-us/aspnet/core/blazor/test)
- [MudBlazor Testing](https://mudblazor.com/getting-started/installation)
