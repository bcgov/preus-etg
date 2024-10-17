using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
	public class ChangeRequestESSViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public int RowSequence { get; set; }

		public string ServiceComponentName { get; set; }
		public int TotalCountOfProviders
		{
			get
			{
				return this.ServiceProviders.Count();
			}
		}
		public List<ChangeRequestESSServiceProviderViewModel> ServiceProviders { get; set; } = new List<ChangeRequestESSServiceProviderViewModel>();
		public List<ChangeRequestESSServiceScopeViewModel> ServiceScopes { get; set; } = new List<ChangeRequestESSServiceScopeViewModel>();
		#endregion

		#region Constructors
		public ChangeRequestESSViewModel()
		{

		}
		public ChangeRequestESSViewModel(EligibleCost eligibleCost)
		{
			if (eligibleCost == null) throw new ArgumentNullException(nameof(eligibleCost));
			this.GrantApplicationId = eligibleCost.TrainingCost.GrantApplicationId;
			this.RowSequence = eligibleCost.EligibleExpenseType.RowSequence;
			this.ServiceComponentName = eligibleCost.EligibleExpenseType.ServiceCategory.Caption;
			this.ServiceProviders = eligibleCost.TrainingProviders.Where(tp => tp.ApprovedTrainingProvider.IsValidated()).Select(tp => tp.ApprovedTrainingProvider).Distinct().Select(tp => new ChangeRequestESSServiceProviderViewModel(tp.ApprovedTrainingProvider, this.GrantApplicationId)).ToList();
			this.ServiceScopes = eligibleCost.Breakdowns.Select(x => new ChangeRequestESSServiceScopeViewModel(x)).ToList();
		}
		#endregion
	}
}