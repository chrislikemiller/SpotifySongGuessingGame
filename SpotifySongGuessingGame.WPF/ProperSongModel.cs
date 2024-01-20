using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{
	public enum ReleaseDateSource
	{
		Spotify,
		Musicbrainz
	}
	[DebuggerDisplay("{Artist} - {SongName} ({ReleaseYear})")]
	public class ProperSongModel : IEquatable<ProperSongModel>
	{
		public string TrackUri { get; set; }
		public string ArtistUri { get; set; }
		public int Popularity { get; set; }
		public string TrackId { get; set; }
		public string ArtistId { get; set; }
		public string Artist { get; set; }
		public string SongName { get; set; }
		public int? ReleaseYear { get; set; }
		public ReleaseDateSource ReleaseDateSource { get; set; }

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

		public static ProperSongModel Parse(SpotifySongData x)
		{
			return new ProperSongModel
			{
				Artist = x.track.artist.name,
				SongName = x.track.name,

				Popularity = x.track.popularity,

				TrackId = x.track.id,
				TrackUri = x.track.uri,

				ArtistId = x.track.artist.id,
				ArtistUri = x.track.artist.uri,

				ReleaseYear = x.track.album.release_date?.ParseYear(),
				ReleaseDateSource = ReleaseDateSource.Spotify
			};
		}

	}
}
