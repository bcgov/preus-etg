using System;
using System.Collections.Generic;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class OverviewTrainingCost
	{
		public int GrantApplicationId { get; set; }
		public TrainingCostStates TrainingCostState { get; set; }
		public int EstimatedParticipants { get; set; }
		public decimal TotalEstimatedCost { get; set; }
		public decimal TotalEstimatedReimbursement { get; set; }
		public int AgreedParticipants { get; set; }
		public decimal TotalAgreedMaxCost { get; set; }
		public decimal AgreedCommitment { get; set; }
		public decimal TotalRequest { get; set; }
		public decimal TotalEmployer { get; set; }
		public decimal TotalCost { get; set; }
		public decimal ESSAveragePerParticipant { get; set; }
		public bool ShowEmployer { get; set; }
		public bool HasOfferBeenIssued { get; set; }
		public Attachment TravelExpenseDocument { get; set; }


		public IEnumerable<EstimatedCostViewModel> EstimatedCosts { get; set; }

		public OverviewTrainingCost()
		{ 
		}

		public OverviewTrainingCost(TrainingCost trainingCost)
		{
			if (trainingCost == null)
				throw new ArgumentNullException(nameof(trainingCost));

			Utilities.MapProperties(trainingCost, this);

			TravelExpenseDocument = trainingCost.TravelExpenseDocument;
			HasOfferBeenIssued = trainingCost?.GrantApplication?.HasOfferBeenIssued() ?? false;
			ShowEmployer = trainingCost.GrantApplication.ReimbursementRate != 1;
		}
	}
}