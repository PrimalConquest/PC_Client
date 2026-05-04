using BattleComunication;
using UnityEngine;
using UnityEngine.UI;

// Two-layer board rendering:
//   _tileGrid  (GridLayoutGroup)  — 49 invisible input-catcher tiles, one per cell.
//                                   Handles swipe detection for any cell a unit occupies.
//   _visualLayer (free RectTransform) — one UnitVisualView per unit, sized to span its
//                                   full shape footprint. Sits on top of the grid.
//
// Add a CanvasGroup to this GameObject so the whole board can be dimmed + input-blocked
// when it is not the active player's turn.
public class BoardView : MonoBehaviour
{
    [Header("Input layer")]
    [SerializeField] RectTransform  _tileGrid;      // has GridLayoutGroup
    [SerializeField] UnitTileView   _tilePrefab;

    [Header("Visual layer")]
    [SerializeField] RectTransform  _visualLayer;   // free RectTransform, no layout group
    [SerializeField] UnitVisualView _unitVisualPrefab;

    CanvasGroup   _canvasGroup;
    GridLayoutGroup _grid;
    UnitTileView[,] _tiles;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _grid        = _tileGrid.GetComponent<GridLayoutGroup>();

        _tiles = new UnitTileView[7, 7];
        for (int y = 0; y < 7; y++)
            for (int x = 0; x < 7; x++)
                _tiles[x, y] = Instantiate(_tilePrefab, _tileGrid);
    }

    public void Render(GameStateDTO state, int playerIndex, bool interactive)
    {
        if (playerIndex >= state.Players.Length) return;

        SetInteractive(interactive);
        ClearAll();

        var player = state.Players[playerIndex];

        foreach (var unit in player.BoardUnits)
            PlaceUnit(unit);

        foreach (var su in player.SpecialUnits)
            if (su.IsOnBoard)
                PlaceSpecialUnit(su);
    }

    // ── Placement helpers ─────────────────────────────────────────────────────

    void PlaceUnit(UnitStateDTO unit)
    {
        // Mark every cell the unit occupies so swipes on any of them report the anchor.
        for (int dy = 0; dy < unit.ShapeHeight; dy++)
            for (int dx = 0; dx < unit.ShapeWidth; dx++)
                SetTile(unit.X + dx, unit.Y + dy, unit);

        // One visual spanning the full footprint.
        SpawnVisual(unit.X, unit.Y, unit.ShapeWidth, unit.ShapeHeight,
                    ColorFor(unit.Color), $"HP:{unit.Health}");
    }

    void PlaceSpecialUnit(SpecialUnitStateDTO unit)
    {
        for (int dy = 0; dy < unit.ShapeHeight; dy++)
            for (int dx = 0; dx < unit.ShapeWidth; dx++)
                SetTile(unit.X + dx, unit.Y + dy, unit);

        SpawnVisual(unit.X, unit.Y, unit.ShapeWidth, unit.ShapeHeight,
                    ColorFor(unit.Key == "commander" ? 0 : 2),
                    $"{unit.Key}\nE:{unit.Energy}/{unit.MaxEnergy}");
    }

    // Assign a unit DTO to a background tile so swipes route to the anchor (X, Y).
    void SetTile(int col, int row, UnitStateDTO unit)
    {
        if (!InBounds(col, row)) return;
        _tiles[col, row].SetUnit(unit);
    }

    void SetTile(int col, int row, SpecialUnitStateDTO unit)
    {
        if (!InBounds(col, row)) return;
        _tiles[col, row].SetSpecialUnit(unit);
    }

    // Instantiate a UnitVisualView in the free visual layer, positioned and sized
    // to span (shapeW x shapeH) grid cells starting at grid anchor (col, row).
    void SpawnVisual(int col, int row, int shapeW, int shapeH, Color color, string label)
    {
        var cell    = _grid.cellSize;
        var spacing = _grid.spacing;

        float pixelW = shapeW * cell.x + (shapeW - 1) * spacing.x;
        float pixelH = shapeH * cell.y + (shapeH - 1) * spacing.y;

        // GridLayoutGroup lays out left-to-right, top-to-bottom in local space.
        // Cell (col, row) top-left corner in _tileGrid local space (y grows downward in UI).
        float originX = col * (cell.x + spacing.x);
        float originY = -(row * (cell.y + spacing.y));   // negative because UI y grows down

        var vis = Instantiate(_unitVisualPrefab, _visualLayer);
        var rt  = vis.GetComponent<RectTransform>();

        // Anchor top-left of visual layer, pivot top-left.
        rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
        rt.pivot     = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(originX, originY);
        rt.sizeDelta        = new Vector2(pixelW, pixelH);

        vis.Set(color, label);
    }

    // ── Bookkeeping ───────────────────────────────────────────────────────────

    void ClearAll()
    {
        foreach (var t in _tiles) t.Clear();

        for (int i = _visualLayer.childCount - 1; i >= 0; i--)
            Destroy(_visualLayer.GetChild(i).gameObject);
    }

    void SetInteractive(bool interactive)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha          = interactive ? 1f : 0.45f;
        _canvasGroup.blocksRaycasts = interactive;
        _canvasGroup.interactable   = interactive;
    }

    static bool InBounds(int x, int y) => x >= 0 && x < 7 && y >= 0 && y < 7;

    static readonly Color[] _colorMap =
    {
        Color.red,
        Color.yellow,
        new Color(0.25f, 0.55f, 1f),
        Color.green,
        Color.gray,
        Color.white,
        new Color(0, 0, 0, 0)
    };

    static Color ColorFor(int index) =>
        (index >= 0 && index < _colorMap.Length) ? _colorMap[index] : Color.white;
}
