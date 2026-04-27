namespace PrimalConquest.Auth
{
    public static class AuthSession
    {
        const string AccessTokenKey  = "access_token";
        const string RefreshTokenKey = "refresh_token";
        const string UserIdKey       = "user_id";
        const string UserNameKey     = "user_name";

        public static string AccessToken  => SecureStorage.Get(AccessTokenKey);
        public static string RefreshToken => SecureStorage.Get(RefreshTokenKey);
        public static string UserId       => SecureStorage.Get(UserIdKey);
        public static string UserName     => SecureStorage.Get(UserNameKey);

        // IsLoggedIn is based on refresh token — the access token is short-lived and may already
        // be expired on startup, but the refresh token lets us silently re-authenticate.
        public static bool IsLoggedIn => !string.IsNullOrEmpty(RefreshToken);

        public static void Save(string accessToken, string refreshToken, string userId, string userName)
        {
            AuthService.SetAuthToken(accessToken);
            SecureStorage.Set(AccessTokenKey,  accessToken);
            SecureStorage.Set(RefreshTokenKey, refreshToken);
            SecureStorage.Set(UserIdKey,       userId);
            SecureStorage.Set(UserNameKey,     userName);
        }

        public static void Clear()
        {
            AuthService.SetAuthToken("");
            SecureStorage.Delete(AccessTokenKey);
            SecureStorage.Delete(RefreshTokenKey);
            SecureStorage.Delete(UserIdKey);
            SecureStorage.Delete(UserNameKey);
        }
    }
}
