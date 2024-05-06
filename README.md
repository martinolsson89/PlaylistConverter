# PlaylistConverter

### Convertering Spotifyplaylist to a Youtube playlist by using Spotify and Youtube APIs. 

## How to use the app:
### - First login in to the app using your google account, this is neccecary beacuse we need to create a playlist on your youtube account.
### - Copy a Spotifyplaylist URL and paste into the input field. 
- Press the get playlist button in order to load the playlist into the app and to view all of the songs on the screen.
- Name your new Youtube playlist and create it by pressing the button "Create playlist". 
- Press transferplaylist and all of the songs will be loaded into your new Youtube playlist.
- When all the songs are transfered you will get a link to your Youtube playlist.

## How it works:
### - By using Spotifys devloper API we can get all information about a playlist by entering the playlist URL. 
- We then can filter the information so we only get the (Artist and Song) for each track in the playlist.
- We then use Youtubes API to create a new playlist on your youtube account.
- We will get a link back that we will give the user when the playlist is done.
- We use Youtubes API again to search for each track that we got from the Spotify API (Artist and Song) and add it to our newly created playlist. 
- This procces will continue until we have gone through all of the tracks.
- When the process is done we will print the playlist URL to the user.


