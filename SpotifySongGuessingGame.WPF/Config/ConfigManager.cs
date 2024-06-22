using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	public enum ConfigKeys
	{
		DatabaseLocation,
		SpotifyClientId,
		SpotifyClientSecret,
		MusicbrainzLoginId,
		MusicbrainzLoginPassword,
		GeniusAPIAccessToken,
	}

	public class ConfigManager
	{
		private string configPath = "config.json";
		private Dictionary<ConfigKeys, string> configs;

		public ConfigManager()
		{
			string configJson = ReadConfig();
			configs = JsonConvert.DeserializeObject<Dictionary<ConfigKeys, string>>(configJson);
		}

		public string Get(ConfigKeys key)
		{
			return configs.TryGetValue(key, out var result) ? result : "";
		}

		public void Set(ConfigKeys key, string value)
		{
			configs[key] = value;
			SaveConfig();
		}

		private void SaveConfig()
		{
			File.WriteAllText(configPath, JsonConvert.SerializeObject(configs ?? new Dictionary<ConfigKeys, string>()));
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
