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
	public class CompletionReportAnswerViewModel : BaseViewModel
	{
		#region Properties
		public int QuestionId { get; set; }
		public int Level { get; set; }
		public int GrantApplicationId { get; set; }
		public int? ParticipantFormId { get; set; }
		public int? EligibleCostBreakdownId { get; set; }
		public int IntAnswer { get; set; }
		public int? CommunityId { get; set; }
		public bool BoolAnswer { get; set; }
		public string StringAnswer { get; set; }

		public int? Naics1Id { get; set; }
		public int? Naics2Id { get; set; }
		public int? Naics3Id { get; set; }
		public int? Naics4Id { get; set; }
		public int? Naics5Id { get; set; }

		public int? Noc1Id { get; set; }
		public int? Noc2Id { get; set; }
		public int? Noc3Id { get; set; }
		public int? Noc4Id { get; set; }

		public string EmployerName { get; set; }
		#endregion

		#region Constructors
		public CompletionReportAnswerViewModel()
		{
		}

		public CompletionReportAnswerViewModel(CompletionReportQuestion question, int level)
		{
			if (question == null) throw new ArgumentNullException(nameof(question));

			this.QuestionId = question.Id;
			this.Level = level;
		}

		public CompletionReportAnswerViewModel(CompletionReportQuestion question, GrantApplication grantApplication, int level) : this(question, level)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.GrantApplicationId = grantApplication.Id;
		}

		public CompletionReportAnswerViewModel(CompletionReportQuestion question, GrantApplication grantApplication, ParticipantForm participant, int level) : this(question, grantApplication, level)
		{
			if (participant == null) throw new ArgumentNullException(nameof(participant));

			this.ParticipantFormId = participant.Id;
		}

		public CompletionReportAnswerViewModel(CompletionReportQuestion question, GrantApplication grantApplication, ParticipantForm participant, ParticipantCompletionReportAnswer answer, int level) : this(question, grantApplication, participant, level)
		{
			this.IntAnswer = answer?.AnswerId ?? 0;
			this.StringAnswer = answer?.OtherAnswer;
			this.CommunityId = answer?.CommunityId;
		}

		public CompletionReportAnswerViewModel(CompletionReportQuestion question, GrantApplication grantApplication, EmployerCompletionReportAnswer answer, int level) : this(question, grantApplication, level)
		{
			this.IntAnswer = answer?.AnswerId ?? 0;
			this.StringAnswer = answer?.OtherAnswer;
		}
		#endregion
	}
}
