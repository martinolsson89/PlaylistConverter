using BlazorApp2.Client.Services;
using BlazorApp2.Server.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;

namespace BlazorApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add services to the container.
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "Google";
                })
                .AddCookie()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "YOUR_CLIENT_ID";
                    googleOptions.ClientSecret = "YOUR_CLIENT_SECRET";
                    googleOptions.Scope.Add(YouTubeService.Scope.Youtube);
                    googleOptions.SaveTokens = true; // Important to save the tokens in the cookie
                });


            builder.Services.AddAuthorization();



            builder.Services.AddCors(policy =>
            {
                policy.AddPolicy("CorsAllAccessPolicy", opt =>
                    opt.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            
            builder.Services.AddHttpClient<SpotifyAPIService>(client =>
                        {
                client.BaseAddress = new Uri("https://api.spotify.com/v1/playlists/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddScoped<YouTubeAPIService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsAllAccessPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.MapGet("/auth/login", (HttpContext httpContext) =>
            {
                string redirectUri = "/auth/callback";
                return Results.Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, new[] { "Google" });
            });

            app.MapGet("/auth/callback", async (HttpContext httpContext) =>
            {
                var authenticateResult = await httpContext.AuthenticateAsync("Cookies");

                if (!authenticateResult.Succeeded)
                    return Results.Redirect("/auth/login");
                
                // Here, you might want to do something with the authenticateResult.Principal or store the tokens
                // Optionally, you can issue a local sign-in to create an application cookie

                // Once authentication is successful, redirect to the CreatePlaylist page on the front end
                
                Console.WriteLine("Login succeeded with Google!");
                string frontendRedirectUri = $"https://localhost:5001/playlist";
                return Results.Redirect(frontendRedirectUri);
            });

            app.MapPost("/CreatePlaylist/{playlistName}", async (string playlistName, HttpContext httpContext) =>
            {
                // Retrieve the saved cookie
                var accessToken = await httpContext.GetTokenAsync("access_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return Results.BadRequest("Access token not found.");
                }

                // Create the YouTubeService using the access token
                var credential = GoogleCredential.FromAccessToken(accessToken);
                using var youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "YouTube Playlist Creator"
                });

                // Rest of your YouTube API call...

                var newPlaylist = new Playlist
                {
                    Snippet = new PlaylistSnippet
                    {
                        Title = playlistName,
                        Description = "Created with YouTube API v3 via ASP.NET Core Minimal API"
                    },
                    Status = new PlaylistStatus { PrivacyStatus = "public" }
                };

                newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

                return Results.Ok(new { PlaylistId = newPlaylist.Id, Message = "Playlist created successfully." });
            });

            app.MapPost("/CreateYouTubePlaylist/{playlistId}/{youtubePlaylistId}", async (HttpContext httpContext, SpotifyAPIService spotifyApiService, YouTubeAPIService youtubeApiService, string playlistId, string youtubePlaylistId) =>
            {
                // Retrieve the saved cookie
                var accessToken = await httpContext.GetTokenAsync("access_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return Results.BadRequest("Access token not found.");
                }

                // Call the Spotify API to get the playlist data
                var spotifyPlaylistData = await spotifyApiService.GetDataAsync(playlistId);

                // Call the YouTube API to create the playlist
               // var youtubePlaylistId = await youtubeApiService.CreatePlaylist(spotifyPlaylistData, httpContext, accessToken);
                
                // Search and Add Videos to Playlist
                int count = 0;

                foreach (var track in spotifyPlaylistData) 
                {
                    if (count == 0)
                    {
                        Console.WriteLine(track);
                        count++;
                    }
                    else
                    {
                        var videoId = await youtubeApiService.SearchVideo($"{track}");
                        await youtubeApiService.AddVideoToPlaylist(videoId, youtubePlaylistId, accessToken);
                    }
                    
                }
                
                
                return Results.Ok();

            });


            app.Run();
        }
    }
}
