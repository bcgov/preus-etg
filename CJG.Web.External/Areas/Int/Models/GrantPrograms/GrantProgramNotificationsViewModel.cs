using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class GrantProgramNotificationsViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public IEnumerable<GrantProgramNotificationViewModel> Notifications { get; set; }
		#endregion

		#region Constructors
		public GrantProgramNotificationsViewModel() { }

		public GrantProgramNotificationsViewModel(GrantProgram grantProgram)
		{
			if (grantProgram == null) throw new ArgumentNullException(nameof(grantProgram));

			this.Id = grantProgram.Id;
			this.RowVersion = Convert.ToBase64String(grantProgram.RowVersion);
			this.Notifications = grantProgram.GrantProgramNotificationTypes.OrderBy(n => n.NotificationType.RowSequence).ThenBy(n => n.NotificationType.Caption).Select(n => new GrantProgramNotificationViewModel(n));
		}
		#endregion
	}
}