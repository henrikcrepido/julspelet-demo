using Julspelet.Components;
using Julspelet.Shared.Services;
using Julspelet.Shared.Services.Networking;
using Julspelet.Hubs;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add SignalR for multiplayer networking
builder.Services.AddSignalR();

// Add game services
// Using Singleton for TournamentService to share state across all connections
// Using Scoped lifetime for Blazor Server to maintain state per user connection
builder.Services.AddSingleton<TournamentService>();
builder.Services.AddScoped<ScoringService>();
builder.Services.AddScoped<GameService>();

// Add networking services for multiplayer
// SignalRNetworkService for web-based P2P networking
builder.Services.AddScoped<INetworkService, SignalRNetworkService>();
builder.Services.AddScoped<IGameSyncService, GameSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map SignalR hub for multiplayer game sessions
app.MapHub<GameHub>("/gamehub");

app.Run();
