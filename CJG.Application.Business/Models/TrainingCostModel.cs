using CJG.Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CJG.Application.Business.Models
{
	public class TrainingCostModel
	{
		#region Properties
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

		public bool AllExpenseTypeAllowMultiple
		{
			get
			{
				return EligibleCosts.All(t => t.EligibleExpenseType.AllowMultiple && t.EligibleExpenseType.IsActive);
			}
		}

		public bool ShouldDisplayEmployerContribution { get; set; }
		public bool ShouldDisplayESSSummary { get; set; }
		public string UserGuidanceCostEstimates { get; set; }
		#endregion

		#region Constructors
		public TrainingCostModel()
		{

		}

		public TrainingCostModel(GrantApplication grantApplication, IEnumerable<EligibleExpenseTypeModel> AllEligibleExpenseTypes, IEnumerable<EligibleExpenseTypeModel> AutoIncludedEligibleExpenseTypes)
		{
			this.GrantApplicationId = grantApplication.Id;
			this.GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			this.ReimbursementRate = grantApplication.ReimbursementRate;
			this.AgreedParticipants = grantApplication.TrainingCost.AgreedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.AgreedParticipants;
			this.EstimatedParticipants = grantApplication.TrainingCost.EstimatedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.EstimatedParticipants;

			var eligibleCosts = !grantApplication.HasOfferBeenIssued() ? grantApplication.TrainingCost.EligibleCosts.Where(ec => !ec.AddedByAssessor).Select(ec => new EligibleCostModel(ec)).ToList() : grantApplication.TrainingCost.EligibleCosts.Select(ec => new EligibleCostModel(ec)).ToList();

			if (eligibleCosts.Count() != AutoIncludedEligibleExpenseTypes.Count())
			{
				eligibleCosts.AddRange(AutoIncludedEligibleExpenseTypes.Where(t => !eligibleCosts.Select(e => e.EligibleExpenseType.Id).Contains(t.Id))
					.Select(eet => new EligibleCostModel(eet) { EstimatedParticipants = this.EstimatedParticipants ?? 0 }).ToArray());
			}

			this.EligibleCosts = eligibleCosts.OrderBy(t => t.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption).ToArray();
			this.TotalEmployer = this.EligibleCosts.Sum(x => x.EstimatedEmployerContribution);
			this.TotalEstimatedCost = this.EligibleCosts.Sum(x => x.EstimatedCost);
			this.TotalRequest = this.EligibleCosts.Sum(x => x.EstimatedReimbursement);
			this.ShouldDisplayEmployerContribution = grantApplication.ReimbursementRate != 1;
			this.ShouldDisplayESSSummary = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			this.UserGuidanceCostEstimates =
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration?.GrantPrograms.Count == 0 ?
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceCostEstimates
				: grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramConfiguration.UserGuidanceCostEstimates;
		}

		public TrainingCostModel(GrantApplication grantApplication)
		{
			this.GrantApplicationId = grantApplication.Id;
			this.GrantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId;
			this.ProgramType = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId;
			this.MaxReimbursementAmt = grantApplication.MaxReimbursementAmt;
			this.ReimbursementRate = grantApplication.ReimbursementRate;
			this.AgreedParticipants = grantApplication.TrainingCost.AgreedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.AgreedParticipants;
			this.EstimatedParticipants = grantApplication.TrainingCost.EstimatedParticipants == 0 ? null : (int?)grantApplication.TrainingCost.EstimatedParticipants;
			this.UserGuidanceCostEstimates =
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration?.GrantPrograms.Count == 0 ?
				grantApplication.GrantOpening.GrantStream.ProgramConfiguration.UserGuidanceCostEstimates
				: grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramConfiguration.UserGuidanceCostEstimates;

			this.EligibleCosts = grantApplication.TrainingCost.EligibleCosts
				.Select(ec => new EligibleCostModel(ec))
				.OrderBy(t => t.EligibleExpenseType.RowSequence)
				.ThenBy(ec => ec.EligibleExpenseType.Caption).ToArray();

			this.TotalAgreedCost = this.EligibleCosts.Sum(x => x.AgreedCost);
			this.TotalEstimatedCost = this.EligibleCosts.Sum(x => x.EstimatedCost);

			if (grantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
			{
				this.TotalAgreedReimbursement = grantApplication.TrainingCost.AgreedCommitment;
				this.TotalAgreedEmployer = this.TotalAgreedCost - this.TotalAgreedReimbursement;

				this.TotalRequest = grantApplication.TrainingCost.TotalEstimatedReimbursement;
				this.TotalEmployer = this.TotalEstimatedCost - this.TotalRequest;
			}
			else {
				this.TotalAgreedEmployer = this.EligibleCosts.Sum(x => x.AgreedEmployerContribution);
				this.TotalAgreedReimbursement = this.EligibleCosts.Sum(x => x.AgreedMaxReimbursement);
				this.TotalEmployer = this.EligibleCosts.Sum(x => x.EstimatedEmployerContribution);
				this.TotalRequest = this.EligibleCosts.Sum(x => x.EstimatedReimbursement);
			}
			this.ShouldDisplayEmployerContribution = grantApplication.ReimbursementRate != 1;
			this.ShouldDisplayESSSummary = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;

			this.NewEligibleCost.AddedByAssessor = true;

			this.ESSAgreedAverage = this.EligibleCosts.Where(x => x.ServiceType == ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.AgreedMaxParticipantCost);
			this.ESSEstimatedAverage = this.EligibleCosts.Where(x => x.ServiceType == ServiceTypes.EmploymentServicesAndSupports).Sum(x => x.EstimatedParticipantCost);
		}
		#endregion
	}
}
