PRINT 'INSERT [UserProgramPreferences]'

INSERT INTO [dbo].[UserGrantProgramPreferences] (
	[UserId]
	, [GrantProgramId]
	, [IsSelected]
	, [DateAdded]
	, [DateUpdated]
)
SELECT 
	u.[Id] as UserId,
	g.[Id] as GrantProgramId,
	CASE WHEN g.[Id] = 2 THEN 1 ELSE 0 END as IsSelected,
	GETUTCDATE(),
	GETUTCDATE()
FROM [dbo].[Users] u, 
     [dbo].[GrantPrograms]  g;
