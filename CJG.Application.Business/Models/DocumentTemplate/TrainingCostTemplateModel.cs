using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class TrainingCostTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public string CourseTitle { get; set; }
		public int AgreedParticipants { get; set; }
		public int EstimatedParticipants { get; set; }
		public decimal TotalAgreedMaxCost { get; set; }
		public decimal AgreedEmployerContribution { get; set; }
		public decimal AgreedMaxReimbursement { get; set; }

		public decimal SumESSAverageCost { get; set; }
		public IEnumerable<EligibleCostTemplateModel> EligibleCosts { get; set; }
		public IEnumerable<EligibleCostTemplateModel> EmploymentServicesAndSupports { get; set; }
		#endregion

		#region Constructors
		public TrainingCostTemplateModel()
		{
		}

		public TrainingCostTemplateModel(TrainingCost trainingCost, bool showAgreedCosts)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost));

			this.Id = trainingCost.GrantApplicationId;
			this.AgreedParticipants = trainingCost.AgreedParticipants;
			this.EstimatedParticipants = trainingCost.EstimatedParticipants;
			this.TotalAgreedMaxCost = trainingCost.TotalAgreedMaxCost;
			this.AgreedEmployerContribution = trainingCost.CalculateAgreedEmployerContribution();
			this.AgreedMaxReimbursement = trainingCost.CalculateAgreedMaxReimbursement();

			var eligibleCosts = trainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0).OrderBy(ec=> ec.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption);

			// CJG-876: Include Zero dollar service lines
			if (trainingCost.GrantApplication.GetProgramType() == ProgramTypes.WDAService) {
				eligibleCosts = trainingCost.EligibleCosts.OrderBy(ec => ec.EligibleExpenseType.RowSequence).ThenBy(ec => ec.EligibleExpenseType.Caption);
			}
			var employmentServicesAndSupports = eligibleCosts.Where(ec => ec.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports);
			this.EligibleCosts = eligibleCosts.Select(o => new EligibleCostTemplateModel(o)).ToArray();
			this.EmploymentServicesAndSupports = employmentServicesAndSupports.Select(o => new EligibleCostTemplateModel(o)).ToArray();
			this.SumESSAverageCost = employmentServicesAndSupports.Sum(x => showAgreedCosts ? x.AgreedMaxParticipantCost : x.EstimatedParticipantCost);
		}
		#endregion
	}
}
