PRINT 'INSERT [GrantProgramNotifications] - CWRG'

INSERT INTO GrantProgramNotifications (
	GrantProgramId
	, NotificationTypeId
	, IsActive
	, DateAdded
	, DateUpdated 
)
SELECT 3
	, Id
	, 1
	, GETUTCDATE()
	, null
FROM dbo.NotificationTypes 
WHERE Id > 28 