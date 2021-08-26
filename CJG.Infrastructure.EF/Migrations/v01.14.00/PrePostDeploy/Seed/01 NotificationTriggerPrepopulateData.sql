PRINT 'INSERT into [NotificationTriggers] - Adding new data'

BEGIN

INSERT INTO dbo.NotificationTriggers (Id, Caption, Description, DateAdded, DateUpdated, IsActive, RowSequence)
VALUES (1, 'Workflow', 'Notifications that are triggered when a workflow change occurs.', GETUTCDATE(), GETUTCDATE(), 1, 0),
		(2, 'Scheduled', 'Notifications that are set to be triggered during a scheduled date.', GETUTCDATE(), GETUTCDATE(), 1, 0)

END

CHECKPOINT