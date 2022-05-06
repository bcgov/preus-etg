using System;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="DateTimeExtensions"/> static class, provides extension methods for <typeparamref name="DateTime"/> objects.
	/// </summary>
	public static class DateTimeExtensions
	{
		/// <summary>
		/// Gets the date for the specified kind and returns local midnight.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToLocalMidnight(this DateTime date)
		{
			var local = date.ToLocalTime();
			return new DateTime(local.Year, local.Month, local.Day, 23, 59, 59, DateTimeKind.Local);
		}

		/// <summary>
		/// Gets the date for the specified kind and returns local morning.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToLocalMorning(this DateTime date)
		{
			var local = date.ToLocalTime();
			return new DateTime(local.Year, local.Month, local.Day, 00, 00, 00, DateTimeKind.Local);
		}

		/// <summary>
		/// Gets the date for the specified kind and returns UTC midnight.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToUtcMidnight(this DateTime date)
		{
			return date.ToLocalMidnight().ToUniversalTime();
		}

		/// <summary>
		/// Gets the date for the specified kind and return UTC morning.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime ToUtcMorning(this DateTime date)
		{
			return date.ToLocalMorning().ToUniversalTime();
		}

		public static Int32? ToUnixTimeSeconds(this DateTime value)
		{
			return (Int32)(value.ToUniversalTime().Subtract(new AppDateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
		}

		public static string ToStringLocalTime(this DateTime value)
		{
			return value.ToLocalTime().ToString("yyyy-MM-dd");
		}

		public static string ToStringLocalDateTime(this DateTime value)
		{
			return value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
		}

		public static string FormatMorning(this DateTime? value)
		{
			return value?.ToLocalMorning().ToString("yyyy-MM-dd");
		}

		public static string FormatMorning(this DateTime value)
		{
			return value.ToLocalMorning().ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// A quick way to compare two dates to see if they are equal to the second.
		/// </summary>
		/// <param name="date"></param>
		/// <param name="compare"></param>
		/// <returns></returns>
		[Obsolete("Method is unused")]
		public static bool AreEqualToTheSecond(this DateTime date, DateTime compare)
		{
			return date.Year == compare.Year && date.Month == compare.Month && date.Day == compare.Day && date.Hour == compare.Hour && date.Minute == compare.Minute && date.Second == compare.Second;
		}
	}
}
