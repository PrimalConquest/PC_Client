using UnityEngine;
using UnityEngine.UI;

public class UITextHandler : MonoBehaviour
{

    [SerializeField] LocalizedString text;
    [SerializeField] Text textField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnEnable()
    {
        LocalizationManager.OnLangueCahnged += SetText;
    }

    public void Start()
    {
        SetText();
    }

    public void OnDestroy()
    {
        LocalizationManager.OnLangueCahnged -= SetText;

    }

    void SetText()
    {
        textField.text = text.Get();
    }
}
