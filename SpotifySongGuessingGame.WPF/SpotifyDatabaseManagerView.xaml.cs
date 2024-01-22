using Newtonsoft.Json;
using SpotifySongGuessingGame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifySongGuessingGame.WPF
{
	public partial class SpotifyDatabaseManagerView : Window
	{
		private readonly ConfigManager configManager;
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly SpotifyCredentialsProvider credentialsProvider;

		public SpotifyDatabaseManagerView(ConfigManager configManager, SpotifyDatabase spotifyDatabase, SpotifyCredentialsProvider credentialsProvider)
		{
			InitializeComponent();

			this.configManager = configManager;
			this.spotifyDatabase = spotifyDatabase;
			this.credentialsProvider = credentialsProvider;
			this.spotifyDatabaseLocationTextBox.Text = configManager.Get(ConfigKeys.DatabaseLocation);
		}

		private void spotifyDatabaseLocationTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			configManager.Set(ConfigKeys.DatabaseLocation, spotifyDatabaseLocationTextBox.Text);
		}

		private async void CreateTopSongPlaylistButtonClicked(object sender, RoutedEventArgs e)
		{
			await GenerateSpotifyCredentialsIfNeeded();
			int topXSongs = 2;
			if (int.TryParse(topXSongTextBox.Text, out var x))
			{
				topXSongs = x;
			}
			var playlistId = downloadTopSongsPlaylistDataTextBox.Text;
			var playlist = await DownloadPlaylist(playlistId);
			var targetFileName = $"{playlistId}_top{topXSongs}";
			if (playlist == null)
			{
				return;
			}
			if (spotifyDatabase.IsPlaylistDownloaded(targetFileName))
			{
				UpdateStatus("Playlist already exists!");
				return;
			}

			var list = playlist.ToList();
			var artists = playlist.Select(x => x.ArtistId).Distinct().ToArray();
			using var client = new HttpClient();
			for (int i = 0; i < artists.Length; i++)
			{
				var url = $@"https://api.spotify.com/v1/artists/{artists[i]}/top-tracks?market=HU";
				UpdateStatus($"Downloading top songs of artists {i} / {artists.Length}...");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer", credentialsProvider.TemporaryKey);

				var response = await client.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsStringAsync();
					var parsed = JsonConvert.DeserializeObject<SpotifyArtistTopSongResponse>(result);
					list.AddRange(parsed.tracks.Select(ProperSongModel.Parse).OrderByDescending(x => x.Popularity).Take(topXSongs));
				}
				else
				{
					var error = await response.Content.ReadAsStringAsync();
					UpdateStatus($"Download failed! {error}");
					continue;
				}
				UpdateStatus($"Downloaded top {topXSongs} by {artists.Length} playlist!");
			}
			await spotifyDatabase.AddSongs(targetFileName, list);
		}


		private async void DownloadPlaylistButtonClicked(object sender, RoutedEventArgs e)
		{
			await GenerateSpotifyCredentialsIfNeeded();

			var playlistId = downloadPlaylistDataTextBox.Text;
			var list = await DownloadPlaylist(playlistId);
			if (list == null) return;

			await spotifyDatabase.AddSongs(playlistId, list);

		}

		private async Task<IEnumerable<ProperSongModel>> DownloadPlaylist(string playlistId)
		{
			var url = $"https://api.spotify.com/v1/playlists/{playlistId}/tracks";

			if (spotifyDatabase.IsPlaylistDownloaded(playlistId))
			{
				UpdateStatus("Playlist already downloaded!");
				return spotifyDatabase.Playlists[playlistId];
			}

			var list = new List<ProperSongModel>();
			using var client = new HttpClient();
			while (url != null)
			{
				UpdateStatus("Downloading...");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer", credentialsProvider.TemporaryKey);

				var response = await client.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsStringAsync();
					var parsed = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(result);

					list.AddRange(parsed.items.Select(ProperSongModel.Parse));

					url = parsed.next;
				}
				else
				{
					UpdateStatus($"Download failed! {await response.Content.ReadAsStringAsync()}");
					return null;
				}
				UpdateStatus($"Download success!");
			}
			return list;
		}

		private async Task GenerateSpotifyCredentialsIfNeeded()
		{
			if (string.IsNullOrEmpty(credentialsProvider.TemporaryKey))
			{
				UpdateStatus("Generating token...");
				await credentialsProvider.GenerateNewKey();
				if (!string.IsNullOrEmpty(credentialsProvider.LastError))
				{
					UpdateStatus($"Token generation failed! {credentialsProvider.LastError}");
				}
				else
				{
					UpdateStatus("Token generated successfully!");
				}
			}
		}

		private void UpdateStatus(string msg)
		{
			Application.Current.Dispatcher.Invoke(() => downloadStatusReportLabel.Text = msg);
		}
	}
}
