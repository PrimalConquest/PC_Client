using AccountComunication;
using PrimalConquest.Events;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PrimalConquest.Auth
{
    // ViewModel — orchestrates the auth flow.
    // Knows nothing about the View. Communicates only by raising event channels.
    public class AuthViewModel : MonoBehaviour
    {

        [Header("Navigation")]
        [SerializeField] string _nextScene = "GameMenu";

        [Header("Communication")]
        [SerializeField] UnityEvent<string> OnError;
        [SerializeField] UnityEvent<bool, string> Loading;

        bool _busy;

        async void Start()
        {
            await TryAutoRefreshAsync();
        }

        public async void Login(string email, string password)
        {
            if (_busy) return;
            SetBusy(true, "loggin in...");

            var (response, error) = await AuthService.Login(email.Trim(), password);
            if (error != null) 
            { 
                SetBusy(false); 
                OnError.Invoke(error); 
                return; 
            }

            CommitSession(response);
            //SetBusy(false);
        }


        public async void Register(string userName, string email, string password)
        {
            if (_busy) return;
            SetBusy(true, "register user...");

            var (response, error) = await AuthService.Register(userName.Trim(), email.Trim(), password);
            if (error != null) 
            { 
                SetBusy(false); 
                OnError.Invoke(error); 
                return; 
            }
            CommitSession(response);
        }

        public async Task DbHealth()
        {
            if (_busy) return;
            SetBusy(true, "health check...");
            var (response, error) = await AuthService.DbHealthCheck();
            if (error != null)
            {
                SetBusy(false);
                OnError.Invoke(error);
                return;
            }
            OnError.Invoke("All good");
            SetBusy(false);
        }

        async Task TryAutoRefreshAsync()
        {
            if (!AuthSession.IsLoggedIn)
            {
                SetBusy(false);
                return;
            }
            SetBusy(true, "auto login...");

            var (response, error) = await AuthService.Refresh(AuthSession.RefreshToken);

            if (error != null) 
            {
                AuthSession.Clear();
                SetBusy(false);
                OnError.Invoke("auto login failed"); 
                return; 
            }

            CommitSession(response);
            //SetBusy(false);
        }

        void SetBusy(bool value, string message = "")
        {
            _busy = value;
            Loading.Invoke(value, message);
        }

        void CommitSession(AuthResponseDTO r)
        {
            AuthSession.Save(r.AccessToken, r.RefreshToken, r.UserId, r.UserName);
            OnAuthComplete();
        }

        void OnAuthComplete()
        {
            SceneManager.LoadScene(_nextScene);
        }
    }
}
