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
			UpdateDatabaseSongCount();
			this.spotifyDatabaseLocationTextBox.Text = configManager.Get(ConfigKeys.DatabaseLocation);
			this.spotifyClientIdTextBox.Text = configManager.Get(ConfigKeys.SpotifyClientId);
			this.spotifyClientSecretTextBox.Text = configManager.Get(ConfigKeys.SpotifyClientSecret);
		}

		private void UpdateDatabaseSongCount()
		{
			databaseNumberOfSongsLabel.Text = $"{spotifyDatabase.Count()} songs";
		}

		private void spotifyClientIdTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			configManager.Set(ConfigKeys.SpotifyClientId, spotifyClientIdTextBox.Text);
		}
		
		private void spotifyClientSecretTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			configManager.Set(ConfigKeys.SpotifyClientSecret, spotifyClientSecretTextBox.Text);
		}

		private void spotifyDatabaseLocationTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			configManager.Set(ConfigKeys.DatabaseLocation, spotifyDatabaseLocationTextBox.Text);
		}

		private async void DownloadPlaylistButtonClicked(object sender, RoutedEventArgs e)
		{
			using (var client = new HttpClient())
			{
				var playlistId = downloadPlaylistDataTextBox.Text;
				var url = $"https://api.spotify.com/v1/playlists/{playlistId}/tracks";
				while (url != null)
				{
					downloadStatusReportLabel.Text = "Downloading...";
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
						"Bearer", credentialsProvider.TemporaryKey);

					var response = await client.GetAsync(url);
					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadAsStringAsync();
						File.WriteAllText($"{playlistId}.json", result);
						var parsed = JsonConvert.DeserializeObject<SpotifyPlaylistResponse>(result);
						downloadStatusReportLabel.Text = $"Success!";
						spotifyDatabase.ParseResponseIntoDatabase(parsed);
						UpdateDatabaseSongCount();

						url = parsed.next;
					}
					else
					{
						downloadStatusReportLabel.Text = $"Download failed! {await response.Content.ReadAsStringAsync()}";
						return;
					}
				}
			}
		}

		private void ReloadDatabaseButtonClicked(object sender, RoutedEventArgs e)
		{
			spotifyDatabase.ReloadDatabase();
			downloadStatusReportLabel.Text = $"Database reloaded";
		}

		private async void TemporaryKeyGenerateClicked(object sender, RoutedEventArgs e)
		{
			await credentialsProvider.GenerateNewKey();
			if (!string.IsNullOrEmpty(credentialsProvider.TemporaryKey))
			{
				downloadStatusReportLabel.Text = "Token generated successfully!";
			}
			else
			{
				downloadStatusReportLabel.Text = $"Token generation failed! {credentialsProvider.LastError}";
			}
		}
	}
}
