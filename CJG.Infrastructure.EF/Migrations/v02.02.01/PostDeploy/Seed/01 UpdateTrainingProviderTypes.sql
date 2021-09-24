PRINT '01. Updating [dbo].[TrainingProviderTypes]'

UPDATE [dbo].[TrainingProviderTypes]
   SET [PrivateSectorValidationType] = 1
		,[DateUpdated] = GETUTCDATE()
 WHERE [Id] = 2
GO

UPDATE [dbo].[TrainingProviderTypes]
   SET [PrivateSectorValidationType] = 1
		,[DateUpdated] = GETUTCDATE()
 WHERE [Id] = 12
GO

PRINT 'Update Completed!'