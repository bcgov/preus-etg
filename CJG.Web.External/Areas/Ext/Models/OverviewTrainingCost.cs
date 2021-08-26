using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class OverviewTrainingCost
	{
		#region Properties
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

		public IEnumerable<EstimatedCostViewModel> EstimatedCosts { get; set; }
		#endregion

		#region Constructors
		public OverviewTrainingCost()
		{ 
		}

		public OverviewTrainingCost(TrainingCost trainingCost)
		{
			if (trainingCost == null) throw new ArgumentNullException(nameof(trainingCost));

			Utilities.MapProperties(trainingCost, this);
			this.HasOfferBeenIssued = trainingCost?.GrantApplication?.HasOfferBeenIssued() ?? false;
			this.ShowEmployer = trainingCost.GrantApplication.ReimbursementRate != 1;
		}
		#endregion  
	}
}