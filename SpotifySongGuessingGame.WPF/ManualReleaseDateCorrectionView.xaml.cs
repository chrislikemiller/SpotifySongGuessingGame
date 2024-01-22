using SpotifySongGuessingGame.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
	public partial class ManualReleaseDateCorrectionView : Window
	{
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly ReleaseDateCorrectionService releaseDateCorrection;
		private string playlistId;
		private int songIndex;
		private ProperSongModel[] songs;
		private ProperSongModel song;

		public ManualReleaseDateCorrectionView(SpotifyDatabase spotifyDatabase, ReleaseDateCorrectionService releaseDateCorrection)
		{
			InitializeComponent();
			this.spotifyDatabase = spotifyDatabase;
			this.releaseDateCorrection = releaseDateCorrection;

			TurnYearControlsOn(false);
			ResetSong();
		}

		private void TurnYearControlsOn(bool turnYearOn)
		{
			filePickerButton.IsEnabled = !turnYearOn;
			startProcessButton.IsEnabled = !turnYearOn;
			nextButton.IsEnabled = turnYearOn;
			stopButton.IsEnabled = turnYearOn;
			yearTextBox.IsEnabled = turnYearOn;
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

		private void StartProcessClicked(object sender, RoutedEventArgs e)
		{
			TurnYearControlsOn(true);

			playlistId = new FileInfo(playlistFilePathLabel.Text).Name.Split(".").First();
			songs = spotifyDatabase.Playlists[playlistId]
				.Where(x => x.ReleaseYearMusicbrainz != x.ReleaseYearSpotify)
				.ToArray();
			NextSong(false);
		}

		private void NextSong(bool doIncrement)
		{
			yearTextBox.Text = "";
			song = songs[songIndex];
			UpdateStatus($"[{songIndex} / {songs.Length}] {song}");
			var sanitized = HttpUtility.UrlEncode($"{song.Artist} {song.SongName} release date");
			Process.Start(new ProcessStartInfo($"https://www.google.com/search?q={sanitized}") { UseShellExecute = true });
			if (doIncrement)
			{
				songIndex++;
			}
		}

		private void UpdateStatus(string msg)
		{
			App.Current.Dispatcher.Invoke(() => statusReportLabel.Text = msg);
		}

		private void nextButton_Click(object sender, RoutedEventArgs e)
		{
			var newYear = yearTextBox.Text;
			if (string.IsNullOrEmpty(newYear))
			{
				Trace.WriteLine($"Skipping {song}");
				NextSong(true);
				return;
			}

			if (int.TryParse(newYear, out int year))
			{
				Trace.WriteLine($"{newYear} - {song}");
				song.ReleaseYearSpotify = year;
				song.ReleaseYearMusicbrainz = year;
				NextSong(true);
			}
			else
			{
				UpdateStatus($"Error! - {songs[songIndex]}");
			}
		}

		private async void stopButton_Click(object sender, RoutedEventArgs e)
		{
			await spotifyDatabase.AddSongs(playlistId, songs);
			ResetSong();
			TurnYearControlsOn(false);
			UpdateStatus("Done!");
		}

		private void ResetSong()
		{
			playlistId = null;
			songs = null;
			song = null;
			songIndex = 0;
		}
	}
}
