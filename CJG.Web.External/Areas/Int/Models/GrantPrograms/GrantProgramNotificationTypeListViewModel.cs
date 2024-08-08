using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
    public class GrantProgramNotificationTypeListViewModel : BaseViewModel
	{
		public IEnumerable<GrantProgramNotificationTypeViewModel> GrantProgramNotificationTypes { get; set; }
		public IEnumerable<KeyValuePair<string, string>> VariableKeywords { get; set; }

		public GrantProgramNotificationTypeListViewModel() { }

		public GrantProgramNotificationTypeListViewModel(IEnumerable<GrantProgramNotificationType> grantProgramNotificationTypes, int grantProgramId)
		{
			if (grantProgramNotificationTypes == null)
				throw new ArgumentNullException("Grant program notification types cannot be null.");

			Id = grantProgramId;
			GrantProgramNotificationTypes = grantProgramNotificationTypes
				.Select(nt => new GrantProgramNotificationTypeViewModel(nt))
				.ToArray();

			var excludedProperties = new[] { "GrantApplication", "Applicant", "GrantApplicationId" };
			VariableKeywords = typeof(NotificationViewModel).GetPropertiesAsKeyValuePairs(excludedProperties);
		}
	}
}