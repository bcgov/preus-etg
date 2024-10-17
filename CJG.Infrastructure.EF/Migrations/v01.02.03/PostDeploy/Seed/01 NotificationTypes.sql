PRINT 'Updating [NotificationTypes]'

UPDATE [dbo].[NotificationTypes]
SET [MilestoneDateName] = N'ChangeTrainingProviderDeny',
	[NotificationTypeName] = N'InsChangeTrainingProviderDenyAdmin'
WHERE [Id] = 15