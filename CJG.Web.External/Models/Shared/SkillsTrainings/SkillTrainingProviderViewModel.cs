using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Models.Shared.SkillsTrainings
{
	public class SkillTrainingProviderViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public int EligibleCostBreakdownId { get; set; }
		public SkillTrainingProviderDetailsViewModel SkillTrainingDetails { get; set; } = new SkillTrainingProviderDetailsViewModel();
		public string RowVersion { get; set; }

		public bool Editable { get; set; }
		public bool RequiresTrainingProviderValidation { get; set; }
		public bool CanValidateTrainingProvider { get; set; }
		#endregion

		#region Constructors
		public SkillTrainingProviderViewModel()
		{
		}

		public SkillTrainingProviderViewModel(GrantApplication grantApplication, IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.GrantApplicationId = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.Editable = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.RequiresTrainingProviderValidation = grantApplication.RequiresTrainingProviderValidation();
			this.CanValidateTrainingProvider = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ValidateTrainingProvider);
		}

		public SkillTrainingProviderViewModel(TrainingProvider trainingProvider, IPrincipal user) : this(trainingProvider?.GetGrantApplication(), user)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));

			this.SkillTrainingDetails = new SkillTrainingProviderDetailsViewModel(trainingProvider);
			this.RequiresTrainingProviderValidation = trainingProvider.TrainingProviderInventoryId == null;
			this.CanValidateTrainingProvider = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);
		}
		#endregion
	}
}
