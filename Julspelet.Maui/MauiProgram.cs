using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Julspelet.Shared.Services;

namespace Julspelet.Maui;

/// <summary>
/// Main entry point for the MAUI Blazor Hybrid application.
/// This configures the app with dependency injection, services, and platform-specific features.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor WebView services
        // This enables hosting Blazor components in a native WebView control
        builder.Services.AddMauiBlazorWebView();

        // Add MudBlazor services for UI components
        builder.Services.AddMudServices();

        // Add game services
        // In MAUI, we use Singleton for state management since each app instance is single-user
        // This differs from Blazor Server where we use Scoped per connection
        builder.Services.AddSingleton<TournamentService>();
        builder.Services.AddSingleton<ScoringService>();
        builder.Services.AddSingleton<GameService>();

#if DEBUG
        // Enable debug logging in development
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
