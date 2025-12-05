# Multiplayer Features

Julspelet supports multiplayer Yatzy games with friends on both web and mobile platforms!

## Overview

Play Yatzy with 2 or more players using two different networking modes:

- **Web Multiplayer** (SignalR): Play from any web browser, anywhere in the world
- **Local Multiplayer** (P2P): Play with friends on the same Wi-Fi network using mobile devices

---

## Web Multiplayer (SignalR)

### How It Works

Web multiplayer uses SignalR technology to relay messages through a central server. This allows you to play with friends anywhere with an internet connection.

### Getting Started

1. **Host a Game**
   - Navigate to "Multiplayer" from the main menu
   - Click "Host New Session"
   - Enter your name
   - Set maximum number of players (2-8)
   - Click "Create Session"
   - Share the 6-digit session code with friends

2. **Join a Game**
   - Navigate to "Multiplayer"  
   - Click "Join by Code"
   - Enter the session code from your friend
   - Enter your name
   - Click "Join Session"

3. **Ready Up & Play**
   - Once in the lobby, click "Ready" when you're set
   - Host can start the game when everyone is ready
   - Take turns rolling dice and scoring points!

### Features

- ✅ Play from anywhere with internet
- ✅ Easy to join with 6-digit code
- ✅ See who's in the lobby in real-time
- ✅ Ready status for all players
- ✅ Host controls game start and settings
- ✅ Automatic reconnection if disconnected briefly

---

## Local Multiplayer (MAUI P2P)

> **Note**: This feature is available on mobile devices (Android, iOS) and desktop (Windows, macOS) apps

### How It Works

Local multiplayer uses peer-to-peer networking over your Wi-Fi network. No internet connection required - just the same local network!

### Getting Started

1. **Host a Game**
   - Open the Julspelet app on your device
   - Navigate to "Multiplayer" → "Session Browser"
   - Click "Host New Session"
   - Enter your name
   - Click "Create Session"
   - Your device starts broadcasting on the network

2. **Join a Game**
   - Open the Julspelet app on another device (same Wi-Fi)
   - Navigate to "Multiplayer" → "Session Browser"
   - Click "Discover Sessions"
   - Select the session you want to join
   - Enter your name
   - Click "Join"

3. **Play!**
   - Click "Ready" when you're set
   - Host starts the game
   - Take turns and enjoy!

### Features

- ✅ No internet required
- ✅ Automatic session discovery
- ✅ Direct device-to-device communication
- ✅ Low latency gameplay
- ✅ Privacy - data stays on your local network
- ✅ Works on same Wi-Fi network

### Requirements

- All devices must be on the same Wi-Fi network
- Firewall must allow:
  - UDP port 47777 (discovery)
  - TCP port 47778 (game communication)

---

## Game Modes

### Classic Mode

Traditional Yatzy scoring with all 15 categories. First player to complete their scorecard wins!

### Tournament Mode

Play bracket-style tournaments with multiple rounds. Compete to be the ultimate Yatzy champion!

---

## Gameplay

### Turn Order

- Players take turns in order
- Each turn consists of up to 3 dice rolls
- After rolling, select a category to score
- Turn automatically passes to next player

### Rolling Dice

1. Click "Roll" to roll all unheld dice
2. Click individual dice to hold/unhold them
3. Roll up to 2 more times (3 rolls total per turn)
4. Must score a category after final roll

### Scoring

- Click a category button to score current dice
- Score is automatically calculated
- Category becomes unavailable after use
- Bonus awarded for upper section scores ≥ 63

### Winning

- Game ends when all players complete their scorecards
- Player with highest total score wins!
- Ties are possible - all tied players share victory

---

## Tips for Best Experience

### Connection Tips

- **Web**: Ensure stable internet connection
- **Mobile**: Stay on same Wi-Fi network throughout game
- **Mobile**: Keep devices reasonably close to Wi-Fi router
- **All**: Avoid switching networks mid-game

### Gameplay Tips

- 👥 Wait for all players to join before starting
- ✅ Use "Ready" button to signal you're ready to play
- 🎯 Plan your strategy - you have 3 rolls per turn
- 🤝 Be patient - wait for other players' turns
- 📱 Keep app in foreground during mobile play

### Troubleshooting

**Can't find session?**
- Verify all devices on same Wi-Fi (local multiplayer)
- Check session code is correct (web multiplayer)
- Try refreshing the session browser

**Connection lost?**
- Check internet/Wi-Fi connection
- Rejoin using same session code
- Host may need to recreate session if gone too long

**Game won't start?**
- Ensure minimum 2 players joined
- All non-host players should click "Ready"
- Only host can start the game

---

## Security & Fair Play

Julspelet includes several anti-cheat measures:

- ✅ **Server validation**: All game actions validated by host/server
- ✅ **Message authentication**: Cryptographic signing prevents tampering
- ✅ **Rate limiting**: Prevents spam and rapid-fire exploits
- ✅ **Dice validation**: Ensures fair random rolls
- ✅ **Score verification**: Validates scores match actual dice values

Play fair and have fun! 🎲

---

## Privacy

- **Web**: Session data stored temporarily on server, deleted after game ends
- **Local**: All data stays on your devices, never leaves local network
- **Both**: No personal information collected or stored
- **Both**: Player names only visible to session participants

---

## Compatibility

| Platform | Web Multiplayer | Local Multiplayer |
|----------|----------------|-------------------|
| Web Browser | ✅ Yes | ❌ No |
| Android | ✅ Yes* | ✅ Yes |
| iOS | ✅ Yes* | ✅ Yes |
| Windows | ✅ Yes | ✅ Yes |
| macOS | ✅ Yes | ✅ Yes |

*Requires internet connection for web multiplayer

### Cross-Platform Play

- ✅ Web ↔ Web (same mode)
- ✅ Mobile ↔ Mobile on same platform (local mode)
- ✅ Android ↔ iOS (local mode, same Wi-Fi)
- ✅ Desktop ↔ Mobile (local mode, same Wi-Fi)
- ❌ Web ↔ Mobile local (different network modes)

---

## Technical Details

### Network Ports

**Local Multiplayer**:
- UDP 47777: Session discovery broadcasts
- TCP 47778: Game communication

**Web Multiplayer**:
- HTTPS/WSS: Standard web ports
- SignalR WebSocket connection

### Message Types

The game exchanges these message types:
- Player join/leave notifications
- Dice roll updates
- Score selections
- Turn changes
- Game start/end events
- Chat messages (future feature)
- Heartbeat/keepalive

### Session Lifecycle

1. **Creation**: Host creates session, gets unique ID
2. **Discovery**: Other players find or join by code
3. **Lobby**: Players ready up, host configures settings
4. **Active**: Game in progress, turns taken
5. **Complete**: Game ends, scores displayed
6. **Cleanup**: Session deleted after inactivity

Sessions automatically expire after 5 minutes of inactivity.

---

## FAQ

**Q: How many players can join?**  
A: 2-8 players for web, 2-6 recommended for local (depends on host device)

**Q: Do I need internet for local multiplayer?**  
A: No! Only need same Wi-Fi network. Internet not required.

**Q: Can I play web multiplayer on mobile?**  
A: Yes, open web browser on mobile and navigate to the game URL.

**Q: What happens if host disconnects?**  
A: Game will end. We recommend host has most stable connection.

**Q: Can spectators watch?**  
A: Not currently supported. Feature planned for future release.

**Q: Is voice chat supported?**  
A: Not built-in. Use external voice chat app if desired.

**Q: Can I save and resume games?**  
A: Not currently. Games must be completed in one session.

**Q: Does it work on mobile data?**  
A: Web multiplayer works on mobile data. Local requires Wi-Fi.

---

## Feedback

Having issues or suggestions? Please report them on our [GitHub repository](https://github.com/henrikcrepido/julspelet-demo/issues).

---

**Happy Playing! 🎉🎲**
