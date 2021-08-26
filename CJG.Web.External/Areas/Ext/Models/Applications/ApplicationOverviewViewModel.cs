using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationOverviewViewModel : BaseApplicationViewModel
	{
		#region Properties
		public DateTime? DateSubmitted { get; set; }
		public DateTime? CheckPrivateSectorsOn { get; set; }
		public bool CanSubmit { get; set; }
		public bool OrganizationCreated { get; set; }
		#endregion

		#region Constructors
		public ApplicationOverviewViewModel()
		{

		}
		public ApplicationOverviewViewModel(GrantApplication grantApplication, ISettingService settingService) : base(grantApplication)
		{
			this.DateSubmitted = grantApplication.DateSubmitted.HasValue ? grantApplication.DateSubmitted.Value.ToLocalTime() : (DateTime?)null;

			// Determine when training providers require additional documentation.
			this.CheckPrivateSectorsOn = ((DateTime?)settingService.Get("CheckPrivateSectorsOn")?.GetValue())?.ToLocalTime().Date;
			this.OrganizationCreated = grantApplication.Organization != null;
			this.CanSubmit = this.ApplicationStateExternal == ApplicationStateExternal.Complete && (this.GrantOpeningState == GrantOpeningStates.Open || this.GrantOpeningState == GrantOpeningStates.OpenForSubmit) && this.OrganizationCreated;
		}
		#endregion
	}
}
