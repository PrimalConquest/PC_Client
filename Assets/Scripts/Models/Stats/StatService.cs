using LoadoutComunication;
using PrimalConquest.Auth;
using System.Threading.Tasks;

public static class StatService
{
    public static Task<(UserStatsDTO result, string error)> GetStats() =>
        CommunicationService.Get<UserStatsDTO>(Endpoints.Stats(AuthSession.UserId));
}
