PRINT 'Updating [GrantApplicationExternalStates]'

-- Adding External State for AmendClaim as part of 266 - Claim Assessment.
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationExternalStates] WHERE [Id] = 26))
BEGIN
	UPDATE [dbo].GrantApplicationExternalStates
	SET [Caption] = N'Amend Claim',
		[Description] = N'This state provides a way for the Assessor to notify the Application Administrator that they should complete a Claim amendment.'
	WHERE [Id] = 26
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationExternalStates] WHERE [Id] = 27))
BEGIN
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] ON 
	INSERT [dbo].[GrantApplicationInternalStates]
		([Id], [Caption], [Description], [IsActive], [RowSequence]) VALUES
		(27, N'Report Completion', N'This state has closed claim reporting and allows only completion reporting for the grant file.', 1, 0)
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] OFF
END