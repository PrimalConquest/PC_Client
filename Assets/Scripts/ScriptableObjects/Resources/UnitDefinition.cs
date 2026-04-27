using SimulationEngine.Source.Enums;
using UnityEngine;

[CreateAssetMenu(menuName = "PrimalConquest/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    [Header("Identity")]
    public string UnitId;

    [Header("Visuals")]
    public Sprite Icon;
    public GameObject InGamePrefab;

    [Header("Localization")]
    public LocalizedString DisplayName;        
    public LocalizedString Description;

    [Header("Rules")]
    public EColor Color;
    public bool IsUnlocked = true;              
}
