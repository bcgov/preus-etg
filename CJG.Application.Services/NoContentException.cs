using System;
using System.Runtime.Serialization;

namespace CJG.Application.Services
{
	[Serializable]
	public class NoContentException : Exception
	{
		public NoContentException() : base("The resource requested does not exist, or you do not have permission to view it.")
		{
		}

		public NoContentException(string message) : base(message)
		{
		}

		public NoContentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoContentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}