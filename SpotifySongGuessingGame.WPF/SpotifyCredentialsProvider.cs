using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{

	public class CredentialsJson
	{
		public string access_token { get; set; }
		public string token_type { get; set; }
		public int expires_in { get; set; }
	}

	public class SpotifyCredentialsProvider
	{
		private readonly ConfigManager configManager;
		public string TemporaryKey { get; private set; }
		public string LastError { get; private set; }

		public SpotifyCredentialsProvider(ConfigManager configManager)
		{
			this.configManager = configManager;
		}

		public async Task GenerateNewKey()
		{
			try
			{
				using var client = new HttpClient();
				client.BaseAddress = new Uri("https://accounts.spotify.com/");
				client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

				var request = new HttpRequestMessage(HttpMethod.Post, "api/token");
				var content = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("grant_type", "client_credentials"),
					new KeyValuePair<string, string>("client_id", configManager.Get(ConfigKeys.SpotifyClientId)),
					new KeyValuePair<string, string>("client_secret", configManager.Get(ConfigKeys.SpotifyClientSecret))
				});
				
				request.Content = content;
				var response = await client.SendAsync(request);
				if (response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var credentials = JsonConvert.DeserializeObject<CredentialsJson>(json);
					TemporaryKey = credentials.access_token;
					LastError = null;
				}
				else
				{
					var error = await response.Content.ReadAsStringAsync();
					LastError = error;
				}
			}
			catch (Exception ex)
			{
				LastError = ex.Message;
			}
		}
	}
}