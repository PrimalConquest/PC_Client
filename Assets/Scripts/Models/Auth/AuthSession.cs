using UnityEngine;

namespace PrimalConquest.Auth
{
    public static class AuthSession
    {
        private const string RefreshTokenKey = "refresh_token";

        public static string AccessToken  { get; private set; }
        public static string RefreshToken { get; private set; }
        public static string UserId       { get; private set; }
        public static string UserName     { get; private set; }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(PlayerPrefs.GetString(RefreshTokenKey, null));

        public static void Load()
        {
            RefreshToken = PlayerPrefs.GetString(RefreshTokenKey, null);
        }

        public static void Save(string accessToken, string refreshToken, string userId, string userName)
        {
            AccessToken  = accessToken;
            RefreshToken = refreshToken;
            UserId       = userId;
            UserName     = userName;

            AuthService.SetAuthToken(accessToken);
            PlayerPrefs.SetString(RefreshTokenKey, refreshToken);
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            AccessToken  = null;
            RefreshToken = null;
            UserId       = null;
            UserName     = null;

            AuthService.SetAuthToken("");
            PlayerPrefs.DeleteKey(RefreshTokenKey);
            PlayerPrefs.Save();
        }
    }
}
