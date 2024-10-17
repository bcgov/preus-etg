using CJG.Application.Services;
using CJG.Infrastructure.BCeID.WebService.BCeID;
using System;

namespace CJG.Infrastructure.BCeID.WebService
{
	/// <summary>
	/// <typeparamref name="BCeIDException"/> class, provides a way to capture exceptions related to BCeID requests.
	/// </summary>
	public class BCeIDException : Exception
	{
		#region Constructors
		public BCeIDException(string message) : base(message)
		{

		}

		public BCeIDException(string message, Exception innerException) : base(message, innerException)
		{

		}

		public BCeIDException(AccountDetailResponse response) : base($"{response.failureCode}: {response.message}")
		{
			if (response.code == ResponseCode.Success)
				throw new InvalidOperationException("BCeID response was successful an exception should not be thrown.");
			else
			{
				switch (response.failureCode)
				{
					case FailureCode.AuthenticationException:
					case FailureCode.AuthorizationException:
						throw new NotAuthorizedException();
					default:
						throw new Exception($"BCeID response was not successful due to '{response.failureCode.ToString()}'");
				}
			}
		}
		#endregion
	}
}
