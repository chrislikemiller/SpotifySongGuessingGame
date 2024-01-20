using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	public class Config
	{
		public string SpotifyDatabasePath { get; set; } = string.Empty;
		public string SpotifyClientId { get; set; } = string.Empty;
		public string SpotifyClientSecret { get; set; } = string.Empty;
	}

	public enum ConfigKeys
	{
		DatabaseLocation,
		SpotifyClientId,
		SpotifyClientSecret
	}

	public class ConfigManager
	{
		private string configPath = "config.json";
		private Config configs;

		public ConfigManager()
		{
			string configJson = ReadConfig();
			configs = JsonConvert.DeserializeObject<Config>(configJson);
		}

		public string Get(ConfigKeys key)
		{
			switch (key)
			{
				case ConfigKeys.DatabaseLocation:
					return configs.SpotifyDatabasePath;
				case ConfigKeys.SpotifyClientId:
					return configs.SpotifyClientId;
				case ConfigKeys.SpotifyClientSecret:
					return configs.SpotifyClientSecret;
				default:
					throw new Exception($"Unknown config path {key}");
			}
		}

		public void Set(ConfigKeys key, string value)
		{
			bool needsSave = false;
			switch (key)
			{
				case ConfigKeys.DatabaseLocation:
					if (configs.SpotifyDatabasePath != value)
					{
						configs.SpotifyDatabasePath = value;
						needsSave = true;
					}
					break;
				case ConfigKeys.SpotifyClientId:
					if (configs.SpotifyClientId != value)
					{
						configs.SpotifyClientId = value;
						needsSave = true;
					}
					break;
				case ConfigKeys.SpotifyClientSecret:
					if (configs.SpotifyClientSecret != value)
					{
						configs.SpotifyClientSecret = value;
						needsSave = true;
					}
					break;
				default:
					throw new Exception($"Unknown config path {key}");
			}
			if (needsSave)
			{
				SaveConfig();
			}
		}

		private void SaveConfig()
		{
			File.WriteAllText(configPath, JsonConvert.SerializeObject(configs ?? new Config()));
		}

		private string ReadConfig()
		{
			//File.Delete(configPath);
			if (!File.Exists(configPath))
			{
				SaveConfig();
			}
			return File.ReadAllText(configPath);
		}

	}
}
