using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationTable", menuName = "Scriptable Objects/LocalizationTable")]
public class LocalizationTable : ScriptableObject
{
    public SystemLanguage Language;
    public List<LocalizationEntry> Entries;

    public string Get(string key)
    {
        var entry = Entries.Find(e => e.Key == key);
        return entry != null ? entry.Value : key; // fallback to key if missing
    }
}

[System.Serializable]
public class LocalizationEntry
{
    public string Key;
    [TextArea] public string Value;
}