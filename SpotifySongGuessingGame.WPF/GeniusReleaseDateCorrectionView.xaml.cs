using SpotifySongGuessingGame.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
		private volatile bool isStarted;

		public GeniusReleaseDateCorrectionView(SpotifyDatabase spotifyDatabase, ConfigManager configManager)
		{
			InitializeComponent();

			Closing += GeniusReleaseDateCorrectionView_Closing;
			this.spotifyDatabase = spotifyDatabase;
			this.configManager = configManager;
			geniusAccessTokenTextBox.Text = configManager.Get(ConfigKeys.GeniusAPIAccessToken);
			isStarted = false;
		}

		private void GeniusReleaseDateCorrectionView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Stop();
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
			await StartProcessInner();
		}

		private async Task StartProcessInner()
		{
			if (isStarted)
			{
				Stop();
				return;
			}

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


			Start();
			var genius = new GeniusAPIService(accessToken);
			var filename = new FileInfo(playlistFile).Name.Split(".").First();
			var songs = spotifyDatabase.Playlists[filename];
			var list = songs.Where(x => x.ReleaseYearAutocorrected == 0);
			int counter = 0;
			var listCount = list.Count();
			if (listCount == 0)
			{
				UpdateStatus("Songs are all updated!");
				Stop();
				return;
			}
			cto = new CancellationTokenSource();
			var failedList = new List<ProperSongModel>();
			foreach (var song in list)
			{
				try
				{
					counter++;
					UpdateStatus($"[{counter} / {listCount}] Querying: {song.Artist} - {song.SongName}");
					if (cto.Token.IsCancellationRequested)
					{
						UpdateStatus($"Cancelled after {counter}, saving...");
						await spotifyDatabase.AddSongs(filename, songs);
						UpdateStatus($"Cancelled after {counter}, saving... done!");
						isStarted = false;
						return;
					}

					var releaseYearStr = await genius.GetReleaseDate(song.Artist, song.SongName);
					if (!string.IsNullOrEmpty(releaseYearStr)
						&& int.TryParse(releaseYearStr, out var releaseYear))
					{
						var previousYear = song.ReleaseYearSpotify;
						song.ReleaseYearAutocorrected = releaseYear;
						if (releaseYear < 1900)
						{
							Trace.WriteLine($"ERROR: weird date {releaseYear} for {song.Artist} {song.SongName}");
						}
						UpdateStatus($"[{counter} / {listCount}] Updated {song.Artist} - {song.SongName} | {previousYear} -> {song.ReleaseYearAutocorrected}");
						await Task.Delay(500);
					}
					else
					{
						failedList.Add(song);
					}
				}
				catch (Exception ex)
				{
					UpdateStatus($"Error in genius api: {ex}");
				}
				Thread.Sleep(500);
			}
			foreach (var item in failedList)
			{
				Trace.WriteLine($"@@ Did not update: {item.Artist} - {item.SongName}");
			}
			await spotifyDatabase.AddSongs(filename, songs);
			Stop();

			UpdateStatus($"Finished! Updated {listCount - failedList.Count} out of {listCount} songs");
		}

		private void UpdateStatus(string msg)
		{
			App.Current.Dispatcher.Invoke(() => statusReportLabel.Text = msg);
		}

		private void Start()
		{
			isStarted = true;
			startProcessButton.Content = "Cancel";
		}
		private void Stop()
		{
			isStarted = false;
			cto?.Cancel();
			startProcessButton.Content = "Start process";
		}
	}
}
