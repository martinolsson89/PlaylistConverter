using Newtonsoft.Json;

namespace BlazorApp2.Data.Models;

public class SpotifyTrack
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("artists")]
    public List<SpotifyArtist> Artists { get; set; }
}

public class SpotifyArtist
{
    [JsonProperty("name")]
    public string Name { get; set; }
}

public class SpotifyPlaylistResponse
{
    [JsonProperty("name")]
    public string PlaylistName { get; set; }

    [JsonProperty("tracks")]
    public SpotifyTracks Tracks { get; set; }
}

public class SpotifyTracks
{
    [JsonProperty("items")]
    public List<SpotifyTrackItem> Items { get; set; }
}

public class SpotifyTrackItem
{
    [JsonProperty("track")]
    public SpotifyTrack Track { get; set; }
}