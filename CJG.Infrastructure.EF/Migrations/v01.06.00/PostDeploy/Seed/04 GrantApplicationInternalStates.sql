PRINT 'Updating [GrantAppliciontInternalStates]'

SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] ON 
INSERT INTO dbo.[GrantApplicationInternalStates] (
	[Id]
	, [Description]
	, [Caption]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
)
VALUES(
	32
	, N'Claim reporting is closed and only completion reporting is allowed at this time.  The external state is "Report Completion"'
	, N'Completion Reporting'
	, 1
	, 0
	, GETUTCDATE()
)
SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] OFF