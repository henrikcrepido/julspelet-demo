# 🎄 Julspelet - Christmas Yatzy Tournament 🎅

A festive multiplayer Yatzy game built with .NET 8 Blazor Server and MudBlazor. Gather your friends and family for a fun Christmas-themed dice tournament!

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor)
![MudBlazor](https://img.shields.io/badge/MudBlazor-8.15.0-594AE2)
![Tests](https://img.shields.io/badge/tests-56%20passing-success)
![Platforms](https://img.shields.io/badge/platforms-Web%20%7C%20Android%20%7C%20iOS%20%7C%20Windows%20%7C%20macOS-blue)

## 🎮 Features

- **🌐 Web Multiplayer**: Play with friends anywhere using SignalR (6-digit session codes)
- **📱 Local P2P Multiplayer**: Connect directly on local Wi-Fi (MAUI - Android/iOS/Windows/macOS)
- **🎲 Classic Yatzy Rules**: All 15 scoring categories with proper bonuses
- **🎯 Interactive Gameplay**: Roll dice up to 3 times per turn, hold dice between rolls
- **📊 Real-time Scoreboard**: Track all players' progress throughout the game
- **🛡️ Anti-Cheat Protection**: Server-authoritative validation with message authentication
- **🎄 Christmas Theme**: Festive UI with holiday colors, animations, and decorations
- **📱 Cross-Platform**: Web, Android, iOS, Windows, and macOS support
- **🔒 Privacy-First**: Local multiplayer data never leaves your network

## 🎲 Game Rules

### Objective
Score the highest total points by filling all 15 categories on your scorecard.

### Gameplay
1. **Join**: Enter your name to join the game (minimum 2 players)
2. **Roll**: Each turn, roll 5 dice up to 3 times
3. **Hold**: Click dice to hold them between rolls
4. **Score**: Choose a category to score after rolling
5. **Win**: Player with the highest total score wins!

### Scoring Categories

#### Upper Section (with 50-point bonus if total ≥ 63)
- **Ones** through **Sixes**: Sum of matching dice

#### Lower Section
- **One Pair**: Sum of highest pair
- **Two Pairs**: Sum of both pairs
- **Three of a Kind**: Sum of three matching dice
- **Four of a Kind**: Sum of four matching dice
- **Small Straight** (1-2-3-4-5): 15 points
- **Large Straight** (2-3-4-5-6): 20 points
- **Full House** (3 of a kind + pair): Sum of all dice
- **Chance**: Sum of all dice (any combination)
- **Yatzy** (5 of a kind): 50 points

## 🌐 Multiplayer

Julspelet supports two multiplayer modes:

### Web Multiplayer (SignalR)
- Play from anywhere with internet
- Host creates session, shares 6-digit code
- Friends join using the code
- Real-time synchronization via SignalR
- Perfect for remote play

### Local Multiplayer (MAUI P2P)
- Play on same Wi-Fi network
- No internet required
- Automatic session discovery
- Direct device-to-device communication
- Low latency, private gaming

**See [MULTIPLAYER.md](MULTIPLAYER.md) for complete guide!**

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A modern web browser
- (Optional) Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/henrikcrepido/julspelet-demo.git
   cd julspelet-demo
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

### Running the Application

**Start the development server:**
```bash
dotnet run
```

The application will be available at `http://localhost:5027` (or the port shown in your terminal).

### Running Tests

**Execute all tests:**
```bash
dotnet test
```

All 56 tests should pass (28 unit tests + 28 integration tests).

**Run specific test suites:**
```bash
# Unit tests (scoring logic)
dotnet test --filter "FullyQualifiedName~ScoringServiceTests"

# Integration tests (game service & networking)
dotnet test --filter "FullyQualifiedName~GameServiceIntegrationTests|FullyQualifiedName~MultiplayerIntegrationTests"
```

## 📁 Project Structure

```
julspelet-demo/
├── Julspelet.Shared/                 # Shared Razor Class Library
│   ├── Components/
│   │   ├── Pages/
│   │   │   ├── SessionBrowser.razor   # Multiplayer session browser
│   │   │   ├── MultiplayerLobby.razor # Game lobby
│   │   │   ├── PlayerJoin.razor       # Player entry page
│   │   │   ├── Game.razor             # Main game board
│   │   │   └── ...                    # Other pages
│   │   └── Layout/
│   │       ├── MainLayout.razor       # Main layout
│   │       └── NavMenu.razor          # Navigation menu
│   ├── Models/
│   │   ├── Networking/
│   │   │   ├── NetworkMessage.cs      # Polymorphic message types
│   │   │   ├── GameSession.cs         # Session information
│   │   │   └── PeerInfo.cs            # Peer connection info
│   │   ├── Die.cs, DiceSet.cs         # Dice models
│   │   ├── GameState.cs, Player.cs    # Game models
│   │   └── ScoreCard.cs, ScoreCategory.cs
│   └── Services/
│       ├── Networking/
│       │   ├── INetworkService.cs      # Network abstraction
│       │   ├── SignalRNetworkService.cs # Web implementation
│       │   ├── SocketNetworkService.cs  # P2P implementation
│       │   ├── GameSyncService.cs       # State synchronization
│       │   ├── MessageValidator.cs      # Anti-cheat validation
│       │   └── MessageAuthenticator.cs  # Message signing
│       ├── GameService.cs              # Game flow management
│       ├── ScoringService.cs           # Yatzy scoring logic
│       └── TournamentService.cs        # Tournament management
├── Julspelet.csproj                  # Web project (Blazor Server)
│   ├── Hubs/
│   │   └── GameHub.cs                 # SignalR hub for web multiplayer
│   └── Program.cs                     # Web app configuration
├── Julspelet.Maui/                   # MAUI Blazor Hybrid project
│   ├── Platforms/                     # Platform-specific code
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── Windows/
│   │   └── MacCatalyst/
│   └── MauiProgram.cs                 # MAUI configuration
├── Tests/
│   ├── ScoringServiceTests.cs         # Scoring logic tests (28)
│   ├── GameServiceIntegrationTests.cs # Game service tests (14)
│   ├── MultiplayerIntegrationTests.cs # Network tests (14)
│   ├── UI-TESTING-GUIDE.md            # Testing documentation
│   └── MULTIPLAYER-TESTING.md         # Multiplayer testing guide
├── MULTIPLAYER.md                    # User multiplayer guide
├── PROGRESS.md                       # Development progress log
└── README.md                         # This file
```

## 🎨 Technology Stack

- **Framework**: .NET 8
- **UI**: Blazor Server (Interactive Server render mode)
- **Component Library**: MudBlazor 8.15.0
- **Styling**: Custom CSS with Christmas theme
- **Testing**: NUnit 4.2.2
- **Architecture**: Service-based with dependency injection

## 🛠️ Development

### Key Blazor Concepts Demonstrated

This project showcases several Blazor patterns ideal for learning:

- **Component Lifecycle**: `OnInitialized`, `Dispose`, `StateHasChanged`
- **Routing**: `@page` directive with dynamic navigation
- **Dependency Injection**: Service registration and `@inject` usage
- **Event Handling**: Button clicks, keyboard events, dice interactions
- **Two-way Binding**: `@bind-Value` for form inputs
- **State Management**: Event-driven UI updates with scoped services
- **Render Modes**: `@rendermode InteractiveServer` for real-time interactivity

### Architecture Patterns

- **Separation of Concerns**: Models, Services, Components clearly separated
- **Service Layer**: Business logic encapsulated in services
- **Dependency Injection**: Loose coupling with scoped service lifetime
- **Event-Driven**: Game state changes trigger UI re-renders
- **Comprehensive Testing**: Unit tests for all scoring rules

### Building for Production

Build in Release mode:
```bash
dotnet build --configuration Release
```

Publish the application:
```bash
dotnet publish --configuration Release --output ./publish
```

## 🧪 Testing

The project includes comprehensive unit tests for all Yatzy scoring rules:

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Generate code coverage
dotnet test /p:CollectCoverage=true
```

**Test Coverage:**

_Unit Tests (28):_
- ✅ All 15 Yatzy scoring categories
- ✅ Edge cases (empty dice, invalid combinations)
- ✅ Bonus calculations and available categories

_Integration Tests (28):_
- ✅ Game service integration (14 tests)
  - Game initialization and player management
  - Dice rolling, holding, and turn progression
  - Scoring and game flow
- ✅ Multiplayer networking (14 tests)
  - Message serialization/deserialization
  - Authentication and validation
  - Anti-cheat protection and rate limiting

**Total: 56 tests, all passing**

### UI Testing

For UI and end-to-end testing guidance, see [Tests/UI-TESTING-GUIDE.md](Tests/UI-TESTING-GUIDE.md).

Recommended approach:
- **Unit Tests** (implemented): Test business logic and scoring rules
- **E2E Tests** (recommended): Use Playwright for complete user workflows
- **Manual Testing**: Use the provided checklist for UI validation

## 🎓 Learning Resources

This project was built as a learning exercise for Blazor. Key learning points:

### For Backend Developers New to Blazor:
- Blazor uses C# for both frontend and backend
- Components (`.razor` files) combine markup and logic
- `@` symbol is used for C# expressions in markup
- State management differs from traditional backend patterns
- SignalR handles real-time UI updates automatically

### Recommended Next Steps:
1. [Official Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
2. [MudBlazor Component Library](https://mudblazor.com/)
3. [Blazor University](https://blazor-university.com/)

## 🤝 Contributing

Contributions are welcome! Here's how:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Code Style Guidelines:
- Follow C# naming conventions (PascalCase for classes/methods, camelCase for local variables)
- Add XML documentation comments for public APIs
- Write unit tests for new scoring logic
- Keep components focused and single-purpose

## 🐛 Troubleshooting

### Common Issues:

**Port Already in Use:**
```bash
# Kill process on port 5027
lsof -ti:5027 | xargs kill -9
```

**Build Errors:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore --force
dotnet build
```

**SignalR Connection Issues:**
- Ensure you're using a modern browser
- Check browser console for errors
- Verify firewall isn't blocking WebSocket connections

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- UI components by [MudBlazor](https://mudblazor.com/)
- Inspired by the classic Yatzy dice game
- Christmas theme for festive fun! 🎄

## 📧 Contact

Henrik Crepido - [@henrikcrepido](https://github.com/henrikcrepido)

Project Link: [https://github.com/henrikcrepido/julspelet-demo](https://github.com/henrikcrepido/julspelet-demo)

---

**Happy Holidays and Happy Coding!** 🎅🎄🎲
