using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Drop target for the commander slot. Only accepts UnitDefinitions from the Commanders list.
public class CommanderSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] Image    _icon;

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
        if (!IsCommander(card.Unit)) return;

        card.gameObject.SetActive(false);
        Destroy(card.gameObject);
        _vm.SetCommander(card.Unit.UnitId);
    }

    
    public void OnPointerClick(PointerEventData e)
    {
        //add check to see if e.clickcont >= 2
        var unit = LoadoutScreenController.Instance?.FindUnit(_vm.State.CommanderId);
        if (unit != null) UnitInfoPopup.Show(unit);
    }

    bool IsCommander(UnitDefinition unit) =>
        unit != null && _vm.Catalogue.Commanders.Contains(unit);
}
