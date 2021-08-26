using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.Applications
{
	public class ApplicationComponentViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public bool CanEdit { get; set; }
		public bool CanAdd { get; set; }
		public string Caption { get; set; }
		public string Category { get; set; }

		#region Service Categories
		public int? ServiceCategoryId { get; set; }
		public ServiceTypes? ServiceCategoryTypeId { get; set; }
		public int? EligibleExpenseTypeId { get; set; }
		public int MaxProviders { get; set; }
		public int MinProviders { get; set; }
		public int RowSequence { get; set; }

		public IEnumerable<ProgramComponentViewModel> Programs { get; set; }
		public IEnumerable<ProviderComponentViewModel> Providers { get; set; }
		#endregion
		#endregion

		#region Constructors
		public ApplicationComponentViewModel() { }

		public ApplicationComponentViewModel(TrainingProvider trainingProvider, IPrincipal user, int rowSequense = 0)
		{
			if (trainingProvider == null) throw new ArgumentNullException(nameof(trainingProvider));
			if (user == null) throw new ArgumentNullException(nameof(user));

			var grantApplication = trainingProvider.GetGrantApplication();
			this.Id = trainingProvider.Id;
			this.RowVersion = Convert.ToBase64String(trainingProvider.RowVersion);
			this.CanEdit = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingProvider);
			this.CanAdd = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
			this.Caption = "Training Provider";
			this.RowSequence = rowSequense;
			this.Category = "TrainingProvider";
			this.EligibleExpenseTypeId = trainingProvider?.EligibleCost?.EligibleExpenseTypeId;

			var providers = new List<ProviderComponentViewModel>();
			if (trainingProvider.RequestedTrainingProvider != null) providers.Add(new ProviderComponentViewModel(trainingProvider.RequestedTrainingProvider, user, true)); // Change Request.
			providers.Add(new ProviderComponentViewModel(trainingProvider, user));
			this.Providers = providers.ToArray(); 
		}

		public ApplicationComponentViewModel(TrainingProgram trainingProgram, IPrincipal user, int rowSequense = 0)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = trainingProgram.Id;
			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			this.CanEdit = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.EditTrainingProgram);
			this.CanAdd = user.CanPerformAction(trainingProgram.GrantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram);
			this.Caption = "Training Program";
			this.RowSequence = rowSequense;
			this.Category = "TrainingProgram";
			this.EligibleExpenseTypeId = trainingProgram?.EligibleCostBreakdown?.EligibleCost?.EligibleExpenseTypeId;

			this.Programs = new[] { new ProgramComponentViewModel(trainingProgram, user) };
		}

		/// <summary>
		/// Creates a new instance of a ApplicationComponentViewModel object.
		/// This component may be a service category.
		/// A service category may have services, programs and/or providers.
		/// </summary>
		/// <param name="eligibleCost"></param>
		/// <param name="user"></param>
		public ApplicationComponentViewModel(EligibleCost eligibleCost, IPrincipal user)
		{
			if (eligibleCost == null) throw new ArgumentNullException(nameof(eligibleCost));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = eligibleCost.Id;
			this.RowVersion = Convert.ToBase64String(eligibleCost.RowVersion);
			this.Caption = eligibleCost.EligibleExpenseType.Caption;
			this.RowSequence = eligibleCost.EligibleExpenseType.RowSequence;
			this.ServiceCategoryId = eligibleCost.EligibleExpenseType.ServiceCategoryId;
			this.ServiceCategoryTypeId = eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;
			this.EligibleExpenseTypeId = eligibleCost.EligibleExpenseTypeId;
			this.MaxProviders = eligibleCost.EligibleExpenseType.MaxProviders;
			this.MinProviders = eligibleCost.EligibleExpenseType.MinProviders;
			this.Category = this.ServiceCategoryTypeId?.ToString("g");

			switch (this.ServiceCategoryTypeId)
			{
				case (ServiceTypes.SkillsTraining):
					this.CanAdd = (eligibleCost.EligibleExpenseType?.ServiceCategory?.MaxPrograms ?? 0) > 0 && user.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.AddOrRemoveTrainingProgram);
					break;
				case (ServiceTypes.EmploymentServicesAndSupports):
					this.CanAdd = eligibleCost.EligibleExpenseType.MaxProviders > 0 && user.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.AddRemoveTrainingProvider);
					this.CanEdit = user.CanPerformAction(eligibleCost.TrainingCost.GrantApplication, ApplicationWorkflowTrigger.EditTrainingCosts); // Has services.
					break;
			}

			this.Programs = eligibleCost.Breakdowns.Where(b => b.TrainingPrograms.Any()).Select(b => new ProgramComponentViewModel(b.TrainingPrograms.FirstOrDefault(), user)).ToArray();

			var providers = new List<ProviderComponentViewModel>();
			foreach (var provider in eligibleCost.TrainingCost.GrantApplication.TrainingProviders.Where(tp => tp.EligibleCostId == eligibleCost.Id && tp.OriginalTrainingProviderId == null)) // Only return original training providers.
			{
				var trainingProvider = provider.ApprovedTrainingProvider ?? provider;
				if (trainingProvider.RequestedTrainingProvider != null && trainingProvider.GetGrantApplication().ApplicationStateInternal.In(ApplicationStateInternal.ChangeRequest, ApplicationStateInternal.ChangeForApproval, ApplicationStateInternal.ChangeForDenial, ApplicationStateInternal.ChangeReturned))
				{
					providers.Add(new ProviderComponentViewModel(trainingProvider.RequestedTrainingProvider, user, true)); // Change Request.
				}
				providers.Add(new ProviderComponentViewModel(trainingProvider, user));
			}
			this.Providers = providers.ToArray();
		}
		#endregion
	}
}