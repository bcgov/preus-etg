using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class EligibleCostBreakdownTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public string EligibleExpenseBreakdownCaption { get; set; }
		#endregion

		#region Constructors
		public EligibleCostBreakdownTemplateModel()
		{
		}

		public EligibleCostBreakdownTemplateModel(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (eligibleCostBreakdown == null)
				throw new ArgumentNullException(nameof(eligibleCostBreakdown));

			this.Id = eligibleCostBreakdown.Id;
			this.EligibleExpenseBreakdownCaption = eligibleCostBreakdown.EligibleExpenseBreakdown.Caption;
		}
		#endregion
	}
}
