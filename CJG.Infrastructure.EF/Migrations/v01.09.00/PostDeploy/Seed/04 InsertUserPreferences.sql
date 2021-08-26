PRINT 'INSERT [UserPreferences]'

INSERT INTO [dbo].[UserPreferences] (
	[UserId]
	, [GrantProgramPreferencesUpdated]
	, [DateAdded]
	, [DateUpdated]
)
SELECT 
	[Id],
	GETUTCDATE(),
	GETUTCDATE(),
	GETUTCDATE()
FROM [dbo].[Users];
