using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface INotificationService : IService
	{
		NotificationQueue GetApplicationNotification(int id);

		PageList<NotificationQueue> GetNotifications(int page, int quantity, NotificationFilter filter);
		PageList<NotificationQueue> GetGrantProgramNotifications(int grantProgramId, int page, int quantity, NotificationFilter filter);
		PageList<NotificationQueue> GetGrantApplicationNotifications(int grantApplicationId, int page, int quantity, string search, NotificationFilter filter);

		IEnumerable<Organization> GetGrantProgramNotificationOrganizations(int grantProgramId);
		IEnumerable<NotificationType> GetGrantProgramNotificationNotificationTypes(int grantProgramId);
		NotificationType GetPIFInvitationNotificationType();

		bool SendNotification(NotificationQueue notification, bool test = false);

		IEnumerable<NotificationQueue> SendNotifications(GrantApplication grantApplication, ApplicationStateInternal originalState, DateTime appDate = default);

		int QueueScheduledNotifications(DateTime appDate = default);

		IEnumerable<NotificationQueue> SendScheduledNotifications(DateTime appDate = default);

		NotificationQueue GenerateNotificationMessage(GrantApplication grantApplication, User user, NotificationType notificationType);
		NotificationQueue GenerateNotificationMessage(GrantApplication grantApplication, User user, GrantProgramNotificationType notificationType);
		
		void HandleWorkflowNotification(GrantApplication grantApplication, NotificationType notificationType);
		void HandleWorkflowNotification(GrantApplication grantApplication, GrantProgramNotificationType notificationType);

		bool CheckNotificationWorkflow(GrantProgramNotificationType grantProgramNotificationType, GrantApplication grantApplication, DateTime appDate = default);
		bool CheckNotificationWorkflow(GrantProgramNotificationType grantProgramNotificationType, GrantApplication grantApplication, ApplicationStateInternal? previousState = null, DateTime appDate = default);

		string ValidateModelKeywords(string text, string[] excludedKeywords = null);
	}
}
