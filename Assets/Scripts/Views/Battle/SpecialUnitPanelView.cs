using BattleComunication;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnitPanelView : MonoBehaviour
{
    [SerializeField] SpecialUnitSlotView _slotPrefab;

    readonly List<SpecialUnitSlotView> _slots = new();

    public void Render(GameStateDTO state, int playerIndex, bool isMyTurn)
    {
        if (playerIndex >= state.Players.Length) return;

        var specials = state.Players[playerIndex].SpecialUnits;

        while (_slots.Count < specials.Length)
            _slots.Add(Instantiate(_slotPrefab, transform));

        for (int i = 0; i < specials.Length; i++)
        {
            _slots[i].gameObject.SetActive(true);
            _slots[i].Bind(specials[i], isMyTurn);
        }

        for (int i = specials.Length; i < _slots.Count; i++)
            _slots[i].gameObject.SetActive(false);
    }
}
