using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ProgramComponentViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public bool CanEdit { get; set; }
		public bool CanChangeEligibility { get; set; }
		public bool CanRemove { get; set; }
		public bool IsEligible { get; set; }
		public TrainingProgramStates State { get; set; }
		public bool AddedByAssessor { get; set; }
		public IEnumerable<ProviderComponentViewModel> Providers { get; set; }
		public ProviderComponentViewModel Provider { get; set; }
		#endregion

		#region Constructors
		public ProgramComponentViewModel() { }

		public ProgramComponentViewModel(TrainingProgram trainingProgram, IPrincipal user)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProgram.Id;
			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			this.Caption = trainingProgram.CourseTitle;
			this.State = trainingProgram.TrainingProgramState;
			this.IsEligible = trainingProgram.EligibleCostBreakdown?.IsEligible ?? true;
			this.AddedByAssessor = trainingProgram.EligibleCostBreakdown?.AddedByAssessor ?? false;
			this.CanEdit = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingProgram);
			this.CanChangeEligibility = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts);

			var providers = new List<ProviderComponentViewModel>();
			var provider = trainingProgram.TrainingProvider.ApprovedTrainingProvider ?? trainingProgram.TrainingProvider;
			if (provider.RequestedTrainingProvider != null && trainingProgram.GrantApplication.ApplicationStateInternal.In(ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.ChangeReturned))
			{
				providers.Add(new ProviderComponentViewModel(provider.RequestedTrainingProvider, user, true)); // Change Request.
			}
			this.Provider = new ProviderComponentViewModel(provider, user);
			providers.Add(this.Provider);
			this.Providers = providers.ToArray();

			this.CanRemove = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram)
				&& this.Providers.All(p => p.IsValidated && !p.IsChangeRequest)
				&& this.AddedByAssessor;
		}
		#endregion
	}
}