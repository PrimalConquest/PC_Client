using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationManager", menuName = "Scriptable Objects/LocalizationManager")]
public class LocalizationManager : ScriptableObject
{
    public static Action OnLangueCahnged;
    public static LocalizationManager Instance { get; private set; }

    [SerializeField] List<LocalizationTable> _tables;

    LocalizationTable _active;

    void Awake()
    {
        Instance = this;
        SetLanguage(Application.systemLanguage);
    }

    public void SetLanguage(SystemLanguage lang)
    {
        _active = _tables.Find(t => t.Language == lang)
                  ?? _tables.Find(t => t.Language == SystemLanguage.English);
        OnLangueCahnged.Invoke();
    }

    public static string Get(string key) => Instance._active.Get(key);
}
