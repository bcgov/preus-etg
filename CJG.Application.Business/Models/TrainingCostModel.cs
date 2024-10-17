using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class TrainingCostModel
	{
		public int GrantApplicationId { get; set; }
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
		public EligibleCostModel NewEligibleCost { get; set; } = new EligibleCostModel();
		public IEnumerable<EligibleCostModel> EligibleCosts { get; set; } = new List<EligibleCostModel>();

		public decimal ESSAgreedAverage { get; set; }
		public decimal ESSEstimatedAverage { get; set; }

		public bool ShouldDisplayEmployerContribution { get; set; }
		public bool ShouldDisplayESSSummary { get; set; }
		public string UserGuidanceCostEstimates { get; set; }

		public bool AllExpenseTypeAllowMultiple
		{
			get
			{
				return EligibleCosts.All(t => t.EligibleExpenseType.AllowMultiple && t.EligibleExpenseType.IsActive);
			}
		}

		public TrainingCostModel()
		{
		}

		public TrainingCostModel(GrantApplication grantApplication, IEnumerable<EligibleExpenseTypeModel> AllEligibleExpenseTypes, IEnumerable<EligibleExpenseTypeModel> AutoIncludedEligibleExpenseTypes)
		{
			GrantApplicationId = grantApplication.Id;
			GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			ReimbursementRate = grantApplication.ReimbursementRate;
			AgreedParticipants = grantApplication.TrainingCost.AgreedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.AgreedParticipants;
			EstimatedParticipants = grantApplication.TrainingCost.EstimatedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.EstimatedParticipants;

			var eligibleCosts = !grantApplication.HasOfferBeenIssued() ? grantApplication.TrainingCost.EligibleCosts.Where(ec => !ec.AddedByAssessor).Select(ec => new EligibleCostModel(ec)).ToList() : grantApplication.TrainingCost.EligibleCosts.Select(ec => new EligibleCostModel(ec)).ToList();

			if (eligibleCosts.Count != AutoIncludedEligibleExpenseTypes.Count())
			{
				eligibleCosts.AddRange(AutoIncludedEligibleExpenseTypes.Where(t => !eligibleCosts.Select(e => e.EligibleExpenseType.Id).Contains(t.Id))
					.Select(eet => new EligibleCostModel(eet) { EstimatedParticipants = EstimatedParticipants ?? 0 }).ToArray());
			}

			EligibleCosts = eligibleCosts.OrderBy(t => t.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption).ToArray();
			TotalEmployer = EligibleCosts.Sum(x => x.EstimatedEmployerContribution);
			TotalEstimatedCost = EligibleCosts.Sum(x => x.EstimatedCost);
			TotalRequest = EligibleCosts.Sum(x => x.EstimatedReimbursement);
			ShouldDisplayEmployerContribution = grantApplication.ReimbursementRate != 1;
			ShouldDisplayESSSummary = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			UserGuidanceCostEstimates =
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration?.GrantPrograms.Count == 0 ?
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceCostEstimates
				: grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramConfiguration.UserGuidanceCostEstimates;
		}

		public TrainingCostModel(GrantApplication grantApplication)
		{
			GrantApplicationId = grantApplication.Id;
			GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			ReimbursementRate = grantApplication.ReimbursementRate;
			AgreedParticipants = grantApplication.TrainingCost.AgreedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.AgreedParticipants;
			EstimatedParticipants = grantApplication.TrainingCost.EstimatedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.EstimatedParticipants;
			UserGuidanceCostEstimates =
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration?.GrantPrograms.Count == 0 ?
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceCostEstimates
				: grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramConfiguration.UserGuidanceCostEstimates;

			EligibleCosts = grantApplication.TrainingCost.EligibleCosts
				.Select(ec => new EligibleCostModel(ec))
				.OrderBy(t => t.EligibleExpenseType.RowSequence)
				.ThenBy(ec => ec.EligibleExpenseType.Caption).ToArray();

			TotalAgreedCost = EligibleCosts.Sum(x => x.AgreedCost);
			TotalEstimatedCost = EligibleCosts.Sum(x => x.EstimatedCost);

			if (grantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				TotalAgreedReimbursement = grantApplication.TrainingCost.AgreedCommitment;
				TotalAgreedEmployer = TotalAgreedCost - TotalAgreedReimbursement;

				TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
				TotalEmployer = TotalEstimatedCost - TotalRequest;
			}
			else {
				TotalAgreedEmployer = EligibleCosts.Sum(x => x.AgreedEmployerContribution);
				TotalAgreedReimbursement = EligibleCosts.Sum(x => x.AgreedMaxReimbursement);
				TotalEmployer = EligibleCosts.Sum(x => x.EstimatedEmployerContribution);
				TotalRequest = EligibleCosts.Sum(x => x.EstimatedReimbursement);
			}

			ShouldDisplayEmployerContribution = grantApplication.ReimbursementRate != 1;
			ShouldDisplayESSSummary = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;

			NewEligibleCost.AddedByAssessor = true;

			ESSAgreedAverage = EligibleCosts.Where(x => x.ServiceType == ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.AgreedMaxParticipantCost);
			ESSEstimatedAverage = EligibleCosts.Where(x => x.ServiceType == ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
		}
	}
}
