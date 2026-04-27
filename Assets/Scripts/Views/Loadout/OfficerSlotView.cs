using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Drop target for one officer slot. Only accepts UnitDefinitions from the Officers list.
public class OfficerSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] Image     _icon;
    [SerializeField] int        _slotIndex;

    LoadoutViewModel _vm;

    public void Init(LoadoutViewModel vm) => _vm = vm;

    public void Refresh(UnitDefinition unit)
    {
        bool hasUnit = unit != null;
        _icon.sprite         = hasUnit ? unit.Icon : null;
        _icon.enabled        = hasUnit;
    }

    public void OnDrop(PointerEventData e)
    {
        var card = e.pointerDrag?.GetComponent<UnitCardView>();
        if (card == null) return;
        if (!IsOfficer(card.Unit)) return;

        card.gameObject.SetActive(false); // hide until RefreshUI rebuilds
        Destroy(card.gameObject);
        _vm.SetOfficer(_slotIndex, card.Unit.UnitId);
    }

    // Double-click filled slot → info popup.
    public void OnPointerClick(PointerEventData e)
    {
        var id = _vm.State.GetOfficerId(_slotIndex);
        if (string.IsNullOrEmpty(id)) return;
        var unit = LoadoutScreenController.Instance?.FindUnit(id);
        if (unit != null) UnitInfoPopup.Show(unit);
    }

    bool IsOfficer(UnitDefinition unit) =>
        unit != null && _vm.Catalogue.Officers.Contains(unit);
}
