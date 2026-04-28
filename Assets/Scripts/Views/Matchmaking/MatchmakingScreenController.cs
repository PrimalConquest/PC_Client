using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchmakingScreenController : MonoBehaviour
{
    [SerializeField] MatchmakingService _service;
    [SerializeField] GameObject         _queuePanel;
    [SerializeField] Text               _statusText;
    [SerializeField] Button             _leaveButton;


    public void Init()
    {
        _queuePanel.SetActive(true);
        SetStatus("Connecting...");
        _leaveButton.interactable = true;
    }

    // ── Button handler ─────────────────────────────────────────────────────────
    public async void OnLeaveClicked()
    {
        _leaveButton.interactable = false;
        _service.LeaveQueueAsync();
    }

    // ── Event handlers ─────────────────────────────────────────────────────────
    public void HandleQueueJoined(int position) => SetStatus($"In queue: #{position}");
    public void HandleError(string msg)         => SetStatus($"Error: {msg}");
    public void HandleMessage(string msg) => SetStatus($"{msg}");

    public void HandleMatchFound(string ip, int port)
    {
        SetStatus("Match found!");
        HidePanel();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    void SetStatus(string msg)
    {
        if (_statusText != null) _statusText.text = msg;
    }

    void HidePanel()
    {
        _leaveButton.interactable = false;
        _queuePanel.SetActive(false);
    }
}
