using CJG.Application.Services;
using CJG.Application.Services.Notifications;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.ProgramNotifications
{
	public class ProgramNotificationViewModel : BaseViewModel
	{
		#region Properties
		public int NotificationTemplateId { get; set; }
		public DateTime? SendDate { get; set; }
		public bool AllApplicants { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		public IEnumerable<ProgramNotificationRecipientViewModel> Recipients { get; set; }
		public ProgramNotificationTemplateViewModel Template { get; set; } = new ProgramNotificationTemplateViewModel();
		#endregion

		#region Constructors
		public ProgramNotificationViewModel() { }

		public ProgramNotificationViewModel(ProgramNotification notification)
		{
			if (notification == null) throw new ArgumentNullException(nameof(notification));

			Utilities.MapProperties(notification, this);
			this.SendDate = notification.SendDate?.ToLocalMorning();
			this.Recipients = notification.ProgramNotificationRecipients.Select(o => new ProgramNotificationRecipientViewModel(o)).ToArray();
			this.Template = new ProgramNotificationTemplateViewModel(notification.NotificationTemplate);
		}
		#endregion

		#region Methods
		public void SetProgramNotification(ProgramNotification model, IProgramNotificationService _programNotificationService = null)
		{
			if (model == null) throw new ArgumentNullException(nameof(model));

			this.Template.Caption = this.Template.Caption ?? this.Caption;
			Utilities.MapProperties(this, model);
			Utilities.MapProperties(this.Template, model.NotificationTemplate);
			model.SendDate = this.SendDate.HasValue ? this.SendDate.Value.ToUtcMorning() : this.SendDate;

			if (model.ProgramNotificationRecipients == null)
			{
				if (!this.AllApplicants)
				{
					model.ProgramNotificationRecipients = this.Recipients.Select(o =>
					{
						var recipient = new ProgramNotificationRecipient();
						Utilities.MapProperties(o, recipient);
						return recipient;
					}).ToArray();
				}
			}
			else
			{
				var currentRecipients = model.ProgramNotificationRecipients.ToArray();

				if (this.AllApplicants)
				{
					foreach (var recipient in currentRecipients)
						_programNotificationService.DeleteRecipient(recipient);
					model.ProgramNotificationRecipients.Clear();
				}
				else
				{
					foreach (var recipient in currentRecipients)
					{
						if (!this.Recipients.Any(o => o.ProgramNotificationId == recipient.ProgramNotificationId && o.GrantProgramId == recipient.GrantProgramId))
						{
							model.ProgramNotificationRecipients.Remove(recipient);
							_programNotificationService.DeleteRecipient(recipient);
						}
					}

					foreach (var recipient in this.Recipients)
					{
						var currentRecipient = model.ProgramNotificationRecipients.FirstOrDefault(o => o.ProgramNotificationId == recipient.ProgramNotificationId && o.GrantProgramId == recipient.GrantProgramId);
						if (currentRecipient == null)
						{
							var newRecipient = new ProgramNotificationRecipient();
							Utilities.MapProperties(recipient, newRecipient);
							model.ProgramNotificationRecipients.Add(newRecipient);
						}
						else
						{
							Utilities.MapProperties(recipient, currentRecipient);
						}
					}
				}
			}
		}
		#endregion
	}
}
