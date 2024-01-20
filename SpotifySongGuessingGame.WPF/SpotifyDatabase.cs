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
		private string databaseLocationPath;

		public SpotifyDatabase(ConfigManager configManager)
		{
			allSongs = new HashSet<ProperSongModel>();
			this.configManager = configManager;
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

		public void ParseResponseIntoDatabase(SpotifyPlaylistResponse response)
		{
			AddSongs(response.items.Select(ProperSongModel.Parse));
		}

		public void AddSongs(IEnumerable<ProperSongModel> newsongs)
		{
			allSongs.AddRangeUnique(newsongs);
			SaveData();
		}

		public void UpdateSongs(Action<ProperSongModel> updateFunc)
		{
			foreach (var song in allSongs)
			{
				updateFunc(song);
			}
		}

		private void SaveData()
		{
			File.WriteAllText(databaseLocationPath, JsonConvert.SerializeObject(allSongs));
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
