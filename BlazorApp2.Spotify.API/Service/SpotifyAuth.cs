using System.Text;
using BlazorApp2.Data.Models;
using Newtonsoft.Json;

namespace BlazorApp2.Spotify.API.Service;

public class SpotifyAuth
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public SpotifyAuth(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var clientId = _configuration["SpotifyConfig:ClientId"];
        var clientSecret = _configuration["SpotifyConfig:ClientSecret"];
        
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Headers = {
                { "Authorization", $"Basic {credentials}" }
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            })
        };

        var response = await _httpClient.SendAsync(tokenRequest);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();


        var tokenResponse = JsonConvert.DeserializeObject<SpotifyToken>(content);

        return tokenResponse!.Access_token;
    }
}