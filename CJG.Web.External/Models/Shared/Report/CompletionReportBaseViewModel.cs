using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Models.Shared.Reports
{
	public class CompletionReportBaseViewModel : BaseViewModel
	{
		public int GrantApplicationId { get; set; }
		public string ProgramName { get; set; }
		public IEnumerable<CompletionReportParticipantDetailsViewModel> Participants { get; set; }
		public IEnumerable<CompletionReportParticipantDetailsViewModel> FilteredParticipants { get; set; }
		public IEnumerable<CompletionReportGroupDetailsViewModel> CompletionReportGroups { get; set; }

		public CompletionReportBaseViewModel()
		{
		}

		public CompletionReportBaseViewModel(GrantApplication grantApplication, ICompletionReportService completionReportService)
		{
			if (grantApplication == null)
				throw new ArgumentNullException(nameof(grantApplication));

			if (completionReportService == null)
				throw new ArgumentNullException(nameof(completionReportService));

			GrantApplicationId = grantApplication.Id;
			ProgramName = grantApplication.GrantOpening.GrantStream.GrantProgram.Name;

			var completionReportGroups = completionReportService.GetCompletionReportGroups(grantApplication.Id).ToArray();
			var participants = grantApplication.ParticipantForms
				.Where(o => o.Approved == true)
				.OrderBy(o => o.LastName)
				.ToArray();

			var filteredParticipants = new List<ParticipantForm>();
			var list = new List<CompletionReportGroupDetailsViewModel>();

			foreach (var group in completionReportGroups)
			{
				CompletionReportGroupDetailsViewModel item;

				// Each page displays different classes of questions.
				// Page 1 of both reports originally select, and here, shows all participants that did not complete the course.
				switch (group.Id)
				{
					case Constants.CompletionReportETGPage1:
					case Constants.CompletionReportETGPage5:
					case Constants.CompletionReportCWRGPage1:
						item = new CompletionReportGroupDetailsViewModel(grantApplication, group, participants);
						break;

					default:
						item = new CompletionReportGroupDetailsViewModel(grantApplication, group, filteredParticipants);
						break;
				}

				// While iterating through page 1, create the filtered participants-- the ones that completed the course.
				if (group.Id == Constants.CompletionReportETGPage1 ||
					group.Id == Constants.CompletionReportCWRGPage1)
				{
					var question = item.Questions.First();
					var answer = question.Level1Answers.First();
					foreach (var participant in participants)
						if (answer.BoolAnswer || question.Level2Answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id)?.BoolAnswer != true)
							filteredParticipants.Add(participant);
				}

				list.Add(item);
			}

			Participants = participants
				.Select(o => new CompletionReportParticipantDetailsViewModel(o))
				.ToArray();

			FilteredParticipants = filteredParticipants
				.Select(o => new CompletionReportParticipantDetailsViewModel(o))
				.ToArray();

			CompletionReportGroups = list.ToArray();
		}
	}
}
