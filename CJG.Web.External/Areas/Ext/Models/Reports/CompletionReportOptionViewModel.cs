using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Areas.Ext.Models.Reports
{
	public class CompletionReportOptionViewModel : BaseViewModel
	{
		#region Properties
		public int QuestionId { get; set; }
		public string Answer { get; set; }
		public int Level { get; set; }
		public bool TriggersNextLevel { get; set; }
		public int NextQuestion { get; set; }
		public bool DisplayOther { get; set; }
		#endregion

		#region Constructors
		public CompletionReportOptionViewModel()
		{
		}

		public CompletionReportOptionViewModel(CompletionReportOption completionReportOption)
		{
			if (completionReportOption == null) throw new ArgumentNullException(nameof(completionReportOption));

			Utilities.MapProperties(completionReportOption, this);
		}
		#endregion
	}
}
