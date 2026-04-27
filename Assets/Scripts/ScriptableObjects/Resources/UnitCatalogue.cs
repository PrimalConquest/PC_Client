using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PrimalConquest/Unit Catalogue")]
public class UnitCatalogue : ScriptableObject
{
    public List<UnitDefinition> Commanders;
    public List<UnitDefinition> Officers;
}
