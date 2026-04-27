using LoadoutComunication;
using PrimalConquest.Auth;
using System.Threading.Tasks;

public static class LoadoutService
{
    public static Task<(LoadoutDTO result, string error)> GetLoadout() =>
        CommunicationService.Get<LoadoutDTO>(Endpoints.Loadout(AuthSession.UserId));

    public static Task<(LoadoutDTO result, string error)> SaveLoadout(LoadoutDTO dto) =>
        CommunicationService.Post<LoadoutDTO, LoadoutDTO>(Endpoints.Loadout(AuthSession.UserId), dto);
}
