
using BlazorApp2.Spotify.API.Service;
using System.Net.Http.Headers;

namespace BlazorApp2.Spotify.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient<SpotifyAuth>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
            builder.Services.AddSingleton<SpotifyAuth>();
            builder.Services.AddSingleton<SpotifyService>();

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddCors(policy =>
            {
                policy.AddPolicy("CorsAllAccessPolicy", opt =>
                    opt.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsAllAccessPolicy");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/SpotifyPlaylist/{playlist_id}",
                async (SpotifyAuth spotifyAuth, SpotifyService spotifyService, string playlist_id) =>
                {
                    var accessToken = await spotifyAuth.GetAccessTokenAsync();
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return Results.Problem("Unable to obtain access token.");
                    }

                    var playlistData = await spotifyService.GetSpotifyPlaylistAsync(accessToken, playlist_id);

                    try
                    {
                        return Results.Ok(playlistData);
                    }
                    catch
                    {
                        return Results.Problem("Unable to obtain playlist data.");
                    }

                });

            app.Run();
        }
    }
}
