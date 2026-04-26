using UnityEngine;

public static class Endpoints
{
    static string _dbPrefix = "/api/db";

    public static string DbHealth() => _dbPrefix + "/healthz";
    public static string Login() => _dbPrefix + "/auth/login";
    public static string Register() => _dbPrefix + "/auth/register";
    public static string Refresh() => _dbPrefix + "/auth/refresh";
    public static string LogOut() => _dbPrefix + "/auth/logout";
    public static string Loadout(string userId) => _dbPrefix + $"/loadout/{userId}";
    public static string Stats(string userId) => _dbPrefix + $"/stats/{userId}";
}
