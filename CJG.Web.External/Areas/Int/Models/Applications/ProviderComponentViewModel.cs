using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ProviderComponentViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public bool CanEdit { get; set; }
		public bool CanRemove { get; set; }
		public bool CanValidate { get; set; }
		public bool IsValidated { get; set; }
		public bool IsChangeRequest { get; set; }
		public bool CanRecommendChangeRequest { get; set; }
		public TrainingProviderStates State { get; set; }
		public bool AddedByAssessor { get; set; }
		public ProviderComponentViewModel RequestedProvider { get; set; }
		#endregion

		#region Constructors
		public ProviderComponentViewModel() { }

		public ProviderComponentViewModel(TrainingProvider trainingProvider, IPrincipal user, bool isChangeRequest = false)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProvider.Id;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.Caption = trainingProvider.Name;
			this.State = trainingProvider.TrainingProviderState;
			this.IsValidated = trainingProvider.TrainingProviderInventoryId.HasValue;
			this.IsChangeRequest = isChangeRequest;
			this.AddedByAssessor = trainingProvider.EligibleCost?.AddedByAssessor ?? false;

			this.CanEdit = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.CanRemove = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
			this.CanValidate = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.ValidateTrainingProvider);
			this.CanRecommendChangeRequest = user.CanPerformAction(trainingProvider, ApplicationWorkflowTrigger.EditTrainingProvider);

			if (trainingProvider.RequestedTrainingProvider != null && trainingProvider.GetGrantApplication().ApplicationStateInternal.In(ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.ChangeReturned))
			{
				this.RequestedProvider = new ProviderComponentViewModel(trainingProvider.RequestedTrainingProvider, user, true);
			}
		}
		#endregion
	}
}