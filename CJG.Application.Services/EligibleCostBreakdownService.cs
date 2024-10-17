using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System.Web;

namespace CJG.Application.Services
{

	public class EligibleCostBreakdownService : Service, IEligibleCostBreakdownService
	{
		#region Variables
		#endregion

		#region Constructors
		public EligibleCostBreakdownService(IDataContext context,
										HttpContextBase httpContext,
										ILogger logger) : base(context, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get the eligible cost for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		public EligibleCostBreakdown Get(int id)
		{
			var eligibleCostBreakdown = Get<EligibleCostBreakdown>(id);

			if (!_httpContext.User.CanPerformAction(eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to view application {eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplicationId}.");

			return eligibleCostBreakdown;
		}

		/// <summary>
		/// Add the specified eligible cost to the datasource.
		/// </summary>
		/// <param name="eligibleCostBreakdown"></param>
		/// <returns></returns>
		public EligibleCostBreakdown Add(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"User does not have access to the specified application {eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplicationId}.");

			_dbContext.EligibleCostBreakdowns.Add(eligibleCostBreakdown);
			CommitTransaction();

			return eligibleCostBreakdown;
		}

		/// <summary>
		/// Update the specified eligible cost in the datasource.
		/// </summary>
		/// <param name="eligibleCostBreakdown"></param>
		/// <returns></returns>
		public EligibleCostBreakdown Update(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"User does not have access to the specified application {eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplicationId}.");

			_dbContext.Update<EligibleCostBreakdown>(eligibleCostBreakdown);
			CommitTransaction();

			return eligibleCostBreakdown;
		}

		/// <summary>
		/// Delete the specified eligible cost from the datasource.
		/// </summary>
		/// <param name="eligibleCostBreakdown"></param>
		public void Delete(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (!_httpContext.User.CanPerformAction(eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"User does not have access to the specified application {eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplicationId}.");

			if (!eligibleCostBreakdown.AddedByAssessor && _httpContext.User.GetUserId() == eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication.AssessorId)
				throw new NotAuthorizedException($"User does not have permission to take this action on the application {eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplicationId}.");

			_dbContext.EligibleCostBreakdowns.Remove(eligibleCostBreakdown);
			CommitTransaction();
		}
		#endregion
	}
}
