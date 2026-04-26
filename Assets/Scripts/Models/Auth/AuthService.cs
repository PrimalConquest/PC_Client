using AccountComunication;
using CommunicationShared;
using SharedUtils;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimalConquest.Auth
{
    public static class AuthService
    {

        public static Task<(ErrorResponseDTO, string)> DbHealthCheck() =>
            CommunicationService.Get<ErrorResponseDTO>(Endpoints.DbHealth());

        public static Task<(AuthResponseDTO response, string error)> Login(
            string email, string password) =>
            CommunicationService.Post<AuthResponseDTO, LoginDTO>(Endpoints.Login(), new LoginDTO { Email = email, Password = password });

        public static Task<(AuthResponseDTO response, string error)> Register(
            string userName, string email, string password) =>
            CommunicationService.Post<AuthResponseDTO, RegisterDTO>(Endpoints.Register(),
                new RegisterDTO { UserName = userName, Email = email, Password = password });

        public static Task<(AuthResponseDTO response, string error)> Refresh(string refreshToken) =>
            CommunicationService.Post<AuthResponseDTO, RefreshRequestDTO>(Endpoints.Refresh(), new RefreshRequestDTO { RefreshToken = refreshToken });

        public static async Task<string> Logout(string refreshToken)
        {
            try
            {
                var body    = (new RefreshRequestDTO { RefreshToken = refreshToken }).ToJson();
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                await CommunicationService._client.PostAsync(Endpoints.LogOut(), content);
                return null;
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static void SetAuthToken(string token)
        {
            CommunicationService._client.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", token);
        }
        
    }
}
