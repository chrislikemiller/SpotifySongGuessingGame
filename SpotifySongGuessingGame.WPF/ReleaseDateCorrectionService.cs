using Newtonsoft.Json;
using SpotifySongGuessingGame.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SpotifySongGuessingGame.WPF
{
	public class ReleaseDateCorrectionService
	{
		public void UpdateAllSongs(IEnumerable<ProperSongModel> songs)
		{
			// todo: make it available for individual songs
			foreach (var song in songs)
			{
				Trace.WriteLine($"Next song: {song.Artist} - {song.SongName}");
				var artistQuery = $"\"{HttpUtility.UrlEncode(song.Artist)}\"";
				var artistUrl = $@"https://musicbrainz.org/ws/2/artist?query={artistQuery}&fmt=json";
				MusicbrainzArtistResponse artistData = null;
				var retryCount = 0;
				do
				{
					artistData = GetMusicBrainzDataForUrl<MusicbrainzArtistResponse>(artistUrl);
					if (artistData.artists?.Length > 0)
					{
						retryCount = 9999;
					}
					retryCount++;
					Trace.WriteLine("Waiting...");
					Thread.Sleep(1000);
				}
				while (retryCount < 8);
				if (artistData.artists == null)
				{
					Trace.WriteLine($"@@@ Could not find {song.Artist}");
					Trace.WriteLine(artistUrl);
					continue;
				}

				SongResponseJson ProperSongModel = null;

				var artistList = GetArtistList(song, artistData);
				int currentArtistIndex = 0;

				foreach (var artist in artistList)
				{
					currentArtistIndex++;
					Trace.WriteLine($"Trying artist {currentArtistIndex}/{artistList.Length}");

					var artistId = artist.id;
					if (string.IsNullOrEmpty(artistId))
					{
						Trace.WriteLine($"Artist not found {song.Artist}");
						continue;
					}
					var songQuery = $"{song.SongName} AND arid:{artistId}";
					var songUrl = $"https://musicbrainz.org/ws/2/recording?query={HttpUtility.UrlEncode(songQuery)}&inc=releases&fmt=json";
					ProperSongModel = GetMusicBrainzDataForUrl<SongResponseJson>(songUrl);

					if (ProperSongModel.recordings?.Length != 0)
					{
						break;
					}

					// todo: retry if no songs found, maybe I picked the wrong artist id
					Trace.WriteLine($"Trying another artist {song.SongName}");
				}

				if (ProperSongModel == null || ProperSongModel.recordings.Length == 0)
				{
					// todo: retry if no songs found, maybe I picked the wrong artist id
					Trace.WriteLine($"Skipping because of no artist found {song.SongName}");
					continue;
				}
				var recordings = ProperSongModel.recordings
					.Where(x => x.releases?.Length > 0)
					.Where(x => x.score > 85);

				var allReleases = recordings.SelectMany(x => x.releases)
				.Where(x => !string.IsNullOrEmpty(x.date));

				string smallestYear = "";
				string firstsmallestYear = "";
				if (allReleases.Any())
				{
					smallestYear = allReleases.Select(ParseYearFromRelease).Min();
					firstsmallestYear = smallestYear;
				}
				if (int.Parse(smallestYear) > song.ReleaseYear)
				{
					var releaseDateUrl = $"https://musicbrainz.org/ws/2/recording/{HttpUtility.UrlEncode(recordings.First().id)}?inc=releases&fmt=json";
					var releaseDateData = GetMusicBrainzDataForUrl<ReleaseDateResponseJson>(releaseDateUrl);
					var years = releaseDateData.releases.Where(x => !string.IsNullOrEmpty(x.date)).Select(ParseYearFromRelease);
					smallestYear = years.OrderBy(x => x).First();
				}
				smallestYear = Math.Min(int.Parse(smallestYear), (int)song.ReleaseYear).ToString();
				Trace.WriteLine($"@@@@ {song.Artist} - {song.SongName} ({song.ReleaseYear}) -> {smallestYear}");
			}
		}

		private string ParseYearFromRelease(Release x)
		{
			string result;
			if (DateTime.TryParse(x.date, out var date))
				result = date.Year.ToString();
			else
			{
				result = int.Parse(x.date[..4]).ToString();
			}
			return result;
		}

		private string PickArtistId(ProperSongModel song, MusicbrainzArtistResponse artistData)
		{
			var matching = artistData.artists.Where(x => string.Equals(x.name, song.Artist)).OrderByDescending(x => x.score).ToArray();
			return matching.FirstOrDefault()?.id;
		}

		private MusicbrainzArtist[] GetArtistList(ProperSongModel song, MusicbrainzArtistResponse artistData)
		{
			return artistData.artists.Where(x => string.Equals(x.name, song.Artist)).OrderByDescending(x => x.score).ToArray();
		}

		private T GetMusicBrainzDataForUrl<T>(string url)
		{
			var uri = new Uri(url);
			var credentials = new NetworkCredential("itslikecsaki", "dvx-hvy7ADF3wmr8rwj", "https://musicbrainz.org");
			var cache = new CredentialCache { { uri, "Digest", credentials } };
			using var request = new HttpClient(new HttpClientHandler() { Credentials = cache, PreAuthenticate = true });
			request.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
			var response = request.GetAsync(url).Result;
			using var responseStream = response.Content.ReadAsStream();
			using var responseStreamReader = new StreamReader(responseStream, Encoding.Default);
			var json = responseStreamReader.ReadToEnd();
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
