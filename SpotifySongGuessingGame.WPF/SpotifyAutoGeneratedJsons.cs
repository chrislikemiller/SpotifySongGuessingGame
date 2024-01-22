using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{

	public class SpotifyArtistTopSongResponse
	{
		public SpotifyTrack[] tracks { get; set; }
	}


	public class SpotifyPlaylistResponse
	{
		public string href { get; set; }
		public SpotifySongData[] items { get; set; }
		public int limit { get; set; }
		public string next { get; set; }
		public int offset { get; set; }
		public object previous { get; set; }
		public int total { get; set; }
	}

	public class SpotifySongData : IEquatable<SpotifySongData>
	{
		public DateTime added_at { get; set; }
		public AddedBy added_by { get; set; }
		public bool is_local { get; set; }
		public object primary_color { get; set; }
		public SpotifyTrack track { get; set; }
		public VideoThumbnail video_thumbnail { get; set; }

		public bool Equals(SpotifySongData other)
		{
			if (other == null) return false;
			if (track?.id == null) return false;
			if (other.track?.id == null) return false;
			return track.id == other.track.id;
		}
	}

	public class AddedBy
	{
		public ExternalUrls external_urls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}

	public class ExternalUrls
	{
		public string spotify { get; set; }
	}

	public class SpotifyTrack
	{
		public Album album { get; set; }
		public SpotifyArtist[] artists { get; set; }
		public SpotifyArtist artist => artists.First();
		public string[] available_markets { get; set; }
		public int disc_number { get; set; }
		public int duration_ms { get; set; }
		public bool episode { get; set; }
		public bool _explicit { get; set; }
		public ExternalIds external_ids { get; set; }
		public ExternalUrls external_urls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public bool is_local { get; set; }
		public string name { get; set; }
		public int popularity { get; set; }
		public string preview_url { get; set; }
		public bool track { get; set; }
		public int track_number { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}

	public class Album
	{
		public string album_type { get; set; }
		public Artist[] artists { get; set; }
		public string[] available_markets { get; set; }
		public ExternalUrls external_urls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public SpotifyImage[] images { get; set; }
		public string name { get; set; }
		public string release_date { get; set; }
		public string release_date_precision { get; set; }
		public int total_tracks { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}


	public class Artist
	{
		public ExternalUrls externalurls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}


	public class SpotifyImage
	{
		public int height { get; set; }
		public string url { get; set; }
		public int width { get; set; }
	}

	public class ExternalIds
	{
		public string isrc { get; set; }
	}

	public class SpotifyArtist
	{
		public ExternalUrls4 externalurls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public string name { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}

	public class ExternalUrls4
	{
		public string spotify { get; set; }
	}

	public class VideoThumbnail
	{
		public object url { get; set; }
	}
}
