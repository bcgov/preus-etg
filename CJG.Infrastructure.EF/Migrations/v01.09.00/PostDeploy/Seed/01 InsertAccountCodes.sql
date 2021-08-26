PRINT 'INSERT [AccountCodes]'

SET IDENTITY_INSERT [dbo].[AccountCodes] ON 

INSERT [dbo].[AccountCodes] (
	[Id]
	, [GLClientNumber]
	, [GLRESP]
	, [GLServiceLine]
	, [GLSTOBNormal]
	, [GLSTOBAccrual]
	, [GLProjectCode]
	, [DateAdded]
	, [DateUpdated]
)
VALUES (
	3
	, N'019'
	, N'11651'
	, N'20921'
	, N'8001'
	, N'8001'
	, N'111CWRD'
	, GETUTCDATE()
	, NULL)

SET IDENTITY_INSERT [dbo].[AccountCodes] OFF