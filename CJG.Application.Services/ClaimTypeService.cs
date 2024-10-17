using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ClaimTypeService"/> class, provides a way to manage claim types.
	/// </summary>
	public class ClaimTypeService : Service, IClaimTypeService
	{
		#region Variables

		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ClaimTypeService"/> object.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ClaimTypeService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns all the claim types for the system.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ClaimType> GetAll()
		{
			return _dbContext.ClaimTypes.ToList();
		}

		/// <summary>
		/// Returns all the claim types for the system filtered by IsActive.
		/// </summary>
		/// <param name="isActive"></param>
		/// <returns></returns>
		public IEnumerable<ClaimType> GetAll(bool isActive)
		{
			return _dbContext.ClaimTypes.AsNoTracking().Where(t => t.IsActive == isActive).ToList();
		}

		/// <summary>
		/// Returns the claim type for the specified Id.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="NoContentException">The claim type does not exist.</exception>
		public ClaimType GetClaimType(ClaimTypes type)
		{
			return _dbContext.ClaimTypes.FirstOrDefault(t => t.Id == type) ?? throw new NoContentException();
		}
		#endregion
	}
}