using LoadoutComunication;
using System.Collections.Generic;

public class LoadoutState
{
    public string       CommanderId = "";
    public List<string> OfficerIds  = new();

    public const int MaxOfficers = 3;

    public void SetCommander(string id) => CommanderId = id;

    public bool TrySetOfficer(int slot, string id)
    {
        if (slot < 0 || slot >= MaxOfficers) return false;
        if (OfficerIds.Contains(id))         return false;

        while (OfficerIds.Count <= slot) OfficerIds.Add("");
        OfficerIds[slot] = id;
        return true;
    }

    public void ClearOfficer(int slot)
    {
        if (slot < 0 || slot >= OfficerIds.Count) return;
        OfficerIds[slot] = "";
    }

    public string GetOfficerId(int slot) =>
        slot < OfficerIds.Count ? OfficerIds[slot] : "";

    public bool IsCommanderSlotEmpty() =>
        string.IsNullOrEmpty(CommanderId);
    public bool IsOfficerSlotEmpty(int slot) =>
        string.IsNullOrEmpty(GetOfficerId(slot));

    public bool Contains(string id) =>
        CommanderId == id || OfficerIds.Contains(id);

    public LoadoutDTO ToDTO()
    {
        LoadoutDTO dto = new();
        dto.CommanderId = CommanderId;
        dto.OfficerIds = OfficerIds;
        return dto;
    }
}
