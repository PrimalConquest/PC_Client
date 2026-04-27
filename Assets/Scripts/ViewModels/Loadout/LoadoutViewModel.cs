using LoadoutComunication;
using PrimalConquest.Auth;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class LoadoutViewModel : MonoBehaviour
{
    [Header("Catalogue")]
    [SerializeField] UnitCatalogue _catalogue;
    [SerializeField] string _defaultCommanderId;

    [Header("Output Channels")]
    [SerializeField] UnityEvent OnLoadoutChanged;
    [SerializeField] UnityEvent<string> OnError;

    public LoadoutState  State     { get; private set; } = new();
    public UnitCatalogue Catalogue => _catalogue;


    public async void Init()
    {
        Debug.Log("Inital load");
        await LoadAsync();
    }

    public async void SetCommander(string unitId)
    {
        State.SetCommander(unitId);
        OnLoadoutChanged.Invoke();
        await SaveAsync();
    }

    public async void SetOfficer(int slot, string unitId)
    {
        if (!State.TrySetOfficer(slot, unitId))
        {
            OnError.Invoke($"Cannot place {unitId} on slot [{slot}]");
            return;
        }
        OnLoadoutChanged.Invoke();
        await SaveAsync();
    }

    public async void ClearOfficer(int slot)
    {
        State.ClearOfficer(slot);
        OnLoadoutChanged.Invoke();
        await SaveAsync();
    }

    async Task LoadAsync()
    {
        if (!AuthSession.IsLoggedIn) 
        {
            OnError.Invoke(LocalizedString.Get("Not logged in"));
            return; 
        }

        var (dto, err) = await LoadoutService.GetLoadout();
        if (err != null || dto == null) 
        {
            OnError.Invoke(LocalizedString.Get("Error fetching loadout"));
            return; 
        }

        if(dto.CommanderId == "")
        {
            ApplyDefaults();
            return;
        }

        State.SetCommander(dto.CommanderId);
        State.OfficerIds = dto.OfficerIds ?? new();
        OnLoadoutChanged.Invoke();
    }

    async Task SaveAsync()
    {
        if (!AuthSession.IsLoggedIn)
        {
            OnError.Invoke(LocalizedString.Get("Not logged in"));
            return;
        }
        var (_, err) = await LoadoutService.SaveLoadout(State.ToDTO());
        if (err != null) OnError.Invoke($"Failed to save loadout: {err}");
    }

    async void ApplyDefaults()
    {
        State = new LoadoutState { CommanderId = _defaultCommanderId };
        OnLoadoutChanged.Invoke();
        await SaveAsync();
    }
}
