using PrimalConquest.Auth;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuSetup : MonoBehaviour
{
    [SerializeField] LoadoutViewModel        _loadoutViewModel;
    [SerializeField] LoadoutScreenController _loadoutScreenController;
    [SerializeField] StatScreenController    _statScreenController;
    [SerializeField] Text                    _playerNameText;

    async void Start() => await InitAsync();

    async Task InitAsync()
    {
        // Restore the Bearer token into the HttpClient on every app launch.
        AuthService.SetAuthToken(AuthSession.AccessToken);

        // Proactively refresh so all subsequent requests have a fresh token.
        if (AuthSession.IsLoggedIn)
        {
            var (refreshed, err) = await AuthService.Refresh(AuthSession.RefreshToken);
            if (err == null && refreshed != null)
                AuthSession.Save(refreshed.AccessToken, refreshed.RefreshToken,
                                 refreshed.UserId,      refreshed.UserName);
        }

        if (_playerNameText != null) _playerNameText.text = AuthSession.UserName;

        _loadoutViewModel.Init();
        _loadoutScreenController.Init();
        _statScreenController.Init();
    }
}
