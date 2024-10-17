using System;
using System.Runtime.Serialization;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="NotificationException"/> class, provides a way to capture and bubble notification related exceptions.
	/// </summary>
	[Serializable]
	public class NotificationException : Exception
	{
		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationException"/> object.
		/// </summary>
		public NotificationException()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationException"/> object.
		/// </summary>
		/// <param name="message"></param>
		public NotificationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationException"/> object.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public NotificationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationException"/> object.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}