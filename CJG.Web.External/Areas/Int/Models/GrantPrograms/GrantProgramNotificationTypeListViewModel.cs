using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System;
using CJG.Application.Services.Notifications;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationTypeListViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<GrantProgramNotificationTypeViewModel> GrantProgramNotificationTypes { get; set; }
		public IEnumerable<KeyValuePair<string, string>> VariableKeywords { get; set; }
		#endregion

		#region Constructors
		public GrantProgramNotificationTypeListViewModel() { }

		public GrantProgramNotificationTypeListViewModel(IEnumerable<GrantProgramNotificationType> grantProgramNotificationTypes, int grantProgramId)
		{
			if (grantProgramNotificationTypes == null) throw new ArgumentNullException("Grant program notification types cannot be null.");

			this.GrantProgramNotificationTypes = grantProgramNotificationTypes.Select(nt => new GrantProgramNotificationTypeViewModel(nt)).ToArray();
			this.Id = grantProgramId;
			var excludedProperties = new[] { "GrantApplication", "Applicant", "GrantApplicationId" };
			this.VariableKeywords = typeof(NotificationViewModel).GetPropertiesAsKeyValuePairs(excludedProperties);
		}
		#endregion
	}
}