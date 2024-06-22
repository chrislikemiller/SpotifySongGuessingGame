using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	// Track ID request
	public class GeniusTrackIdObject
	{
		public Meta meta { get; set; }
		public TrackIdResponse response { get; set; }
	}

	public class Meta
	{
		public int status { get; set; }
	}

	public class TrackIdResponse
	{
		public Hit[] hits { get; set; }
	}

	public class Hit
	{
		public object[] highlights { get; set; }
		public string index { get; set; }
		public string type { get; set; }
		public Result result { get; set; }
	}

	public class Result
	{
		public int annotation_count { get; set; }
		public string api_path { get; set; }
		public string artist_names { get; set; }
		public string full_title { get; set; }
		public string header_image_thumbnail_url { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public int lyrics_owner_id { get; set; }
		public string lyrics_state { get; set; }
		public string path { get; set; }
		public int? pyongs_count { get; set; }
		public string relationships_index_url { get; set; }
		public Release_Date_Components release_date_components { get; set; }
		public string release_date_for_display { get; set; }
		public string release_date_with_abbreviated_month_for_display { get; set; }
		public string song_art_image_thumbnail_url { get; set; }
		public string song_art_image_url { get; set; }
		public Stats stats { get; set; }
		public string title { get; set; }
		public string title_with_featured { get; set; }
		public string url { get; set; }
		public Featured_Artists[] featured_artists { get; set; }
		public Primary_Artist primary_artist { get; set; }
		public Primary_Artist[] primary_artists { get; set; }
	}

	public class Release_Date_Components
	{
		public int year { get; set; }
		public int? month { get; set; }
		public int? day { get; set; }
	}

	public class Primary_Artist
	{
		public string api_path { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public string image_url { get; set; }
		public bool is_meme_verified { get; set; }
		public bool is_verified { get; set; }
		public string name { get; set; }
		public string url { get; set; }
		public int iq { get; set; }
	}

	public class Featured_Artists
	{
		public string api_path { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public string image_url { get; set; }
		public bool is_meme_verified { get; set; }
		public bool is_verified { get; set; }
		public string name { get; set; }
		public string url { get; set; }
	}


	// Song request

	public class SongRequestObject
	{
		public Meta meta { get; set; }
		public SongResponse response { get; set; }
	}

	public class SongResponse
	{
		public Song song { get; set; }
	}

	public class Song
	{
		public int annotation_count { get; set; }
		public string api_path { get; set; }
		public object apple_music_id { get; set; }
		public string apple_music_player_url { get; set; }
		public string artist_names { get; set; }
		public Description description { get; set; }
		public string embed_content { get; set; }
		public bool featured_video { get; set; }
		public string full_title { get; set; }
		public string header_image_thumbnail_url { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public string language { get; set; }
		public int lyrics_owner_id { get; set; }
		public object lyrics_placeholder_reason { get; set; }
		public string lyrics_state { get; set; }
		public string path { get; set; }
		public object pyongs_count { get; set; }
		public object recording_location { get; set; }
		public string relationships_index_url { get; set; }
		public object release_date { get; set; }
		public object release_date_for_display { get; set; }
		public object release_date_with_abbreviated_month_for_display { get; set; }
		public string song_art_image_thumbnail_url { get; set; }
		public string song_art_image_url { get; set; }
		public Stats stats { get; set; }
		public string title { get; set; }
		public string title_with_featured { get; set; }
		public string url { get; set; }
		public Current_User_Metadata current_user_metadata { get; set; }
		public string song_art_primary_color { get; set; }
		public string song_art_secondary_color { get; set; }
		public string song_art_text_color { get; set; }
		public Album album { get; set; }
		public object[] custom_performances { get; set; }
		public Description_Annotation description_annotation { get; set; }
		public object[] featured_artists { get; set; }
		public object lyrics_marked_complete_by { get; set; }
		public object lyrics_marked_staff_approved_by { get; set; }
		public object[] media { get; set; }
		public Primary_Artist primary_artist { get; set; }
		public Primary_Artist[] primary_artists { get; set; }
		public object[] producer_artists { get; set; }
		public Song_Relationships[] song_relationships { get; set; }
		public object[] translation_songs { get; set; }
		public object[] verified_annotations_by { get; set; }
		public object[] verified_contributors { get; set; }
		public object[] verified_lyrics_by { get; set; }
		public Writer_Artists[] writer_artists { get; set; }
	}

	public class Description
	{
		public Dom dom { get; set; }
	}

	public class Dom
	{
		public string tag { get; set; }
		public Child[] children { get; set; }
	}

	public class Child
	{
		public string tag { get; set; }
		public string[] children { get; set; }
	}

	public class Stats
	{
		public int accepted_annotations { get; set; }
		public int contributors { get; set; }
		public int iq_earners { get; set; }
		public int transcribers { get; set; }
		public int unreviewed_annotations { get; set; }
		public int verified_annotations { get; set; }
		public bool hot { get; set; }
		public int pageviews { get; set; }
	}

	public class Current_User_Metadata
	{
		public string[] permissions { get; set; }
		public string[] excluded_permissions { get; set; }
		public Interactions interactions { get; set; }
		public Relationships relationships { get; set; }
		public Iq_By_Action iq_by_action { get; set; }
	}

	public class Interactions
	{
		public bool pyong { get; set; }
		public bool following { get; set; }
	}

	public class Relationships
	{
	}

	public class Iq_By_Action
	{
	}

	public class Album
	{
		public string api_path { get; set; }
		public string cover_art_url { get; set; }
		public string full_title { get; set; }
		public int id { get; set; }
		public string name { get; set; }
		public string release_date_for_display { get; set; }
		public string url { get; set; }
		public Artist artist { get; set; }
	}

	public class Artist
	{
		public string api_path { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public string image_url { get; set; }
		public bool is_meme_verified { get; set; }
		public bool is_verified { get; set; }
		public string name { get; set; }
		public string url { get; set; }
	}

	public class Description_Annotation
	{
		public string _type { get; set; }
		public int annotator_id { get; set; }
		public string annotator_login { get; set; }
		public string api_path { get; set; }
		public string classification { get; set; }
		public string fragment { get; set; }
		public int id { get; set; }
		public bool is_description { get; set; }
		public string path { get; set; }
		public Range range { get; set; }
		public int song_id { get; set; }
		public string url { get; set; }
		public object[] verified_annotator_ids { get; set; }
		public Annotatable annotatable { get; set; }
		public Annotation[] annotations { get; set; }
	}

	public class Range
	{
		public string content { get; set; }
	}

	public class Annotatable
	{
		public string api_path { get; set; }
		public Client_Timestamps client_timestamps { get; set; }
		public string context { get; set; }
		public int id { get; set; }
		public string image_url { get; set; }
		public string link_title { get; set; }
		public string title { get; set; }
		public string type { get; set; }
		public string url { get; set; }
	}

	public class Client_Timestamps
	{
		public int updated_by_human_at { get; set; }
		public int lyrics_updated_at { get; set; }
	}

	public class Annotation
	{
		public string api_path { get; set; }
		public Body body { get; set; }
		public int comment_count { get; set; }
		public bool community { get; set; }
		public object custom_preview { get; set; }
		public bool has_voters { get; set; }
		public int id { get; set; }
		public bool pinned { get; set; }
		public string share_url { get; set; }
		public object source { get; set; }
		public string state { get; set; }
		public string url { get; set; }
		public bool verified { get; set; }
		public int votes_total { get; set; }
		public Current_User_Metadata1 current_user_metadata { get; set; }
		public Author[] authors { get; set; }
		public object[] cosigned_by { get; set; }
		public object rejection_comment { get; set; }
		public object verified_by { get; set; }
	}

	public class Body
	{
		public Dom1 dom { get; set; }
	}

	public class Dom1
	{
		public string tag { get; set; }
	}

	public class Current_User_Metadata1
	{
		public object[] permissions { get; set; }
		public string[] excluded_permissions { get; set; }
		public Interactions1 interactions { get; set; }
		public Iq_By_Action1 iq_by_action { get; set; }
	}

	public class Interactions1
	{
		public bool cosign { get; set; }
		public bool pyong { get; set; }
		public object vote { get; set; }
	}

	public class Iq_By_Action1
	{
	}

	public class Author
	{
		public float attribution { get; set; }
		public object pinned_role { get; set; }
		public User user { get; set; }
	}

	public class User
	{
		public string api_path { get; set; }
		public Avatar avatar { get; set; }
		public string header_image_url { get; set; }
		public string human_readable_role_for_display { get; set; }
		public int id { get; set; }
		public int iq { get; set; }
		public string login { get; set; }
		public string name { get; set; }
		public string role_for_display { get; set; }
		public string url { get; set; }
		public Current_User_Metadata2 current_user_metadata { get; set; }
	}

	public class Avatar
	{
		public Tiny tiny { get; set; }
		public Thumb thumb { get; set; }
		public Small small { get; set; }
		public Medium medium { get; set; }
	}

	public class Tiny
	{
		public string url { get; set; }
		public Bounding_Box bounding_box { get; set; }
	}

	public class Bounding_Box
	{
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Thumb
	{
		public string url { get; set; }
		public Bounding_Box1 bounding_box { get; set; }
	}

	public class Bounding_Box1
	{
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Small
	{
		public string url { get; set; }
		public Bounding_Box2 bounding_box { get; set; }
	}

	public class Bounding_Box2
	{
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Medium
	{
		public string url { get; set; }
		public Bounding_Box3 bounding_box { get; set; }
	}

	public class Bounding_Box3
	{
		public int width { get; set; }
		public int height { get; set; }
	}

	public class Current_User_Metadata2
	{
		public object[] permissions { get; set; }
		public string[] excluded_permissions { get; set; }
		public Interactions2 interactions { get; set; }
	}

	public class Interactions2
	{
		public bool following { get; set; }
	}

	public class Song_Relationships
	{
		public string relationship_type { get; set; }
		public string type { get; set; }
		public object[] songs { get; set; }
	}

	public class Writer_Artists
	{
		public string api_path { get; set; }
		public string header_image_url { get; set; }
		public int id { get; set; }
		public string image_url { get; set; }
		public bool is_meme_verified { get; set; }
		public bool is_verified { get; set; }
		public string name { get; set; }
		public string url { get; set; }
	}

}
