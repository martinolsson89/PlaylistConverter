namespace BlazorApp2.Data.Models;

public class SpotifyToken
{
    public string Access_token { get; set; }

    public string Token_type { get; set; }

    public int Expires_in{ get; set; }
}