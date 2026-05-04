using BattleComunication;
using UnityEngine;
using UnityEngine.UI;

public class SpecialUnitSlotView : MonoBehaviour
{
    [SerializeField] Text   _nameText;
    [SerializeField] Slider _energyBar;
    [SerializeField] Text   _costText;
    [SerializeField] Button _activateButton;

    public static System.Action<string> OnActivate;

    string _unitKey;

    public void Bind(SpecialUnitStateDTO data, bool isMyTurn)
    {
        _unitKey = data.Key;

        _nameText.text = data.Key;

        _energyBar.minValue = 0;
        _energyBar.maxValue = Mathf.Max(data.MaxEnergy, 1);
        _energyBar.value    = data.Energy;

        _costText.text = $"Cost: {data.ActivationCost}";

        bool canClick = isMyTurn && data.CanActivate && data.IsOnBoard;
        _activateButton.interactable = canClick;
    }

    public void OnActivateClicked()
    {
        OnActivate?.Invoke(_unitKey);
    }
}
