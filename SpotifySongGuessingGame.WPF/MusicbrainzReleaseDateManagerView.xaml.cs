using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
		private readonly ReleaseDateCorrectionService releaseDateCorrection;

		public MusicbrainzReleaseDateManagerView(SpotifyDatabase spotifyDatabase, ReleaseDateCorrectionService releaseDateCorrection)
		{
			InitializeComponent();
			this.spotifyDatabase = spotifyDatabase;
			this.releaseDateCorrection = releaseDateCorrection;
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
			var list = songs.Where(x => x.ReleaseYearMusicbrainz == 0);
			int counter = 1;
			var listCount = list.Count();
			UpdateStatus("Starting...");
			foreach (var song in list)
			{
				try
				{
					var previousYear = song.ReleaseYearSpotify;
					await releaseDateCorrection.UpdateSong(song);
					UpdateStatus($"[{counter++} / {listCount}] Updated {song.Artist} - {song.SongName} | {previousYear} -> {song.ReleaseYearSpotify}");
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}
			try
			{
				await spotifyDatabase.AddSongs(filename, songs);
				UpdateStatus("Done!");
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		private void UpdateStatus(string msg)
		{
			App.Current.Dispatcher.Invoke(() => statusReportLabel.Text = msg);
		}
	}
}
