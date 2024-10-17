using System;
using System.Threading;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="AppDateTime"/> class, provides a way to manage <typeparamref name="DateTime"/> values throughout the application and also provide a way to mock a fake date for testing purposes.
	/// By default ensures all <typeparamref name="DateTime"/> values are in of UTC kind.
	/// </summary>
	public class AppDateTime
	{
		#region Variables
		private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		private static DateTime? _internalNow;
		#endregion

		#region Properties
		/// <summary>
		/// get/set - The internally set Now DateTime value.
		/// </summary>
		private static DateTime? _InternalNow
		{
			get
			{
				try
				{
					_lock.EnterReadLock();
					return _internalNow ?? DateTime.Now;
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
			set
			{
				try
				{
					_lock.EnterWriteLock();
					_internalNow = value;
				}
				finally
				{
					_lock.ExitWriteLock();
				}
			}
		}

		private DateTime Date { get; set; }

		/// <summary>
		/// get - The DateTimeKind of the application date.
		/// </summary>
		public DateTimeKind Kind
		{
			get { return this.Date.Kind; }
		}

		/// <summary>
		/// get - The application date year.
		/// </summary>
		public int Year
		{
			get { return this.Date.Year; }
		}

		/// <summary>
		/// get - The application date month.
		/// </summary>
		public int Month
		{
			get { return this.Date.Month; }
		}

		/// <summary>
		/// get - The application date day.
		/// </summary>
		public int Day
		{
			get { return this.Date.Day; }
		}

		/// <summary>
		/// get - The application date hour.
		/// </summary>
		public int Hour
		{
			get { return this.Date.Hour; }
		}

		/// <summary>
		/// get - The application date minute.
		/// </summary>
		public int Minute
		{
			get { return this.Date.Minute; }
		}

		/// <summary>
		/// get - The application date second.
		/// </summary>
		public int Second
		{
			get { return this.Date.Second; }
		}

		/// <summary>
		/// get - The application date millisecond.
		/// </summary>
		public int Millisecond
		{
			get { return this.Date.Millisecond; }
		}

		/// <summary>
		/// get - The application date day of the week.
		/// </summary>
		public DayOfWeek DayOfWeek
		{
			get { return this.Date.DayOfWeek; }
		}

		/// <summary>
		/// get - The application date day of the year.
		/// </summary>
		public int DayOfYear
		{
			get { return this.Date.DayOfYear; }
		}

		/// <summary>
		/// get - The application current date.
		/// </summary>
		public static DateTime Now
		{
			get { return _InternalNow.Value.ToLocalTime(); }
		}

		/// <summary>
		/// get - The application current date.
		/// </summary>
		public static DateTime UtcNow
		{
			get { return _InternalNow.Value.ToUniversalTime(); }
		}

		/// <summary>
		/// get - The application current date at 12:00AM.
		/// </summary>
		public static DateTime UtcMorning
		{
			get { return new DateTime(AppDateTime.Now.Year, AppDateTime.Now.Month, AppDateTime.Now.Day, 0, 0, 0, DateTimeKind.Local).ToUniversalTime(); }
		}

		public static DateTime CurrentFYStartDateMorning
		{
			get
			{
				// Based on a standard FY : April 01 to Mar 31
				return AppDateTime.Now.Month > 3 ? new DateTime(AppDateTime.Now.Year, 4, 1, 00, 00, 00, DateTimeKind.Local) : new DateTime(AppDateTime.Now.Year - 1, 4, 1, 00, 00, 00, DateTimeKind.Local); // Morning
			}
		}

		public static DateTime CurrentFYEndDateMidnight
		{
			get
			{
				// Based on a standard FY : April 01 to Mar 31
				return AppDateTime.Now.Month > 3 ? new DateTime(AppDateTime.Now.Year + 1, 3, 31, 23, 59, 59, DateTimeKind.Local) : new DateTime(AppDateTime.Now.Year, 3, 31, 23, 59, 59, DateTimeKind.Local); // Midnight
			}
		}
		#endregion

		#region Constructors
		public AppDateTime()
		{
			this.Date = DateTime.SpecifyKind(_InternalNow.Value, DateTimeKind.Local).ToUniversalTime();
		}

		public AppDateTime(int year, int month, int day) : this(year, month, day, 0, 0, 0, 0)
		{
		}

		public AppDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond = 0)
		{
			this.Date = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Local).ToUniversalTime();
		}
		#endregion

		#region Methods
		public static implicit operator DateTime(AppDateTime value)
		{
			return value.Date;
		}

		public DateTime AddDays(double days)
		{
			return this.Date.AddDays(days);
		}

		public DateTime AddMonths(int months)
		{
			return this.Date.AddMonths(months);
		}

		public DateTime AddYears(int years)
		{
			return this.Date.AddYears(years);
		}

		public DateTime AddHours(double hours)
		{
			return this.Date.AddHours(hours);
		}

		public DateTime AddMinutes(double minutes)
		{
			return this.Date.AddMinutes(minutes);
		}

		public DateTime AddSeconds(double seconds)
		{
			return this.Date.AddSeconds(seconds);
		}

		public DateTime AddMilliseconds(double milliseconds)
		{
			return this.Date.AddMilliseconds(milliseconds);
		}

		public override string ToString()
		{
			return this.Date.ToString();
		}

		public string ToString(string format)
		{
			return this.Date.ToString(format);
		}

		/// <summary>
		/// Set the internal Now value to test different datetime scenarios.
		/// </summary>
		/// <param name="today"></param>
		public static void SetNow(DateTime today)
		{
			_InternalNow = today.ToUniversalTime();
		}

		/// <summary>
		/// Set the internal Now value to test different datetime scenarios.
		/// </summary>
		/// <param name="year"></param>
		/// <param name="month"></param>
		/// <param name="day"></param>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <param name="millisecond"></param>
		public static void SetNow(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
		{
			_InternalNow = new AppDateTime(year, month, day, hour, minute, second, millisecond);
		}

		/// <summary>
		/// Reseting the internal Now will allow all dates to use the current machine time.
		/// </summary>
		public static void ResetNow()
		{
			_InternalNow = null;
		}
		#endregion
	}
}