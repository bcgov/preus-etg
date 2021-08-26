using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	/// <summary>
	/// <typeparamref name="NotificationTypeService"/> class, provides a way to manage notifications types.
	/// </summary>
	public class NotificationTypeService : Service, INotificationTypeService
	{
		#region Variables
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="NotificationTypeService"/> class.
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public NotificationTypeService(
			IDataContext dbContext,
			HttpContextBase httpContext,
			ILogger logger) : base(dbContext, httpContext, logger)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns an array of notification types filtered by 'isActive' argument.
		/// </summary>
		/// <param name="isActive"></param>
		/// <returns></returns>
		public IEnumerable<NotificationType> Get(bool? isActive)
		{
			return _dbContext.NotificationTypes.Where(nt => nt.IsActive == (isActive ?? true)).ToArray();
		}

		/// <summary>
		/// Return the notification type for the specified 'id' or throw NoContentException if not found.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public NotificationType Get(int id)
		{
			return Get<NotificationType>(id);
		}

		/// <summary>
		/// Returns a page of filtered notification types.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public PageList<NotificationType> Get(int page, int quantity, NotificationTypeFilter filter)
		{
			if (page <= 0) page = 1;
			if (quantity <= 0 || quantity > 100) quantity = 10;

			var query = _dbContext.NotificationTypes.Where(nt => true);

			// Filter
			if (!String.IsNullOrWhiteSpace(filter.Caption)) query = query.Where(n => n.Caption.Contains(filter.Caption));
			if (filter.NotificationTriggerId > 0) query = query.Where(n => n.NotificationTriggerId == filter.NotificationTriggerId);

			var total = query.Count();

			// Order By
			var orderBy = !String.IsNullOrWhiteSpace(filter.OrderBy) ? filter.OrderBy : $"{nameof(NotificationType.Caption)} desc";
			query = query.OrderByProperty(orderBy).Skip((page - 1) * quantity).Take(quantity);

			return new PageList<NotificationType>(page, quantity, total, query.ToArray());
		}

		/// <summary>
		/// Returns an array of notification triggers filtered by 'isActive' argument.
		/// </summary>
		/// <param name="isActive"></param>
		/// <returns></returns>
		public IEnumerable<NotificationTrigger> GetTriggerTypes(bool? isActive)
		{
			return _dbContext.NotificationTriggers.Where(nt => nt.IsActive == (isActive ?? true)).ToArray();
		}

		/// <summary>
		/// Adds the notification type to the datasource.
		/// </summary>
		/// <param name="notificationType"></param>
		public void Add(NotificationType notificationType)
		{
			if (notificationType == null)
				throw new ArgumentNullException(nameof(notificationType));

			if (_dbContext.NotificationTypes.Any(n => n.Caption == notificationType.Caption))
				throw new InvalidOperationException("A notification type by the name of '" + notificationType.Caption + "' already exists.");

			notificationType.NotificationTrigger = _dbContext.NotificationTriggers.FirstOrDefault(t => t.Id == notificationType.NotificationTriggerId);
						
			_dbContext.NotificationTemplates.Add(notificationType.NotificationTemplate); // Save the default notification template
			_dbContext.NotificationTypes.Add(notificationType);

			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Updates the specified notification type in the datasource.
		/// </summary>
		/// <param name="notificationType"></param>
		public void Update(NotificationType notificationType)
		{
			if (notificationType == null)
				throw new ArgumentNullException(nameof(notificationType));

			_dbContext.Update(notificationType.NotificationTemplate); // Save the default notification template
			_dbContext.Update(notificationType);
			_dbContext.CommitTransaction();
		}

		/// <summary>
		/// Deletes the specified notification type from the datasource.
		/// </summary>
		/// <param name="notificationType"></param>
		public void Delete(NotificationType notificationType)
		{
			if (notificationType == null)
				throw new ArgumentNullException(nameof(notificationType));

			// Delete the default notification template if only being used by this notification type
			if (_dbContext.NotificationTypes.Select(n => n.NotificationTemplateId == notificationType.NotificationTemplateId).Count() == 1) 
				_dbContext.NotificationTemplates.Remove(notificationType.NotificationTemplate);
			
			// Delete any related grant program notification types
			var grantProgramNotificationTypes = _dbContext.GrantProgramNotificationTypes.Where(g => g.NotificationTemplate.Id == notificationType.NotificationTemplate.Id).ToArray();
			foreach (var item in grantProgramNotificationTypes) {
				_dbContext.GrantProgramNotificationTypes.Remove(item);
				var template = _dbContext.NotificationTemplates.FirstOrDefault(n => n.Id == item.NotificationTemplateId);
				if (template != null) _dbContext.NotificationTemplates.Remove(template);
			}
			_dbContext.NotificationTypes.Remove(notificationType); // Delete the notification type
			_dbContext.CommitTransaction();
		}
	}
	#endregion
}
