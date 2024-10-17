PRINT 'UPDATE [NotificationTypes] - Disable some notifications'

update [dbo].[NotificationTypes]
set [IsActive] = 0
where [NotificationTemplateId] in (5, 6)
