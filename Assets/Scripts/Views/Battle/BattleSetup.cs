using PrimalConquest.Auth;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleSetup : MonoBehaviour
{
    [SerializeField] BattleService _battleService;
    [SerializeField] Text          _statusText;

    async void Start() => await InitAsync();

    async Task InitAsync()
    {
        SetStatus("Authenticating...");

        AuthSession.Load();

        var (refreshed, err) = await AuthService.Refresh(AuthSession.RefreshToken);
        if (err != null || refreshed == null)
        {
            AuthSession.Clear();
            BattleSession.Clear();
            SceneManager.LoadScene("MainMenu");
            return;
        }
        AuthSession.Save(refreshed.AccessToken, refreshed.RefreshToken,
                         refreshed.UserId,      refreshed.UserName);

        SetStatus("Connecting to game server...");
        await _battleService.ConnectAsync();
    }

    void SetStatus(string msg)
    {
        if (_statusText != null) _statusText.text = msg;
    }
}
