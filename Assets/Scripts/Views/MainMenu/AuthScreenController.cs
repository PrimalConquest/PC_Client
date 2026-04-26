using System.Collections;
using PrimalConquest.Auth;
using PrimalConquest.Events;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PrimalConquest.UI
{
    // View — purely responsible for:
    //   1. Forwarding user input to the ViewModel
    //   2. Subscribing to event channels and updating the UI
    // No business logic lives here.
    public class AuthScreenController : MonoBehaviour
    {
        [Header("ViewModel")]
        [SerializeField] AuthViewModel _vm;

        [Header("Panels")]
        [SerializeField] GameObject _mainPanel;
        [SerializeField] GameObject _loadingPanel;
        [SerializeField] GameObject _loginPanel;
        [SerializeField] GameObject _registerPanel;
        [SerializeField] Button _loginTabButton;
        [SerializeField] Button _registerTabButton;

        [Header("Login form")]
        [SerializeField] InputField _loginEmail;
        [SerializeField] InputField _loginPassword;

        [Header("Register form")]
        [SerializeField] InputField _regUsername;
        [SerializeField] InputField _regEmail;
        [SerializeField] InputField _regPassword;

        [Header("Labels")]
        [SerializeField] Text _statusLabel;
        [SerializeField] Text _loadingLabel;

        [Header("Spinner")]
        [SerializeField] RectTransform _spinnerRect;

        Coroutine _spinCoroutine;

        public void Start()
        {
            ShowLogin();
        }

        public void OnBusyChanged(bool busy, string message)
        {
            _mainPanel.SetActive(!busy);
            _loadingPanel.SetActive(busy);

            if (busy)
            {
                _spinCoroutine = StartCoroutine(SpinLoop());
                _loadingLabel.text = message;
            }
            else if (_spinCoroutine != null)
            {
                StopCoroutine(_spinCoroutine);
                _spinCoroutine = null;
            }
        }

        public void OnHealthclicked()
        {
            _vm.DbHealth();
        }

        public void OnLoginClicked()
        {
            if (!ValidateLogin()) return;
            _vm.Login(_loginEmail.text, _loginPassword.text);
        }

        public void OnRegisterClicked()
        {
            if (!ValidateRegister()) return;
            _vm.Register(_regUsername.text, _regEmail.text, _regPassword.text);
        }


        bool ValidateLogin()
        {
            if (string.IsNullOrWhiteSpace(_loginEmail.text))
            { ShowError("Please enter your email."); return false; }
            if (string.IsNullOrWhiteSpace(_loginPassword.text))
            { ShowError("Please enter your password."); return false; }
            return true;
        }

        bool ValidateRegister()
        {
            if (string.IsNullOrWhiteSpace(_regUsername.text))
            { ShowError("Username is required."); return false; }
            if (string.IsNullOrWhiteSpace(_regEmail.text))
            { ShowError("Email is required."); return false; }
            if (_regPassword.text.Length < 8)
            { ShowError("Password must be at least 8 characters."); return false; }
            return true;
        }

        IEnumerator SpinLoop()
        {
            while (true) { _spinnerRect.Rotate(0f, 0f, -360f * Time.deltaTime); yield return null; }
        }

        public void ShowError(string msg)   { _statusLabel.text = msg; }

        public void ShowLogin()
        {
            _loginPanel.SetActive(true);
            _loginTabButton.interactable = false;
            _registerPanel.SetActive(false);
            _registerTabButton.interactable = true;
        }

        public void ShowRegister()
        {
            _loginPanel.SetActive(false);
            _loginTabButton.interactable = true;
            _registerTabButton.interactable = false;
            _registerPanel.SetActive(true);
        }
    }
}
