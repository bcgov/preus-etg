using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Models.Shared.Reports
{
	public class CompletionReportBaseViewModel : BaseViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public string ProgramName { get; set; }
		public IEnumerable<CompletionReportParticipantDetailsViewModel> Participants { get; set; }
		public IEnumerable<CompletionReportParticipantDetailsViewModel> FilteredParticipants { get; set; }
		public IEnumerable<CompletionReportGroupDetailsViewModel> CompletionReportGroups { get; set; }
		#endregion

		#region Constructors
		public CompletionReportBaseViewModel()
		{
		}

		public CompletionReportBaseViewModel(GrantApplication grantApplication, ICompletionReportService completionReportService)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));
			if (completionReportService == null) throw new ArgumentNullException(nameof(completionReportService));

			this.GrantApplicationId = grantApplication.Id;
			this.ProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;

			var completionReportGroups = completionReportService.GetCompletionReportGroups(grantApplication.Id).ToArray();
			var participants = grantApplication.ParticipantForms.OrderBy(o => o.LastName).ToArray();
			var filteredParticipants = new List<ParticipantForm>();
			var list = new List<CompletionReportGroupDetailsViewModel>();

			foreach (var group in completionReportGroups)
			{
				CompletionReportGroupDetailsViewModel item;

				// Each page displays different classes of questions.
				// Page 1 of both reports originally select, and here, shows all participants that did not complete the course.
				switch (group.Id)
				{
					case Core.Entities.Constants.CompletionReportETGPage1:
					case Core.Entities.Constants.CompletionReportETGPage5:
					case Core.Entities.Constants.CompletionReportCWRGPage1:
						item = new CompletionReportGroupDetailsViewModel(grantApplication, group, participants);
						break;
					default:
						item = new CompletionReportGroupDetailsViewModel(grantApplication, group, filteredParticipants);
						break;
				}

				// While iterating through page 1, create the filtered participants-- the ones that completed the course.
				if (group.Id == Core.Entities.Constants.CompletionReportETGPage1 ||
					group.Id == Core.Entities.Constants.CompletionReportCWRGPage1)
				{
					var question = item.Questions.First();
					var answer = question.Level1Answers.First();
					foreach (var participant in participants)
						if (answer.BoolAnswer || question.Level2Answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id)?.BoolAnswer != true)
							filteredParticipants.Add(participant);
				}

				list.Add(item);
			}

			this.Participants = participants.Select(o => new CompletionReportParticipantDetailsViewModel(o)).ToArray();
			this.FilteredParticipants = filteredParticipants.Select(o => new CompletionReportParticipantDetailsViewModel(o)).ToArray();
			this.CompletionReportGroups = list.ToArray();
		}
		#endregion
	}
}
