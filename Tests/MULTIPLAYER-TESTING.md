# Multiplayer Testing Guide

This guide covers how to test the multiplayer and networking features of Julspelet.

## Table of Contents

- [Overview](#overview)
- [Test Environment Setup](#test-environment-setup)
- [Testing SignalR Web Multiplayer](#testing-signalr-web-multiplayer)
- [Testing MAUI P2P Multiplayer](#testing-maui-p2p-multiplayer)
- [Cross-Platform Testing](#cross-platform-testing)
- [Automated Tests](#automated-tests)
- [Common Issues](#common-issues)

---

## Overview

Julspelet supports two network modes:

1. **SignalR (Web)**: For browser-based multiplayer using SignalR relay server
2. **Socket (P2P)**: For local network multiplayer on MAUI (Android/iOS/Windows/macOS)

---

## Test Environment Setup

### Prerequisites

- **Web Testing**: Modern web browser (Chrome, Edge, Firefox, Safari)
- **MAUI Testing**: Physical devices or emulators with network capabilities
- **Development Server**: .NET 8 SDK installed

### Starting the Web Server

```bash
cd /workspaces/julspelet-demo
dotnet run --project Julspelet.csproj
```

The server will start on `http://localhost:5027` (or `http://0.0.0.0:5027` in dev container)

---

## Testing SignalR Web Multiplayer

### Test Scenario 1: Create and Join Session

**Steps**:

1. Open browser tab 1 - Navigate to `/session-browser`
2. Click "Host New Session" tab
3. Enter host name: "Player1"
4. Set max players: 4
5. Click "Create Session"
6. Note the 6-digit session code displayed

7. Open browser tab 2 (incognito/private mode recommended)
8. Navigate to `/session-browser`
9. Click "Join by Code" tab
10. Enter the session code from step 6
11. Enter player name: "Player2"
12. Click "Join Session"

**Expected Results**:
- ✅ Player1 sees Player2 join in the lobby
- ✅ Both players see each other in the players list
- ✅ Player1 (host) sees "Start Game" button
- ✅ Player2 sees "Ready" button

### Test Scenario 2: Player Ready State

**Steps**:

1. Continue from Scenario 1
2. As Player2, click "Ready" button
3. Observe lobby updates
4. Click "Ready" again to unready

**Expected Results**:
- ✅ Player2's ready status shows as "Ready" with green indicator
- ✅ Player1 sees Player2's ready status update in real-time
- ✅ Clicking again toggles back to "Not Ready"

### Test Scenario 3: Start Game

**Steps**:

1. Continue from Scenario 2
2. As Player2, click "Ready"
3. As Player1, select game mode (Classic or Tournament)
4. Click "Start Game"

**Expected Results**:
- ✅ Both players navigate to `/game` page
- ✅ Game initializes with both players
- ✅ Turn order is established
- ✅ First player can roll dice

### Test Scenario 4: Disconnect Handling

**Steps**:

1. Set up a game with 2+ players
2. Close one player's browser tab/window
3. Observe remaining players' screens

**Expected Results**:
- ✅ Disconnected player removed from session within 30 seconds
- ✅ Remaining players see notification
- ✅ Game continues if minimum players remain
- ✅ Game ends gracefully if too few players

### Test Scenario 5: Concurrent Sessions

**Steps**:

1. Create Session A with Player1 and Player2
2. Create Session B with Player3 and Player4 (different browsers/devices)
3. Start both games simultaneously
4. Play turns in each game

**Expected Results**:
- ✅ Sessions remain isolated
- ✅ No cross-session message leakage
- ✅ Both games operate independently
- ✅ Server handles concurrent SignalR connections

---

## Testing MAUI P2P Multiplayer

### Test Scenario 1: Local Network Discovery

**Requirements**: 2+ devices on same Wi-Fi network

**Steps**:

1. Device 1 - Open MAUI app
2. Navigate to "Multiplayer" → "Session Browser"
3. Click "Host New Session"
4. Enter host name and click "Create"
5. App starts listening on UDP port 47777

6. Device 2 - Open MAUI app
7. Navigate to "Session Browser"
8. Click "Discover Sessions" tab
9. Wait for discovery (up to 5 seconds)

**Expected Results**:
- ✅ Device 2 discovers Device 1's session
- ✅ Session shows host name, player count, and local IP
- ✅ Device 2 can click "Join" to connect

### Test Scenario 2: Direct Connection

**Steps**:

1. Continue from Scenario 1
2. Device 2 clicks "Join" on discovered session
3. TCP connection established to host on port 47778

**Expected Results**:
- ✅ Connection established within 2 seconds
- ✅ Device 1 shows Device 2 joined
- ✅ Both devices see player list
- ✅ SignalR indicators show "Local Network" mode

### Test Scenario 3: Message Relay

**Steps**:

1. Continue from Scenario 2
2. Device 2 clicks "Ready"
3. Device 1 starts game
4. Device 1 rolls dice

**Expected Results**:
- ✅ Ready status relayed through host to all clients
- ✅ Game start message broadcast to all
- ✅ Dice roll visible on all devices in real-time
- ✅ Turn progression synchronized

### Test Scenario 4: Connection Resilience

**Steps**:

1. Set up 3-device game
2. Turn off Wi-Fi on one client device
3. Observe host and remaining client

**Expected Results**:
- ✅ Host detects disconnection within 10 seconds
- ✅ Remaining devices notified
- ✅ Game continues with remaining players
- ✅ No crashes or hanging connections

### Test Scenario 5: Network Switch

**Steps**:

1. Host creates session on Wi-Fi A
2. Client joins on Wi-Fi A
3. Host switches to Wi-Fi B (different network)

**Expected Results**:
- ✅ Connection drops gracefully
- ✅ Client sees "Disconnected" status
- ✅ No app crashes
- ✅ Client can return to session browser

---

## Cross-Platform Testing

### Web ↔ Mobile Compatibility

While direct Web ↔ Mobile P2P is not supported, test session isolation:

**Steps**:

1. Create session on Web (SignalR)
2. Create session on MAUI (Socket)
3. Verify sessions don't interfere

**Expected Results**:
- ✅ Sessions remain separate
- ✅ No network port conflicts
- ✅ Each uses appropriate transport

### Platform-Specific Features

| Feature | Web (SignalR) | MAUI (P2P) |
|---------|--------------|------------|
| Session Discovery | Session Code | UDP Broadcast |
| Connection | WebSocket | TCP Socket |
| Relay Required | Yes (Server) | No (Direct) |
| Internet Required | Yes | No (LAN only) |
| Max Players | Unlimited* | Limited by host |

*Server capacity dependent

---

## Automated Tests

### Running Integration Tests

```bash
# Run all multiplayer tests
dotnet test --filter "FullyQualifiedName~MultiplayerIntegrationTests"

# Run specific test
dotnet test --filter "FullyQualifiedName~MessageAuthenticator_SignAndVerify"
```

### Key Test Coverage

✅ **Message Serialization**: Polymorphic JSON serialization  
✅ **Message Authentication**: HMAC-SHA256 signing and verification  
✅ **Message Validation**: Timing, rate limiting, dice roll validation  
✅ **GameSession Models**: Session creation and management  
✅ **PeerInfo Models**: Peer connection state  

### Coverage Areas

- ✅ Network message types (11 types)
- ✅ Anti-cheat validation (dice rolls, scores)
- ✅ Rate limiting (10 msg/sec max)
- ✅ Replay attack prevention (30-second window)
- ✅ Session lifecycle management

---

## Common Issues

### Issue: Session Not Found

**Symptoms**: "Session not found" error when joining

**Causes**:
- Session code typo
- Session expired (no activity for 5+ minutes)
- Host disconnected

**Solutions**:
- Verify session code carefully
- Have host check session is still active
- Create new session if needed

### Issue: Connection Timeout

**Symptoms**: "Connection timeout" when joining

**Causes**:
- Firewall blocking ports 47777/47778 (MAUI)
- Different network subnets
- SignalR WebSocket blocked (Web)

**Solutions**:
- Check firewall settings
- Ensure devices on same Wi-Fi network (MAUI)
- Try different network or hotspot
- Check browser console for WebSocket errors (Web)

### Issue: Player Not Syncing

**Symptoms**: Actions from one player not visible to others

**Causes**:
- SignalR connection dropped
- Message validation failing
- Network latency

**Solutions**:
- Check browser/app console for errors
- Verify all players connected (check player list)
- Refresh/rejoin session
- Check network connection quality

### Issue: Game Won't Start

**Symptoms**: "Start Game" button doesn't work

**Causes**:
- Minimum player count not met
- Not all players ready (if required)
- Only host can start game

**Solutions**:
- Ensure at least 2 players joined
- Have all non-host players click "Ready"
- Verify you are the host

### Issue: Discovery Not Finding Sessions (MAUI)

**Symptoms**: No sessions appear in discovery list

**Causes**:
- Devices on different networks
- UDP port 47777 blocked
- Host not actually hosting

**Solutions**:
- Confirm same Wi-Fi network
- Check router/firewall UDP settings
- Try "Join by IP" instead
- Restart both apps

### Issue: Rate Limit Exceeded

**Symptoms**: "Rate limit exceeded" errors in console

**Causes**:
- Sending too many messages too quickly (>10/sec)
- Rapid clicking or automation

**Solutions**:
- Wait 1 second before retrying
- Don't spam buttons
- Rate limiting protects against cheating

---

## Testing Checklist

### Pre-Release Testing

- [ ] Web: 2-player game start to finish
- [ ] Web: 4-player game start to finish
- [ ] Web: Player disconnect mid-game
- [ ] Web: Host disconnect mid-game
- [ ] Web: Concurrent sessions (3+ simultaneous)
- [ ] MAUI: Session discovery on LAN
- [ ] MAUI: Direct TCP connection
- [ ] MAUI: 2-device game completion
- [ ] MAUI: Network disconnect handling
- [ ] Cross: Web and MAUI don't interfere
- [ ] Security: Invalid dice rolls rejected
- [ ] Security: Invalid scores rejected
- [ ] Security: Rate limiting enforced
- [ ] Security: Old messages rejected

### Performance Testing

- [ ] 10+ concurrent web sessions
- [ ] Message latency < 100ms on LAN
- [ ] No memory leaks during long games
- [ ] Smooth dice animations on all devices

---

## Advanced Testing

### Load Testing (Web)

```bash
# Simulate 100 concurrent connections
# Requires custom load testing tool
```

### Packet Capture (MAUI)

Use Wireshark to inspect UDP/TCP traffic:
- Filter: `udp.port == 47777 or tcp.port == 47778`
- Verify message format and frequency
- Check for unnecessary retransmissions

### Security Testing

- Attempt to send messages from wrong player ID
- Try to submit invalid dice values (7, 8, 9)
- Send messages with old timestamps
- Exceed rate limits intentionally

All should be rejected by validation layer.

---

## Reporting Issues

When reporting multiplayer bugs, include:

1. **Platform**: Web (browser + version) or MAUI (device + OS)
2. **Network Mode**: SignalR or Socket P2P
3. **Steps to Reproduce**: Detailed sequence
4. **Expected vs Actual**: What should vs did happen
5. **Console Logs**: Browser console or app logs
6. **Network Info**: Same network? Firewalls?
7. **Session Info**: Number of players, who is host

---

**Last Updated**: December 5, 2025  
**Version**: Phase 9.3
