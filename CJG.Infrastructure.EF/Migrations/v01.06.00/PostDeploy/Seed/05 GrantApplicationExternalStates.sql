PRINT 'Updating [GrantApplicationExternalStates]'

SET IDENTITY_INSERT [dbo].[GrantApplicationExternalStates] ON 
INSERT INTO dbo.[GrantApplicationExternalStates] (
	[Id]
	, [Description]
	, [Caption]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
)
VALUES (
	26
	, N'This state provides a way for the Assessor to notify the Application Administrator that they should complete a Claim amendment.'
	, N'Amend Claim'
	, 1
	, 0
	, GETUTCDATE()
), (
	27
	, N'This state has closed claim reporting and allows only completion reporting for the grant file.'
	, N'Report Completion'
	, 1
	, 0
	, GETUTCDATE()
)
SET IDENTITY_INSERT [dbo].[GrantApplicationExternalStates] OFF