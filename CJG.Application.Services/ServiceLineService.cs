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
	/// <typeparamref name="ServiceLineService"/> class, provides a way to manage service categories.
	/// </summary>
	public class ServiceLineService : Service, IServiceLineService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ServiceLineService"/> object and initalizes it.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public ServiceLineService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger
			) : base(context, httpContext, logger)
		{
			
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the service line for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ServiceLine Get(int id)
		{
			return Get<ServiceLine>(id);
		}

		/// <summary>
		/// Adds the specified service line to the datasource.
		/// </summary>
		/// <param name="serviceLine"></param>
		/// <returns></returns>
		public ServiceLine Add(ServiceLine serviceLine)
		{
			if (serviceLine == null)
				throw new ArgumentNullException(nameof(serviceLine));

			_dbContext.ServiceLines.Add(serviceLine);

			return serviceLine;
		}

		/// <summary>
		/// Delete the specified service line from the datasource.
		/// </summary>
		/// <param name="serviceLine"></param>
		public void Delete(ServiceLine serviceLine)
		{
			this.Remove(serviceLine);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Delete the specified service line from the context.
		/// </summary>
		/// <param name="serviceLine"></param>
		public void Remove(ServiceLine serviceLine)
		{
			var entity = _dbContext.ServiceLines.Find(serviceLine.Id);

			if (entity.EligibleExpenseBreakdowns.Any())
			{
				if (_dbContext.EligibleCostBreakdowns.Any(ecb => ecb.EligibleExpenseBreakdown.ServiceLineId == serviceLine.Id))
					throw new InvalidOperationException($"Cannot delete the '{serviceLine.Caption}' service line because it is currently associated to a grant application.");

				foreach (var breakdown in entity.EligibleExpenseBreakdowns.ToArray())
				{
					_dbContext.EligibleExpenseBreakdowns.Remove(breakdown);
				}
			}

			if (_dbContext.TrainingPrograms.Any(tp => tp.ServiceLineId == serviceLine.Id))
				throw new InvalidOperationException($"Cannot delete the '{serviceLine.Caption}' service line because it is currently associated to a grant application.");

			foreach (var breakdown in serviceLine.ServiceLineBreakdowns.ToArray())
			{
				if (_dbContext.TrainingPrograms.Any(tp => tp.ServiceLineBreakdownId == breakdown.Id))
					throw new InvalidOperationException($"Cannot delete the '{breakdown.Caption}' service line breakdown because it is currently associated to a grant application.");

				_dbContext.ServiceLineBreakdowns.Remove(breakdown);
			}

			_dbContext.ServiceLines.Remove(serviceLine);
		}
		
		/// <summary>
		/// Return all the service lines for the specified service category.
		/// </summary>
		/// <param name="serviceCategoryId"></param>
		/// <returns></returns>
		public IEnumerable<ServiceLine> GetAllForServiceCategory(int serviceCategoryId)
		{
			return _dbContext.ServiceLines.AsNoTracking().Where(t => t.ServiceCategoryId == serviceCategoryId).ToArray() ;
		}

		/// <summary>
		/// Update the specified service line.
		/// </summary>
		/// <param name="serviceLine"></param>
		/// <returns></returns>
		public ServiceLine Update(ServiceLine serviceLine)
		{
			_dbContext.Update<ServiceLine>(serviceLine);
			_dbContext.CommitTransaction();

			return serviceLine;
		}
		#endregion
	}
}