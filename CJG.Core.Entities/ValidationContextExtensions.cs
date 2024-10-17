using CJG.Infrastructure.Entities;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Web;

namespace CJG.Core.Entities
{
	/// <summary>
	/// ValidationContextExtensions static class, provides extension methods for ValidationContext object.
	/// </summary>
	public static class ValidationContextExtensions
	{
		/// <summary>
		/// Extract the current DbContext from the ValidationContext items.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns>The current <typeparamref name="DbContext"/>.</returns>
		public static IDataContext GetDbContext(this ValidationContext validationContext)
		{
			if (validationContext.Items.ContainsKey("DbContext"))
				return validationContext.Items["DbContext"] as IDataContext;

			return null;
		}

		/// <summary>
		/// Extract the current DbEntityEntry from the ValidationContext items.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns>The current <typeparamref name="DbEntityEntry"/>.</returns>
		public static DbEntityEntry GetDbEntityEntry(this ValidationContext validationContext)
		{
			if (validationContext.Items.ContainsKey("Entry"))
				return validationContext.Items["Entry"] as DbEntityEntry;

			return null;
		}

		/// <summary>
		/// Extract the current ILogger from the ValidationContext.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public static ILogger GetLogger(this ValidationContext validationContext)
		{
			if (validationContext.Items.ContainsKey("Logger"))
				return validationContext.Items["Logger"] as ILogger;

			return null;
		}

		/// <summary>
		/// Extract the current HttpContextBase from the ValidationContext.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public static HttpContextBase GetHttpContext(this ValidationContext validationContext)
		{
			if (validationContext.Items.ContainsKey("HttpContext"))
				return validationContext.Items["HttpContext"] as HttpContextBase;

			return null;
		}
	}
}
