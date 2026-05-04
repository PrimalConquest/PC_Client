using MatchmakingComunication;
using Microsoft.AspNetCore.SignalR.Client;
using PrimalConquest.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MatchmakingService : MonoBehaviour, IMatchmakingClient
{
    public string BattleServerIp   { get; private set; } = "";
    public int    BattleServerPort { get; private set; }

    HubConnection _connection;

    [SerializeField] string _gameMenuSceneName  = "GameMenu";
    [SerializeField] string _battleSceneName    = "Battle";

    public UnityEvent<int>         OnQueueJoined;
    public UnityEvent<string, int> OnMatchFound;
    public UnityEvent<string> OnMessage;
    public UnityEvent<string>      OnError;

    bool _inQueue;

    // ── Public API ─────────────────────────────────────────────────────────────

    public async void Init()
    {
        if (_connection != null)
            await DisconnectAsync();

        var token      = AuthSession.AccessToken;
        var mainThread = SynchronizationContext.Current;

        _connection = new HubConnectionBuilder()
            .WithUrl(AuthConfig.BaseUrl + Endpoints.MatchmakingHub(), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        _connection.On<int>        (nameof(IMatchmakingClient.QueueJoined),     pos        => mainThread.Post(_ => QueueJoined(pos),        null));
        _connection.On             (nameof(IMatchmakingClient.QueueLeft),        ()         => mainThread.Post(_ => QueueLeft(),              null));
        _connection.On<string, int>(nameof(IMatchmakingClient.MatchFound),       (ip, port) => mainThread.Post(_ => MatchFound(ip, port),    null));
        _connection.On<string>     (nameof(IMatchmakingClient.MatchmakingError), msg        => mainThread.Post(_ => MatchmakingError(msg),    null));

        _connection.Closed += ex =>
        {
            if (ex != null) OnError.Invoke($"Connection lost: {ex.Message}");
            return Task.CompletedTask;
        };

        _inQueue = false;

        try
        {
            await _connection.StartAsync();
            Debug.Log("[Matchmaking] Connected, invoking JoinQueue...");
            OnMessage.Invoke("Joining queue....");
            await _connection.InvokeAsync(nameof(IMatchmakingHub.JoinQueue));
            Debug.Log("[Matchmaking] JoinQueue returned — waiting for server callback");
        }
        catch (Exception ex)
        {
            OnError.Invoke($"Could not connect to matchmaking: {ex.Message}");
            await DisconnectAsync();
        }
    }

    public async void LeaveQueueAsync()
    {
        if (_connection == null)
        {
            OnError.Invoke("Could not leave queue — no active connection.");
            return;
        }
        try   {
            if(_inQueue)
            {
                OnMessage.Invoke("Leaving queue....");
                await _connection.InvokeAsync(nameof(IMatchmakingHub.LeaveQueue));
            }
            else
            {
                await QueueLeft();
            }
        }
        catch { }
    }

    // ── IMatchmakingClient ─────────────────────────────────────────────────────

    public Task QueueJoined(int position)
    {
        Debug.Log($"[Matchmaking] QueueJoined pos={position}");
        OnQueueJoined.Invoke(position);
        _inQueue = true;
        return Task.CompletedTask;
    }

    public Task QueueLeft()
    {
        Debug.Log("[Matchmaking] QueueLeft");
        OnMessage.Invoke("Going back to menu...");
        SceneManager.LoadScene(_gameMenuSceneName);
        return Task.CompletedTask;
    }

    public Task MatchFound(string serverIp, int serverPort)
    {
        Debug.Log($"[Matchmaking] MatchFound {serverIp}:{serverPort}");
        BattleServerIp   = serverIp;
        BattleServerPort = serverPort;
        BattleSession.Set(serverIp, serverPort);
        OnMatchFound.Invoke(serverIp, serverPort);
        SceneManager.LoadScene(_battleSceneName);
        return Task.CompletedTask;
    }

    public Task MatchmakingError(string message)
    {
        Debug.LogError($"[Matchmaking] MatchmakingError: {message}");
        OnError.Invoke(message);
        return Task.CompletedTask;
    }

    // ── Internal ───────────────────────────────────────────────────────────────

    async Task DisconnectAsync()
    {
        if (_connection == null)
        {
            OnError.Invoke("Could not disconnect fro matchmaking — no active connection.");
            return;
        }
        try   { await _connection.StopAsync(); }
        catch (Exception ex) 
        { 
            OnError.Invoke($"Could not stop matchmaking connection: {ex.Message}"); 
        }
        await _connection.DisposeAsync();
        _connection = null;

    }

    async void OnDestroy() => await DisconnectAsync();
}
