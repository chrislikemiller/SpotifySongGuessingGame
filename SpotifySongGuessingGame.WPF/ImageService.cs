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
using System.Diagnostics;

namespace SpotifySongGuessingGame.WPF
{
	class ImageService
	{
		private readonly ConfigManager configManager;

		public ImageService(ConfigManager configManager)
		{
			this.configManager = configManager;
		}

		public async Task GenerateAllImages(string playlistId, IEnumerable<ProperSongModel> songs)
		{
			var dir = Path.Combine(configManager.Get(ConfigKeys.DatabaseLocation), playlistId);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			var counter = 0;
			var array = songs.ToArray();
			foreach (var song in array)
			{
				var filename = $"{song.TrackId}.png";
				var codeFilename = $"code_{filename}";
				var filePath = Path.Combine(dir, filename);
				var codeFilePath = Path.Combine(dir, codeFilename);
				Trace.WriteLine($"[{counter} / {array.Length}] Saving {song} to {filePath}");

				var img = await GenerateImage(song);
				var codeImg = await GenerateSpotifyCode(song);
				img.Save(filePath, ImageFormat.Png);
				codeImg.Save(codeFilePath, ImageFormat.Png);
			}
		}

		public Task<Image> GenerateImage(ProperSongModel song)
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
					releaseYearTextSize = drawing.MeasureString(song.ReleaseYearSpotify.ToString(), releaseYearFont, maxSize);
				}
			}
			return Task.Run<Image>(() =>
			{
				var heightOffset = 10f;
				var retImg = new Bitmap((int)maxSize.Width, (int)maxSize.Height);
				using var textBrush = new SolidBrush(Color.Black);
				using var drawing = Graphics.FromImage(retImg);
				var stringFormat = new StringFormat { Alignment = StringAlignment.Center };

				drawing.Clear(Color.White);
				drawing.DrawString(song.Artist, artistSongNameFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, artistTextSize.Height), stringFormat);
				heightOffset += artistTextSize.Height;
				drawing.DrawString(song.ReleaseYearSpotify.ToString(), releaseYearFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, releaseYearTextSize.Height), stringFormat);
				heightOffset += releaseYearTextSize.Height;
				drawing.DrawString(song.SongName, artistSongNameFont, textBrush, new RectangleF(0, heightOffset, maxSize.Width, songNameTextSize.Height), stringFormat);
				drawing.Save();

				return retImg;
			});
		}

		private async Task<Image> GenerateSpotifyCode(ProperSongModel song)
		{
			var url = $@"https://scannables.scdn.co/uri/plain/png/FFFFFF/black/640/spotify:track:{song.TrackId}";
			using var client = new HttpClient();
			var bytes = await client.GetByteArrayAsync(url);
			using var ms = new MemoryStream(bytes);
			var original = new Bitmap(ms);
			var resized = new Bitmap(620, 620);
			var drawing = Graphics.FromImage(resized);
			drawing.Clear(Color.White);
			drawing.DrawImage(original, (resized.Width - original.Width) / 2, (resized.Height - original.Height) / 2);
			drawing.Save();
			return resized;
		}


		public void CreateCollage(
			string playlistId,
			IEnumerable<ProperSongModel> songs,
			int nth,
			int outputColumns = 4,
			int outputRows = 5)
		{
			var playlistDir = Path.Combine(configManager.Get(ConfigKeys.DatabaseLocation), playlistId);
			var collagesDir = Path.Combine(playlistDir, "collages");
			if (!Directory.Exists(collagesDir))
			{
				Directory.CreateDirectory(collagesDir);
			}

			int frontCounter = 0;
			int backCounter = 0;
			int imageSize = 620;
			var img = new Bitmap(outputColumns * imageSize, outputRows * imageSize);
			using var drawing = Graphics.FromImage(img);
			var array = songs.ToArray();


			// front
			drawing.Clear(Color.White);
			Trace.WriteLine($"Front for {playlistId}");
			for (int x = 0; x < outputColumns; x += 1)
			{
				for (int y = 0; y < outputRows; y++)
				{
					var filename = $"{array[frontCounter].TrackId}.png";
					var filePath = Path.Combine(playlistDir, filename);
					var thumbnail = Image.FromFile(filePath);
					drawing.DrawImage(thumbnail, x * imageSize, y * imageSize);
					frontCounter++;
				}
			}
			var frontPath = Path.Combine(collagesDir, $"{playlistId}_{nth}_0front.png");
			Trace.WriteLine($"Saving {frontPath}");
			img.Save(frontPath, ImageFormat.Png);


			// back
			drawing.Clear(Color.White);
			Trace.WriteLine($"Back for {playlistId}");
			for (int x = outputColumns - 1; x >= 0; x -= 1)
			{
				for (int y = 0; y < outputRows; y++)
				{
					var codeFilename = $"code_{array[backCounter].TrackId}.png";
					var codeFilePath = Path.Combine(playlistDir, codeFilename);
					var thumbnail = Image.FromFile(codeFilePath);
					drawing.DrawImage(thumbnail, x * imageSize, y * imageSize);
					backCounter++;
				}
			}
			var backPath = Path.Combine(collagesDir, $"{playlistId}_{nth}_1back.png");
			Trace.WriteLine($"Saving {backPath}");
			img.Save(backPath, ImageFormat.Png);
		}
	}
}

