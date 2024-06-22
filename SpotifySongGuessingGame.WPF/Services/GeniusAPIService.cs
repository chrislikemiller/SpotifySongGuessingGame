using Newtonsoft.Json;
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
			Trace.WriteLine($"Getting release date for {artist} - {song}");
			var releaseDate = await GetTrackReleaseDateAsync(artist.Trim(), song.Trim());
			if (!string.IsNullOrEmpty(releaseDate))
			{
				Trace.WriteLine($"Found: {artist} - {song} release date: {releaseDate}");
				return releaseDate;
			}
			else
			{
				Trace.WriteLine($"NOT FOUND track ID: {artist} - {song}");
				return "";
			}
		}

		private async Task<string> GetTrackReleaseDateAsync(string artist, string song)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
				var query = Uri.EscapeDataString($"{artist} {song}");
				var response = await httpClient.GetAsync($"{baseUrl}/search?q={query}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				try
				{
					var model = JsonConvert.DeserializeObject<GeniusTrackIdObject>(content);
					if (model.response.hits.Length > 0)
					{
						foreach (var hit in model.response.hits)
						{
							Trace.WriteLine($">>> {hit.result.primary_artist.name} - {hit.result.title}");
						}

						var songs = model.response.hits.Where(
							x => x.result.title_with_featured.ToLower().Contains(song.ToLower())).ToArray();
						var artists = model.response.hits.Where(
							x => x.result.primary_artist.name.ToLower().Contains(artist.ToLower())).ToArray();
						var picks = songs.Intersect(artists).Distinct().ToArray();

						var pick = picks.FirstOrDefault()?.result;

						if (pick != null)
						{
							if (pick.release_date_components == null)
							{
								var releaseDate = await GetTrackReleaseDateAsyncOld(pick.id);
								if (DateTime.TryParse(releaseDate, out var date))
								{
									return date.Year.ToString();
								}
								else return "";
							}
							return pick.release_date_components.year.ToString();
						}
					}
					else
					{
						return "";
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					Trace.WriteLine($"ERROR for {artist} - {song}");
					Trace.WriteLine(content);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
				return "";
			}
		}

		private async Task<string> GetTrackReleaseDateAsyncOld(int trackId)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

					var response = await httpClient.GetAsync($"{baseUrl}/songs/{trackId}");
					response.EnsureSuccessStatusCode();

					var content = await response.Content.ReadAsStringAsync();
					var model = JsonConvert.DeserializeObject<SongRequestObject>(content);

					try
					{
						return model.response.song.release_date?.ToString() ?? "";
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return "";
		}
	}
}

