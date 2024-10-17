using CJG.Web.External.Areas.Int.Models.GrantStreams;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class StreamEligibilityViewModel
    {
        public string Name { get; set; }
        public bool EligibilityEnabled { get; set; }
        public string EligibilityRequirements { get; set; }
        public string EligibilityQuestion { get; set; }
        public bool  EligibilityRequired{ get; set; }
		public IEnumerable<GrantStreamQuestionViewModel> StreamEligibilityQuestions { get; set; }
	}
}
