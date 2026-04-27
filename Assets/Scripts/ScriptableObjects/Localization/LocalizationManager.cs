using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationManager", menuName = "Scriptable Objects/LocalizationManager")]
public class LocalizationManager : ScriptableObject
{
    const string PrefKey = "localization_language";

    public static Action OnLangueCahnged;
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] List<LocalizationTable> _tables;

    LocalizationTable _active;

    public void Init()
    {
        Instance = this;

        // First launch: no saved pref → fall back to system language.
        // Subsequent launches: restore the player's last chosen language.
        SystemLanguage lang = PlayerPrefs.HasKey(PrefKey)
            ? (SystemLanguage)PlayerPrefs.GetInt(PrefKey)
            : Application.systemLanguage;

        _SetLanguage(lang);
    }

    void _SetLanguage(SystemLanguage lang)
    {
        _active = _tables.Find(t => t.Language == lang)
               ?? _tables.Find(t => t.Language == SystemLanguage.English);
        OnLangueCahnged?.Invoke();
    }

    public static void SetLanguage(SystemLanguage lang)
    {
        PlayerPrefs.SetInt(PrefKey, (int)lang);
        PlayerPrefs.Save();
        Instance._SetLanguage(lang);
    }

    public static string Get(string key)
    {
        return (Instance == null) ? "setup" : Instance._active.Get(key);
    }
}
