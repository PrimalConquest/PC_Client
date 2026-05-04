using PrimalConquest.Auth;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        AuthSession.Load();

        var (refreshed, err) = await AuthService.Refresh(AuthSession.RefreshToken);
        if (err != null || refreshed == null)
        {
            AuthSession.Clear();
            SceneManager.LoadScene("MainMenu");
            return;
        }
        AuthSession.Save(refreshed.AccessToken, refreshed.RefreshToken,
                         refreshed.UserId,      refreshed.UserName);

        if (_playerNameText != null) _playerNameText.text = AuthSession.UserName;

        _loadoutViewModel.Init();
        _loadoutScreenController.Init();
        _statScreenController.Init();
    }
}
