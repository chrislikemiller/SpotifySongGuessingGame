using Newtonsoft.Json;
using SpotifySongGuessingGame.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifySongGuessingGame.WPF
{
	public class SpotifyDatabase
	{
		private string databaseLocationPath;

		public SpotifyDatabase(ConfigManager configManager)
		{
			databaseLocationPath = configManager.Get(ConfigKeys.DatabaseLocation);
			Playlists = new Dictionary<string, HashSet<ProperSongModel>>();
			LoadAllPlaylists();
		}

		public Dictionary<string, HashSet<ProperSongModel>> Playlists { get; set; }

		public async Task AddSongs(string playlistId, IEnumerable<ProperSongModel> newsongs)
		{
			GetPlaylist(playlistId).AddRangeUnique(newsongs);
			await SaveData(playlistId);
		}

		private HashSet<ProperSongModel> GetPlaylist(string playlistId)
		{
			if (!Playlists.ContainsKey(playlistId))
			{
				Playlists[playlistId] = new HashSet<ProperSongModel>();
			}
			return Playlists[playlistId];
		}

		public bool IsPlaylistDownloaded(string playlistId)
		{
			return File.Exists(GetPlaylistFilePath(playlistId));
		}

		private Task SaveData(string playlistId)
		{
			return File.WriteAllTextAsync(GetPlaylistFilePath(playlistId), JsonConvert.SerializeObject(Playlists[playlistId]));
		}

		private void LoadAllPlaylists()
		{
			if (string.IsNullOrEmpty(databaseLocationPath))
			{
				MessageBox.Show("Welcome!\nTo use the app properly, please set the following:\n- Download Folder in Spotify Song Manager\n- Spotify app client ID/secret in configs\n- Musicbrainz ID/PW in configs (if it applies to you)", "Welcome", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			if (!Directory.Exists(databaseLocationPath))
			{
				Directory.CreateDirectory(databaseLocationPath);
			}
			var files = Directory.GetFiles(databaseLocationPath);
			var playlistIds = files.Select(x => new FileInfo(x).Name.Split(".").First());
			foreach (var playlistId in playlistIds)
			{
				var path = GetPlaylistFilePath(playlistId);
				if (!File.Exists(path))
				{
					File.CreateText(path);
				}

				var databaseJson = File.ReadAllText(path);
				Playlists[playlistId] = JsonConvert.DeserializeObject<HashSet<ProperSongModel>>(databaseJson) ?? new HashSet<ProperSongModel>();


				Trace.WriteLine("Grouped by year:");
				var groups = Playlists[playlistId].GroupBy(x => x.ReleaseYearMusicbrainz / 10);
				foreach (var g in groups)
				{
					Trace.WriteLine($"{g.Key}0's | {g.Count()}");
				}
				var groups2 = Playlists[playlistId].GroupBy(x => x.SongName).Where(x => x.Count() > 1);
				var toRemove = new List<ProperSongModel>();
				foreach (var g in groups2)
				{
					Trace.WriteLine($"{g.Key} | {g.Count()}");
					toRemove.Add(g.OrderBy(x => x.Popularity).First());
				}
			}
		}

		private string GetPlaylistFilePath(string playlistId)
		{
			return Path.Combine(databaseLocationPath, $"{playlistId}.json");
		}
	}
}
