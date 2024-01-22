using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{
	[DebuggerDisplay("{Artist} - {SongName} | {ReleaseYearSpotify} | {ReleaseYearMusicbrainz}")]
	public class ProperSongModel : IEquatable<ProperSongModel>
	{
		public string TrackUri { get; set; }
		public string ArtistUri { get; set; }
		public int Popularity { get; set; }
		public string TrackId { get; set; }
		public string ArtistId { get; set; }
		public string Artist { get; set; }
		public string SongName { get; set; }
		public int ReleaseYearSpotify { get; set; }
		public int ReleaseYearMusicbrainz { get; set; }

		public bool Equals(ProperSongModel other)
		{
			if (other == null) return false;
			return string.Equals(SongName, other.SongName, StringComparison.OrdinalIgnoreCase)
				&& string.Equals(Artist, other.Artist, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ProperSongModel);
		}

		public override int GetHashCode()
		{
			return TrackId.GetHashCode();
		}

		public static ProperSongModel Parse(SpotifyTrack x)
		{
			return new ProperSongModel
			{
				Artist = x.artist.name,
				SongName = x.name,

				Popularity = x.popularity,

				TrackId = x.id,
				TrackUri = x.uri,

				ArtistId = x.artist.id,
				ArtistUri = x.artist.uri,

				ReleaseYearSpotify = x.album.release_date?.ParseYear() ?? int.MaxValue,
				ReleaseYearMusicbrainz = 0
			};
		}

		public static ProperSongModel Parse(SpotifySongData x)
		{
			return Parse(x.track);
		}

		public override string ToString()
		{
			return $"{Artist} - {SongName} | {ReleaseYearSpotify} (or {ReleaseYearMusicbrainz})";
		}

	}
}
