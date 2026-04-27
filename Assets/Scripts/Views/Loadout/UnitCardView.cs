using SimulationEngine.Source.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles display, drag-and-drop, and double-click detection for a single unit card.
[RequireComponent(typeof(CanvasGroup))]
public class UnitCardView : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Image   _icon;
    [SerializeField] Image _border;

    public UnitDefinition Unit { get; private set; }

    Canvas        _canvas;
    RectTransform _rect;
    CanvasGroup   _group;
    Transform     _originalParent;
    Vector2       _originalPosition;

    const float DoubleClickInterval = 0.3f;
    float _lastClickTime = -1f;

    // ── Setup ──────────────────────────────────────────────────────────────────

    public void Setup(UnitDefinition unit)
    {
        Unit            = unit;
        _icon.sprite    = unit.Icon;
        _border.color = unit.Color switch
        {
            EColor.None => Color.black,
            EColor.Any => Color.magenta,
            EColor.Gray => Color.gray,
            EColor.Red => Color.red,
            EColor.Green => Color.green,
            EColor.Yellow => Color.yellow, 
            EColor.Blue => Color.blue,
            _ => Color.white
        };
    }

    void Awake()
    {
        _rect  = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
    }

    // Lazy canvas lookup — the card may be instantiated before a canvas is in the hierarchy.
    Canvas GetCanvas() => _canvas ??= GetComponentInParent<Canvas>();

    // ── Drag ───────────────────────────────────────────────────────────────────

    public void OnBeginDrag(PointerEventData e)
    {
        _originalParent   = transform.parent;
        _originalPosition = _rect.anchoredPosition;

        // Float on top of everything by reparenting to the root canvas.
        transform.SetParent(GetCanvas().transform, true);
        _group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        _rect.anchoredPosition += e.delta / GetCanvas().scaleFactor;
    }

    public void OnEndDrag(PointerEventData e)
    {
        _group.blocksRaycasts = true;

        // If no slot reparented this card (successful drop destroys & rebuilds via RefreshUI),
        // it is still on the canvas root — snap back to the catalog.
        if (transform.parent == GetCanvas().transform)
        {
            transform.SetParent(_originalParent, false);
            _rect.anchoredPosition = _originalPosition;
        }
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (Time.unscaledTime - _lastClickTime <= DoubleClickInterval)
            UnitInfoPopup.Show(Unit);
        _lastClickTime = Time.unscaledTime;
    }
}
