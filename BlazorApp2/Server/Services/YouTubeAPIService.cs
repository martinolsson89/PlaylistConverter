using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BlazorApp2.Data.Models;
using Microsoft.AspNetCore.Authentication;

namespace BlazorApp2.Server.Services;

public class YouTubeAPIService
{

    private readonly HttpClient _httpClient;

    public YouTubeAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<string> CreatePlaylist(List<string> playlist, HttpContext context, string accessToken)
    {
        // Extract the playlist name from the first item
        //var playlistName = playlist.FirstOrDefault()?.Name ?? "Default Playlist Name";

        var playlistName = playlist.FirstOrDefault();

        // Prepare the request body
        var requestBody = new
        {
            snippet = new
            {
                title = playlistName,
                description = "Playlist created from Spotify data.",
                privacyStatus = "public"
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        // Retrieve the access token from the authentication properties
        // var authenticateResult = await context.AuthenticateAsync("Cookies");
        // var accessToken = authenticateResult.Properties.GetTokens().FirstOrDefault(t => t.Name == "access_token")?.Value;
        //
        // if (string.IsNullOrEmpty(accessToken))
        // {
        //     // Handle the case where there is no token
        //     Console.WriteLine("Access token is null");
        //     return null;
        // }

        // Set up the HttpClient request
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Make the POST request to the YouTube Data API
        string APIkey = "YOUR_API_KEY";
        var response = await _httpClient.PostAsync($"https://youtube.googleapis.com/youtube/v3/playlists?part=id%2Csnippet&key={APIkey}", content);
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            // Log the errorResponse or throw a detailed exception
            throw new HttpRequestException($"YouTube API call failed: {response.StatusCode}\n{errorResponse}");
        }

        response.EnsureSuccessStatusCode();

        

        // Deserialize the response to get the playlist ID
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
        var responseData = JsonSerializer.Deserialize<YoutubePlaylistResponse>(responseContent);

        return responseData.id;
    }


    public async Task<string> SearchVideo(string query)
    {
        //https://www.googleapis.com/youtube/v3/search
        string APIkey = "YOUR_API_KEY";


        var response = await _httpClient.GetAsync($"https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=1&q={query}&type=video&key={APIkey}");
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            // Log the errorResponse or throw a detailed exception
            throw new HttpRequestException($"YouTube API call failed: {response.StatusCode}\n{errorResponse}");
        }

        response.EnsureSuccessStatusCode();

        // Deserialize the response to get the playlist ID
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
        var responseData = JsonSerializer.Deserialize<YoutubeSearchVideo>(responseContent);

        string videoId = null;
        foreach (var item in responseData.items)
        {
            videoId = item.id.videoId;
            // Use videoId as needed
        }

        return videoId;

    }
    
    public async Task AddVideoToPlaylist(string video_Id, string playlist_Id, string accessToken)
    {
        // https://www.googleapis.com/youtube/v3/playlistItems
        string APIkey = "YOUR_API_KEY";

        // Prepare the request body
        var requestBody = new
        {
            snippet = new
            {
                playlistId = playlist_Id,
                resourceId = new 
                {
                    kind = "youtube#video",
                    videoId = video_Id
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        // // Retrieve the access token from the authentication properties
        // var authenticateResult = await context.AuthenticateAsync("Cookies");
        // var accessToken = authenticateResult.Properties.GetTokens().FirstOrDefault(t => t.Name == "access_token")?.Value;
        //
        // if (string.IsNullOrEmpty(accessToken))
        // {
        //     // Handle the case where there is no token
        //     
        // }

        // Set up the HttpClient request
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var content = new StringContent(json, Encoding.UTF8, "application/json");


        //var response = await _httpClient.PostAsync($"https://www.googleapis.com/youtube/v3/playlistItems?part=id&key={APIkey}", content);
        // You might not need the API key in the URL if you're using OAuth 2.0 authorization
        var response = await _httpClient.PostAsync("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            // Log the errorResponse or throw a detailed exception
            throw new HttpRequestException($"YouTube API call failed: {response.StatusCode}\n{errorResponse}");
        }

        response.EnsureSuccessStatusCode();

    }



}