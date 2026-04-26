using UnityEngine;

namespace PrimalConquest.Auth
{
    public static class AuthConfig
    {
        // Swap to your ingress host for prod builds
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public const string BaseUrl = "http://172.21.44.89:80";
#else
        public const string BaseUrl = "http://172.21.44.89:80";
#endif
    }
}
