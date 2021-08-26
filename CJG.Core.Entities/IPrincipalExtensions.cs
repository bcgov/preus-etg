using System;
using System.Security.Claims;
using System.Security.Principal;

namespace CJG.Core.Entities
{
	public static class IPrincipalExtensions
	{

		/// <summary>
		/// Get the user account type for the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static AccountTypes? GetAccountType(this IPrincipal user)
		{
			var cp = user as ClaimsPrincipal;

			if (cp == null)
				return null;

			return (AccountTypes)Enum.Parse(typeof(AccountTypes), cp.FindFirst(AppClaimTypes.AccountType)?.Value, true);
		}

		/// <summary>
		/// Deteremine if the user is an external user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsExternalUser(this IPrincipal user)
		{
			return user.GetAccountType() == AccountTypes.External;
		}
	}
}
