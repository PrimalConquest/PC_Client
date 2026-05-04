using BattleComunication;
using UnityEngine;
using UnityEngine.EventSystems;

// Invisible input-catcher tile — one per grid cell.
// Tracks whichever unit (or special unit) occupies this cell.
// When swiped, reports the unit's ANCHOR position (X, Y) — not this tile's position —
// so multi-cell units always receive commands at their origin regardless of which cell
// the player swiped.
public class UnitTileView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    UnitStateDTO        _unit;
    SpecialUnitStateDTO _special;
    Vector2             _pointerDownPos;

    // x, y = board anchor of the unit; direction = EDirection int (Up=0,Down=1,Left=2,Right=3)
    public static System.Action<int, int, int> OnSwipe;

    public bool IsOccupied => _unit != null || _special != null;

    public void Clear()
    {
        _unit    = null;
        _special = null;
    }

    public void SetUnit(UnitStateDTO u)
    {
        _unit    = u;
        _special = null;
    }

    public void SetSpecialUnit(SpecialUnitStateDTO s)
    {
        _special = s;
        _unit    = null;
    }

    public void OnPointerDown(PointerEventData e) => _pointerDownPos = e.position;

    public void OnPointerUp(PointerEventData e)
    {
        if (!IsOccupied) return;

        var delta = e.position - _pointerDownPos;
        if (delta.magnitude < 20f) return;

        // EDirection: Up=0, Down=1, Left=2, Right=3
        int dir = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
            ? (delta.x > 0 ? 3 : 2)
            : (delta.y > 0 ? 0 : 1);

        // Always send the anchor position, not the tile's own grid position.
        int anchorX = _unit?.X ?? _special!.X;
        int anchorY = _unit?.Y ?? _special!.Y;
        OnSwipe?.Invoke(anchorX, anchorY, dir);
    }
}
