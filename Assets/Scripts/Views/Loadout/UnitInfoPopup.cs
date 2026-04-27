using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Overlay panel displaying unit icon, name, and description.
// Activated via static Show(); closed by the close button wired in the Inspector.
public class UnitInfoPopup : MonoBehaviour
{
    static UnitInfoPopup _instance;

    [SerializeField] GameObject _panel;
    [SerializeField] Image      _icon;
    [SerializeField] Text   _nameLabel;
    [SerializeField] Text   _descriptionLabel;

    public void Init()
    {
        _instance = this;
    }

    public static void Show(UnitDefinition unit)
    {
        if (_instance == null || unit == null) return;

        _instance._icon.sprite            = unit.Icon;
        _instance._icon.enabled           = unit.Icon != null;
        _instance._nameLabel.text         = unit.DisplayName.Get();
        _instance._descriptionLabel.text  = unit.Description.Get();
        _instance._panel.SetActive(true);
    }

    public static void Hide()
    {
        if (_instance != null) _instance._panel.SetActive(false);
    }
}
