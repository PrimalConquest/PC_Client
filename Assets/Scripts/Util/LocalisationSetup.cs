using UnityEngine;

public class LocalisationSetup : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] LocalizationManager _locMan;
    void Start()
    {
        _locMan.Init();
    }
}
