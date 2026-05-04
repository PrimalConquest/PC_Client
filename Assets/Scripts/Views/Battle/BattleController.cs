using BattleComunication;
using UnityEngine;
using UnityEngine.UI;

// Wires BattleService events to the two-board battle UI.
// Scene layout expectation:
//   _enemyBoardView  (top)    — always dimmed, never interactive
//   _myBoardView     (bottom) — bright + interactive only on your turn
//   _specialPanel             — your special units sidebar
//   _turnText                 — "Your turn" / "<name>'s turn"
//   _movesText                — "Moves: N"
//   _endTurnButton            — enabled only on your turn
public class BattleController : MonoBehaviour
{
    [Header("Service")]
    [SerializeField] BattleService _battleService;

    [Header("Boards")]
    [SerializeField] BoardView _myBoardView;     // bottom — this client's board
    [SerializeField] BoardView _enemyBoardView;  // top    — opponent's board

    [Header("Sidebar")]
    [SerializeField] SpecialUnitPanelView _specialPanel;

    [Header("HUD")]
    [SerializeField] Text   _turnText;
    [SerializeField] Text   _movesText;
    [SerializeField] Button _endTurnButton;
    [SerializeField] Text   _errorText;

    int _myPlayerIndex = -1;

    void Awake()
    {
        UnitTileView.OnSwipe           += OnSwipe;
        SpecialUnitSlotView.OnActivate += OnActivate;
    }

    void OnDestroy()
    {
        UnitTileView.OnSwipe           -= OnSwipe;
        SpecialUnitSlotView.OnActivate -= OnActivate;
    }

    // ── BattleService event listeners (wire in inspector) ─────────────────────

    public void OnPlayerIndexReceived(int index)
    {
        _myPlayerIndex = index;
        Debug.Log($"[Battle] I am player {index}");
    }

    public void OnGameSetup(GameStateDTO state) => ApplyState(state);
    public void OnGameState(GameStateDTO state) => ApplyState(state);

    public void OnConnectionError(string msg)
    {
        if (_errorText != null) _errorText.text = msg;
        Debug.LogError($"[Battle] {msg}");
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    void ApplyState(GameStateDTO state)
    {
        if (_myPlayerIndex < 0 || state.Players.Length < 2) return;

        int enemyIndex = 1 - _myPlayerIndex;
        bool isMyTurn  = state.ActivePlayer == _myPlayerIndex;

        // My board (bottom): interactive only when it's my turn
        _myBoardView.Render(state, _myPlayerIndex, isMyTurn);

        // Enemy board (top): always read-only
        _enemyBoardView.Render(state, enemyIndex, false);

        // Sidebar: my special units, enabled only on my turn
        _specialPanel.Render(state, _myPlayerIndex, isMyTurn);

        // Turn indicator
        if (_turnText != null)
            _turnText.text = isMyTurn
                ? "Your turn"
                : $"{state.Players[state.ActivePlayer].Name}'s turn";

        // Move counter for the active player (show my moves when it's my turn)
        if (_movesText != null)
            _movesText.text = isMyTurn
                ? $"Moves: {state.Players[_myPlayerIndex].CurrentMoves}"
                : "";

        // End turn button
        if (_endTurnButton != null)
            _endTurnButton.interactable = isMyTurn;
    }

    async void OnSwipe(int x, int y, int direction)
    {
        // Guard: swipes can only reach here from my board (enemy board has
        // blocksRaycasts=false), but double-check turn ownership.
        if (_myPlayerIndex < 0 || _battleService == null) return;
        try   { await _battleService.SendMoveCommand(x, y, direction); }
        catch (System.Exception ex) { Debug.LogError($"[Battle] SendMoveCommand: {ex.Message}"); }
    }

    async void OnActivate(string unitKey)
    {
        if (_battleService == null) return;
        try   { await _battleService.SendActivateCommand(unitKey); }
        catch (System.Exception ex) { Debug.LogError($"[Battle] SendActivate: {ex.Message}"); }
    }

    public async void OnEndTurnClicked()
    {
        if (_battleService == null) return;
        try   { await _battleService.SendEndTurn(); }
        catch (System.Exception ex) { Debug.LogError($"[Battle] SendEndTurn: {ex.Message}"); }
    }
}
