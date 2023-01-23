using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantStreams
{
	public class GrantStreamQuestionViewModel : BaseViewModel
	{
		public int GrantStreamId { get; set; }
		public string EligibilityRequirements { get; set; }

		[Required(ErrorMessage = "Eligibility question is required."), Display(Name = "Eligibility Question")]
		public string EligibilityQuestion { get; set; }

		public bool IsActive { get; set; }

		public bool EligibilityPositiveAnswerRequired { get; set; }

		public bool EligibilityRationaleAnswerAllowed { get; set; }
		public string EligibilityRationaleAnswerLabel { get; set; }
		public int EligibilityPositiveAnswerPriorityScore { get; set; }

		// Answer- returned when asking user when creating Grant Application
		public bool? EligibilityAnswer { get; set; }

		// Rationale - returned when asking user when creating Grant Application
		[AllowHtml]
		public string RationaleAnswer { get; set; }

		public int RowSequence { get; set; }
		public string RowVersion { get; set; }

		public GrantStreamQuestionViewModel() { }

		public GrantStreamQuestionViewModel(GrantStreamEligibilityQuestion grantQuestion)
		{
			if (grantQuestion == null)
				throw new ArgumentNullException(nameof(grantQuestion));

			Utilities.MapProperties(grantQuestion, this);

			RowVersion = Convert.ToBase64String(grantQuestion.RowVersion);
		}
	}
}
