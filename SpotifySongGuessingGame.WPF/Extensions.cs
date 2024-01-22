using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifySongGuessingGame.Common
{
	public static class Extensions
	{
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> toAdd)
		{
			foreach (var item in toAdd)
			{
				source.Add(item);
			}
		}

		public static void AddRangeUnique<T>(this ICollection<T> source, IEnumerable<T> toAdd)
		{
			AddRange(source, toAdd.Distinct());
		}

		public static int ParseYear(this string dateString)
		{
			try
			{
				if (string.IsNullOrEmpty(dateString))
					return int.MaxValue;

				return DateTime.TryParse(dateString, out var date)
					? date.Year
					: Convert.ToInt32(dateString[..4]);
			}
			catch
			{
				return int.MaxValue;
			}
		}

		public static string ReverseWords(this string str)
		{
			return string.Join(" ", str.Split(" ").Reverse());
		}
	}
}
