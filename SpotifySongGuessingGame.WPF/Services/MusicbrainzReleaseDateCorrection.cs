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
	public class MusicbrainzReleaseDateCorrection
	{
		private readonly ConfigManager configManager;

		public MusicbrainzReleaseDateCorrection(ConfigManager configManager)
		{
			this.configManager = configManager;
		}

		public async Task UpdateSong(ProperSongModel song, CancellationToken token)
		{
			Trace.WriteLine($"\nNext song: {song.Artist} - {song.SongName}");

			MusicbrainzArtistResponse artistData = null;
			IEnumerable<Recording> recordings = null;
			int smallestYear = int.MaxValue;
			var artistQueryOriginal = $"\"{HttpUtility.UrlEncode(song.Artist)}\"";
			var artistQueryReversed = $"\"{HttpUtility.UrlEncode(song.Artist.ReverseWords())}\"";
			var artistQueryRelaxed = $"{HttpUtility.UrlEncode(song.Artist)}";
			var queries = new[] { artistQueryOriginal, artistQueryReversed, artistQueryRelaxed }.Distinct();
			foreach (var artistQuery in queries)
			{
				var artistUrl = $@"https://musicbrainz.org/ws/2/artist?query={artistQuery}&fmt=json";
				Trace.WriteLine($"ARTIST QUERY: {artistUrl}");

				var retryCount = 0;
				while (retryCount < 10)
				{
					token.ThrowIfCancellationRequested();

					artistData = await GetMusicBrainzDataForUrl<MusicbrainzArtistResponse>(artistUrl);
					if (artistData.artists?.Length > 0)
					{
						break;
					}

					if (artistData.created == DateTime.MinValue)
					{
						retryCount++;
						Trace.WriteLine($"Retrying. Server might be overloaded. ");
						Thread.Sleep(1000);
					}
					else // the query was answered correctly, it just had 0 artists
					{
						break;
					}
				}

				if (artistData.artists.Length == 0)
				{
					Trace.WriteLine($"No artists exist in database for {song}");
					Trace.WriteLine(artistUrl);
					continue;
				}

				SongResponseJson songResponse = null;

				int currentArtistIndex = 0;
				var artistList = artistData.artists
					.OrderByDescending(x => x.score)
					.ToArray();
				var songQuery = "";
				var songUrl = "";
				foreach (var artist in artistList)
				{
					token.ThrowIfCancellationRequested();

					currentArtistIndex++;
					Trace.WriteLine($"Trying artist: {artist.name} ({currentArtistIndex} / {artistList.Count()})");

					songQuery = $"{song.SongName} AND arid:{artist.id}";
					songUrl = $"https://musicbrainz.org/ws/2/recording?query={HttpUtility.UrlEncode(songQuery)}&inc=releases&fmt=json";
					songResponse = await GetMusicBrainzDataForUrl<SongResponseJson>(songUrl);

					if (songResponse.recordings != null && songResponse.recordings.Length > 0)
					{
						Trace.WriteLine($"Found recordings for {song}");
						break;
					}
				}

				recordings = songResponse.recordings?.Where(x => x.releases?.Length > 0);

				Trace.WriteLine(songUrl);
				if (recordings?.Any() == true)
				{
					Trace.WriteLine($"Found {recordings.Count()} recordings");
					var allReleases = recordings
						.SelectMany(x => x.releases)
						.Where(x => !string.IsNullOrEmpty(x.date));
					smallestYear = GetSmallestYear(allReleases);
					break;
				}
				Trace.WriteLine($"No recordings exist of {song} ({artistQuery})");
			}

			if (recordings?.Any() != true)
			{
				Trace.WriteLine($"Exhausted all options for {song}");
				song.ReleaseYearAutocorrected = -1;
				return;
			}

			if (smallestYear > song.ReleaseYearSpotify)
			{
				Trace.WriteLine($"Smallest year was bigger than spotify, checking more releases");
				var id = recordings.FirstOrDefault()?.id;
				if (id == null)
				{
					song.ReleaseYearAutocorrected = smallestYear;
					Trace.WriteLine($"More releases didn't help, saving {smallestYear}");
					return;
				}
				var releaseQuery = HttpUtility.UrlEncode(id);
				var releaseDateUrl = $"https://musicbrainz.org/ws/2/recording/{releaseQuery}?inc=releases&fmt=json";
				var releaseDateData = await GetMusicBrainzDataForUrl<ReleaseDateResponseJson>(releaseDateUrl);
				if (releaseDateData.releases != null)
				{
					smallestYear = GetSmallestYear(releaseDateData.releases);
				}
			}
			Trace.WriteLine($"@@@ {song.Artist} - {song.SongName} || {song.ReleaseYearSpotify} -> {smallestYear}");
			song.ReleaseYearAutocorrected = smallestYear;
		}

		private int GetSmallestYear(IEnumerable<Release> releases) => releases.Select(x => x.date.ParseYear()).Min();

		private async Task<T> GetMusicBrainzDataForUrl<T>(string url)
		{
			Thread.Sleep(1000);
			var uri = new Uri(url);
			var credentials = new NetworkCredential(configManager.Get(ConfigKeys.MusicbrainzLoginId), configManager.Get(ConfigKeys.MusicbrainzLoginPassword), "https://musicbrainz.org");
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
