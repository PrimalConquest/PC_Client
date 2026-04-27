using System;
using UnityEngine;

[Serializable]
public class LocalizedString
{
    
    [SerializeField] string _key;
    public string Get()
    {
        return LocalizationManager.Get(_key);
    }

    public static string Get(string key)
    {
        return LocalizationManager.Get(key);
    }
}
