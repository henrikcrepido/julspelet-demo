using Julspelet.Shared.Models;

namespace Julspelet.Shared.Services;

/// <summary>
/// Service for managing Yatzy tournaments.
/// Handles tournament creation, player registration, bracket generation, and match progression.
/// </summary>
public class TournamentService
{
    private readonly List<Tournament> _tournaments = new();
    
    /// <summary>
    /// Event raised when a tournament is created.
    /// </summary>
    public event Action<Tournament>? TournamentCreated;
    
    /// <summary>
    /// Event raised when a player joins a tournament.
    /// </summary>
    public event Action<Tournament, Player>? PlayerJoined;
    
    /// <summary>
    /// Event raised when a tournament starts.
    /// </summary>
    public event Action<Tournament>? TournamentStarted;
    
    /// <summary>
    /// Event raised when a match is completed.
    /// </summary>
    public event Action<Tournament, Match>? MatchCompleted;
    
    /// <summary>
    /// Event raised when a tournament round advances.
    /// </summary>
    public event Action<Tournament, TournamentRound>? RoundAdvanced;
    
    /// <summary>
    /// Event raised when a tournament is completed.
    /// </summary>
    public event Action<Tournament>? TournamentCompleted;

    public TournamentService()
    {
    }

    /// <summary>
    /// Creates a new tournament.
    /// </summary>
    /// <param name="tournamentName">Name of the tournament.</param>
    /// <param name="gameMasterName">Name of the game master.</param>
    /// <param name="maxPlayers">Maximum number of players (default: 8).</param>
    /// <returns>The created tournament.</returns>
    public Tournament CreateTournament(string tournamentName, string gameMasterName, int maxPlayers = 8)
    {
        // Validate max players (must be power of 2 and >= 4)
        if (maxPlayers < 4 || !IsPowerOfTwo(maxPlayers))
        {
            throw new ArgumentException("Max players must be a power of 2 and at least 4 (e.g., 4, 8, 16)");
        }

        var tournament = new Tournament
        {
            Name = tournamentName,
            GameMaster = new GameMaster { Name = gameMasterName },
            MaxPlayers = maxPlayers,
            MinPlayers = maxPlayers / 2 // Allow starting with half capacity
        };

        _tournaments.Add(tournament);
        TournamentCreated?.Invoke(tournament);
        
        return tournament;
    }

    /// <summary>
    /// Gets all available tournaments (not started).
    /// </summary>
    public List<Tournament> GetAvailableTournaments()
    {
        return _tournaments.Where(t => t.IsAcceptingPlayers).ToList();
    }

    /// <summary>
    /// Gets all tournaments.
    /// </summary>
    public List<Tournament> GetAllTournaments()
    {
        return _tournaments.ToList();
    }

    /// <summary>
    /// Gets a specific tournament by ID.
    /// </summary>
    public Tournament? GetTournament(string tournamentId)
    {
        return _tournaments.FirstOrDefault(t => t.Id == tournamentId);
    }

    /// <summary>
    /// Registers a player for a tournament.
    /// </summary>
    public bool JoinTournament(string tournamentId, string playerName)
    {
        var tournament = GetTournament(tournamentId);
        if (tournament == null || !tournament.IsAcceptingPlayers)
        {
            return false;
        }

        // Check if player name already exists
        if (tournament.RegisteredPlayers.Any(p => p.Name == playerName))
        {
            return false;
        }

        var player = new Player { Name = playerName };
        tournament.RegisteredPlayers.Add(player);
        PlayerJoined?.Invoke(tournament, player);
        
        return true;
    }

    /// <summary>
    /// Starts a tournament and generates the bracket.
    /// </summary>
    public bool StartTournament(string tournamentId)
    {
        var tournament = GetTournament(tournamentId);
        if (tournament == null || !tournament.CanStart)
        {
            return false;
        }

        tournament.HasStarted = true;
        tournament.StartedAt = DateTime.UtcNow;
        
        // Generate bracket based on number of players
        GenerateBracket(tournament);
        
        TournamentStarted?.Invoke(tournament);
        return true;
    }

    /// <summary>
    /// Generates the tournament bracket with matches.
    /// </summary>
    private void GenerateBracket(Tournament tournament)
    {
        var players = tournament.RegisteredPlayers.ToList();
        var playerCount = players.Count;

        // Shuffle players for random seeding
        var random = new Random();
        players = players.OrderBy(p => random.Next()).ToList();

        // Determine starting round based on player count
        TournamentRound startingRound;
        if (playerCount <= 2)
        {
            startingRound = TournamentRound.Final;
        }
        else if (playerCount <= 4)
        {
            startingRound = TournamentRound.SemiFinals;
        }
        else
        {
            startingRound = TournamentRound.QuarterFinals;
        }

        tournament.CurrentRound = startingRound;

        // Create matches for the starting round
        CreateMatchesForRound(tournament, startingRound, players);
    }

    /// <summary>
    /// Creates matches for a specific round.
    /// </summary>
    private void CreateMatchesForRound(Tournament tournament, TournamentRound round, List<Player> players)
    {
        var matchNumber = 1;
        
        // Create matches by pairing players
        for (int i = 0; i < players.Count; i += 2)
        {
            if (i + 1 < players.Count)
            {
                var match = new Match
                {
                    Round = round,
                    MatchNumber = matchNumber++,
                    Players = new List<Player> { players[i], players[i + 1] }
                };
                
                // Initialize game state for this match
                match.GameState = new GameState();
                match.GameState.Players.Add(players[i]);
                match.GameState.Players.Add(players[i + 1]);
                
                tournament.Matches.Add(match);
            }
        }
    }

    /// <summary>
    /// Starts a specific match within a tournament.
    /// </summary>
    public bool StartMatch(string tournamentId, string matchId)
    {
        var tournament = GetTournament(tournamentId);
        var match = tournament?.Matches.FirstOrDefault(m => m.Id == matchId);
        
        if (match == null || match.GameState == null)
        {
            return false;
        }

        match.StartedAt = DateTime.UtcNow;
        
        // Initialize the game state
        match.GameState.CurrentPlayerIndex = 0;
        match.GameState.IsGameStarted = true;
        
        return true;
    }

    /// <summary>
    /// Completes a match and determines the winner.
    /// </summary>
    public bool CompleteMatch(string tournamentId, string matchId)
    {
        var tournament = GetTournament(tournamentId);
        var match = tournament?.Matches.FirstOrDefault(m => m.Id == matchId);
        
        if (match == null || match.GameState == null || match.IsCompleted)
        {
            return false;
        }

        // Determine winner (highest total score)
        var winner = match.Players
            .OrderByDescending(p => p.ScoreCard.GetTotalScore())
            .FirstOrDefault();

        if (winner == null)
        {
            return false;
        }

        match.Winner = winner;
        match.IsCompleted = true;
        match.CompletedAt = DateTime.UtcNow;
        match.GameState.IsGameComplete = true;
        
        if (tournament != null)
        {
            MatchCompleted?.Invoke(tournament, match);
            
            // Check if all matches in current round are completed
            CheckRoundCompletion(tournament);
        }
        
        return true;
    }

    /// <summary>
    /// Checks if all matches in the current round are completed and advances if so.
    /// </summary>
    private void CheckRoundCompletion(Tournament tournament)
    {
        var currentRoundMatches = tournament.Matches
            .Where(m => m.Round == tournament.CurrentRound)
            .ToList();

        if (!currentRoundMatches.All(m => m.IsCompleted))
        {
            return;
        }

        // All matches completed, advance to next round
        AdvanceToNextRound(tournament);
    }

    /// <summary>
    /// Advances the tournament to the next round.
    /// </summary>
    private void AdvanceToNextRound(Tournament tournament)
    {
        var winners = tournament.Matches
            .Where(m => m.Round == tournament.CurrentRound && m.Winner != null)
            .Select(m => m.Winner!)
            .ToList();

        TournamentRound nextRound;
        
        switch (tournament.CurrentRound)
        {
            case TournamentRound.QuarterFinals:
                nextRound = TournamentRound.SemiFinals;
                break;
            case TournamentRound.SemiFinals:
                nextRound = TournamentRound.Final;
                break;
            case TournamentRound.Final:
                nextRound = TournamentRound.Completed;
                CompleteTournament(tournament, winners.First());
                return;
            default:
                return;
        }

        tournament.CurrentRound = nextRound;
        CreateMatchesForRound(tournament, nextRound, winners);
        RoundAdvanced?.Invoke(tournament, nextRound);
    }

    /// <summary>
    /// Completes the tournament.
    /// </summary>
    private void CompleteTournament(Tournament tournament, Player winner)
    {
        tournament.IsCompleted = true;
        tournament.CompletedAt = DateTime.UtcNow;
        tournament.Winner = winner;
        tournament.CurrentRound = TournamentRound.Completed;
        
        TournamentCompleted?.Invoke(tournament);
    }

    /// <summary>
    /// Gets the game state for a specific match.
    /// </summary>
    public GameState? GetMatchGameState(string tournamentId, string matchId)
    {
        var tournament = GetTournament(tournamentId);
        var match = tournament?.Matches.FirstOrDefault(m => m.Id == matchId);
        return match?.GameState;
    }

    /// <summary>
    /// Gets all matches for a specific round.
    /// </summary>
    public List<Match> GetMatchesForRound(string tournamentId, TournamentRound round)
    {
        var tournament = GetTournament(tournamentId);
        return tournament?.Matches.Where(m => m.Round == round).ToList() ?? new List<Match>();
    }

    /// <summary>
    /// Checks if a number is a power of 2.
    /// </summary>
    private bool IsPowerOfTwo(int n)
    {
        return n > 0 && (n & (n - 1)) == 0;
    }

    /// <summary>
    /// Removes a player from a tournament (only before it starts).
    /// </summary>
    public bool RemovePlayer(string tournamentId, string playerId)
    {
        var tournament = GetTournament(tournamentId);
        if (tournament == null || tournament.HasStarted)
        {
            return false;
        }

        var player = tournament.RegisteredPlayers.FirstOrDefault(p => p.Id.ToString() == playerId);
        if (player == null)
        {
            return false;
        }

        tournament.RegisteredPlayers.Remove(player);
        return true;
    }

    /// <summary>
    /// Deletes a tournament (only before it starts).
    /// </summary>
    public bool DeleteTournament(string tournamentId)
    {
        var tournament = GetTournament(tournamentId);
        if (tournament == null || tournament.HasStarted)
        {
            return false;
        }

        _tournaments.Remove(tournament);
        return true;
    }
}
