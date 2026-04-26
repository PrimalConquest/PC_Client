using CommunicationShared;
using PrimalConquest.Auth;
using SharedUtils;
using SharedUtils.Source.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

public static class CommunicationService
{
    public static readonly HttpClient _client = BuildClient();

    static HttpClient BuildClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(AuthConfig.BaseUrl),
            Timeout = TimeSpan.FromSeconds(15),
        };
        client.DefaultRequestHeaders.Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    public static async Task<(R result, string error)> Get<R>(string path) where R : class
    {
        HttpResponseMessage response;
        string responseText;
        try
        {
            Debug.Log($"HTTP get {_client.BaseAddress}{path}");
            response = await _client.GetAsync(path);
            responseText = await response.Content.ReadAsStringAsync();
        }
        catch (TaskCanceledException)
        {
            return (default, "Request timed out. Check your connection.");
        }
        catch (HttpRequestException ex)
        {
            return (default, $"Network error: {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var msg = TryParseError(responseText)
                   ?? $"Server error ({(int)response.StatusCode})";
            return (default, msg);
        }

        try { return (DTO<R>.FromJson(responseText), null); }
        catch (Exception ex) { return (default, $"Unexpected response: {ex.Message}"); }
    }

    public static async Task<(R result, string error)> Post<R, S>(string path, DTO<S> body) where S : class where R : class
    {
        var json = body.ToJson();
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        string responseText;
        try
        {
            Debug.Log($"HTTP post {_client.BaseAddress}{path} + {content}");
            response = await _client.PostAsync(path, content);
            responseText = await response.Content.ReadAsStringAsync();
        }
        catch (TaskCanceledException)
        {
            return (default, "Request timed out. Check your connection.");
        }
        catch (HttpRequestException ex)
        {
            return (default, $"Network error: {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var msg = TryParseError(responseText)
                   ?? $"Server error ({(int)response.StatusCode})";
            return (default, msg);
        }

        try { return (DTO<R>.FromJson(responseText), null); }
        catch (Exception ex) { return (default, $"Unexpected response: {ex.Message}"); }
    }

    static string TryParseError(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            var err = JsonUtility.FromJson<ErrorResponseDTO>(json);
            return string.IsNullOrEmpty(err?.Message) ? null : err.Message;
        }
        catch { return null; }
    }
}
