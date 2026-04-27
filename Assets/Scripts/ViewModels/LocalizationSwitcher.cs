using UnityEngine;

public class LocalizationSwitcher : MonoBehaviour
{
    public void SwitchToENG()
    {
        LocalizationManager.SetLanguage(SystemLanguage.English);
    }

    public void SwitchToBG()
    {
        LocalizationManager.SetLanguage(SystemLanguage.Bulgarian);
    }
}
