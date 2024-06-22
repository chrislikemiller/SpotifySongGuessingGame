using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifySongGuessingGame.WPF
{
	public partial class MusicbrainzReleaseDateManagerView : Window
	{
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly MusicbrainzReleaseDateCorrection releaseDateCorrection;
		private CancellationTokenSource cto;

		public MusicbrainzReleaseDateManagerView(SpotifyDatabase spotifyDatabase, MusicbrainzReleaseDateCorrection releaseDateCorrection)
		{
			InitializeComponent();
			this.spotifyDatabase = spotifyDatabase;
			this.releaseDateCorrection = releaseDateCorrection;

			Closing += MusicbrainzReleaseDateManagerView_Closing;
		}

		private void MusicbrainzReleaseDateManagerView_Closing(object sender, EventArgs e)
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
			var filename = new FileInfo(playlistFilePathLabel.Text).Name.Split(".").First();
			var songs = spotifyDatabase.Playlists[filename];
			var list = songs.Where(x => x.ReleaseYearAutocorrected == 0);
			int counter = 1;
			var listCount = list.Count();
			cto = new CancellationTokenSource();

			UpdateStatus("Starting...");
			StartProcessButton.IsEnabled = false;
			foreach (var song in list)
			{
				try
				{
					var previousYear = song.ReleaseYearSpotify;
					await releaseDateCorrection.UpdateSong(song, cto.Token);
					UpdateStatus($"[{counter++} / {listCount}] Updated {song.Artist} - {song.SongName} | {previousYear} -> {song.ReleaseYearAutocorrected}");
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}
			try
			{
				await spotifyDatabase.AddSongs(filename, songs);
				UpdateStatus("Saved successfully!");
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				UpdateStatus($"Faield to save to db! {ex.Message}");
			}
			StartProcessButton.IsEnabled = true;
		}

		private void UpdateStatus(string msg)
		{
			App.Current.Dispatcher.Invoke(() => statusReportLabel.Text = msg);
		}
	}
}
