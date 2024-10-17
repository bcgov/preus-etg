using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
    public class GrantStreamEligibilityViewModel : BaseViewModel
	{
		public string Name { get; set; }
		public bool EligibilityEnabled { get; set; }
		public string EligibilityRequirements { get; set; }
		public string EligibilityQuestion { get; set; }
		public bool EligibilityRequired { get; set; }
		public IEnumerable<GrantStreamQuestionViewModel> StreamEligibilityQuestions { get; set; }

		public GrantStreamEligibilityViewModel() { }

		public GrantStreamEligibilityViewModel(GrantStream grantStream, IGrantStreamService grantStreamService)
		{
			if (grantStream == null)
				throw new ArgumentNullException(nameof(grantStream));

			Name = grantStream.Name;
			EligibilityEnabled = grantStream.EligibilityEnabled;
			EligibilityRequirements = grantStream.EligibilityRequirements;
			EligibilityQuestion = grantStream.EligibilityQuestion;
			EligibilityRequired = grantStream.EligibilityRequired;
			StreamEligibilityQuestions = grantStreamService.GetGrantStreamQuestions(grantStream.Id)
				.Where(l=> l.IsActive)
				.Select(n => new GrantStreamQuestionViewModel(n)).ToList();
		}
	}
}