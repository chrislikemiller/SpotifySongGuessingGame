using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{
	public class MusicbrainzArtistResponse
	{
		public DateTime created { get; set; }
		public int count { get; set; }
		public int offset { get; set; }
		public MusicbrainzArtist[] artists { get; set; }
	}

	[DebuggerDisplay("{name}")]
	public class MusicbrainzArtist
	{
		public string id { get; set; }
		public string type { get; set; }
		public string typeid { get; set; }
		public int score { get; set; }
		public string genderid { get; set; }
		public string name { get; set; }
		public string sortname { get; set; }
		public string gender { get; set; }
		public string country { get; set; }
		public Area area { get; set; }
		public BeginArea beginarea { get; set; }
		public string disambiguation { get; set; }
		public string[] ipis { get; set; }
		public string[] isnis { get; set; }
		public LifeSpan2 lifespan { get; set; }
		public Alias[] aliases { get; set; }
		public Tag[] tags { get; set; }
	}

	public class Area
	{
		public string id { get; set; }
		public string type { get; set; }
		public string typeid { get; set; }
		public string name { get; set; }
		public string sortname { get; set; }
		public LifeSpan lifespan { get; set; }
	}

	public class LifeSpan
	{
		public object ended { get; set; }
	}

	public class BeginArea
	{
		public string id { get; set; }
		public string type { get; set; }
		public string typeid { get; set; }
		public string name { get; set; }
		public string sortname { get; set; }
		public LifeSpan1 lifespan { get; set; }
	}

	public class LifeSpan1
	{
		public object ended { get; set; }
	}

	public class LifeSpan2
	{
		public string begin { get; set; }
		public bool? ended { get; set; }
		public string end { get; set; }
	}

	public class Alias
	{
		public string sortname { get; set; }
		public string typeid { get; set; }
		public string name { get; set; }
		public object locale { get; set; }
		public string type { get; set; }
		public object primary { get; set; }
		public object begindate { get; set; }
		public object enddate { get; set; }
	}

	public class Tag
	{
		public int count { get; set; }
		public string name { get; set; }
	}

	public class SongResponseJson
	{
		public DateTime created { get; set; }
		public int count { get; set; }
		public int offset { get; set; }
		public Recording[] recordings { get; set; }
	}

	[DebuggerDisplay("{score} - {title} - {releases.Length}")]
	public class Recording
	{
		public string id { get; set; }
		public int score { get; set; }
		public string title { get; set; }
		public int length { get; set; }
		public object video { get; set; }
		public ArtistCredit[] artistcredit { get; set; }
		public string firstreleasedate { get; set; }
		public Release[] releases { get; set; }
		public string[] isrcs { get; set; }
		public string disambiguation { get; set; }
	}

	public class ArtistCredit
	{
		public string joinphrase { get; set; }
		public string name { get; set; }
		public Artist artist { get; set; }
	}

	//public class Artist
	//{
	//	public string id { get; set; }
	//	public string name { get; set; }
	//	public string sortname { get; set; }
	//	public string disambiguation { get; set; }
	//	public Alias[] aliases { get; set; }
	//}

	//public class Alias
	//{
	//	public string sortname { get; set; }
	//	public string typeid { get; set; }
	//	public string name { get; set; }
	//	public string locale { get; set; }
	//	public string type { get; set; }
	//	public bool? primary { get; set; }
	//	public string begindate { get; set; }
	//	public string enddate { get; set; }
	//}

	[DebuggerDisplay("{title} - {date}")]
	public class Release
	{
		public string id { get; set; }
		public string statusid { get; set; }
		public int count { get; set; }
		public string title { get; set; }
		public string status { get; set; }
		public ArtistCredit1[] artistcredit { get; set; }
		public ReleaseGroup releasegroup { get; set; }
		public int trackcount { get; set; }
		public Medium[] media { get; set; }
		public string date { get; set; }
		public string country { get; set; }
		public ReleaseEvents[] releaseevents { get; set; }
		public string disambiguation { get; set; }
	}

	public class ReleaseGroup
	{
		public string id { get; set; }
		public string typeid { get; set; }
		public string primarytypeid { get; set; }
		public string title { get; set; }
		public string primarytype { get; set; }
		public string[] secondarytypes { get; set; }
		public string[] secondarytypeids { get; set; }
	}

	public class ArtistCredit1
	{
		public string name { get; set; }
		public CreditedArtist artist { get; set; }
		public string joinphrase { get; set; }
	}

	public class CreditedArtist
	{
		public string id { get; set; }
		public string name { get; set; }
		public string sortname { get; set; }
		public string disambiguation { get; set; }
	}

	public class Medium
	{
		public int position { get; set; }
		public string format { get; set; }
		public MusicbrainzTrack[] track { get; set; }
		public int trackcount { get; set; }
		public int trackoffset { get; set; }
	}

	public class MusicbrainzTrack
	{
		public string id { get; set; }
		public string number { get; set; }
		public string title { get; set; }
		public int length { get; set; }
	}

	public class ReleaseEvents
	{
		public string date { get; set; }
		public Area area { get; set; }
	}

	public class ReleaseDateResponseJson
	{
		public bool video { get; set; }
		public string length { get; set; }
		public string firstreleasedate { get; set; }
		public string disambiguation { get; set; }
		public string title { get; set; }
		public Release[] releases { get; set; }
		public string id { get; set; }
	}

	public class TextRepresentation
	{
		public object script { get; set; }
		public string language { get; set; }
	}
}
