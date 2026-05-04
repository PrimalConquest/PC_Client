using Microsoft.AspNetCore.SignalR.Client;
using PrimalConquest.Auth;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BattleService : MonoBehaviour
{
    public UnityEvent    OnConnected;
    public UnityEvent<string> OnError;

    HubConnection _connection;

    public async Task ConnectAsync()
    {
        var token = AuthSession.AccessToken;
        var url   = $"http://{BattleSession.Ip}:{BattleSession.Port}{Endpoints.BattleHub()}";

        _connection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .Build();

        _connection.On<bool>("RecieveCommand",  _ => { });
        _connection.On      ("ReceiveGameSetup", () => { });

        _connection.Closed += ex =>
        {
            if (ex != null) OnError.Invoke($"BattleServer connection lost: {ex.Message}");
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
