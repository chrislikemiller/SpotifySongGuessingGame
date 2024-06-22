using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using MessageBox = System.Windows.MessageBox;

namespace SpotifySongGuessingGame.WPF
{
	/// <summary>
	/// Interaction logic for GoogleSearchReleaseDateParserView.xaml
	/// </summary>
	public partial class GeniusReleaseDateCorrectionView : Window
	{
		private CancellationTokenSource cto;
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly ConfigManager configManager;

		public GeniusReleaseDateCorrectionView(SpotifyDatabase spotifyDatabase, ConfigManager configManager)
		{

			InitializeComponent();

			Closing += GeniusReleaseDateCorrectionView_Closing;
			this.spotifyDatabase = spotifyDatabase;
			this.configManager = configManager;
			geniusAccessTokenTextBox.Text = configManager.Get(ConfigKeys.GeniusAPIAccessToken);
		}

		private void GeniusReleaseDateCorrectionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			cto?.Cancel();
		}

		private void FilePickerClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "|*.json";
			var result = dialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				playlistFilePathLabel.Text = dialog.FileName;
			}
		}

		private async void StartProcessClicked(object sender, RoutedEventArgs e)
		{
			var accessToken = geniusAccessTokenTextBox.Text;
			if (string.IsNullOrEmpty(accessToken))
			{
				MessageBox.Show("Must enter Genius API access token first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			configManager.Set(ConfigKeys.GeniusAPIAccessToken, geniusAccessTokenTextBox.Text);

			var playlistFile = playlistFilePathLabel.Text;
			if (string.IsNullOrEmpty(playlistFile))
			{
				MessageBox.Show("Must pick a playlist file first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			StartProcessButton.IsEnabled = false;
			var genius = new GeniusAPIService(accessToken);
			var filename = new FileInfo(playlistFile).Name.Split(".").First();
			var songs = spotifyDatabase.Playlists[filename];
			var list = songs.Where(x => x.ReleaseYearAutocorrected == 0);
			int counter = 1;
			var listCount = list.Count();
			cto = new CancellationTokenSource();
			foreach (var song in list)
			{
				try
				{
					if (cto.Token.IsCancellationRequested)
					{
						return;
					}
					var releaseDate = await genius.GetReleaseDate(song.Artist, song.SongName);
					if (DateTime.TryParse(releaseDate, out var date))
					{
						var previousYear = song.ReleaseYearSpotify;
						song.ReleaseYearAutocorrected = date.Year;
						if (date.Year < 1900)
						{
							Trace.WriteLine($"ERROR: weird date {date.Year} for {song.Artist} {song.SongName}");
						}
						UpdateStatus($"[{counter++} / {listCount}] Updated {song.Artist} - {song.SongName} | {previousYear} -> {song.ReleaseYearAutocorrected}");
					}
				}
				catch (Exception ex)
				{
					UpdateStatus($"Error in genius api: {ex}");
				}
				Thread.Sleep(500);
			}
			await spotifyDatabase.AddSongs(filename, songs);
			StartProcessButton.IsEnabled = true;
		}

		private void UpdateStatus(string msg)
		{
			App.Current.Dispatcher.Invoke(() => statusReportLabel.Text = msg);
		}
	}
}
