using MatchmakingComunication;
using Microsoft.AspNetCore.SignalR.Client;
using PrimalConquest.Auth;
using System;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Implements IMatchmakingClient so the compiler enforces correct method signatures.
// _connection.On() still requires a name string; nameof() keeps it refactor-safe.
public class MatchmakingService : MonoBehaviour, IMatchmakingClient
{
    //public static MatchmakingService Instance { get; private set; }

    public string BattleServerIp   { get; private set; } = "";
    public int    BattleServerPort { get; private set; }

    HubConnection _connection;


    [SerializeField] string _gameMenuSceneName = "GameMenu";

    public UnityEvent<int> OnQueueJoined;
    public UnityEvent<string, int> OnMatchFound;
    public UnityEvent<string> OnError;

    public async void Init()
    {
        //Instance = this;
        await ConnectAndJoinAsync();
    }

    // ── IMatchmakingClient ─────────────────────────────────────────────────────
    // These are the real implementations — registered below via nameof().

    public Task QueueJoined(int position)
    {
        OnQueueJoined.Invoke(position);
        return Task.CompletedTask;
    }

    public Task QueueLeft()
    {
        OnError.Invoke(LocalizedString.Get("Leaving Queue..."));
        SceneManager.LoadScene(_gameMenuSceneName);
        return Task.CompletedTask;
    }

    public Task MatchFound(string serverIp, int serverPort)
    {
        BattleServerIp   = serverIp;
        BattleServerPort = serverPort;
        OnMatchFound.Invoke(serverIp, serverPort);
        return Task.CompletedTask;
    }

    public Task MatchmakingError(string message)
    {
        OnError.Invoke(message);
        return Task.CompletedTask;
    }

    // ── Connection lifecycle ───────────────────────────────────────────────────

    public async Task ConnectAndJoinAsync()
    {
        if (_connection != null)
            await DisconnectAsync();

        _connection = new HubConnectionBuilder()
            .WithUrl(AuthConfig.BaseUrl + Endpoints.MatchmakingHub(), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(AuthSession.AccessToken);
            })
            .Build();

        _connection.On<int>         (nameof(IMatchmakingClient.QueueJoined),      QueueJoined);
        _connection.On              (nameof(IMatchmakingClient.QueueLeft),         QueueLeft);
        _connection.On<string, int> (nameof(IMatchmakingClient.MatchFound),        MatchFound); 
        _connection.On<string>      (nameof(IMatchmakingClient.MatchmakingError),  MatchmakingError);

        _connection.Closed += ex =>
        {
            if (ex != null) OnError?.Invoke($"Connection lost: {ex.Message}");
            return Task.CompletedTask;
        };

        try
        {
            await _connection.StartAsync();
            await _connection.InvokeAsync(nameof(IMatchmakingHub.JoinQueue));
        }
        catch (Exception ex)
        {
            OnError?.Invoke($"Could not connect to matchmaking: {ex.Message}");
            await DisconnectAsync();
        }
    }

    public async Task LeaveQueueAsync()
    {
        if (_connection == null) return;
        try   { await _connection.InvokeAsync(nameof(IMatchmakingHub.LeaveQueue)); }
        catch { }
        await DisconnectAsync();
    }

    async Task DisconnectAsync()
    {
        if (_connection == null) return;
        try   { await _connection.StopAsync(); }
        catch { }
        await _connection.DisposeAsync();
        _connection = null;
    }

    async void OnDestroy() => await DisconnectAsync();
}
