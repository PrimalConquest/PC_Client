using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutScreenController : MonoBehaviour
{
    public static LoadoutScreenController Instance { get; private set; }

    [Header("ViewModel")]
    [SerializeField] LoadoutViewModel _vm;

    [Header("Error")]
    [SerializeField] Text _errorText;

    [Header("Slot Views")]
    [SerializeField] CommanderSlotView  _commanderSlot;
    [SerializeField] OfficerSlotView[]  _officerSlots;

    [Header("Catalog Containers")]
    [SerializeField] Transform _commanderCatalogContainer;
    [SerializeField] Transform _officerCatalogContainer;

    [Header("Prefabs")]
    [SerializeField] UnitCardView _unitCardPrefab;

    [Header("Battle")]
    [SerializeField] Button _battleButton;

    public void Init()
    {
        Instance = this;

        _commanderSlot.Init(_vm);

        foreach (var slot in _officerSlots)
            slot.Init(_vm);

        RefreshUI();
    }

    public void ShowError(string msg) { _errorText.text = msg; }

    public void RefreshUI()
    {
        RefreshSlots();
        RebuildCatalog();
        HandleButtonValid();
    }

    void HandleButtonValid()
    {
        _battleButton.interactable = !_vm.State.IsCommanderSlotEmpty();
    }

    void RefreshSlots()
    {
        var commander = FindUnit(_vm.State.CommanderId);
        _commanderSlot.Refresh(commander);

        for (int i = 0; i < _officerSlots.Length; i++)
        {
            var officer = FindUnit(_vm.State.GetOfficerId(i));
            _officerSlots[i].Refresh(officer);
        }
    }

    void RebuildCatalog()
    {
        ClearContainer(_commanderCatalogContainer);
        ClearContainer(_officerCatalogContainer);

        foreach (var unit in _vm.Catalogue.Commanders)
        {
            if (_vm.State.Contains(unit.UnitId)) continue;
            SpawnCard(unit, _commanderCatalogContainer);
        }

        foreach (var unit in _vm.Catalogue.Officers)
        {
            if (_vm.State.Contains(unit.UnitId)) continue;
            SpawnCard(unit, _officerCatalogContainer);
        }
    }

    void ClearContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    void SpawnCard(UnitDefinition unit, Transform container)
    {
        var card = Instantiate(_unitCardPrefab, container);
        card.Setup(unit);
    }

    public UnitDefinition FindUnit(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (var u in _vm.Catalogue.Commanders)
            if (u.UnitId == id) return u;

        foreach (var u in _vm.Catalogue.Officers)
            if (u.UnitId == id) return u;

        return null;
    }
}
