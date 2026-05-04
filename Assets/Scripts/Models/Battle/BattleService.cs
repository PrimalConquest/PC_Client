using BattleComunication;
using Microsoft.AspNetCore.SignalR.Client;
using PrimalConquest.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleService : MonoBehaviour
{
    public UnityEvent<int>          OnPlayerIndex;
    public UnityEvent<GameStateDTO> OnGameSetup;
    public UnityEvent<GameStateDTO> OnGameState;
    public UnityEvent<string>       OnError;
    public UnityEvent               OnConnected;

    HubConnection _connection;

    public async Task ConnectAsync()
    {
        var token = AuthSession.AccessToken;
        var url   = $"http://{BattleSession.Ip}:{BattleSession.Port}{Endpoints.BattleHub()}";
        var ctx   = SynchronizationContext.Current;

        _connection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        _connection.On<int>("ReceivePlayerIndex",
            idx   => ctx.Post(_ => OnPlayerIndex.Invoke(idx), null));

        _connection.On<GameStateDTO>("ReceiveGameSetup",
            state => ctx.Post(_ => OnGameSetup.Invoke(state), null));

        _connection.On<GameStateDTO>("ReceiveGameState",
            state => ctx.Post(_ => OnGameState.Invoke(state), null));

        _connection.On<string>("ReceiveError",
            msg   => ctx.Post(_ => OnError.Invoke(msg), null));

        _connection.Closed += ex =>
        {
            if (ex != null) ctx.Post(_ => OnError.Invoke($"BattleServer connection lost: {ex.Message}"), null);
            return Task.CompletedTask;
        };

        try
        {
            await _connection.StartAsync();
            Debug.Log($"[Battle] Connected to BattleServer at {url}");
            OnConnected.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Battle] Could not connect to BattleServer: {ex.Message}");
            OnError.Invoke($"Could not connect to BattleServer: {ex.Message}");
        }
    }

    public Task SendMoveCommand(int x, int y, int direction) =>
        _connection.InvokeAsync("SendMoveCommand", x, y, direction);

    public Task SendActivateCommand(string unitKey) =>
        _connection.InvokeAsync("SendActivateCommand", unitKey);

    public Task SendPlaceCommand(string unitKey, int x, int y) =>
        _connection.InvokeAsync("SendPlaceCommand", unitKey, x, y);

    public Task SendEndTurn() =>
        _connection.InvokeAsync("SendEndTurn");

    public async Task DisconnectAsync()
    {
        if (_connection == null) return;
        try   { await _connection.StopAsync(); }
        catch { }
        await _connection.DisposeAsync();
        _connection = null;
    }

    async void OnDestroy() => await DisconnectAsync();
}
