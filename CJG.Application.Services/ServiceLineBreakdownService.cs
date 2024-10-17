using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="ServiceLineBreakdownService"/> class, provides a way to manage service line breakdowns in the datasource.
	/// </summary>
	public class ServiceLineBreakdownService : Service, IServiceLineBreakdownService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceLineBreakdownService"/> class and initializes it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ServiceLineBreakdownService(IDataContext context, HttpContextBase httpContext, ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods

		/// <summary>
		/// Get all the active service line breakdowns for the specified service line.
		/// </summary>
		/// <param name="serviceLineId"></param>
		/// <returns></returns>
		public IEnumerable<ServiceLineBreakdown> GetAll(bool isActive)
		{
			return _dbContext.ServiceLineBreakdowns.AsNoTracking().Where(x => x.IsActive == isActive).OrderBy(o => o.RowSequence).ThenBy(o => o.Caption);
		}

		/// <summary>
		/// Get all the active service line breakdowns for the specified service line.
		/// </summary>
		/// <param name="serviceLineId"></param>
		/// <returns></returns>
		public IEnumerable<ServiceLineBreakdown> GetAllForServiceLine(int serviceLineId, bool isActive)
		{
			return _dbContext.ServiceLineBreakdowns.AsNoTracking().Where(x => x.ServiceLineId == serviceLineId && x.IsActive == isActive);
		}

		/// <summary>
		/// Get all the service line breakdowns for the specified service line.
		/// </summary>
		/// <param name="serviceLineId"></param>
		/// <returns></returns>
		public IEnumerable<ServiceLineBreakdown> GetAllForServiceLine(int serviceLineId)
		{
			return _dbContext.ServiceLineBreakdowns.AsNoTracking().Where(x => x.ServiceLineId == serviceLineId);
		}

		/// <summary>
		/// Get the service line breakdown for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ServiceLineBreakdown Get(int id)
		{
			return Get<ServiceLineBreakdown>(id);
		}

		/// <summary>
		/// Adds the specified service line breakdown to the datasource.
		/// </summary>
		/// <param name="serviceLineBreakdown"></param>
		/// <returns></returns>
		public ServiceLineBreakdown Add(ServiceLineBreakdown serviceLineBreakdown)
		{
			if (serviceLineBreakdown == null)
				throw new ArgumentNullException(nameof(serviceLineBreakdown));

			Add(serviceLineBreakdown);

			return serviceLineBreakdown;
		}

		/// <summary>
		/// Updates the specified service line breakdown in the datasource.
		/// </summary>
		/// <param name="serviceLineBreakdown"></param>
		/// <returns></returns>
		public ServiceLineBreakdown Update(ServiceLineBreakdown serviceLineBreakdown)
		{
			if (serviceLineBreakdown == null)
				throw new ArgumentNullException(nameof(serviceLineBreakdown));

			_dbContext.Update<ServiceLineBreakdown>(serviceLineBreakdown);
			_dbContext.CommitTransaction();

			return serviceLineBreakdown;
		}

		/// <summary>
		/// Deletes the specified service line breakdown from the datasource.
		/// </summary>
		/// <param name="serviceLineBreakdown"></param>
		public void Delete(ServiceLineBreakdown serviceLineBreakdown)
		{
			this.Remove(serviceLineBreakdown);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Deletes the specified service line breakdown from the context.
		/// </summary>
		/// <param name="serviceLineBreakdown"></param>
		public void Remove(ServiceLineBreakdown serviceLineBreakdown)
		{
			if (serviceLineBreakdown == null)
				throw new ArgumentNullException(nameof(serviceLineBreakdown));

			if (_dbContext.TrainingPrograms.Any(tp => tp.ServiceLineBreakdownId == serviceLineBreakdown.Id))
				throw new InvalidOperationException($"Cannot delete the '{serviceLineBreakdown.Caption}' service line breakdown because it is currently associated to a grant application.");

			_dbContext.ServiceLineBreakdowns.Remove(serviceLineBreakdown);
		}
		#endregion
	}
}
