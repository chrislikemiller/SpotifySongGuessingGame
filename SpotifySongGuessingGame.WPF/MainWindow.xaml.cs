using System.Windows;

namespace SpotifySongGuessingGame.WPF
{
	public partial class MainWindow : Window
	{
		private readonly ConfigManager configManager;
		private readonly SpotifyDatabase spotifyDatabase;
		private readonly SpotifyCredentialsProvider credentialsProvider;
		private readonly ReleaseDateCorrectionService releaseDateCorrection;

		public MainWindow()
		{
			InitializeComponent();

			configManager = new ConfigManager();
			releaseDateCorrection = new ReleaseDateCorrectionService(configManager);
			spotifyDatabase = new SpotifyDatabase(configManager);
			credentialsProvider = new SpotifyCredentialsProvider(configManager);
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

		private void ConfigManagerClicked(object sender, RoutedEventArgs e)
		{
			var window = new ConfigView(configManager);
			window.ShowDialog();
		}

	}
}
