using System;
using System.Linq;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;

namespace CJG.Web.External.Areas.Ext.Models.Applications
{
	public class ApplicationOverviewViewModel : BaseApplicationViewModel
	{
		public DateTime? DateSubmitted { get; set; }
		public DateTime? CheckPrivateSectorsOn { get; set; }
		public int SelectedNewUser { get; set; }
		public bool CanSubmit { get; set; }
		public bool OrganizationCreated { get; set; }
		public int MaxParticipantsAllowed { get; set; }
		public bool AllOutcomesAreReported { get; set; }

		public ApplicationOverviewViewModel()
		{

		}

		public ApplicationOverviewViewModel(GrantApplication grantApplication, ISettingService settingService) : base(grantApplication)
		{
			DateSubmitted = grantApplication.DateSubmitted.HasValue ? grantApplication.DateSubmitted.Value.ToLocalTime() : (DateTime?)null;

			// Determine when training providers require additional documentation.
			CheckPrivateSectorsOn = ((DateTime?)settingService.Get("CheckPrivateSectorsOn")?.GetValue())?.ToLocalTime().Date;
			OrganizationCreated = grantApplication.Organization != null;

			CanSubmit = ApplicationStateExternal == ApplicationStateExternal.Complete && (GrantOpeningState == GrantOpeningStates.Open || GrantOpeningState == GrantOpeningStates.OpenForSubmit) && OrganizationCreated;
			CanReportParticipants = grantApplication.CanReportParticipants;

			MaxParticipantsAllowed = grantApplication.GetMaxParticipants();

			//a value of zero indicates that the user has not selected an outcome from the list
			AllOutcomesAreReported = !grantApplication.ParticipantForms.Any(p => p.ExpectedParticipantOutcome == 0);
		}
	}
}