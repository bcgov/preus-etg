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
	/// <typeparamref name="EligibleExpenseBreakdownService"/> class, provides service methods related to <typeparamref name="EligibleExpenseBreakdown"/> objects.
	/// </summary>
	public class EligibleExpenseBreakdownService : Service, IEligibleExpenseBreakdownService
	{
		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="EligibleExpenseBreakdownService"/> and initializes the specified properties.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public EligibleExpenseBreakdownService(
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
		   
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the eligible expense breakdown for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public EligibleExpenseBreakdown Get(int id)
		{
			return Get<EligibleExpenseBreakdown>(id);
		}

		/// <summary>
		/// Add the specified eligible expense breakdown to the datasource.
		/// </summary>
		/// <param name="breakdown"></param>
		/// <returns></returns>
		public EligibleExpenseBreakdown Add(EligibleExpenseBreakdown breakdown)
		{
			_dbContext.EligibleExpenseBreakdowns.Add(breakdown);
			_dbContext.Commit();
			return breakdown;
		}

		/// <summary>
		/// Delete the specified eligible expense breakdown from the datasource.
		/// </summary>
		/// <param name="breakdown"></param>
		public void Delete(EligibleExpenseBreakdown breakdown)
		{
			var entity = Get<EligibleExpenseBreakdown>(breakdown.Id);

			if (entity.ClaimBreakdownCosts.Select(t => t.ClaimEligibleCost).Any() ||
				entity.ExpenseType.EligibleCosts.Any())
			{
				throw new InvalidOperationException("This breakdown is used in existing grant file and can’t be deleted");
			}

			_dbContext.EligibleExpenseBreakdowns.Remove(entity);
			_dbContext.Commit();
		}

		/// <summary>
		/// Get all the active eligible expense breakdowns for the specified eligible expense type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<EligibleExpenseBreakdown> GetAllActiveForEligibleExpenseType(int id)
		{
			return _dbContext.EligibleExpenseBreakdowns.AsNoTracking().Where(t => t.EligibleExpenseTypeId == id && t.IsActive).OrderBy(eeb => eeb.RowSequence).ThenBy(eeb => eeb.Caption).ToArray();
		}

		/// <summary>
		/// Get all the eligible expense breakdowns for the specified eligible expense type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IEnumerable<EligibleExpenseBreakdown> GetAllForEligibleExpenseType(int id)
		{
			return _dbContext.EligibleExpenseBreakdowns.AsNoTracking().Where(t => t.EligibleExpenseTypeId == id).OrderBy(eeb => eeb.RowSequence).ThenBy(eeb => eeb.Caption).ToArray();
		}

		/// <summary>
		/// Update the specified eligible expense breakdown in the datasource.
		/// </summary>
		/// <param name="breakdown"></param>
		/// <returns></returns>
		public EligibleExpenseBreakdown Update(EligibleExpenseBreakdown breakdown)
		{
			_dbContext.Update<EligibleExpenseBreakdown>(breakdown);
			_dbContext.Commit();
			return breakdown;
		}

		/// <summary>
		/// Get the active eligible expense breakdown associated with the specified service line.
		/// </summary>
		/// <param name="serviceLineId"></param>
		/// <returns></returns>
		public EligibleExpenseBreakdown GetForServiceLine(int serviceLineId)
		{
			return _dbContext.EligibleExpenseBreakdowns.FirstOrDefault(b => b.ServiceLineId == serviceLineId && b.IsActive);
		}
		#endregion
	}
}
