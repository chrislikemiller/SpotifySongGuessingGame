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
		public async Task UpdateSong(ProperSongModel song)
		{
			if (song.ReleaseDateSource == ReleaseDateSource.Musicbrainz)
			{
				Trace.WriteLine($"Already updated: {song}");
				return;
			}
			Trace.WriteLine($"\nNext song: {song.Artist} - {song.SongName}");

			MusicbrainzArtistResponse artistData = null;
			var artistQuery = $"\"{HttpUtility.UrlEncode(song.Artist)}\"";
			var artistUrl = $@"https://musicbrainz.org/ws/2/artist?query={artistQuery}&fmt=json";
			var retryCount = 0;
			while (retryCount < 20)
			{
				artistData = await GetMusicBrainzDataForUrl<MusicbrainzArtistResponse>(artistUrl);
				if (artistData.artists?.Length > 0)
				{
					break;
				}
				retryCount++;
				Trace.WriteLine("Retrying. Server might be overloaded.");
			}

			if (artistData.artists.Length == 0)
			{
				Trace.WriteLine($"No artists exist in database for {song}");
				Trace.WriteLine(artistUrl);
				return;
			}

			SongResponseJson songModel = null;

			int currentArtistIndex = 0;
			var artistList = artistData.artists
				.OrderByDescending(x => x.score)
				.ToArray();

			foreach (var artist in artistList)
			{
				currentArtistIndex++;
				Trace.WriteLine($"Trying artist: {artist.name} ({currentArtistIndex} / {artistList.Count()})");

				var songQuery = $"{song.SongName} AND arid:{artist.id}";
				var songUrl = $"https://musicbrainz.org/ws/2/recording?query={HttpUtility.UrlEncode(songQuery)}&inc=releases&fmt=json";
				songModel = await GetMusicBrainzDataForUrl<SongResponseJson>(songUrl);

				if (songModel.recordings?.Length != 0)
				{
					Trace.WriteLine($"Found recordings for {song.SongName} by {song.Artist}");
					break;
				}
				Trace.WriteLine($"Trying another artist for {song.SongName}");
			}

			if (songModel.recordings?.Length == 0)
			{
				Trace.WriteLine($"Skipping because of no artist found {song.SongName}");
				return;
			}
			var recordings = songModel.recordings
				.Where(x => x.releases?.Length > 0)
				.Where(x => x.score > 50);

			var allReleases = recordings
				.SelectMany(x => x.releases)
				.Where(x => !string.IsNullOrEmpty(x.date));

			int smallestYear = int.MaxValue;
			if (allReleases.Any())
			{
				smallestYear = GetSmallestYear(allReleases);
			}
			if (smallestYear > song.ReleaseYear)
			{
				Trace.WriteLine($"Smallest year was bigger than spotify, checking more releases");
				var releaseDateUrl = $"https://musicbrainz.org/ws/2/recording/{HttpUtility.UrlEncode(recordings.First().id)}?inc=releases&fmt=json";
				var releaseDateData = await GetMusicBrainzDataForUrl<ReleaseDateResponseJson>(releaseDateUrl);
				smallestYear = GetSmallestYear(releaseDateData.releases);
			}
			smallestYear = Math.Min(smallestYear, song.ReleaseYear);
			Trace.WriteLine($"@@@ {song.Artist} - {song.SongName} || {song.ReleaseYear} -> {smallestYear}");
			song.ReleaseDateSource = ReleaseDateSource.Musicbrainz;
			song.ReleaseYear = smallestYear;
		}

		private int GetSmallestYear(IEnumerable<Release> releases) => releases.Select(x => x.date.ParseYear()).Min();

		private async Task<T> GetMusicBrainzDataForUrl<T>(string url)
		{
			Thread.Sleep(1000); // server is sensitive to overloading, it can throttle you

			var uri = new Uri(url);
			var credentials = new NetworkCredential("itslikecsaki", "dvx-hvy7ADF3wmr8rwj", "https://musicbrainz.org");
			var cache = new CredentialCache { { uri, "Digest", credentials } };
			using var request = new HttpClient(new HttpClientHandler() { Credentials = cache, PreAuthenticate = true });
			request.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
			HttpResponseMessage response = null;
			try
			{
				response = await request.GetAsync(url);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
			using var responseStream = response.Content.ReadAsStream();
			using var responseStreamReader = new StreamReader(responseStream, Encoding.Default);
			var json = responseStreamReader.ReadToEnd();
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
