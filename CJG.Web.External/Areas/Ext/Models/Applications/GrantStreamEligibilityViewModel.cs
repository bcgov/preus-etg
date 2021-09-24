using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class GrantStreamEligibilityViewModel : BaseViewModel
	{
		#region Properties
		public string Name { get; set; }
		public bool EligibilityEnabled { get; set; }
		public string EligibilityRequirements { get; set; }
		public string EligibilityQuestion { get; set; }
		public bool EligibilityRequired { get; set; }
		public IEnumerable<GrantStreamQuestionViewModel> StreamEligibilityQuestions { get; set; }
		#endregion

		#region Constructors
		public GrantStreamEligibilityViewModel() { }

		public GrantStreamEligibilityViewModel(GrantStream grantStream, IGrantStreamService grantStreamService)
		{
			if (grantStream == null) throw new ArgumentNullException(nameof(grantStream));

			this.Name = grantStream.Name;
			this.EligibilityEnabled = grantStream.EligibilityEnabled;
			this.EligibilityRequirements = grantStream.EligibilityRequirements;
			this.EligibilityQuestion = grantStream.EligibilityQuestion;
			this.EligibilityRequired = grantStream.EligibilityRequired;
			StreamEligibilityQuestions = grantStreamService.GetGrantStreamQuestions(grantStream.Id)
				.Where(l=> l.IsActive == true)
				.Select(n => new GrantStreamQuestionViewModel(n)).ToList();
		}
		#endregion
	}
}