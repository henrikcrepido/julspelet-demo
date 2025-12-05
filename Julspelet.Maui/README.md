# Julspelet MAUI - Build Instructions

## Overview
This is the .NET MAUI Blazor Hybrid project for Julspelet, enabling the game to run on multiple platforms including Android, iOS, Windows, and macOS.

## Prerequisites

### For Android Development
- Android SDK (API 21 or higher)
- Android Emulator or physical device
- Java Development Kit (JDK) 11 or higher

### For iOS/macOS Development
- macOS with Xcode 14 or higher
- iOS Simulator or physical device
- Apple Developer account (for device deployment)

### For Windows Development
- Windows 10/11 with Windows SDK
- Visual Studio 2022 with MAUI workload

## Building the Project

### Install .NET MAUI Workload
```bash
dotnet workload install maui
```

### Build for Specific Platforms

#### Android
```bash
dotnet build -f net8.0-android
```

#### iOS (requires macOS)
```bash
dotnet build -f net8.0-ios
```

#### Windows
```bash
dotnet build -f net8.0-windows10.0.19041.0
```

#### macOS Catalyst
```bash
dotnet build -f net8.0-maccatalyst
```

## Running the Application

### Android
```bash
dotnet run -f net8.0-android
```

### iOS Simulator (macOS only)
```bash
dotnet run -f net8.0-ios
```

## Dev Container Limitation

**Note**: The current dev container environment (Linux) does not support building MAUI projects as it lacks the platform-specific SDKs. To build and test the MAUI project:

1. Clone the repository on a machine with the appropriate platform SDK
2. Install the .NET MAUI workload
3. Build and run for your target platform

## Project Structure

- `Components/` - MAUI-specific Blazor components
- `Platforms/` - Platform-specific code (Android, iOS, Windows, macOS)
- `Resources/` - App icons, splash screens, fonts, and styles
- `wwwroot/` - Static web assets for the Blazor WebView
- `MauiProgram.cs` - Application entry point and DI configuration
- `MainPage.xaml` - Main page hosting the BlazorWebView

## Features

- **Shared Components**: Uses components from Julspelet.Shared library
- **P2P Networking**: Supports peer-to-peer multiplayer via local network (sockets) or internet (SignalR relay)
- **Offline Support**: Can play locally without internet connection
- **Cross-Platform**: Single codebase runs on Android, iOS, Windows, and macOS
- **Native Performance**: Runs natively with platform-specific optimizations

## Configuration

Service registration and configuration is done in `MauiProgram.cs`. Network services are configured to use:
- **Socket-based networking** for local network P2P (default for MAUI)
- **SignalR networking** as fallback for internet-based games

## Troubleshooting

### Build Errors
- Ensure all workloads are installed: `dotnet workload list`
- Restore packages: `dotnet restore`
- Clean and rebuild: `dotnet clean && dotnet build`

### Platform-Specific Issues
- **Android**: Check Android SDK path in environment variables
- **iOS**: Ensure Xcode command line tools are installed
- **Windows**: Verify Windows SDK installation

## Documentation

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Blazor Hybrid Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/)
- [MudBlazor Documentation](https://mudblazor.com/)
