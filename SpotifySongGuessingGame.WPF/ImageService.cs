using SpotifySongGuessingGame.Common;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.WPF
{
	class ImageService
	{
		private readonly SpotifyDatabase spotifyDatabase;

		public ImageService(SpotifyDatabase spotifyDatabase)
		{
			this.spotifyDatabase = spotifyDatabase;
		}

		public void GenerateAllImages()
		{
			foreach (var song in spotifyDatabase)
			{
				var file = @$"D:\song_guessing_game_images\songdata\songdata_{song.TrackId}.png";
				if (File.Exists(file))
				{
					continue;
				}
				var img = GenerateSongDataImage(song);
				img.Save(file, ImageFormat.Png);
			}
		}

		public static Image GenerateSongDataImage(ProperSongModel song)
		{
			SizeF artistTextSize;
			SizeF songNameTextSize;
			SizeF releaseYearTextSize;

			var maxSize = new SizeF(620, 620);
			var artistSongNameFont = new Font(new FontFamily("Segoe UI"), 48, System.Drawing.FontStyle.Italic);
			var releaseYearFont = new Font("Segoe UI", 132, System.Drawing.FontStyle.Bold);

			using (var img = new Bitmap(1, 1))
			{
				using (Graphics drawing = Graphics.FromImage(img))
				{
					artistTextSize = drawing.MeasureString(song.Artist, artistSongNameFont, maxSize);
					songNameTextSize = drawing.MeasureString(song.SongName, releaseYearFont, maxSize);
					releaseYearTextSize = drawing.MeasureString(song.ReleaseYear.ToString(), releaseYearFont, maxSize);
				}
			}
			var retImg = new Bitmap((int)maxSize.Width, (int)maxSize.Height);
			using (var drawing = Graphics.FromImage(retImg))
			{
				var stringFormat = new StringFormat
				{
					Alignment = StringAlignment.Center
				};

				drawing.Clear(Color.White);
				using (var textBrush = new SolidBrush(Color.Black))
				{
					var heightOffset = 10f;
					drawing.DrawString(song.Artist, artistSongNameFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, artistTextSize.Height), stringFormat);
					heightOffset += artistTextSize.Height;
					drawing.DrawString(song.ReleaseYear.ToString(), releaseYearFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, releaseYearTextSize.Height), stringFormat);
					heightOffset += releaseYearTextSize.Height;
					drawing.DrawString(song.SongName, artistSongNameFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, songNameTextSize.Height), stringFormat);
					var attr = new ImageAttributes();
					drawing.Save();
				}
			}
			return retImg;
		}
		public FileInfo CreateCollage(FileInfo[] imagesPaths, int imageSize, int outputColumns, int outputRows, bool reverseRows)
		{
			int counter = 0;
			var paths = imagesPaths.OrderBy(x => x.FullName).Take(outputRows * outputColumns).ToArray();

			using (var outputImage = new Bitmap(outputColumns * imageSize, outputRows * imageSize))
			{
				using (var outputGfx = Graphics.FromImage(outputImage))
				{
					for (int x = reverseRows ? outputColumns - 1 : 0;
						reverseRows ? x >= 0 : x < outputColumns;
						x += reverseRows ? -1 : 1)
					{
						for (int y = 0; y < outputRows; y++)
						{
							var thumbnail = Image.FromFile(paths[counter].FullName);
							outputGfx.DrawImage(thumbnail, x * imageSize, y * imageSize);
							counter++;
						}
					}
				}

				var tempPng = Path.GetTempFileName() + ".png";
				outputImage.Save(tempPng, ImageFormat.Png);

				return new FileInfo(tempPng);
			}
		}

		private async void CreateSpotifyCodes()
		{
			var songDataDir = $@"D:\song_guessing_game_images\songdata";
			var spotifyCodeDir = $@"D:\song_guessing_game_images\spotifycode";
			var files = Directory.EnumerateFiles(songDataDir).Where(x => x.Contains("songdata")).Select(x => new FileInfo(x));
			var trackIds = files.Select(x => x.Name.Replace("songdata_", "").Replace(".png", "")).ToArray();

			for (int i = 0; i < trackIds.Length; i++)
			{
				var currentTrackId = trackIds[i];

				var url = $@"https://scannables.scdn.co/uri/plain/png/FFFFFF/black/640/spotify:track:{currentTrackId}";
				var saveFileLocation = Path.Combine(spotifyCodeDir, $"spotifycode_{currentTrackId}.png");
				if (File.Exists(saveFileLocation))
				{
					continue;
				}
				using (HttpClient client = new HttpClient())
				{
					var bytes = await client.GetByteArrayAsync(url);
					Thread.Sleep(new Random().Next(100, 250));
					using (MemoryStream ms = new MemoryStream(bytes))
					{
						var original = new Bitmap(ms);
						var resized = new Bitmap(620, 620);
						Graphics g = Graphics.FromImage(resized);
						g.Clear(Color.White);
						g.DrawImage(original, (resized.Width - original.Width) / 2, (resized.Height - original.Height) / 2);
						resized.Save(saveFileLocation, ImageFormat.Png);
					}
				}
			}
		}
	}
}
