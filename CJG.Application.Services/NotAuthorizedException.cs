using System;
using System.Runtime.Serialization;

namespace CJG.Application.Services
{
	[Serializable]
	public class NotAuthorizedException : Exception
	{
		public NotAuthorizedException() : base("You are not authorized to view this resource, or perform this action.")
		{
		}

		public NotAuthorizedException(string message) : base(message)
		{
		}

		public NotAuthorizedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}