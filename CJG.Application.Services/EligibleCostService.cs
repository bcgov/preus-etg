using System.Collections.Generic;
using System.Linq;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="EligibleCostService"/> class, provides a way to manage access to eligible costs.
	/// </summary>
	public class EligibleCostService : Service, IEligibleCostService
	{
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingService"/> class.
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public EligibleCostService(IDataContext dbContext,
							   HttpContextBase httpContext,
							   ILogger logger) : base(dbContext, httpContext, logger)
		{
		}

		/// <summary>
		/// Get the eligible cost for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		public EligibleCost Get(int id)
		{
			var eligibleCost = Get<EligibleCost>(id);
			if (eligibleCost == null)
				throw new NoContentException(nameof(eligibleCost));
			if (!_httpContext.User.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to view application {eligibleCost.TrainingCost.GrantApplicationId}.");

			return eligibleCost;
		}

		/// <summary>
		/// Get all the eligible costs for the specified grant application of the specified service type.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable<EligibleCost> GetForGrantApplication(int grantApplicationId, ServiceTypes type)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to view application {grantApplicationId}.");

			return _dbContext.EligibleCosts.Where(o => o.GrantApplicationId == grantApplicationId && o.EligibleExpenseType.ServiceCategory.ServiceTypeId == type).ToArray();
		}

		/// <summary>
		/// Get all the eligible costs for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		public IEnumerable<EligibleCost> GetForGrantApplication(int grantApplicationId)
		{
			var grantApplication = Get<GrantApplication>(grantApplicationId);

			if (!_httpContext.User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to view application {grantApplicationId}.");

			return _dbContext.EligibleCosts.Where(o => o.GrantApplicationId == grantApplicationId).ToArray();
		}

		/// <summary>
		/// Add the specified eligible cost to the datasource.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public EligibleCost Add(EligibleCost eligibleCost)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"Use does not have permission to edit training cost.");

			_dbContext.EligibleCosts.Add(eligibleCost);
			_dbContext.CommitTransaction();

			return eligibleCost;
		}

		/// <summary>
		/// Update the specified eligible cost in the datasource.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <returns></returns>
		public EligibleCost Update(EligibleCost eligibleCost)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"Use does not have permission to edit training cost.");

			_dbContext.Update<EligibleCost>(eligibleCost);
			_dbContext.CommitTransaction();

			return eligibleCost;
		}

		/// <summary>
		/// Delete the specified eligible cost from the datasource.
		/// </summary>
		/// <param name="eligibleCost"></param>
		public void Delete(EligibleCost eligibleCost)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"Use does not have permission to edit training cost.");

			foreach (var breakdown in eligibleCost.Breakdowns)
			{
				_dbContext.EligibleCostBreakdowns.Remove(breakdown);
			}

			_dbContext.EligibleCosts.Remove(eligibleCost);
			_dbContext.CommitTransaction();
		}
	}
}
