using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class TrainingServiceProviderViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public TrainingProviderDetailsViewModel TrainingProvider { get; set; }
		public ServiceProviderDetailsViewModel ServiceProvider { get; set; }
		public string RowVersion { get; set; }
		public string EligibleCostBreakdownRowVersion { get; set; }
		#endregion

		#region Constructors
		public TrainingServiceProviderViewModel()
		{
		}

		public TrainingServiceProviderViewModel(TrainingProvider trainingProvider)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			var grantApplication = trainingProvider.GetGrantApplication();
			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.TrainingProvider = new TrainingProviderDetailsViewModel(trainingProvider)
			{
				GrantApplicationId = grantApplication.Id
			};
			this.EligibleCostBreakdownRowVersion = trainingProvider.TrainingProgram?.EligibleCostBreakdown != null ?
											Convert.ToBase64String(trainingProvider.TrainingProgram.EligibleCostBreakdown.RowVersion) : null;
		}

		public TrainingServiceProviderViewModel(TrainingProvider trainingProvider, EligibleExpenseType eligibleExpenseType)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (eligibleExpenseType == null) throw new ArgumentNullException(nameof(eligibleExpenseType));

			var grantApplication = trainingProvider.GetGrantApplication();
			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ServiceProvider = new ServiceProviderDetailsViewModel(trainingProvider, eligibleExpenseType)
			{
				GrantApplicationId = grantApplication.Id
			};
			this.EligibleCostBreakdownRowVersion = trainingProvider.TrainingProgram?.EligibleCostBreakdown != null ?
											Convert.ToBase64String(trainingProvider.TrainingProgram.EligibleCostBreakdown.RowVersion) : null;
		}

		public TrainingServiceProviderViewModel(GrantApplication grantApplication, EligibleExpenseType eligibleExpenseType)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (eligibleExpenseType == null) throw new ArgumentNullException(nameof(eligibleExpenseType));

			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.ServiceProvider = new ServiceProviderDetailsViewModel(eligibleExpenseType)
			{
				GrantApplicationId = grantApplication.Id
			};
		}
		#endregion
	}
}
