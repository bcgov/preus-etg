PRINT 'Update [GrantProgramNotifications]'

DELETE dbo.[GrantProgramNotifications]

SELECT DISTINCT
	gp.[Id] AS [GrantProgramId]
	, nt.[Id] AS [NotificationTypeId]
INTO #GrantProgramNotifications
FROM dbo.[NotificationTypes] nt
	, dbo.[GrantPrograms] gp
LEFT JOIN dbo.[GrantProgramNotifications] gpn ON gp.[Id] = gpn.[GrantProgramId]
WHERE gpn.[GrantProgramId] IS NULL
ORDER BY gp.[Id], nt.[Id]

INSERT INTO dbo.[GrantProgramNotifications] (
	[GrantProgramId]
	, [NotificationTypeId]
	, [IsActive]
	, [DateAdded]
) 
SELECT
	[GrantProgramId]
	, [NotificationTypeId]
	, 1
	, GETUTCDATE()
FROM #GrantProgramNotifications

DROP TABLE #GrantProgramNotifications