namespace BlazorApp2.Data.Models;

public class SpotifyPlaylistTracks
{
    public List<Item> Items { get; set; }
}
public class Item
{
    public Track Track { get; set; }
}

public class Track
{
    public string Name { get; set; }
    public List<Artist> Artists { get; set; }
}

public class Artist
{
    public string Name { get; set; }
}