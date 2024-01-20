using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{
	public class SpotifyPlaylistResponse
	{
		public string href { get; set; }
		public SpotifySongData[] items { get; set; }
		public int limit { get; set; }
		public object next { get; set; }
		public int offset { get; set; }
		public object previous { get; set; }
		public int total { get; set; }
	}

	public class SpotifySongData : IEquatable<SpotifySongData>
	{
		public DateTime addedat { get; set; }
		public AddedBy addedby { get; set; }
		public bool islocal { get; set; }
		public object primarycolor { get; set; }
		public SpotifyTrack track { get; set; }
		public VideoThumbnail videothumbnail { get; set; }

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
		public ExternalUrls externalurls { get; set; }
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
		public string[] availablemarkets { get; set; }
		public int discnumber { get; set; }
		public int durationms { get; set; }
		public bool episode { get; set; }
		public bool isExplicit { get; set; }
		public ExternalIds externalids { get; set; }
		public ExternalUrls externalurls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public bool islocal { get; set; }
		public string name { get; set; }
		public int popularity { get; set; }
		public string previewurl { get; set; }
		public bool track { get; set; }
		public int tracknumber { get; set; }
		public string type { get; set; }
		public string uri { get; set; }
	}

	public class Album
	{
		public string albumtype { get; set; }
		public Artist[] artists { get; set; }
		public string[] availablemarkets { get; set; }
		public ExternalUrls externalurls { get; set; }
		public string href { get; set; }
		public string id { get; set; }
		public SpotifyImage[] images { get; set; }
		public string name { get; set; }
		public string releasedate { get; set; }
		public string releasedateprecision { get; set; }
		public int totaltracks { get; set; }
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
