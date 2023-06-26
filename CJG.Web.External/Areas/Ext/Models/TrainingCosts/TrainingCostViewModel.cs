using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.TrainingProviders;

namespace CJG.Web.External.Areas.Ext.Models.TrainingCosts
{
    public class TrainingCostViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string RowVersion { get; set; }

		public bool IsEditable { get; set; }

		public int GrantProgramId { get; set; }
		public ProgramTypes ProgramType { get; set; }
		public int? AgreedParticipants { get; set; }

		[Required(ErrorMessage = "You must enter the number of participants.")]
		public int? EstimatedParticipants { get; set; }
		public decimal MaxReimbursementAmt { get; set; }
		public double ReimbursementRate { get; set; }
		public decimal TotalEstimatedCost { get; set; }
		public decimal TotalEmployer { get; set; }
		public decimal TotalRequest { get; set; }

		public decimal TotalAgreedCost { get; set; }
		public decimal TotalAgreedEmployer { get; set; }
		public decimal TotalAgreedReimbursement { get; set; }
		public IEnumerable<EligibleCostViewModel> EligibleCosts { get; set; } = new List<EligibleCostViewModel>();

		public decimal ESSAgreedAverage { get; set; }
		public decimal ESSEstimatedAverage { get; set; }

		public bool AllExpenseTypeAllowMultiple
		{
			get
			{
				return EligibleCosts.All(t => t.EligibleExpenseType.AllowMultiple && t.EligibleExpenseType.IsActive);
			}
		}

		public bool RequireTravelExpenseForm { get; set; }
		public TrainingCostAttachmentViewModel TravelExpenseDocument { get; set; }

		public bool ShouldDisplayEmployerContribution { get; set; }
		public bool ShouldDisplayESSSummary { get; set; }
		public string UserGuidanceCostEstimates { get; set; }

		public int MaxUploadSize { get; set; }

		public TrainingCostViewModel() { }

		public TrainingCostViewModel(GrantApplication grantApplication, IPrincipal user, IGrantStreamService grantStreamService)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));
			if (grantStreamService == null)
				throw new ArgumentNullException(nameof(grantStreamService));

			//var eligibleExpenseTypes = grantStreamService.GetAllEligibleExpenseTypes(grantApplication.GrantOpening.GrantStreamId).Select(eet => new EligibleExpenseTypeViewModel(eet));
			var autoIncludeEligibleExpenseTypes = grantStreamService.GetAutoIncludeActiveEligibleExpenseTypes(grantApplication.GrantOpening.GrantStreamId).ToArray();

			GrantApplicationId = grantApplication.Id;
			RowVersion = Convert.ToBase64String(grantApplication.TrainingCost.RowVersion);

			IsEditable = user.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingCosts);

			GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			ReimbursementRate = grantApplication.ReimbursementRate;
			AgreedParticipants = grantApplication.TrainingCost.AgreedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.AgreedParticipants;
			EstimatedParticipants = grantApplication.TrainingCost.EstimatedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.EstimatedParticipants;

			var eligibleCosts = !grantApplication.HasOfferBeenIssued()
				? grantApplication.TrainingCost.EligibleCosts
					.Where(ec => !ec.AddedByAssessor)
					.Select(ec => new EligibleCostViewModel(ec))
					.ToList()
				: grantApplication.TrainingCost.EligibleCosts
					.Select(ec => new EligibleCostViewModel(ec))
					.ToList();

			if (eligibleCosts.Count != autoIncludeEligibleExpenseTypes.Count())
			{
				eligibleCosts.AddRange(autoIncludeEligibleExpenseTypes.Where(t => !eligibleCosts.Select(e => e.EligibleExpenseType.Id).Contains(t.Id))
					.Select(eet => new EligibleCostViewModel(eet) { EstimatedParticipants = EstimatedParticipants ?? 0 }).ToArray());
			}

			EligibleCosts = eligibleCosts
				.OrderBy(t => t.EligibleExpenseType.RowSequence)
				.ThenBy(ec => ec.EligibleExpenseType.Caption)
				.ToArray();

			if (grantApplication.TrainingCost != null)
			{
				TotalEstimatedCost = grantApplication.TrainingCost.TotalEstimatedCost;
				TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
				TotalEmployer = TotalEstimatedCost - TotalRequest;
			}

			var haveAnyTravelExpenses = HaveAnyTravelExpenses();
			RequireTravelExpenseForm = haveAnyTravelExpenses;

			TravelExpenseDocument = new TrainingCostAttachmentViewModel(grantApplication.TrainingCost?.TravelExpenseDocument, Id, RowVersion);

			ShouldDisplayEmployerContribution = grantApplication.ReimbursementRate != 1;
			ShouldDisplayESSSummary = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			UserGuidanceCostEstimates =
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration?.GrantPrograms.Count == 0 ?
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceCostEstimates
				: grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramConfiguration.UserGuidanceCostEstimates;

			var maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			MaxUploadSize = maxUploadSize / 1024 / 1024;
		}

		public GrantApplication UpdateTrainingCosts(IGrantApplicationService grantApplicationService, IAttachmentService attachmentService, HttpPostedFileBase[] files)
		{
			if (grantApplicationService == null)
				throw new ArgumentNullException(nameof(grantApplicationService));

			var grantApplication = grantApplicationService.Get(GrantApplicationId);

			var trainingCost = grantApplication.TrainingCost;
			trainingCost.RowVersion = Convert.FromBase64String(RowVersion);
			trainingCost.EstimatedParticipants = EstimatedParticipants.Value;

			// Remove any eligible cost that exists in the datasource but not in the updated training cost.
			var currentCostIds = EligibleCosts.Select(x => x.Id).ToArray();
			var removeEligibleCosts = trainingCost.EligibleCosts.Where(ec => !currentCostIds.Contains(ec.Id)).ToArray();
			var currentBreakdownIds = EligibleCosts.SelectMany(t => t.Breakdowns).Select(t => t.Id);
			var removeEligibleCostBreakdownIds = trainingCost.EligibleCosts.SelectMany(t => t.Breakdowns).Where(t => !currentBreakdownIds.Contains(t.Id)).Select(b => b.Id).Distinct().ToArray();

			// Remove eligible costs.
			foreach (var remove in removeEligibleCosts)
			{
				trainingCost.EligibleCosts.Remove(remove);
			}

			// Update eligible costs.
			foreach (var cost in EligibleCosts)
			{
				var expenseType = grantApplicationService.Get<EligibleExpenseType>(cost.EligibleExpenseType.Id);
				var eligibleCost = cost.Id == 0 ? new EligibleCost(grantApplication, expenseType, cost.EstimatedCost, cost.EstimatedParticipants) : grantApplicationService.Get<EligibleCost>(cost.Id);
				eligibleCost.EligibleExpenseTypeId = cost.EligibleExpenseType.Id;
				eligibleCost.EligibleExpenseType = expenseType;

				switch (cost.EligibleExpenseType.ExpenseTypeId)
				{
					case (ExpenseTypes.ParticipantLimited):
					case (ExpenseTypes.NotParticipantLimited):
					case (ExpenseTypes.AutoLimitEstimatedCosts):
						eligibleCost.EstimatedParticipants = EstimatedParticipants.Value;
						break;
					default:
						eligibleCost.EstimatedParticipants = cost.EstimatedParticipants;
						break;
				}
				eligibleCost.EstimatedParticipantCost = cost.EstimatedParticipantCost;
				eligibleCost.EstimatedCost = cost.EstimatedCost;
				eligibleCost.ExpenseExplanation = eligibleCost.EligibleExpenseType.RequireExplanation() ? cost.ExpenseExplanation : null;

				foreach (var breakdown in cost.Breakdowns)
				{
					var breakdownEntity = grantApplicationService.Get<EligibleCostBreakdown>(breakdown.Id);
					breakdownEntity.EstimatedCost = breakdown.EstimatedCost;
				}

				eligibleCost.EligibleExpenseTypeId = cost.EligibleExpenseType.Id;

				// Remove breakdowns.
				foreach (var breakdown in eligibleCost.Breakdowns.Where(b => removeEligibleCostBreakdownIds.Contains(b.Id)).ToArray())
				{
					eligibleCost.Breakdowns.Remove(breakdown);
				}

				eligibleCost.RecalculateEstimatedCost();
				eligibleCost.RecalculateAgreedCosts();

				if (cost.Id == 0)
				{
					trainingCost.EligibleCosts.Add(eligibleCost);
				}
			}

			CreateParticipantInvitations(trainingCost);

			UpdateAttachments(trainingCost, attachmentService, files);

			trainingCost.RecalculateEstimatedCosts();
			trainingCost.RecalculateAgreedCosts();

			grantApplicationService.UpdateTrainingCosts(grantApplication);
			return grantApplication;
		}

		private void CreateParticipantInvitations(TrainingCost trainingCost)
		{
			if (!trainingCost.GrantApplication.UsePIFInvitations)
				return;

			var invitations = trainingCost.GrantApplication.ParticipantInvitations.ToList();

			var currentInvites = invitations.Count;
			var currentTakenInvites = invitations.Count(i => i.ParticipantInvitationStatus != ParticipantInvitationStatus.Empty);
			var expectedInvitations = trainingCost.EstimatedParticipants;

			// If we have the required amount of invitations
			if (expectedInvitations == currentInvites)
				return;

			if (currentInvites < expectedInvitations)
			{
				var createHowMany = expectedInvitations - currentInvites;
				var invites = CreateParticipantInvitations(trainingCost, createHowMany);

				foreach (var invite in invites)
					trainingCost.GrantApplication.ParticipantInvitations.Add(invite);
			}

			if (expectedInvitations < currentInvites)
			{
				var invitesToRemove = invitations.Where(i => i.ParticipantInvitationStatus == ParticipantInvitationStatus.Empty);

				foreach (var invite in invitesToRemove)
					trainingCost.GrantApplication.ParticipantInvitations.Remove(invite);
			}
		}

		private static List<ParticipantInvitation> CreateParticipantInvitations(TrainingCost trainingCost, int expectedInvitations)
		{
			var invites = new List<ParticipantInvitation>();

			for (var i = 0; i < expectedInvitations; i++)
			{
				invites.Add(new ParticipantInvitation
				{
					IndividualKey = Guid.NewGuid(),
					GrantApplicationId = trainingCost.GrantApplicationId,
					ExpectedParticipantOutcome = 0, // Set no enum
					ParticipantInvitationStatus = ParticipantInvitationStatus.Empty,
				});
			}

			return invites;
		}

		/// <summary>
		/// Add/update/remove attachments associated with the specific properties of the training provider.
		/// </summary>
		/// <param name="trainingCost"></param>
		/// <param name="attachmentService"></param>
		/// <param name="files"></param>
		private void UpdateAttachments(TrainingCost trainingCost, IAttachmentService attachmentService, HttpPostedFileBase[] files = null)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost));

			if (attachmentService == null)
				throw new ArgumentNullException(nameof(attachmentService));

			// If we have no travel expenses (ie: they've been deleted), remove the existing Travel Document
			var haveAnyTravelExpenses = HaveAnyTravelExpenses();
			if (!haveAnyTravelExpenses && trainingCost.TravelExpenseDocument != null)
				RemoveTravelDocument(trainingCost, attachmentService);

			// If files were provided add/update the attachments for the specified properties.
			if (files == null || !files.Any())
				return;

			if (TravelExpenseDocument?.Index != null && files.Count() > TravelExpenseDocument.Index)
			{
				var attachment = files[TravelExpenseDocument.Index.Value].UploadFile(TravelExpenseDocument.Description, TravelExpenseDocument.FileName, permittedFileTypesKey: "TravelExpensePermittedAttachmentTypes");
				attachment.Id = TravelExpenseDocument.Id;

				if (TravelExpenseDocument.Id == 0)
				{
					trainingCost.TravelExpenseDocument = attachment;
					attachmentService.Add(attachment);
					TravelExpenseDocument.Id = attachment.Id;
				}
				else
				{
					attachment.RowVersion = Convert.FromBase64String(TravelExpenseDocument.RowVersion);
					attachmentService.Update(attachment);
				}
			}

			if (trainingCost.TravelExpenseDocumentId.HasValue && trainingCost.TravelExpenseDocumentId != TravelExpenseDocument?.Id)
				RemoveTravelDocument(trainingCost, attachmentService);

			trainingCost.TravelExpenseDocumentId = trainingCost.TravelExpenseDocument?.Id;
		}

		private bool HaveAnyTravelExpenses()
		{
			return EligibleCosts.Any(ec => ec.EligibleExpenseType.Caption.StartsWith("Travel -"));
		}

		private static void RemoveTravelDocument(TrainingCost trainingCost, IAttachmentService attachmentService)
		{
			// Remove the prior attachment because it has been replaced.
			var attachment = attachmentService.Get(trainingCost.TravelExpenseDocumentId.Value);
			trainingCost.TravelExpenseDocumentId = null;
			attachmentService.Remove(attachment);
		}
	}
}