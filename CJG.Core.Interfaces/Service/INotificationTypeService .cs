using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface INotificationTypeService : IService
	{
		IEnumerable<NotificationType> Get(bool? isActive);
		NotificationType Get(int id);
		PageList<NotificationType> Get(int page, int quantity, NotificationTypeFilter filter);
		IEnumerable<NotificationTrigger> GetTriggerTypes(bool? isActive = true);
		void Add(NotificationType notificationType);
		void Update(NotificationType notificationType);
		void Delete(NotificationType notificationType);
	}
}
