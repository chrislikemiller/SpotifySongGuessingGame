using SpotifySongGuessingGame.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SpotifySongGuessingGame.WPF
{
	public partial class MainWindow : Window
	{
		private readonly ConfigManager configManager;
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly SpotifyCredentialsProvider credentialsProvider;
		private readonly ImageService imageService;
		private readonly ReleaseDateCorrectionService releaseDateCorrection;

		public MainWindow()
		{
			InitializeComponent();

			configManager = new ConfigManager();
			releaseDateCorrection = new ReleaseDateCorrectionService(configManager);
			spotifyDatabase = new SpotifyDatabase(configManager);
			credentialsProvider = new SpotifyCredentialsProvider(configManager);
			imageService = new ImageService(configManager);
		}

		private void SpotifyDataBaseOpenClicked(object sender, RoutedEventArgs e)
		{
			var window = new SpotifyDatabaseManagerView(configManager, spotifyDatabase, credentialsProvider);
			window.ShowDialog();
		}

		private void MusicbrainzManagerClicked(object sender, RoutedEventArgs e)
		{
			var window = new MusicbrainzReleaseDateManagerView(spotifyDatabase, releaseDateCorrection);
			window.ShowDialog();
		}

		private void ManualReleaseDateManagerClicked(object sender, RoutedEventArgs e)
		{
			var window = new ManualReleaseDateCorrectionView(spotifyDatabase, releaseDateCorrection);
			window.ShowDialog();
		}

		private void ConfigManagerClicked(object sender, RoutedEventArgs e)
		{
			var window = new ConfigView(configManager);
			window.ShowDialog();
		}

		private async void GenerateImagesClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "JSON files|*.json";
			var result = dialog.ShowDialog();

			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			try
			{
				var playlistId = new FileInfo(dialog.FileName).Name.Split(".").First();
				var songs = spotifyDatabase.Playlists[playlistId];

				await imageService.GenerateAllImages(playlistId, songs);
				MessageBox.Show($"Generated {songs.Count} images!");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error! {ex}");
			}
		}

		private void GenerateIconCollageClicked(object sender, RoutedEventArgs e)
		{
			var path = @"black_rectangle_icon.png";
			imageService.CreateCollageOfToken(path);
		}

		private void CreateCollagesClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "JSON files|*.json";
			var result = dialog.ShowDialog();

			if (result != System.Windows.Forms.DialogResult.OK)
				return;

			try
			{
				var playlistId = new FileInfo(dialog.FileName).Name.Split(".").First();
				var songs = spotifyDatabase.Playlists[playlistId].ToList();

				var splitLists = songs.SplitList(20);
				for (int i = 0; i < splitLists.Count; i++)
				{
					imageService.CreateCollage(playlistId, splitLists[i], i);
				}
				MessageBox.Show("Creating collages finished");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error! {ex}");
			}
		}

	}
}
