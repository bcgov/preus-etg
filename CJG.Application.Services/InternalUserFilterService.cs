using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class InternalUserFilterService : Service, IInternalUserFilterService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public InternalUserFilterService(
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the InternalUserFilter for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public InternalUserFilter Get(int id)
		{
			return Get<InternalUserFilter>(id);
		}

		/// <summary>
		/// Get an array of InternalUserFilter for the current user.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InternalUserFilter> GetForUser()
		{
			var userId = _httpContext.User.GetUserId().Value;
			return _dbContext.InternalUserFilters.AsNoTracking().Where(x => x.InternalUserId == userId).OrderBy(f => f.RowSequence).ThenBy(f => f.Name).ToArray();
		}

		/// <summary>
		/// Add the specified InternalUserFilter to the datasource.
		/// </summary>
		/// <param name="filter"></param>
		public void Add(InternalUserFilter filter)
		{
			var userId = _httpContext.User.GetUserId().Value;
			filter.InternalUserId = userId;
			_dbContext.InternalUserFilters.Add(filter);
			CommitTransaction();
		}

		/// <summary>
		/// Delete the specified InternalUserFilter from the datasource.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="id"></param>
		public void Delete(InternalUserFilter filter)
		{
			var userId = _httpContext.User.GetUserId().Value;
			var originalUserId = (int)_dbContext.OriginalValue(filter, nameof(InternalUserFilter.InternalUserId));
			if (originalUserId != userId) throw new NotAuthorizedException("User does not have permission to delete filter.");

			_dbContext.InternalUserFilters.Remove(filter);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Update the specified InternalUserFilter in the datasource.
		/// </summary>
		/// <param name="filter"></param>
		public void Update(InternalUserFilter filter)
		{
			var userId = _httpContext.User.GetUserId().Value;
			var originalUserId = (int)_dbContext.OriginalValue(filter, nameof(InternalUserFilter.InternalUserId));
			if (originalUserId != userId) throw new NotAuthorizedException("User does not have permission to update filter.");

			_dbContext.Update<InternalUserFilter>(filter);
			_dbContext.CommitTransaction();
		}
		#endregion
	}
}
