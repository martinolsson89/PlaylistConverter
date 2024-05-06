using System.Net.Http.Json;
using BlazorApp2.Data.Models;

namespace BlazorApp2.Client.Services;

public class SpotifyAPIService
{
    private readonly HttpClient _httpClient;

    public SpotifyAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /*public async Task<IEnumerable<PlaylistItem>> GetPlaylistDataAsync(string playlistId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:8888/SpotifyPlaylist/{playlistId}");
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"API request failed: {response.StatusCode}\n{errorContent}");
        }


        var playlistData = await response.Content.ReadFromJsonAsync<IEnumerable<PlaylistItem>>();
        return playlistData ?? new List<PlaylistItem>();
    }*/

    public async Task<List<string>> GetDataAsync(string playlistId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:8888/SpotifyPlaylist/{playlistId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"API request failed: {response.StatusCode}\n{errorContent}");
        }

        var playlistData = await response.Content.ReadFromJsonAsync<List<string>>();
        return playlistData ?? new List<string>();
    }
}