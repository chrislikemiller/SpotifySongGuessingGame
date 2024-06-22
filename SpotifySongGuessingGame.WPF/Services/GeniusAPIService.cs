using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	class GeniusAPIService
	{
		private static readonly string baseUrl = "https://api.genius.com";
		private readonly string accessToken;

		public GeniusAPIService(string accessToken)
		{
			this.accessToken = accessToken;
		}

		public async Task<string> GetReleaseDate(string artist, string song)
		{
			var trackId = await GetTrackIdAsync(artist, song);
			if (trackId != null)
			{
				var releaseDate = await GetTrackReleaseDateAsync(trackId);
				Trace.WriteLine($"Artist: {artist}, Song: {song} - Release Date: {releaseDate}");
				return releaseDate;
			}
			else
			{
				Trace.WriteLine($"Artist: {artist}, Song: {song} - Track ID not found");
				return null;
			}
		}

		private async Task<string> GetTrackIdAsync(string artist, string song)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
				var query = Uri.EscapeDataString($"{artist} {song}");
				var response = await httpClient.GetAsync($"{baseUrl}/search?q={query}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);

				try
				{
					var trackId = json["response"]?["hits"]?[0]?["result"]?["id"]?.ToString();
					return trackId;
				}
				catch (ArgumentOutOfRangeException)
				{
					Trace.WriteLine($"ERROR for {artist} - {song}");
					Trace.WriteLine(content);
					return "";
				}
			}
		}

		private async Task<string> GetTrackReleaseDateAsync(string trackId)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

				var response = await httpClient.GetAsync($"{baseUrl}/songs/{trackId}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var json = JObject.Parse(content);

				var releaseDate = json["response"]?["song"]?["release_date"]?.ToString();
				return releaseDate ?? "";
			}
		}
	}
}

