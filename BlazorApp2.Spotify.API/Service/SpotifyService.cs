using BlazorApp2.Data.Models;
using Newtonsoft.Json;

namespace BlazorApp2.Spotify.API.Service;

public class SpotifyService
{

    private readonly HttpClient _httpClient;

    public SpotifyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<string> GetPlaylistNameAsync(string accessToken, string playlistId)
    {

        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/{playlistId}?market=se")
        {
            Headers = { { "Authorization", $"Bearer {accessToken}" } }
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        var playlist = JsonConvert.DeserializeObject<SpotifyPlaylistName>(jsonResponse);

        return playlist!.Name;
    }

    public async Task<string> GetPlaylistTracksAsync(string accessToken, string playlistId, string market = "se")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/{playlistId}/tracks?market={market}")
        {
            Headers = { { "Authorization", $"Bearer {accessToken}" } }
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<List<PlaylistItem>> GetPlaylistDataAsync(string accessToken, string playlistId)
    {

        var playlistName = await GetPlaylistNameAsync(accessToken, playlistId);
        if (playlistName == null)
        {
            //return Results.Problem("Unable to fetch playlist data.");
            Console.WriteLine("Unable to fetch playlist data.");
            return null;
        }

        var playlistTracksJSON = await GetPlaylistTracksAsync(accessToken, playlistId);
        if (playlistTracksJSON == null)
        {
            //return Results.Problem("Unable to fetch playlist data.");
            Console.WriteLine("Unable to fetch playlist data.");
            return null;
        }

        var playlistTracks = JsonConvert.DeserializeObject<SpotifyPlaylistTracks>(playlistTracksJSON);
        
        // var artistAndSongs = playlistTracks.Items
        //     .SelectMany(item => item.Track.Artists
        //         .Select(artist => new { Artist = artist.Name, Song = item.Track.Name }))
        //     .ToList();
        //
        //
        // return Results.Ok(playlistTracksJSON);

        var artistAndSongs = new List<PlaylistItem>();
        artistAndSongs.Add(new PlaylistItem { Name = playlistName });
        
        artistAndSongs.AddRange(playlistTracks!.Items
            .SelectMany(item => item.Track.Artists
                .Select(artist => new PlaylistItem { Artist = artist.Name, Song = item.Track.Name }))
            .ToList());
        
        //return Results.Ok(artistAndSongs);
        return artistAndSongs;
    }

    public async Task<List<string>> GetSpotifyPlaylistAsync(string accessToken, string playlistId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/{playlistId}?market=se");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var playlistResponse = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(content);

        var result = new List<string> { playlistResponse.PlaylistName };

        foreach (var item in playlistResponse.Tracks.Items)
        {
            var trackName = item.Track.Name;
            var artistName = item.Track.Artists[0].Name; // Assuming there's at least one artist per track
            result.Add($"{artistName} - {trackName}");
        }

        return result;
    }
}