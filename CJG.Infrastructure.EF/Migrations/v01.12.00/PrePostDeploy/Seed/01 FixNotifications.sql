PRINT 'Restore Grant Program Notification Settings'

-- Re-insert grant program notification values.
INSERT INTO dbo.[GrantProgramNotifications] (
	[GrantProgramId]
	, [NotificationTypeId]
	, [NotificationTemplateId]
	, [IsActive]
	, [DateAdded]
) 
SELECT 
	[GrantProgramId]
	, [NotificationTypeId]
	, [NotificationTemplateId]
	, 1
	, GETUTCDATE()
FROM #GrantProgramNotifications

-- Fix the schedule queue by mapping the templates to the queue.
UPDATE nsq
SET NotificationTemplateId = gpn.NotificationTemplateId
FROM dbo.[NotificationScheduleQueue] nsq
JOIN dbo.[GrantApplications] ga ON nsq.GrantApplicationId = ga.Id
JOIN dbo.[GrantOpenings] g ON ga.GrantOpeningId = g.Id
JOIN dbo.[GrantStreams] gs ON g.GrantStreamId = gs.Id
JOIN #GrantProgramNotifications gpn ON gs.GrantProgramId = gpn.GrantProgramId

DROP TABLE #GrantProgramNotifications

-- Delete the grant program notifications that are for employer administrators
DELETE FROM dbo.[GrantProgramNotifications]
WHERE [NotificationTypeId] IN (16, 17, 18, 19)

-- Delete the notification templates that are for employer administrators
DELETE nt
FROM dbo.[NotificationTemplates] nt
LEFT JOIN dbo.[GrantProgramNotifications] gpn ON nt.[Id] = gpn.[NotificationTemplateId]
WHERE gpn.[NotificationTemplateId] IS NULL

-- Delete the notification types that are for employer administrators
DELETE FROM dbo.[NotificationTypes]
WHERE [Id] IN (16, 17, 18, 19)