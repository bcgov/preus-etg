PRINT 'Updating [GrantApplications].[ScheduledNotificationsEnabled]'

-- By default all applications should send notifications.
UPDATE dbo.[GrantApplications]
SET [ScheduledNotificationsEnabled] = 1

CHECKPOINT