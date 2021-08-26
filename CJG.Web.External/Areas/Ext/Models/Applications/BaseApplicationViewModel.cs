using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class BaseApplicationViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public string FileNumber { get; set; }
		public string FileName { get; set; }
		public string FullName { get; set; }
		public string GrantProgramName { get; set; }
		public string GrantStreamName { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public ApplicationStateExternal ApplicationStateExternal { get; set; }
		public GrantOpeningStates GrantOpeningState { get; set; }
		public DateTime? GrantOpeningOpeningDate { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public DateTime GrantOpeningTrainingPeriodStartDate { get; set; }
		public DateTime GrantOpeningTrainingPeriodEndDate { get; set; }
		public bool EligibilityConfirmed { get; set; }
		public bool HasValidDate { get; set; }
		public OverviewTrainingProvider TrainingProvider { get; set; }
		public OverviewTrainingProgram TrainingProgram { get; set; }
		public OverviewProgramDescription ProgramDescription { get; set; }
		public OverviewTrainingCost TrainingCost { get; set; }
		public IEnumerable<DynamicProcess> DynamicProcesses { get; set; }
		public int EstimatedParticipants { get; set; }
		public bool EnableAttachments { get; set; } = false;
		public string AttachmentsHeader { get; set; }
		public int AttachmentsState { get; set; }
		public int AttachmentCount { get; set; }
		public bool AttachmentsRequired { get; set; }
		#endregion

		#region Constructors
		public BaseApplicationViewModel()
		{

		}

		public BaseApplicationViewModel(GrantApplication grantApplication)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.FileNumber = grantApplication.FileNumber;
			this.FileName = string.IsNullOrWhiteSpace(grantApplication.TrainingPrograms.FirstOrDefault()?.CourseTitle)
				? string.Format("Training Program Title")
				: grantApplication.TrainingPrograms.FirstOrDefault().CourseTitle;
			this.GrantProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.GrantStreamName = grantApplication.GrantOpening.GrantStream.Name;
			this.GrantOpeningOpeningDate = grantApplication.GrantOpening.OpeningDate == DateTime.MinValue ? null : (DateTime?)grantApplication.GrantOpening.OpeningDate.ToLocalMorning();
			this.StartDate = grantApplication.StartDate.ToLocalTime();
			this.EndDate = grantApplication.EndDate.ToLocalTime();
			this.ApplicationStateExternal = grantApplication.ApplicationStateExternal;
			this.GrantOpeningState = grantApplication.GrantOpening.State;

			this.GrantOpeningTrainingPeriodStartDate = grantApplication.GrantOpening.TrainingPeriod.StartDate;
			this.GrantOpeningTrainingPeriodEndDate = grantApplication.GrantOpening.TrainingPeriod.EndDate;

			this.FullName = grantApplication.GrantOpening.GrantStream.FullName;
			this.EligibilityConfirmed = grantApplication.EligibilityConfirmed();
			this.HasValidDate = grantApplication.HasValidDates(grantApplication.GrantOpening.TrainingPeriod.StartDate, grantApplication.GrantOpening.TrainingPeriod.EndDate);
			this.EnableAttachments = grantApplication.GrantOpening.GrantStream.AttachmentsIsEnabled;
			this.AttachmentsRequired = grantApplication.GrantOpening.GrantStream.AttachmentsRequired;
			this.AttachmentsHeader = grantApplication.GrantOpening.GrantStream.AttachmentsHeader;			
			this.AttachmentCount = grantApplication.Attachments.Count();
			this.AttachmentsState = this.AttachmentsRequired && grantApplication.ApplicationStateExternal == ApplicationStateExternal.ApplicationWithdrawn ? 1 : grantApplication.Attachments.Any() ? 2 : 0;

			if (this.ProgramType == ProgramTypes.EmployerGrant)
			{
				var trianingProgram = grantApplication.TrainingPrograms.FirstOrDefault();
				if (trianingProgram != null)
				{
					this.TrainingProgram = new OverviewTrainingProgram(trianingProgram);
				}

				var trainingProvider = grantApplication.TrainingProviders.FirstOrDefault() ?? trianingProgram?.TrainingProviders.FirstOrDefault();
				if (trainingProvider != null)
				{
					this.TrainingProvider = new OverviewTrainingProvider(trainingProvider);
				}
			}
			else
			{
				this.ProgramDescription = new OverviewProgramDescription(grantApplication);
				this.DynamicProcesses = grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Where(x => x.ServiceCategory.ServiceTypeId != ServiceTypes.Administration && x.IsActive).Select(x => new DynamicProcess(grantApplication, x)).OrderBy(x => x.RowSequence).ToList();
				if (!string.IsNullOrEmpty(ProgramDescription.Description))
					this.FileName = ProgramDescription.Description;
			}

			if (grantApplication.TrainingCost != null)
			{
				this.TrainingCost = new OverviewTrainingCost(grantApplication.TrainingCost);
				if (grantApplication.TrainingCost.EligibleCosts != null)
				{
					var hasOfferBeenIssued = grantApplication.HasOfferBeenIssued();
					if (!hasOfferBeenIssued)
					{
						this.TrainingCost.EstimatedCosts = grantApplication.TrainingCost.EligibleCosts.Where(t => !t.AddedByAssessor).Select(x => new EstimatedCostViewModel(x, grantApplication)).ToArray();
						this.TrainingCost.TotalCost = grantApplication.TrainingCost.TotalEstimatedCost;
						this.TrainingCost.TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
					}
					else
					{
						this.TrainingCost.EstimatedCosts = grantApplication.TrainingCost.EligibleCosts.Where(t => t.AgreedMaxReimbursement > 0).Select(x => new EstimatedCostViewModel(x, grantApplication)).ToArray();
						this.TrainingCost.TotalCost = grantApplication.TrainingCost.TotalAgreedMaxCost;
						this.TrainingCost.TotalRequest = grantApplication.TrainingCost.AgreedCommitment;
					}

					this.TrainingCost.TotalEmployer = this.TrainingCost.TotalCost - this.TrainingCost.TotalRequest;
					this.TrainingCost.ESSAveragePerParticipant = this.TrainingCost.EstimatedCosts.Where(x => x.ServiceType == (int?)ServiceTypes.EmploymentServicesAndSupports).Sum(x => !hasOfferBeenIssued ? x.EstimatedParticipantCost : x.AgreedMaxParticipantCost);
				}
			}
		}
		#endregion
	}
}