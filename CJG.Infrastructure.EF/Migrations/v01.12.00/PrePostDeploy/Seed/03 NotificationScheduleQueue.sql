PRINT 'Update [NotificationScheduleQueue] - Resync templates'

-- Set the notification template for each notification in the queue.
UPDATE q
SET [NotificationTemplateId] = gpn.[NotificationTemplateId]
FROM dbo.[NotificationScheduleQueue] q
JOIN dbo.[GrantProgramNotifications] gpn ON q.[NotificationTypeId] = gpn.[NotificationTypeId]
JOIN dbo.[GrantApplications] ga ON q.[GrantApplicationId] = ga.[Id]
JOIN dbo.[GrantOpenings] g ON ga.[GrantOpeningId] = g.[Id]
JOIN dbo.[GrantStreams] gs ON g.[GrantStreamId] = gs.[Id] AND gpn.[GrantProgramId] = gs.[GrantProgramId]