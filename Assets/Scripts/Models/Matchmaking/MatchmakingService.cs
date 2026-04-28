using MatchmakingComunication;
using Microsoft.AspNetCore.SignalR.Client;
using PrimalConquest.Auth;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MatchmakingService : MonoBehaviour, IMatchmakingClient
{
    public string BattleServerIp   { get; private set; } = "";
    public int    BattleServerPort { get; private set; }

    HubConnection _connection;

    [SerializeField] string _gameMenuSceneName = "GameMenu";

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

        var token = AuthSession.AccessToken;

        _connection = new HubConnectionBuilder()
            .WithUrl(AuthConfig.BaseUrl + Endpoints.MatchmakingHub(), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        _connection.On<int>        (nameof(IMatchmakingClient.QueueJoined),     QueueJoined);
        _connection.On             (nameof(IMatchmakingClient.QueueLeft),        QueueLeft);
        _connection.On<string, int>(nameof(IMatchmakingClient.MatchFound),       MatchFound);
        _connection.On<string>     (nameof(IMatchmakingClient.MatchmakingError), MatchmakingError);

        _connection.Closed += ex =>
        {
            if (ex != null) OnError.Invoke($"Connection lost: {ex.Message}");
            return Task.CompletedTask;
        };

        _inQueue = false;

        try
        {
            await _connection.StartAsync();
            OnMessage.Invoke("Joining queue....");
            await _connection.InvokeAsync(nameof(IMatchmakingHub.JoinQueue));
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
        OnQueueJoined.Invoke(position);
        _inQueue = true;
        return Task.CompletedTask;
    }

    public Task QueueLeft()
    {
        OnMessage.Invoke("Going back to menu...");
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
