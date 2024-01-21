using Newtonsoft.Json;
using SpotifySongGuessingGame.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	public class SpotifyDatabase : IEnumerable<ProperSongModel>
	{
		private HashSet<ProperSongModel> allSongs;

		private readonly ConfigManager configManager;
		private readonly ReleaseDateCorrectionService releaseDateCorrectionService;
		private string databaseLocationPath;

		public event Action<string> MessageReceived;

		public SpotifyDatabase(ConfigManager configManager, ReleaseDateCorrectionService releaseDateCorrectionService)
		{
			allSongs = new HashSet<ProperSongModel>();
			this.configManager = configManager;
			this.releaseDateCorrectionService = releaseDateCorrectionService;
			ReloadDatabase();
		}

		public void ReloadDatabase()
		{
			databaseLocationPath = configManager.Get(ConfigKeys.DatabaseLocation);
			if (!string.IsNullOrWhiteSpace(databaseLocationPath))
			{
				LoadData();
			}
		}

		public Task ParseResponseIntoDatabase(SpotifyPlaylistResponse response)
		{
			return AddSongs(response.items.Select(ProperSongModel.Parse));
		}

		private async Task AddSongs(IEnumerable<ProperSongModel> newsongs)
		{
			allSongs.AddRangeUnique(newsongs);
			await UpdateReleaseDates();
			await SaveData();
		}

		public async Task UpdateReleaseDates()
		{
			int counter = 0;
			foreach (var song in allSongs)
			{
				counter++;
				MessageReceived?.Invoke($"({counter} / {allSongs.Count}) Updating: {song.Artist} - {song.SongName}");
				await releaseDateCorrectionService.UpdateSong(song);
			}
		}

		private Task SaveData()
		{
			return File.WriteAllTextAsync(databaseLocationPath, JsonConvert.SerializeObject(allSongs));
		}

		private void LoadData()
		{
			if (!File.Exists(databaseLocationPath))
			{
				File.WriteAllText(databaseLocationPath, JsonConvert.SerializeObject(new List<ProperSongModel>()));
			}

			var databaseJson = File.ReadAllText(databaseLocationPath);
			allSongs = JsonConvert.DeserializeObject<HashSet<ProperSongModel>>(databaseJson) ?? new HashSet<ProperSongModel>();
		}

		public IEnumerator<ProperSongModel> GetEnumerator()
		{
			return allSongs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return allSongs.GetEnumerator();
		}
	}
}
