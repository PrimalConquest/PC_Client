using PrimalConquest.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutViewMOdel : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] string _mainMenuScene = "MainMenu";

    public async void Logout()
    {
        var error = await AuthService.Logout();
        if (error != null)
        {
            return;
        }

        AuthSession.Clear();

        SceneManager.LoadScene(_mainMenuScene);
    }
}
